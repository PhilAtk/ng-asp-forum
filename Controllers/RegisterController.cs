using Microsoft.AspNetCore.Mvc;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase {
	private readonly ILogger<RegisterController> _logger;
	private ForumContext _db;
	private ForumAuthenticator _auth;
	private ForumEmail _email;

	public RegisterController(ILogger<RegisterController> logger, ForumContext db, ForumAuthenticator auth, ForumEmail email) {
		_logger = logger;
		_db = db;
		_auth = auth;
		_email = email;
	}

	public class RegisterData {
		public string? username {get; set;}
		public string? email {get; set;}
		public string? password {get; set;}
	};

	[HttpPost]
	[Route("request")]
	public IActionResult RegisterRequest(RegisterData data) {

		if (string.IsNullOrWhiteSpace(data.username)) {
			return BadRequest("No username provided");
		}

		if (string.IsNullOrWhiteSpace(data.email)) {
			return BadRequest("No email provided");
		}

		if (string.IsNullOrWhiteSpace(data.password)) {
			return BadRequest("No password provided");
		}

		try {
			int user_count = 0;
			user_count += _db.Users.Where(u => u.userName == data.username).Count();
			
			if (user_count > 0) {
				return BadRequest("Username already exists");
			}

			user_count += _db.Users.Where(u => u.email == data.email).Count();

			if (user_count > 0) {
				return BadRequest("Email already in use");
			}

			var hashedPass = _auth.Hash(data.password);
			
			var regCode = _auth.GetRandom6charCode();

			var user = new ForumUser{
				userName = data.username,
				password = hashedPass,
				email = data.email,
				userState = userState.AWAIT_REG,
				userRole = userRole.USER,
				code = regCode
			};

			_db.Add(user);
			_db.SaveChanges();

			//var token = _auth.GenerateRegisterToken(user.userID);
			_email.sendRegistrationConfirmation(user.email, regCode);

			return Ok();
		}

		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}

	public class RegisterConfData {
		public string? token {get; set;}
	};

	[HttpPost]
	[Route("confirm")]
	public IActionResult VerifyRegister(RegisterConfData data) {

		if (data.token == null) {
			return BadRequest("No token provided for validation");
		}

		try {
			var user = _db.Users.Where(u => u.code == data.token).First();

			if (user.userState != userState.AWAIT_REG) {
				// We shouldn't be here
				return BadRequest();
			}

			user.userState = userState.ACTIVE;
			user.code = null;

			_db.Update(user);
			_db.SaveChanges();
			return Ok();	
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}
}