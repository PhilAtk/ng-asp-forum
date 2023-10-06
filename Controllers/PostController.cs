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
			return BadRequest("No auth token provided");
		}

		try {
			var res = _post.GetPostAuditResponse(id, auth);
			return Ok(res);
		}
		catch (Exception e) {
			// TODO: Separate exceptions
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

		try {
			_post.DeletePost(id, auth);
			return Ok();
		}
		catch (Exception e) {
			// TODO: Separate exceptions
			_logger.LogError("Error deleting post: " + e.Message);
			return StatusCode(500);
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
			return BadRequest("No auth token provided");
		}

		try {
			_post.EditPost(id, data.text, auth);
			return Ok();
		}
		
		catch (Exception e) {
			// TODO: Separate exceptions
			_logger.LogError("Error updating post: " + e.Message);
			return StatusCode(500);
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
			return BadRequest("No auth token provided");
		}

		try {
			var post = _post.CreatePost(data.threadID, data.text, auth);

			var baseURL = Request.Scheme + "://" + Request.Host + '/';
			return Created(baseURL + "thread/" + post.thread?.threadID + "#" + post.postID, post);
		}
		catch (Exception e) {
			_logger.LogError(e.Message);
			return StatusCode(500);
		}
	}
}