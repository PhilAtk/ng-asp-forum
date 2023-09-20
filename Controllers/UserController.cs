using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ng_asp_forum.Migrations;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
	private readonly ILogger<UserController> _logger;
	private ForumContext _db;
	private ForumAuthenticator _auth;

	public UserController(ILogger<UserController> logger, ForumContext db, ForumAuthenticator auth) {
		_logger = logger;
		_db = db;
		_auth = auth;
	}

	public class UserAuditResponse {
		public ForumUser user {get; set;}
		public List<ForumUserAudit> audits {get; set;}
	}

	[HttpGet]
	[Route("audit/{id}")]
	public ActionResult<UserAuditResponse> GetUserAudit(int id) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		int viewerID;
		if (!_auth.VerifyBearerToken(auth, out viewerID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var viewer = _db.Users.Where(u => u.userID == viewerID).First();
			if (viewer == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if (viewer.userRole >= userRole.ADMIN) {
				var user = _db.Users
					.Where(u => u.userID == id)
					.First();
				if (user == null) {
					return NotFound("No user found with the ID supplied for audit");
				}

				var audits = _db.UserAudits
					.Where(a => a.user.userID == id)
					.OrderByDescending(a => a.date)
					.ToList();

				var res = new UserAuditResponse {
					user = user,
					audits = audits
				};

				return Ok(res);
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}	
	}

	[HttpGet]
	public ActionResult<IEnumerable<ForumUser>> GetUserList() {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		int viewerID;
		if (!_auth.VerifyBearerToken(auth, out viewerID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var viewer = _db.Users.Where(u => u.userID == viewerID).First();
			if (viewer == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if (viewer.userRole >= userRole.ADMIN) {
				// TODO: Make sure we don't spill any info we shouldn't
				var list = _db.Users;

				return Ok(list.ToArray());
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}

	[HttpGet]
	[Route("{id}")]
	public ActionResult<ForumUser> GetUser(int id) {
		var user = _db.Users
			.Where(u => u.userID == id)
			.First();

		if (user == null) {
			return NotFound("No user was found with the specified ID");
		}

		return user;
	}

	public class UserEditData {
		public string? bio {get; set;}
	};

	[HttpPatch]
	[Route("{id}")]
	public ActionResult UpdateUser(int id, UserEditData data) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		int editorID;
		if (!_auth.VerifyBearerToken(auth, out editorID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var user_to_edit = _db.Users.Where(u => u.userID == id).First();
			if (user_to_edit == null) {
				return NotFound("No user found with the given userID");
			}

			var editor = _db.Users.Where(u => u.userID == editorID).First();
			if (editor == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if ((	editor.userID == user_to_edit.userID && editor.userState >= userState.ACTIVE) ||
				editor.userRole >= userRole.ADMIN) {
					user_to_edit.bio = data.bio;
					_db.SaveChanges();
					return Ok();
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError("Error editing user: " + e.Message);
			return StatusCode(500);
		}
	}

	public class AdminUserEditData {
		public userRole role {get; set;}
		public userState state {get; set;}
	};

	public class BanData {
		public string? reason {get; set;}
	}

	[HttpPost]
	[Route("ban/{id}")]
	public ActionResult Ban(int id, BanData data) {
		var auth = Request.Cookies["auth"];

		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		int editorUserID;
		if (!_auth.VerifyBearerToken(auth, out editorUserID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var user = _db.Users.Where(u => u.userID == id).First();
			if (user == null) {
				return NotFound("No user found with the given userID");
			}

			var editor = _db.Users.Where(u => u.userID == editorUserID).First();
			if (editor == null) {
				return NotFound("No user found with the given auth credentials");
			}

			if (editor.userRole >= userRole.ADMIN) {

				var audit = new ForumUserAudit {
					date = DateTime.Now,
					user = user,
					action = userAction.BAN,
					info = "Reason: " + data.reason
				};
				_db.Add(audit);
				
				user.userState = userState.BANNED;
				_db.SaveChanges();

				return Ok();
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError("Error banning user: " + e.Message);
			return StatusCode(500);
		}
	}

	[HttpPost]
	[Route("unban/{id}")]
	public ActionResult Unban(int id) {
		var auth = Request.Cookies["auth"];

		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		int editorUserID;
		if (!_auth.VerifyBearerToken(auth, out editorUserID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var user = _db.Users.Where(u => u.userID == id).First();
			if (user == null) {
				return NotFound("No user found with the given userID");
			}

			var editor = _db.Users.Where(u => u.userID == editorUserID).First();
			if (editor == null) {
				return NotFound("No user found with the given auth credentials");
			}

			if (editor.userRole >= userRole.ADMIN) {
				
				var audit = new ForumUserAudit {
					date = DateTime.Now,
					user = user,
					action = userAction.UNBAN,
				};
				_db.Add(audit);

				user.userState = userState.ACTIVE;
				_db.SaveChanges();

				return Ok();
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError("Error unbanning user: " + e.Message);
			return StatusCode(500);
		}
	}
}