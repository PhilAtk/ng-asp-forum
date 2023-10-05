using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ng_asp_forum.Migrations;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
	private readonly ILogger<UserController> _logger;

	private UserService _user;

	public UserController(ILogger<UserController> logger, UserService user) {
		_logger = logger;

		_user = user;
	}

	[HttpGet]
	[Route("audit/{id}")]
	public ActionResult<UserAuditResponse> GetUserAudit(int id) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		try {
			var res = _user.GetUserAudit(id, auth);
			return Ok(res);
		}

		catch (Exception e) {
			// TODO: Separate exceptions
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

		try {
			var userList = _user.GetUserList(auth);
			return Ok(userList);
		}
		catch (Exception e) {
			// TODO: Separate Exceptions
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}

	[HttpGet]
	[Route("{id}")]
	public ActionResult<ForumUser> GetUser(int id) {
		
		// TODO: Change to a front-end safe viewmodel
		ForumUser res;

		try {
			res = _user.GetUser(id);
		}
		catch {
			// TODO: Separate out exceptions
			return NotFound();
		}

		return res;
	}

	public class UserEditData {
		public string? bio {get; set;}
	};

	[HttpPatch]
	[Route("{id}")]
	public ActionResult UpdateUserBio(int id, UserEditData data) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		try {
			_user.UpdateUserBio(id, data.bio, auth);
			return Ok();
		}
		catch (Exception e) {
			// TODO: Separate exceptions
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

		try {
			_user.BanUser(id, data.reason, auth);
			return Ok();
		}
		catch (Exception e) {
			// TODO: Separate exceptions
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

		try {
			_user.UnbanUser(id, auth);
			return Ok();
		}
		catch (Exception e) {
			// TODO: Separate exceptions
			_logger.LogError("Error unbanning user: " + e.Message);
			return StatusCode(500);
		}
	}
}