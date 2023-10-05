public class LoginResult {
	public string? userName {get; set;}
	public int userID {get; set;}
	public int userRole {get; set;}
	public int userState {get; set;}
	public string? token {get; set;}
}

public class UserAuditResponse {
	public ForumUser user {get; set;}
	public List<ForumUserAudit> audits {get; set;}
}