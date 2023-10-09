public class LoginResult {
	public string? userName {get; set;}
	public int userID {get; set;}
	public int userRole {get; set;}
	public int userState {get; set;}
	public string? token {get; set;}
}

/// <summary>
/// Frontend-safe response object for user audits.
/// 
/// ForumUser is ommitted since user is separately passed in the parent object of this data.
/// </summary>
public class UserAuditViewmodel {
	public int auditID {get; set;}
	public DateTime date {get; set;}
	public userAction action {get; set;}
	public string? info {get; set;}

	public UserAuditViewmodel(ForumUserAudit audit) {
		auditID = audit.auditID;
		date = audit.date;
		action = audit.action;
		info = audit.info;
	}
}

/// <summary>
/// Frontend-safe reponse object for User information
/// </summary>
public class UserViewmodel {
	public int userID {get; set;}
	public string? userName {get; set;}
	public userState userState {get; set;}
	public userRole userRole {get; set;}
	public string? bio {get; set;}

	public UserViewmodel(ForumUser user) {
		userID = user.userID;
		userName = user.userName;
		userState = user.userState;
		userRole = user.userRole;
		bio = user.bio;
	}
}

public class UserAuditResponse {
	public UserViewmodel user {get; set;}
	public List<UserAuditViewmodel> audits {get; set;}
}

