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

	public class PostCreateData {
		public string? text {get; set;}
		public int threadID {get; set;}
	};

	public class PostEditData {
		public string? text {get; set;}
	}

	[HttpDelete]
	[Route("{id}")]
	public IActionResult Delete(int id) {

		var auth = Request.Cookies["auth"];

		if (auth == null) {
			return BadRequest("No auth token provided");
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

			// TODO: Change verification to allow if admin/sysop
			int userID;
			if (!_auth.VerifyBearerToken(auth, out userID) || post.author.userID != userID) {
				return Unauthorized();
			}

			// TODO: Change this to handle if admin/sysop is trying to delete
			if (post.author.userState == userState.BANNED) {
				return Unauthorized();
			}

			// TODO: Keep in an audit log?
			// Add a "visible" property to posts?
			_db.Remove(post);
			_db.SaveChanges();
		}
		catch (Exception e) {
			_logger.LogError("Error deleting post: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
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

			int userID;
			if (!_auth.VerifyBearerToken(auth, out userID) || post.author.userID != userID) {
				return Unauthorized();
			}

			if (post.author.userState == userState.BANNED) {
				return Unauthorized();
			}

			// TODO: Mark the post as edited
			// TODO: Add a timestamp for last edit
			post.text = data.text;
			_db.SaveChanges();
		}
		
		catch (Exception e) {
			_logger.LogError("Error updating post: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}

	[HttpPost]
	public IActionResult Post(PostCreateData data) {

		if (data.text == null) {
			return BadRequest("No post text provided");
		}

		var auth = Request.Cookies["auth"];

		if (auth == null) {
			return BadRequest("No auth token provided");
		}

		int userID;
		if (!_auth.VerifyBearerToken(auth, out userID)) {
			return Unauthorized();
		}

		try {
			var thread = _db.Threads.Where(t => t.threadID == data.threadID).First();

			if (thread == null) {
				return BadRequest("Thread doesn't exist");
			}

			var author = _db.Users.Where(u => u.userID == userID).First();

			if (author == null) {
				return BadRequest("No user found with the provided userID");
			}

			var post = new ForumPost{
				date = DateTime.Now,
				author = author,
				text = data.text,
				thread = thread
			};

			_db.Add(post);
			_db.SaveChanges();

			return Ok();
		}

		catch (Exception e) {
			Console.WriteLine(e.Message);
			return StatusCode(500);
		}
	}
}