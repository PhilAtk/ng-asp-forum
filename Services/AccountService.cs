using Microsoft.AspNetCore.Mvc;

public class AccountService {
	private readonly ILogger<AccountService> _logger;

	private UserRepository _userRepo;

	private ForumAuthenticator _auth;
	private ForumEmail _email;

	public AccountService(ILogger<AccountService> logger, UserRepository userRepo, ForumAuthenticator auth, ForumEmail email) {
		_logger = logger;

		_userRepo = userRepo;
		_auth = auth;
		_email = email;
	}

	public LoginResult Login(string username, string password) {
		var user = _userRepo.GetUserByUsername(username);
		if (user == null) {
			// TODO: Don't announce this? Just say incorrect login?
			throw new Exception("User not found");
		}

		if (string.IsNullOrWhiteSpace(user.password)) {
			// TODO: If this happens, we might want to require a password reset
			throw new Exception("NO PASSWORD FOUND FOR USER");
		}

		switch (user.userState) {
			case userState.ACTIVE:
				if (_auth.HashVerify(password, user.password)) {
					var token = _auth.GenerateBearerToken(user.userID);

					var res = new LoginResult{
						token = token,
						userName = user.userName,
						userID = user.userID,
						userRole = (int)user.userRole,
						userState = (int)user.userState,
					};

					return res;
				}
				// TODO: Don't announce this? Just say incorrect login?
				throw new Exception("Incorrect Password");

			// TODO: Create exception types for each of these?
			case userState.AWAIT_REG:
				throw new Exception("Please confirm registration before logging in");
			case userState.BANNED:
				throw new Exception("This account has been banned");
			case userState.DISABLED:
				throw new Exception("This account has been disabled");
			default:
				throw new Exception("Failed to log in");
		}
	}

	public void ResetPasswordByEmail(string email) {
		var user = _userRepo.GetUserByEmail(email);
		if (user == null) {
			// TODO: Simply log and return without throwing exception?
			// We shouldn't announce whether there was an account or not
			throw new Exception("No user found with the given email");
		}

		// Must have already registered, or not been banned
		if (user.userState != userState.ACTIVE) {
			// TODO: Is this actually something we should bother with? Maybe at least not banned?
			throw new Exception("Please finish registration before requesting a password reset");
		}

		// Generate a reset code
		var resetCode = _auth.GetRandom6charCode();

		// TODO: Have these methods throw exceptions themselves if things go wrong
		_userRepo.SetUserCode(user, resetCode);
		_email.sendPasswordReset(email, resetCode);
	}

	public void VerifyAndResetPassword(string resetCode, string newPassword) {
		
		// Find the user for this code
		var user = _userRepo.GetUserByCode(resetCode);
		if (user == null) {
			// TODO: Don't announce this?
			throw new Exception("No user found awaiting reset");
		}
	
		var hashedPass = _auth.Hash(newPassword);
		_userRepo.SetUserHashedPass(user, hashedPass);
	}

	public void RegisterUser(string username, string email, string password) {
		var user = _userRepo.GetUserByUsername(username);
		if (user != null) {
			// TODO: Create exception type for username/email already used
			throw new Exception("Username already exists");
		}

		user = _userRepo.GetUserByEmail(email);
		if (user != null) {
			throw new Exception("User already exists with the given email");
		}

		user = new ForumUser{
			userName = username,
			password = _auth.Hash(password),
			email = email,
			userState = userState.AWAIT_REG,
			userRole = userRole.USER,
			code = _auth.GetRandom6charCode()
		};
		
		_userRepo.RegisterUser(user);
		_email.sendRegistrationConfirmation(user.email, user.code);
	}

	public void ConfirmRegistration(string regCode) {
		var user = _userRepo.GetUserByCode(regCode);

		if (user == null) {
			throw new Exception("No user found");
		}

		if (user.userState != userState.AWAIT_REG) {
			throw new Exception("Registration is already confirmed");
		}

		_userRepo.SetUserRegConfirmed(user);
	}
}