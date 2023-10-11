using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asptest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase {
	private readonly ILogger<PostController> _logger;
	private PostService _post;

	public PostController(ILogger<PostController> logger, PostService post) {
		_logger = logger;

		_post = post;
	}

	[HttpGet]
	[Route("audit/{id}")]
	public ActionResult<PostAuditResponse> GetPostAudit(int id) {
		var auth = Request.Cookies["auth"];
		if (string.IsNullOrWhiteSpace(auth)) {
			return Unauthorized("No auth token provided");
		}

		try {
			var res = _post.GetPostAuditResponse(id, auth);
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

	[HttpDelete]
	[Route("{id}")]
	public IActionResult Delete(int id) {

		var auth = Request.Cookies["auth"];
		if (auth == null) {
			return Unauthorized("No auth token provided");
		}

		try {
			_post.DeletePost(id, auth);
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
			return Unauthorized("No auth token provided");
		}

		try {
			_post.EditPost(id, data.text, auth);
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
			return Unauthorized("No auth token provided");
		}

		try {
			var post = _post.CreatePost(data.threadID, data.text, auth);

			var baseURL = Request.Scheme + "://" + Request.Host + '/';
			return Created(baseURL + "thread/" + data.threadID + "#" + post.postID, post);
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