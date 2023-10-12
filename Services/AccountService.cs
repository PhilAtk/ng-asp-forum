using System.Security;
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
			// Don't announce what the issue is, prevents guessing user/password
			throw new SecurityException();
		}

		if (string.IsNullOrWhiteSpace(user.password)) {
			// TODO: If this happens, we might want to require a password reset
			throw new KeyNotFoundException("NO PASSWORD FOUND FOR USER");
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
				// Don't announce what the issue is, prevents guessing user/password
				throw new SecurityException();

			case userState.AWAIT_REG:
				throw new SecurityException("Please confirm registration before logging in");
			case userState.BANNED:
				throw new SecurityException("This account has been banned");
			case userState.DISABLED:
				throw new SecurityException("This account has been disabled");
			default:
				throw new SecurityException();
		}
	}

	public void ResetPasswordByEmail(string email) {
		var user = _userRepo.GetUserByEmail(email);
		if (user == null) {
			// We shouldn't announce whether there was an account or not
			return;
		}

		// Generate a reset code
		var resetCode = _auth.GetRandom6charCode();

		_userRepo.SetUserCode(user, resetCode);
		_email.sendPasswordReset(email, resetCode);
	}

	public void VerifyAndResetPassword(string resetCode, string newPassword) {
		
		// Find the user for this code
		var user = _userRepo.GetUserByCode(resetCode);
		if (user == null) {
			throw new SecurityException("Invalid verification code");
		}
	
		var hashedPass = _auth.Hash(newPassword);
		_userRepo.SetUserHashedPass(user, hashedPass);
	}

	public void RegisterUser(string username, string email, string password) {
		var user = _userRepo.GetUserByUsername(username);
		if (user != null) {
			throw new ArgumentException("Username already exists");
		}

		user = _userRepo.GetUserByEmail(email);
		if (user != null) {
			throw new ArgumentException("User already exists with the given email");
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
			throw new KeyNotFoundException("No user found");
		}

		if (user.userState != userState.AWAIT_REG) {
			throw new ArgumentException("Registration is already confirmed");
		}

		_userRepo.SetUserRegConfirmed(user);
	}
}