using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase {
	private readonly ILogger<PostController> _logger;
	private ForumContext _db;
	private ForumAuthenticator _auth;

	public PostController(ILogger<PostController> logger, ForumContext db, ForumAuthenticator auth) {
		_logger = logger;
		_db = db;
		_auth = auth;
	}

	public class PostAuditResponse {
		public ForumPost post {get; set;}
		public List<ForumPostAudit> audits {get;set;}
	}

	[HttpGet]
	[Route("audit/{id}")]
	public ActionResult<PostAuditResponse> GetUserAudit(int id) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		int viewerID;
		if (!_auth.VerifyBearerToken(auth, out viewerID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var viewer = _db.Users.Where(u => u.userID == viewerID).First();
			if (viewer == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if (viewer.userRole >= userRole.ADMIN) {
				var post = _db.Posts
					.Where(p => p.postID == id)
					.First();
				if (post == null) {
					return NotFound("No post found with the ID supplied for audit");
				}

				var audits = _db.PostAudits
					.Where(a => a.post.postID == id)
					.OrderByDescending(a => a.date)
					.ToList();

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
			var post = _db.Posts
				.Where(p => p.postID == id)
				.Include(p => p.author)
				.First();
			if (post == null) {
				return NotFound("No post found with the given postID");
			}
			if (post.author == null) {
				return NotFound("No valid author found for the given post");
			}

			var editor = _db.Users.Where(u => u.userID == editorID).First();
			if (editor == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if ((	editor.userID == post.author.userID && editor.userState >= userState.ACTIVE) ||
				editor.userRole >= userRole.ADMIN) {
					_db.Remove(post);
					_db.SaveChanges();
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
			var post = _db.Posts
				.Where(p => p.postID == id)
				.Include(p => p.author)
				.First();
			if (post == null) {
				return NotFound("No post found with the given postID");
			}
			if (post.author == null) {
				return NotFound("No valid author found for the given post");
			}

			var editor = _db.Users.Where(u => u.userID == editorID).First();
			if (editor == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if ((	editor.userID == post.author.userID && editor.userState >= userState.ACTIVE) ||
				editor.userRole >= userRole.ADMIN) {
					var audit = new ForumPostAudit {
						date = DateTime.Now,
						post = post,
						action = postAction.EDIT,
						info = "Previous Text: '" + post.text +"'"
					};
					_db.Add(audit);

					post.edited = true;
					post.dateModified = DateTime.Now;
					post.text = data.text;
					_db.SaveChanges();
					return Ok();
			}
		}
		
		catch (Exception e) {
			_logger.LogError("Error updating post: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
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
			var thread = _db.Threads.Where(t => t.threadID == data.threadID).First();
			if (thread == null) {
				return NotFound("Thread doesn't exist");
			}

			var author = _db.Users.Where(u => u.userID == authorID).First();
			if (author == null) {
				return NotFound("No user found with the provided userID");
			}

			if (author.userState >= userState.ACTIVE) {
				var post = new ForumPost{
					date = DateTime.Now,
					author = author,
					text = data.text,
					thread = thread,
					edited = false
				};

				var audit = new ForumPostAudit {
					date = DateTime.Now,
					post = post,
					action = postAction.CREATE,
				};
				_db.Add(audit);

				_db.Add(post);
				_db.SaveChanges();

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