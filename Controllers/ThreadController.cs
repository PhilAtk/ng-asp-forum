using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
	
namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThreadController : ControllerBase {
	private readonly ILogger<ThreadController> _logger;
	private ForumContext _db;
	private ForumAuthenticator _auth;

	public ThreadController(ILogger<ThreadController> logger, ForumContext db, ForumAuthenticator auth) {
		_logger = logger;
		_db = db;
		_auth = auth;
	}

	public class ThreadCreateData {
		public string? topic {get; set;}
		public string? text {get; set;}
	};

	public class ThreadEditData {
		public string? topic {get; set;}
	};

    	[HttpGet]
    	public ActionResult<IEnumerable<ForumThread>> GetThreadList() {
		try {
			// TODO: Make sure this lazy loads, don't want to load every post all at once
			var list = _db.Threads
				.Include(t => t.posts)
				.Include(t => t.author)
				.OrderByDescending(t => t.posts.OrderByDescending(p => p.date).First());

			return Ok(list.ToArray());
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}

	[HttpGet]
	[Route("{id}")]
	public ActionResult<ForumThread> GetThread(int id) {
		try {
			var thread = _db.Threads
                		.Where(t => t.threadID == id)
				.Include(t => t.author)
				.Include(t => t.posts)
				.ThenInclude(p => p.author)
				.First();

			if (thread == null) {
				_logger.LogTrace("No thread found for ID: %d", id);
				return NotFound();
			}

			return thread;
		}
		
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}

	}

	[HttpDelete]
	[Route("{id}")]
	public IActionResult DeleteThread(int id) {

		var auth = Request.Cookies["auth"];

		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		try {
			var thread = _db.Threads.Where(t => t.threadID == id).First();

			if (thread == null) {
				return NotFound("No thread found with the given threadID");

			}

			if (thread.author == null) {
				return NotFound("No valid author for the given thread");
			}

			// TODO: Change verification to allow if admin/sysop
			int userID;
			if (!_auth.VerifyBearerToken(auth, out userID) || thread.author.userID != userID) {
				return Unauthorized();
			}

			// TODO: Change this to handle if admin/sysop is trying to delete
			if (thread.author.userState == userState.BANNED) {
				return Unauthorized();
			}

			// TODO: Keep in an audit log?
			// Add a "visible" property to posts?
			_db.Remove(thread);
			_db.SaveChanges();
		}
		catch (Exception e) {
			_logger.LogError("Error deleting thread: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}

	[HttpPatch]
	[Route("{id}")]
	public IActionResult UpdateThread(int id, ThreadEditData data) {
		if (data.topic == null) {
			return BadRequest("No topic text provided");
		}

		var auth = Request.Cookies["auth"];

		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		try {
			var thread = _db.Threads
				.Where(t => t.threadID == id)
				.Include(t => t.author)
				.First();

			if (thread == null) {
				return NotFound("No post found with the given postID");
			}

			if (thread.author == null) {
				return NotFound("No valid author for the given thread");
			}

			int userID;
			if (!_auth.VerifyBearerToken(auth, out userID) || thread.author.userID != userID) {
				return Unauthorized();
			}

			if (thread.author.userState == userState.BANNED) {
				return Unauthorized();
			}

			// TODO: Mark the post as edited
			// TODO: Add a timestamp for last edit
			thread.topic = data.topic;
			_db.SaveChanges();
		}
		catch (Exception e) {
			_logger.LogError("Error updating thread: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}

	[HttpPost]
	public IActionResult PostThread(ThreadCreateData data) {

		var auth = Request.Cookies["auth"];

		if (string.IsNullOrWhiteSpace(auth)) {
			return BadRequest("No auth token provided");
		}

		if (string.IsNullOrWhiteSpace(data.topic)) {
			return BadRequest("No topic provided");
		}

		if (string.IsNullOrWhiteSpace(data.text)) {
			return BadRequest("No post text provided");
		}

		int userID;
		if (!_auth.VerifyBearerToken(auth, out userID)) {
			return Unauthorized();
		}

		try {
			var author = _db.Users.Where(u => u.userID == userID).First();

			if (author == null) {
				return BadRequest("No user found with the provided userID");
			}

			var thread = new ForumThread{
				date = DateTime.Now,
				topic = data.topic,
				author = author,
				posts = new List<ForumPost>()
			};

			thread.posts.Add(new ForumPost{
				date = thread.date,
				text = data.text,
				author = author,
				thread = thread
			});

			_db.Add(thread);
			_db.SaveChanges();
			
			var baseURL = Request.Scheme + "://" + Request.Host + '/';

			return Created(baseURL + "thread/" + thread.threadID, thread);
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	} 
}