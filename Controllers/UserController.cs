using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

	[HttpGet]
	public ActionResult<IEnumerable<ForumUser>> GetUserList() {
		try {
			// TODO: Make sure we don't spill any info we shouldn't
			var list = _db.Users;

			return Ok(list.ToArray());
		}
		catch (Exception e) {
			// TODO: Log this instead
			Console.WriteLine(e.Message);
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

		try {
			var user = _db.Users.Where(u => u.userID == id).First();

			if (user == null) {
				return NotFound("No user found with the given userID");
			}

			// TODO: Change verification to allow if admin/sysop
			int userID;
			if (!_auth.VerifyBearerToken(auth, out userID) || user.userID != userID) {
				return Unauthorized();
			}

			// TODO: Change this to handle if admin/sysop is trying to delete
			if (user.userState == userState.BANNED) {
				return Unauthorized();
			}

			user.bio = data.bio;
			_db.SaveChanges();
		}
		catch (Exception e) {
			_logger.LogError("Error editing user: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}

	public class AdminUserEditData {
		public userRole role {get; set;}
		public userState state {get; set;}
	};

	[HttpPatch]
	[Route("admin/{id}")]
	public ActionResult UpdateUserAdmin(int id, AdminUserEditData data) {
		var auth = Request.Cookies["auth"];

		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		try {
			var user = _db.Users.Where(u => u.userID == id).First();

			if (user == null) {
				return NotFound("No user found with the given userID");
			}

			int editorUserID;
			if (!_auth.VerifyBearerToken(auth, out editorUserID)) {
				return Unauthorized();
			}

			var editor = _db.Users.Where(u => u.userID == editorUserID).First();

			if (editor == null) {
				return NotFound("No user found with the given auth credentials");
			}

			if (editor.userRole < userRole.ADMIN) {
				return Unauthorized();
			}

			user.userState = data.state;
			user.userRole = data.role;
			_db.SaveChanges();
		}
		catch (Exception e) {
			_logger.LogError("Error editing user: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}
}