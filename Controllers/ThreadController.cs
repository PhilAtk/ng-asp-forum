using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThreadController : ControllerBase {
	private readonly ILogger<ThreadController> _logger;
	private ThreadService _thread;

	public ThreadController(ILogger<ThreadController> logger, ThreadService thread) {
		_logger = logger;

		_thread = thread;
	}

    	[HttpGet]
    	public ActionResult<IEnumerable<ThreadViewmodel>> GetThreadList() {
		try {
			var list = _thread.GetThreadList();
			return Ok(list);
		}

		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet]
	[Route("audit/{id}")]
	public ActionResult<ThreadAuditResponse> GetThreadAudit(int id) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return Unauthorized("No auth token provided");
		}

		try {
			var res = _thread.GetThreadAudit(id, auth);
			return Ok(res);
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet]
	[Route("{id}")]
	public ActionResult<ThreadResponse> GetThread(int id) {
		try {
			var res = _thread.GetThreadResponseByID(id);
			return res;
		}
		
		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpDelete]
	[Route("{id}")]
	public IActionResult DeleteThread(int id) {

		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return Unauthorized("No auth token provided");
		}

		try {
			_thread.DeleteThread(id, auth);
			return Ok();
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
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
			return Unauthorized("No auth token provided");
		}

		try {
			_thread.EditThread(id, data.topic, auth);
			return Ok();
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	public class ThreadCreateData {
		public string? topic {get; set;}
		public string? text {get; set;}
	};

	public class ThreadCreateResult {
		public int threadID {get; set;}
	}

	[HttpPost]
	public IActionResult PostThread(ThreadCreateData data) {

		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return Unauthorized("No auth token provided");
		}

		if (string.IsNullOrWhiteSpace(data.topic)) {
			return BadRequest("No topic provided");
		}

		if (string.IsNullOrWhiteSpace(data.text)) {
			return BadRequest("No post text provided");
		}

		try {
			int threadID = _thread.CreateThread(data.topic, data.text, auth);

			var baseURL = Request.Scheme + "://" + Request.Host + '/';
			// TODO: Actually return a thread object? Do in a clean way
			return Created(baseURL + "thread/" + threadID, new ThreadCreateResult{threadID = threadID});
		}

		catch (KeyNotFoundException e) {
			return NotFound();
		}
		catch (SecurityException e) {
			return Unauthorized();
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	} 
}