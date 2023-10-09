public class ThreadService {
	private readonly ILogger<ThreadService> _logger;
	private ThreadRepository _threadRepo;
	private UserRepository _userRepo;
	private PostRepository _postRepo;
	private ForumAuthenticator _auth;

	public ThreadService(ILogger<ThreadService> logger, ThreadRepository threadRepo, UserRepository userRepo, PostRepository postRepo, ForumAuthenticator auth) {
		_logger = logger;

		_threadRepo = threadRepo;
		_userRepo = userRepo;
		_postRepo = postRepo;

		_auth = auth;
	}

	public List<ThreadViewmodel> GetThreadList() {
		var threadsBackend = _threadRepo.GetThreadList();
		List<ThreadViewmodel> threads = new List<ThreadViewmodel>();

		threadsBackend.ForEach(t => threads.Add(new ThreadViewmodel(t)));

		return threads;
	}

	public ForumThread GetThreadByID(int threadID) {
		return _threadRepo.GetThreadByID(threadID);
	}

	public ThreadAuditResponse GetThreadAudit(int threadID, string auth) {
		if (_auth.TokenIsAdmin(auth)) {
			var auditsBackend = _threadRepo.GetThreadAudits(threadID);
			List<ThreadAuditViewmodel> audits = new List<ThreadAuditViewmodel>();

			auditsBackend.ForEach(a => audits.Add(new ThreadAuditViewmodel(a)));

			return new ThreadAuditResponse{
				thread = new ThreadViewmodel(_threadRepo.GetThreadByID(threadID)),
				audits = audits
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

				// TODO: Move creation of thread/post objects to repo?
				_threadRepo.PostThread(thread);

				var op = new ForumPost{
					date = thread.date,
					text = text,
					author = author,
					thread = thread
				};

				_postRepo.CreatePost(op);
				return thread.threadID;
			}
		}

		// Should also cover falling out of the above if user is not active
		throw new Exception("Not Authorized");
	}

	public void DeleteThread(int threadID, string auth) {
		var thread = _threadRepo.GetThreadByID(threadID);

		if (_auth.TokenIsAdmin(auth) || _auth.TokenIsUser(auth, thread.author.userID)) {
			// TODO: Make DeleteThread in the repo just take threadID?
			_threadRepo.DeleteThread(thread);
		}

		else {
			throw new Exception("Not authorized");
		}
	}

	public void EditThread(int threadID, string topic, string auth) {
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