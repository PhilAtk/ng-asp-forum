public class ThreadService {
	private readonly ILogger<ThreadService> _logger;
	private ThreadRepository _threadRepo;
	private UserRepository _userRepo;
	private ForumAuthenticator _auth;

	public ThreadService(ILogger<ThreadService> logger, ThreadRepository threadRepo, UserRepository userRepo, ForumAuthenticator auth) {
		_logger = logger;

		_threadRepo = threadRepo;
		_userRepo = userRepo;

		_auth = auth;
	}

	public List<ForumThread> GetThreadList() {
		return _threadRepo.GetThreadList();
	}

	public ForumThread GetThreadByID(int threadID) {
		return _threadRepo.GetThreadByID(threadID);
	}

	public ThreadAuditResponse GetThreadAudit(int threadID, string auth) {
		if (_auth.TokenIsAdmin(auth)) {
			return new ThreadAuditResponse{
				thread = _threadRepo.GetThreadByID(threadID),
				audits = _threadRepo.GetThreadAudits(threadID)
			};
		}

		else {
			throw new Exception("Not authorized");
		}
	}

	public int CreateThread(string topic, string text, string auth) {
		int authorID;
		if (_auth.VerifyBearerToken(auth, out authorID)) {
			var author = _userRepo.GetUserByID(authorID);

			if (author.userState >= userState.ACTIVE) {
				var thread = new ForumThread{
					date = DateTime.Now,
					topic = topic,
					author = author,
					posts = new List<ForumPost>()
				};

				thread.posts.Add(new ForumPost{
					date = thread.date,
					text = text,
					author = author,
					thread = thread
				});

				// TODO: Move creation of thread/post objects to repo?
				_threadRepo.PostThread(thread);
				return thread.threadID;
			}
		}

		// Should also cover falling out of the above if user is not active
		throw new Exception("Not Authorized");
	}

	public void DeleteThread(int threadID, string auth) {
		var thread = GetThreadByID(threadID);

		if (_auth.TokenIsAdmin(auth) || _auth.TokenIsUser(auth, thread.author.userID)) {
			// TODO: Make DeleteThread in the repo just take threadID?
			_threadRepo.DeleteThread(thread);
		}

		else {
			throw new Exception("Not authorized");
		}
	}

	public void UpdateThread(int threadID, string topic, string auth) {
		var thread = GetThreadByID(threadID);

		// TODO: Do this in a cleaner way where the token isn't parsed twice
		if (_auth.TokenIsAdmin(auth) || _auth.TokenIsUser(auth, thread.author.userID)) {
			// TODO: Make DeleteThread in the repo just take threadID?
			_threadRepo.EditThread(GetThreadByID(threadID), topic);
		}

		else {
			throw new Exception("Not authorized");
		}	
	}
}