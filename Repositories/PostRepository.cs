
using Microsoft.EntityFrameworkCore;

public class PostRepository {
	private readonly ILogger<PostRepository> _logger;
	private ForumContext _db;

	public PostRepository(ILogger<PostRepository> logger, ForumContext db) {
		_logger = logger;
		_db = db;
	}

	// TODO: Require a threadID and look that up, instead of assuming post will have it set?
	public void CreatePost(ForumPost post) {
		var audit = new ForumPostAudit {
			date = DateTime.Now,
			post = post,
			action = postAction.CREATE,
		};
		_db.Add(audit);

		_db.Add(post);
		_db.SaveChanges();
	}

	public ForumPost GetPost(int id) {
		return _db.Posts
			.Where(p => p.postID == id)
			.Include(p => p.author)
			.First();
	}

	public void DeletePost(ForumPost post) {
		_db.Remove(post);
		_db.SaveChanges();
	}

	public void EditPost(ForumPost post, string newText) {
		var audit = new ForumPostAudit {
			date = DateTime.Now,
			post = post,
			action = postAction.EDIT,
			info = "Previous Text: '" + post.text +"'"
		};
		_db.Add(audit);

		post.edited = true;
		post.dateModified = DateTime.Now;
		post.text = newText;
		_db.SaveChanges();
	}

	public List<ForumPostAudit> GetPostAudits(int id) {
		// TODO: Make sure this lazy loads, don't want to load every post all at once
		return _db.PostAudits
			.Where(a => a.post.postID == id)
			.OrderByDescending(a => a.date)
			.ToList();
	}
}