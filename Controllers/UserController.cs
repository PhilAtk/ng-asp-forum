using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ng_asp_forum.Migrations;
using System.Security;

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
			return Unauthorized("No auth token provided");
		}

		try {
			var res = _user.GetUserAudit(id, auth);
			return Ok(res);
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet]
	public ActionResult<IEnumerable<UserViewmodel>> GetUserList() {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return Unauthorized("No auth token provided");
		}

		try {
			var userList = _user.GetUserList(auth);
			return Ok(userList);
		}

		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet]
	[Route("{id}")]
	public ActionResult<UserViewmodel> GetUser(int id) {
		
		// TODO: Change to a front-end safe viewmodel
		UserViewmodel res;

		try {
			res = _user.GetUser(id);
			return Ok(res);
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	public class UserEditData {
		public string? bio {get; set;}
	};

	[HttpPatch]
	[Route("{id}")]
	public ActionResult UpdateUserBio(int id, UserEditData data) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return Unauthorized("No auth token provided");
		}

		try {
			_user.UpdateUserBio(id, data.bio, auth);
			return Ok();
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
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
			return Unauthorized("No auth token provided");
		}

		try {
			_user.BanUser(id, data.reason, auth);
			return Ok();
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPost]
	[Route("unban/{id}")]
	public ActionResult Unban(int id) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return Unauthorized("No auth token provided");
		}

		try {
			_user.UnbanUser(id, auth);
			return Ok();
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}