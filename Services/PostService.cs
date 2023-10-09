public class PostService {
	private readonly ILogger<PostService> _logger;

	private PostRepository _postRepo;
	private UserRepository _userRepo;
	private ThreadRepository _threadRepo;

	private ForumAuthenticator _auth;

	public PostService(ILogger<PostService> logger, PostRepository postRepo, UserRepository userRepo, ThreadRepository threadRepo, ForumAuthenticator auth) {
		_logger = logger;

		_postRepo = postRepo;
		_userRepo = userRepo;
		_threadRepo = threadRepo;
		_auth = auth;
	}

	public PostAuditResponse GetPostAuditResponse(int postID, string auth) {
		if (_auth.TokenIsAdmin(auth)) {
			var auditsBackend = _postRepo.GetPostAudits(postID);

			List<PostAuditViewmodel> audits = new List<PostAuditViewmodel>();
			auditsBackend.ForEach(a => audits.Add(new PostAuditViewmodel(a)));

			return new PostAuditResponse{
				post = new PostViewmodel(_postRepo.GetPost(postID)),
				audits = audits
			};
		}

		else {
			throw new Exception("Not authorized");
		}
	}

	public PostViewmodel CreatePost(int threadID, string text, string auth) {
		int authorID;
		if (_auth.VerifyBearerToken(auth, out authorID)) {

			var author = _userRepo.GetUserByID(authorID);

			if (author.userState >= userState.ACTIVE) {
				var thread = _threadRepo.GetThreadByID(threadID);

				// TODO: Have the Repo method look up the thread?
				var post = new ForumPost{
					date = DateTime.Now,
					author = author,
					text = text,
					thread = thread,
					edited = false
				};
				_postRepo.CreatePost(post);
				return new PostViewmodel(post);
			}
		}

		throw new Exception("Not authorized");
	}

	public void DeletePost(int postID, string auth) {
		var post = _postRepo.GetPost(postID);

		if (_auth.TokenIsAdmin(auth) || _auth.TokenIsUser(auth, post.author.userID)) {
			// TODO: Make DeletePost in the repo just take postID?
			_postRepo.DeletePost(post);
		}

		else {
			throw new Exception("Not authorized");
		}
	}

	public void EditPost(int postID, string text, string auth) {
		var post = _postRepo.GetPost(postID);

		// TODO: Do this in a cleaner way where the token isn't parsed twice
		if (_auth.TokenIsAdmin(auth) || _auth.TokenIsUser(auth, post.author.userID)) {
			// TODO: Make EditPost in the repo just take postID?
			_postRepo.EditPost(post, text);
		}

		else {
			throw new Exception("Not authorized");
		}	
	}
}