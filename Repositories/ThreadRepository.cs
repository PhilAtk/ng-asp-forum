public class ThreadRepository {
	private readonly ILogger<ThreadRepository> _logger;
	private ForumContext _db;

	public ThreadRepository(ILogger<ThreadRepository> logger, ForumContext db) {
		_logger = logger;
		_db = db;
	}

	public ForumThread GetThreadByID(int threadID) {
		return _db.Threads
			.Where(t => t.threadID == threadID)
			.First();
	}
}