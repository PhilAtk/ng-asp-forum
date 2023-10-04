using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
	
namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThreadController : ControllerBase {
	private readonly ILogger<ThreadController> _logger;
	private ThreadRepository _threadRepo;
	private UserRepository _userRepo;
	private ForumAuthenticator _auth;

	public ThreadController(ILogger<ThreadController> logger, ThreadRepository threadRepo, UserRepository userRepo, ForumAuthenticator auth) {
		_logger = logger;

		_threadRepo = threadRepo;
		_userRepo = userRepo;

		_auth = auth;
	}

    	[HttpGet]
    	public ActionResult<IEnumerable<ForumThread>> GetThreadList() {
		try {
			var list = _threadRepo.GetThreadList();

			return Ok(list.ToArray());
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}

	public class ThreadAuditResponse {
		public ForumThread thread {get; set;}
		public List<ForumThreadAudit> audits {get;set;}
	}

	[HttpGet]
	[Route("audit/{id}")]
	public ActionResult<ThreadAuditResponse> GetThreadAudit(int id) {
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
				var thread = _threadRepo.GetThreadByID(id);
				if (thread == null) {
					return NotFound("No thread found with the ID supplied for audit");
				}

				var audits = _threadRepo.GetThreadAudits(id);

				var res = new ThreadAuditResponse {
					thread = thread,
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

	[HttpGet]
	[Route("{id}")]
	public ActionResult<ForumThread> GetThread(int id) {
		try {
			var thread = _threadRepo.GetThreadByID(id);
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

		int editorID;
		if (!_auth.VerifyBearerToken(auth, out editorID)) {
			return Unauthorized("Bearer token is not valid");
		}

		var editor = _userRepo.GetUserByID(editorID);
		if (editor == null) {
			return NotFound("No user found with the ID supplied by bearer token");
		}

		try {
			var thread = _threadRepo.GetThreadByID(id);
			if (thread == null) {
				return NotFound("No thread found with the given threadID");
			}
			if (thread.author == null) {
				return NotFound("No valid author for the given thread");
			}

			if ((	editor.userID == thread.author.userID && editor.userState >= userState.ACTIVE) ||
				editor.userRole >= userRole.ADMIN) {
					_threadRepo.DeleteThread(thread);
					return Ok();
			}
		}
		catch (Exception e) {
			_logger.LogError("Error deleting thread: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}

	public class ThreadEditData {
		public string? topic {get; set;}
	};

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

		int editorID;
		if (!_auth.VerifyBearerToken(auth, out editorID)) {
			return Unauthorized("Bearer token is not valid");
		}

		try {
			var thread = _threadRepo.GetThreadByID(id);
			if (thread == null) {
				return NotFound("No post found with the given postID");
			}
			if (thread.author == null) {
				return NotFound("No valid author for the given thread");
			}

			var editor = _userRepo.GetUserByID(editorID);
			if (editor == null) {
				return NotFound("No user found with the ID supplied by bearer token");
			}

			if ((	editor.userID == thread.author.userID && editor.userState >= userState.ACTIVE) ||
				editor.userRole >= userRole.ADMIN) {
					_threadRepo.EditThread(thread, data.topic);
					return Ok();
			}
		}
		catch (Exception e) {
			_logger.LogError("Error updating thread: " + e.Message);
			return StatusCode(500);
		}

		return Ok();
	}

	public class ThreadCreateData {
		public string? topic {get; set;}
		public string? text {get; set;}
	};

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

		int authorID;
		if (!_auth.VerifyBearerToken(auth, out authorID)) {
			return Unauthorized();
		}

		try {
			var author = _userRepo.GetUserByID(authorID);
			if (author == null) {
				return NotFound("No user found with the provided userID");
			}

			if (author.userState >= userState.ACTIVE) {
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

				// TODO: Move creation of thread/post objects to repo?
				_threadRepo.PostThread(thread);
				
				var baseURL = Request.Scheme + "://" + Request.Host + '/';
				return Created(baseURL + "thread/" + thread.threadID, thread);
			}

			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	} 
}