using System.Security;

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

	public ThreadResponse GetThreadResponseByID(int threadID) {
		var thread = _threadRepo.GetThreadByID(threadID);
		if (thread == null) {
			throw new KeyNotFoundException();
		}
		var posts = new List<PostViewmodel>();

		var postsBackend = _postRepo.GetPostsByID(threadID);
		postsBackend.ForEach(p => posts.Add(new PostViewmodel(p)));

		var res = new ThreadResponse{
			thread = new ThreadViewmodel(thread),
			posts = posts
		};

		return res;
	}

	public ThreadAuditResponse GetThreadAudit(int threadID, string auth) {
		var thread = _threadRepo.GetThreadByID(threadID);
		if (thread == null) {
			throw new KeyNotFoundException();
		}

		if (_auth.TokenIsAdmin(auth)) {
			var auditsBackend = _threadRepo.GetThreadAudits(threadID);
			List<ThreadAuditViewmodel> audits = new List<ThreadAuditViewmodel>();

			auditsBackend.ForEach(a => audits.Add(new ThreadAuditViewmodel(a)));

			return new ThreadAuditResponse{
				thread = new ThreadViewmodel(thread),
				audits = audits
			};
		}

		else {
			throw new SecurityException("Not authorized");
		}
	}

	public int CreateThread(string topic, string text, string auth) {
		int authorID;
		if (_auth.VerifyBearerToken(auth, out authorID)) {
			var author = _userRepo.GetUserByID(authorID);
			if (author == null) {
				throw new KeyNotFoundException("No user found for the given authorization");
			}

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
		throw new SecurityException("Not Authorized");
	}

	public void DeleteThread(int threadID, string auth) {
		var thread = _threadRepo.GetThreadByID(threadID);
		if (thread == null) {
			throw new KeyNotFoundException();
		}

		if (_auth.TokenIsAdmin(auth) || _auth.TokenIsUser(auth, thread.author.userID)) {
			// TODO: Make DeleteThread in the repo just take threadID?
			_threadRepo.DeleteThread(thread);
		}

		else {
			throw new SecurityException("Not authorized");
		}
	}

	public void EditThread(int threadID, string topic, string auth) {
		var thread = _threadRepo.GetThreadByID(threadID);
		if (thread == null) {
			throw new KeyNotFoundException();
		}

		// TODO: Do this in a cleaner way where the token isn't parsed twice
		if (_auth.TokenIsAdmin(auth) || _auth.TokenIsUser(auth, thread.author.userID)) {
			// TODO: Make DeleteThread in the repo just take threadID?
			_threadRepo.EditThread(thread, topic);
		}

		else {
			throw new SecurityException("Not authorized");
		}	
	}
}