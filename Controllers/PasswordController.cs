using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordController : ControllerBase {
	private readonly ILogger<PasswordController> _logger;
	private ForumContext _db;
	private ForumAuthenticator _auth;
	private ForumEmail _email;

	public PasswordController(ILogger<PasswordController> logger, ForumContext db, ForumAuthenticator auth, ForumEmail email) {
		_logger = logger;
		_db = db;
		_auth = auth;
		_email = email;
	}

	public class ForgotData {
		public string? email {get; set;}
	};

	[HttpPost]
	[Route("forgot")]
	public IActionResult PasswordResetRequest(ForgotData data) {
		var user = _db.Users.Where(u => u.email == data.email).First();

		if (user != null) {
			// Must have already registered, or not been banned
			if (user.userState != userState.ACTIVE) {
				return Unauthorized("Please finish registration before requesting a password reset");
			}

			if (string.IsNullOrWhiteSpace(user.email)) {
				// TODO: This would be very bad, ask them to contact an admin
				return StatusCode(500);
			}

			// Generate a token with this userID
			var token = _auth.GenerateResetToken(user.userID);

			try {
				_email.sendPasswordReset(user.email, token);
			}
			catch (Exception e) {
				_logger.LogError(e.Message);
				return StatusCode(500); // It might have been our fault idk
			}
		}

		return Ok();
	}

	public class ResetData {
		public string? token {get; set;}
		public string? password {get; set;}
	}

	[HttpPost]
	[Route("reset")]
	public IActionResult VerifyPasswordReset(ResetData data) {

		// TODO: Don't let the same token be used twice

		if (data.token == null ) {
			return BadRequest("No token provided for password reset");
		}

		if (data.password == null) {
			return BadRequest("No password provided");
		}

		// Verify the token
		int userID;
		if (!_auth.VerifyResetToken(data.token, out userID)) {
			return Unauthorized();
		}

		// Find the user with that userID
		var user = _db.Users.Where(u => u.userID == userID).First();

		// Update the password
		user.password = _auth.Hash(data.password);

		try {
			_db.Update(user);
			_db.SaveChanges();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		return Ok();
	}
}