using Microsoft.AspNetCore.Mvc;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase {
	private readonly ILogger<AccountController> _logger;

	private AccountService _account;

	public AccountController(ILogger<AccountController> logger, AccountService account) {
		_logger = logger;

		_account = account;
	}

	public class LoginData {
		public string? username {get; set;}
		public string? password {get; set;}
	};

	[HttpPost]
	[Route("login")]
	public IActionResult Login(LoginData data) {
		if (string.IsNullOrWhiteSpace(data.password) || string.IsNullOrWhiteSpace(data.username)) {
			return BadRequest();
		}

		try {
			var res = _account.Login(data.username, data.password);
			return Ok(res);
		}

		catch (Exception e) {
			// TODO: Separate out server errors with exception types
			_logger.LogError(e.Message);
			return BadRequest();
		}
	}

	public class ForgotData {
		public string? email {get; set;}
	};

	[HttpPost]
	[Route("password/forgot")]
	public IActionResult PasswordResetRequest(ForgotData data) {
		if (data.email == null) {
			return BadRequest("No email provided for password reset");
		}

		try {
			_account.ResetPasswordByEmail(data.email);
		}
		catch {
			// TODO: Add custom exception types to check if bad request
			// Probably shouldn't announce if the email wasn't found for security reasons
			return StatusCode(500);
		}

		// TODO: Wrap the message in an object to pull out of on the frontend?
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

		try {
			_account.VerifyAndResetPassword(data.token, data.password);
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
	[Route("register/request")]
	public IActionResult RequestRegister(RegisterData data) {

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
			_account.RegisterUser(data.username, data.email, data.password);
			return Ok();
		}

		catch (Exception e) {
			// TODO: Separate out exceptions
			_logger.LogError(e.Message);
			return BadRequest();
		}
	}

	public class RegisterConfData {
		public string? token {get; set;}
	};

	[HttpPost]
	[Route("register/confirm")]
	public IActionResult ConfirmRegistration(RegisterConfData data) {

		if (data.token == null) {
			return BadRequest("No token provided for validation");
		}

		try {
			_account.ConfirmRegistration(data.token);
			return Ok();	
		}
		catch (Exception e) {
			// TODO: Separate out exceptions
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}
}