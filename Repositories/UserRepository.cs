// TODO: Pull out declarations into IUserRepository for Tests
public class UserRepository {
	private readonly ILogger<UserRepository> _logger;
	private ForumContext _db;

	public UserRepository(ILogger<UserRepository> logger, ForumContext db) {
		_logger = logger;
		_db = db;
	}

	public List<ForumUser> GetUsers() {
		return _db.Users.ToList();
	}

	public ForumUser GetUserByID(int id) {
		return _db.Users
			.Where(u => u.userID == id)
			.First();
	}

	public ForumUser? GetUserByUsername(string username) {
		try {
			return _db.Users
				.Where(u => u.userName == username)
				.First();
		}

		catch {
			// TODO: Something cleaner that explicitly says if there's nothing?
			return null;
		}		
	}

	public ForumUser GetUserByEmail(string email) {
		try {
			return _db.Users
				.Where(u => u.email == email)
				.First();
		}

		catch {
			// TODO: Something cleaner that explicitly says if there's nothing?
			return null;
		}
	}

	public ForumUser GetUserByCode(string code) {
		return _db.Users
			.Where(u => u.code == code)
			.First();
	}

	public List<ForumUserAudit> GetUserAudits(int id) {
		return _db.UserAudits
			.Where(a => a.user.userID == id)
			.OrderByDescending(a => a.date)
			.ToList();
	}

	public void RegisterUser(ForumUser user) {

		_db.Users.Add(user);

		var audit = new ForumUserAudit {
			date = DateTime.Now,
			user = user,
			action = userAction.REGISTER,
		};
		_db.UserAudits.Add(audit);

		_db.SaveChanges();
	}

	public void BanUser(ForumUser user, string? reason) {

		// TODO: Get user by ID, don't let them supply it
		var audit = new ForumUserAudit {
			date = DateTime.Now,
			user = user,
			action = userAction.BAN,
			info = "Reason: " + (string.IsNullOrWhiteSpace(reason) ? "NONE" : reason)
		};
		_db.Add(audit);
				
		user.userState = userState.BANNED;
		_db.SaveChanges();
	}

	public void UnbanUser(ForumUser user) {
		var audit = new ForumUserAudit {
			date = DateTime.Now,
			user = user,
			action = userAction.UNBAN,
		};
		_db.Add(audit);

		user.userState = userState.ACTIVE;
		_db.SaveChanges();
	}

	public void SetUserBio(ForumUser user, string? newBio) {
		user.bio = newBio;
		_db.SaveChanges();
	}

	public void SetUserCode(ForumUser user, string code) {

		var audit = new ForumUserAudit {
			date = DateTime.Now,
			user = user,
			action = userAction.PASS_FORGOT,
		};
		_db.UserAudits.Add(audit);

		user.code = code;

		_db.SaveChanges();
	}

	public void SetUserHashedPass(ForumUser user, string hashedPass) {
		
		user.password = hashedPass;
		user.code = null;

		var audit = new ForumUserAudit {
			date = DateTime.Now,
			user = user,
			action = userAction.PASS_RESET,
		};
		_db.UserAudits.Add(audit);

		_db.SaveChanges();
	}

	public void SetUserRegConfirmed(ForumUser user) {

		var audit = new ForumUserAudit {
			date = DateTime.Now,
			user = user,
			action = userAction.REGISTER_CONFIRM,
		};
		_db.UserAudits.Add(audit);

		user.userState = userState.ACTIVE;
		user.code = null;

		_db.SaveChanges();
	}
}