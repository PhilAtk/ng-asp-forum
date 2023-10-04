using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase {
	private readonly ILogger<PostController> _logger;
	private PostRepository _postRepo;
	private UserRepository _userRepo;
	private ThreadRepository _threadRepo;
	private ForumAuthenticator _auth;

	public PostController(ILogger<PostController> logger, PostRepository postRepo, UserRepository userRepo, ThreadRepository threadRepo, ForumAuthenticator auth) {
		_logger = logger;

		_postRepo = postRepo;
		_userRepo = userRepo;
		_threadRepo = threadRepo;

		_auth = auth;
	}

	public class PostAuditResponse {
		public ForumPost post {get; set;}
		public List<ForumPostAudit> audits {get;set;}
	}

	[HttpGet]
	[Route("audit/{id}")]
	public ActionResult<PostAuditResponse> GetPostAudit(int id) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		int viewerID;
		if (!_auth.VerifyBearerToken(auth, out viewerID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var viewer = _userRepo.GetUserByID(viewerID);
			if (viewer == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if (viewer.userRole >= userRole.ADMIN) {
				var post = _postRepo.GetPost(id);
				if (post == null) {
					return NotFound("No post found with the ID supplied for audit");
				}

				var audits = _postRepo.GetPostAudits(id);

				var res = new PostAuditResponse {
					post = post,
					audits = audits
				};

				return Ok(res);
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}	
	}

	[HttpDelete]
	[Route("{id}")]
	public IActionResult Delete(int id) {

		var auth = Request.Cookies["auth"];
		if (auth == null) {
			return BadRequest("No auth token provided");
		}

		int editorID;
		if (!_auth.VerifyBearerToken(auth, out editorID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var post = _postRepo.GetPost(id);
			if (post == null) {
				return NotFound("No post found with the given postID");
			}

			var editor = _userRepo.GetUserByID(editorID);
			if (editor == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if ((	editor.userID == post.author?.userID && editor.userState >= userState.ACTIVE) ||
				editor.userRole >= userRole.ADMIN) {
					_postRepo.DeletePost(post);
					return Ok();
			}
		}
		catch (Exception e) {
			_logger.LogError("Error deleting post: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}

	public class PostEditData {
		public string? text {get; set;}
	}

	[HttpPatch]
	[Route("{id}")]
	public IActionResult Edit(int id, PostEditData data) {

		if (data.text == null) {
			return BadRequest("No post text provided");
		}

		var auth = Request.Cookies["auth"];
		if (auth == null) {
			return BadRequest("No auth token provided");
		}

		int editorID;
		if (!_auth.VerifyBearerToken(auth, out editorID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var post = _postRepo.GetPost(id);
			if (post == null) {
				return NotFound("No post found with the given postID");
			}
			if (post.author == null) {
				return NotFound("No valid author found for the given post");
			}

			var editor = _userRepo.GetUserByID(editorID);
			if (editor == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if ((	editor.userID == post.author.userID && editor.userState >= userState.ACTIVE) ||
				editor.userRole >= userRole.ADMIN) {
					_postRepo.EditPost(post, data.text);
					return Ok();
			}
		}
		
		catch (Exception e) {
			_logger.LogError("Error updating post: " + e.Message);
			return StatusCode(500);
		}

		return BadRequest();
	}

	public class PostCreateData {
		public string? text {get; set;}
		public int threadID {get; set;}
	};

	[HttpPost]
	public IActionResult Post(PostCreateData data) {

		if (data.text == null) {
			return BadRequest("No post text provided");
		}

		var auth = Request.Cookies["auth"];

		if (auth == null) {
			return BadRequest("No auth token provided");
		}

		int authorID;
		if (!_auth.VerifyBearerToken(auth, out authorID)) {
			return Unauthorized();
		}

		try {
			var thread = _threadRepo.GetThreadByID(data.threadID);
			if (thread == null) {
				return NotFound("Thread doesn't exist");
			}

			var author = _userRepo.GetUserByID(authorID);
			if (author == null) {
				return NotFound("No user found with the provided userID");
			}

			if (author.userState >= userState.ACTIVE) {
				// TODO: Decouple a bit by having the repo look up the thread?
				var post = new ForumPost{
					date = DateTime.Now,
					author = author,
					text = data.text,
					thread = thread,
					edited = false
				};
				_postRepo.CreatePost(post);

				return Ok();
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}
}