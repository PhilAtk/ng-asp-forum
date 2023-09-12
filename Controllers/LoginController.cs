using Microsoft.AspNetCore.Mvc;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase {
	private readonly ILogger<LoginController> _logger;
	private ForumContext _db;
	private ForumAuthenticator _auth;

	public LoginController(ILogger<LoginController> logger, ForumContext db, ForumAuthenticator auth) {
		_logger = logger;
		_db = db;
		_auth = auth;
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
			Console.WriteLine(e.Message);
			return StatusCode(500);
		}
	}
}