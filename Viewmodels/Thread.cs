public class ThreadViewmodel {
	public int threadID {get; set;}
	public UserViewmodel author {get; set;}
	public DateTime date {get; set;}
	public DateTime? dateModified {get; set;}
	public bool edited {get; set;}
	public string? topic {get; set;}
	public int numPosts {get; set;}

	public ThreadViewmodel(ForumThread thread) {
		threadID = thread.threadID;
		author = new UserViewmodel(thread.author);
		date = thread.date;
		dateModified = thread.dateModified;
		edited = thread.edited;
		topic = thread.topic;
		numPosts = thread.posts.Count;
	}
}

public class ThreadResponse {
	public ThreadViewmodel thread {get; set;}
	public List<PostViewmodel> posts {get; set;}
}

public class ThreadAuditViewmodel {
	public int auditID {get; set;}
	public DateTime date {get; set;}
	public threadAction action {get; set;}
	public string? info {get; set;}

	public ThreadAuditViewmodel(ForumThreadAudit audit) {
		auditID = audit.auditID;
		date = audit.date;
		action = audit.action;
		info = audit.info;
	}
}

public class ThreadAuditResponse {
	public ThreadViewmodel thread {get; set;}
	public List<ThreadAuditViewmodel> audits {get;set;}
}