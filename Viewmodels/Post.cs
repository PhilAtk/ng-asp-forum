public class PostViewmodel {
	public int postID {get; set;}
	public UserViewmodel author {get; set;}
	public DateTime date {get; set;}
	public DateTime? dateModified {get; set;}
	public bool edited {get; set;}
	public string? text {get; set;}

	public PostViewmodel(ForumPost post) {
		postID = post.postID;
		author = new UserViewmodel(post.author);
		date = post.date;
		dateModified = post.dateModified;
		edited = post.edited;
		text = post.text;
	}
}

public class PostAuditViewmodel {
	public int auditID {get; set;}
	public DateTime date {get; set;}
	public postAction action {get; set;}
	public string? info {get; set;}

	public PostAuditViewmodel(ForumPostAudit audit) {
		auditID = audit.auditID;
		date = audit.date;
		action = audit.action;
		info = audit.info;
	}
}

public class PostAuditResponse {
	public PostViewmodel post {get; set;}
	public List<PostAuditViewmodel> audits {get;set;}
}