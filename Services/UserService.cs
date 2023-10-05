public class UserService {
	private readonly ILogger<UserService> _logger;

	private UserRepository _userRepo;
	private ForumAuthenticator _auth;

	public UserService(ILogger<UserService> logger, UserRepository userRepo, ForumAuthenticator auth) {
		_userRepo = userRepo;
		_auth = auth;
	}

	public ForumUser GetUser(int userID) {

		// TODO: Pack this into a viewmodel safe to return to the frontend
		var user = _userRepo.GetUserByID(userID);

		if (user == null) {
			// TODO: Move this to the repo itself
			throw new Exception("No user was found with the specified ID");
		}

		return user;
	}

	public List<ForumUser> GetUserList(string auth) {
		if (_auth.TokenIsAdmin(auth)) {
			// TODO: Switch to use Viewmodel to make sure we don't spill any info we shouldn't
			return _userRepo.GetUsers();
		}

		else {
			throw new Exception("Unauthorized");
		}
	}

	public UserAuditResponse GetUserAudit(int userID, string auth) {
		if (_auth.TokenIsAdmin(auth)) {
			return new UserAuditResponse{
				user = _userRepo.GetUserByID(userID),
				audits = _userRepo.GetUserAudits(userID)
			};
		}

		else {
			throw new Exception("Not authorized");
		}
	}

	public void UpdateUserBio(int id, string? bio, string auth) {
		var user_to_edit = _userRepo.GetUserByID(id);
		if (user_to_edit == null) {
			throw new Exception("No user found with the given userID");
		}

		if (_auth.TokenIsUser(auth, id)) {
			// TODO: Pass ID to this, don't require getting a user first
			_userRepo.SetUserBio(user_to_edit, bio);
		}

		else {
			throw new Exception("Not Authorized");
		}
	}

	public void BanUser(int id, string? reason, string auth) {
		var user = _userRepo.GetUserByID(id);
		if (user == null) {
			throw new Exception("No user found with the given userID");
		}

		if (_auth.TokenIsAdmin(auth)) {
			_userRepo.BanUser(user, reason);
		}

		else {
			throw new Exception("Not authorized to ban user");
		}
	}

	public void UnbanUser(int id, string auth) {
		var user = _userRepo.GetUserByID(id);
		if (user == null) {
			throw new Exception("No user found with the given userID");
		}

		if (_auth.TokenIsAdmin(auth)) {
			_userRepo.UnbanUser(user);
		}

		else {
			// TODO: Separate out other exceptions
			throw new Exception("Not authorized to unban user");
		}
	}
}