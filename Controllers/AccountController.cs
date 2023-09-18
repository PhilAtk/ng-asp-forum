using Microsoft.AspNetCore.Mvc;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase {
	private readonly ILogger<AccountController> _logger;
	private ForumContext _db;
	private ForumAuthenticator _auth;
	private ForumEmail _email;

	public AccountController(ILogger<AccountController> logger, ForumContext db, ForumAuthenticator auth, ForumEmail email) {
		_logger = logger;
		_db = db;
		_auth = auth;
		_email = email;
	}

	public class LoginData {
		public string? username {get; set;}
		public string? password {get; set;}
	};

	public class LoginResult {
		public string? userName {get; set;}
		public int userID {get; set;}
		public int userRole {get; set;}
		public int userState {get; set;}
		public string? token {get; set;}
	}

	[HttpPost]
	[Route("login")]
	public IActionResult Login(LoginData data) {
		try {
			var user = _db.Users.Where(u => u.userName == data.username).First();

			if (
				user == null || 
				string.IsNullOrWhiteSpace(data.password) || 
				string.IsNullOrWhiteSpace(data.username)) {
					return BadRequest();
			}

			if (string.IsNullOrWhiteSpace(user.password)) {
				// TODO: If this happens, we might want to require a password reset
				return StatusCode(500);
			}

			switch (user.userState) {
				case userState.ACTIVE:
					if (_auth.HashVerify(data.password, user.password)) {
						var token = _auth.GenerateBearerToken(user.userID);

						var res = new LoginResult{
							token = token,
							userName = user.userName,
							userID = user.userID,
							userRole = (int)user.userRole,
							userState = (int)user.userState,
						};

						return Ok(res);
					}
					return Unauthorized("Incorrect Password");

				case userState.AWAIT_REG:
					return Unauthorized("Please confirm registration before logging in");
				case userState.BANNED:
					return Unauthorized("This account has been banned");
				case userState.DISABLED:
					return Unauthorized("This account has been disabled");
				default:
					return Unauthorized("Failed to log in");
			}
		}

		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}

	public class ForgotData {
		public string? email {get; set;}
	};

	[HttpPost]
	[Route("password/forgot")]
	public IActionResult PasswordResetRequest(ForgotData data) {
		var user = _db.Users.Where(u => u.email == data.email).First();

		if (user == null) {
			return NotFound();
		}

		// Must have already registered, or not been banned
		if (user.userState != userState.ACTIVE) {
			return Unauthorized("Please finish registration before requesting a password reset");
		}

		if (string.IsNullOrWhiteSpace(user.email)) {
			// TODO: This would be very bad, ask them to contact an admin
			return StatusCode(500);
		}

		// Generate a reset code
		var resetCode = _auth.GetRandom6charCode();

		try {
			user.code = resetCode;
			_db.SaveChanges();

			_email.sendPasswordReset(user.email, resetCode);
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500); // It might have been our fault idk
		}

		return Ok();
	}

	public class ResetData {
		public string? token {get; set;}
		public string? password {get; set;}
	}

	[HttpPost]
	[Route("password/reset")]
	public IActionResult VerifyPasswordReset(ResetData data) {

		if (data.token == null ) {
			return BadRequest("No token provided for password reset");
		}

		if (data.password == null) {
			return BadRequest("No password provided");
		}

		// Find the user for this code
		var user = _db.Users.Where(u => u.code == data.token).First();

		if (user == null) {
			return NotFound();
		}

		try {		
			// Update the password
			user.password = _auth.Hash(data.password);
			user.code = null;
			_db.SaveChanges();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		return Ok();
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

			var user = new ForumUser{
				userName = data.username,
				password = _auth.Hash(data.password),
				email = data.email,
				userState = userState.AWAIT_REG,
				userRole = userRole.USER,
				code = _auth.GetRandom6charCode()
			};

			_db.Add(user);
			_db.SaveChanges();

			_email.sendRegistrationConfirmation(user.email, user.code);

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

			_db.SaveChanges();

			return Ok();	
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}
}