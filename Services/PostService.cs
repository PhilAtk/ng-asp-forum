using System.Security;

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
			var post = _postRepo.GetPost(postID);
			if (post == null) {
				throw new KeyNotFoundException();
			}

			var auditsBackend = _postRepo.GetPostAudits(postID);

			List<PostAuditViewmodel> audits = new List<PostAuditViewmodel>();
			auditsBackend.ForEach(a => audits.Add(new PostAuditViewmodel(a)));

			return new PostAuditResponse{
				post = new PostViewmodel(post),
				audits = audits
			};
		}

		else {
			throw new SecurityException("Not authorized");
		}
	}

	public PostViewmodel CreatePost(int threadID, string text, string auth) {
		int authorID;
		if (_auth.VerifyBearerToken(auth, out authorID)) {

			var author = _userRepo.GetUserByID(authorID);
			if (author.userState >= userState.ACTIVE) {

				var thread = _threadRepo.GetThreadByID(threadID);
				if (thread == null) {
					throw new KeyNotFoundException();
				}

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

		throw new SecurityException("Not authorized");
	}

	public void DeletePost(int postID, string auth) {
		var post = _postRepo.GetPost(postID);
		if (post == null) {
			throw new KeyNotFoundException();
		}

		if (_auth.TokenIsAdminOrUser(auth, post.author.userID)) {
			_postRepo.DeletePost(post);
		}

		else {
			throw new SecurityException("Not authorized");
		}
	}

	public void EditPost(int postID, string text, string auth) {
		var post = _postRepo.GetPost(postID);
		if (post == null){
			throw new KeyNotFoundException();
		}

		if (_auth.TokenIsAdminOrUser(auth, post.author.userID)) {
			_postRepo.EditPost(post, text);
		}

		else {
			throw new SecurityException("Not authorized");
		}	
	}
}