using Microsoft.EntityFrameworkCore;

public class ThreadRepository {
	private readonly ILogger<ThreadRepository> _logger;
	private ForumContext _db;

	public ThreadRepository(ILogger<ThreadRepository> logger, ForumContext db) {
		_logger = logger;
		_db = db;
	}

	public List<ForumThread> GetThreadList() {
		return _db.Threads
			.Include(t => t.author)
			.Include(t => t.posts)
			.OrderByDescending(t => t.posts.OrderByDescending(p => p.date).First())
			.ToList();
	}

	public ForumThread GetThreadByID(int threadID) {
		return _db.Threads
			.Where(t => t.threadID == threadID)
			.Include(t => t.author)
			.First();
	}

	public List<ForumThreadAudit> GetThreadAudits(int id) {
		return _db.ThreadAudits
			.Where(a => a.thread.threadID == id)
			.OrderByDescending(a => a.date)
			.ToList();
	}

	// TODO: Take threadID instead of a thread object for the following methods?

	public void PostThread(ForumThread thread) {
		var audit = new ForumThreadAudit {
			date = DateTime.Now,
			thread = thread,
			action = threadAction.CREATE,
		};
		_db.Add(audit);

		_db.Add(thread);
		_db.SaveChanges();
	}

	public void EditThread(ForumThread thread, string newTopic) {
		var audit = new ForumThreadAudit {
			date = DateTime.Now,
			thread = thread,
			action = threadAction.EDIT,
			info = "Previous Topic: '" + thread.topic +"'"
		};
		_db.Add(audit);

		thread.edited = true;
		thread.dateModified = DateTime.Now;
		thread.topic = newTopic;

		_db.SaveChanges();
	}

	public void DeleteThread(ForumThread thread) {
		_db.Remove(thread);
		_db.SaveChanges();
	}
}