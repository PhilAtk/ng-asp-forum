using System.Security;

public class UserService {
	private readonly ILogger<UserService> _logger;

	private UserRepository _userRepo;
	private ForumAuthenticator _auth;

	public UserService(ILogger<UserService> logger, UserRepository userRepo, ForumAuthenticator auth) {
		_logger = logger;
		_userRepo = userRepo;
		_auth = auth;
	}

	public UserViewmodel GetUser(int userID) {

		var user = _userRepo.GetUserByID(userID);
		if (user == null) {
			throw new KeyNotFoundException("No user was found with the specified ID");
		}

		return new UserViewmodel(user);
	}

	public List<UserViewmodel> GetUserList(string auth) {
		if (_auth.TokenIsAdmin(auth)) {
			var usersBackend = _userRepo.GetUsers();
			List<UserViewmodel> users = new List<UserViewmodel>();

			usersBackend.ForEach(u => users.Add(new UserViewmodel(u)));

			return users;
		}

		else {
			throw new SecurityException("Unauthorized");
		}
	}

	public UserAuditResponse GetUserAudit(int userID, string auth) {
		var user = _userRepo.GetUserByID(userID);
		if (user == null) {
			throw new KeyNotFoundException();
		}

		if (_auth.TokenIsAdmin(auth)) {
			var auditsBackend = _userRepo.GetUserAudits(userID);
			List<UserAuditViewmodel> audits = new List<UserAuditViewmodel>();

			auditsBackend.ForEach(a => audits.Add(new UserAuditViewmodel(a)));

			return new UserAuditResponse{
				user = new UserViewmodel(user),
				audits = audits
			};
		}

		else {
			throw new SecurityException("Not authorized");
		}
	}

	public void UpdateUserBio(int id, string? bio, string auth) {
		var user_to_edit = _userRepo.GetUserByID(id);
		if (user_to_edit == null) {
			throw new KeyNotFoundException("No user found with the given userID");
		}

		if (_auth.TokenIsUser(auth, id)) {
			_userRepo.SetUserBio(user_to_edit, bio);
		}

		else {
			throw new SecurityException("Not Authorized");
		}
	}

	public void BanUser(int id, string? reason, string auth) {
		var user = _userRepo.GetUserByID(id);
		if (user == null) {
			throw new KeyNotFoundException("No user found with the given userID");
		}

		if (_auth.TokenIsAdmin(auth)) {
			_userRepo.BanUser(user, reason);
		}

		else {
			throw new SecurityException("Not authorized to ban user");
		}
	}

	public void UnbanUser(int id, string auth) {
		var user = _userRepo.GetUserByID(id);
		if (user == null) {
			throw new KeyNotFoundException("No user found with the given userID");
		}

		if (_auth.TokenIsAdmin(auth)) {
			_userRepo.UnbanUser(user);
		}

		else {
			throw new SecurityException("Not authorized to unban user");
		}
	}
}