using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class ForumContext : DbContext
{
	public DbSet<ForumThread> Threads { get; set; }
	public DbSet<ForumThreadAudit> ThreadAudits {get; set;}
	public DbSet<ForumPost> Posts { get; set; }
	public DbSet<ForumPostAudit> PostAudits {get; set;}
	public DbSet<ForumUser> Users { get; set; }
	public DbSet<ForumUserAudit> UserAudits {get; set;}

	public ForumContext(DbContextOptions<ForumContext> options) : base(options) {}
}

public enum threadAction {
	CREATE,
	EDIT
}

public class ForumThreadAudit {
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int auditID {get; set;}

	public DateTime date {get; set;}

	public ForumThread thread {get; set;}

	public threadAction action {get; set;}

	public string? info {get; set;}
}

public class ForumThread {
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int threadID {get; set;}

	public ForumUser author {get; set;}

	public DateTime date {get; set;}
	public DateTime? dateModified {get; set;}
	public bool edited {get; set;}

	public string? topic {get; set;}

	public ICollection<ForumPost> posts {get; set;}
}

public enum postAction {
	CREATE,
	EDIT
}

public class ForumPostAudit {
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int auditID {get; set;}

	public DateTime date {get; set;}

	public ForumPost post {get; set;}

	public postAction action {get; set;}

	public string? info {get; set;}
}

public class ForumPost {
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int postID {get; set;}

	[JsonIgnore]
	public ForumThread? thread {get; set;}

	public ForumUser? author {get; set;}

	public DateTime date {get; set;}
	public DateTime? dateModified {get; set;}
	public bool edited {get; set;}
	public string? text {get; set;}
}
	
public enum userState {
	BANNED = -2,
	DISABLED = -1,
	AWAIT_REG = 0,
	ACTIVE,
}

public enum userRole {
	USER,
	VIP,
	ADMIN,
	SYSOP
}

// TODO: Separate Account and Profile information
public class ForumUser {
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int userID {get; set;}

	public string? userName {get; set;}

	[JsonIgnore]
	public string? password {get; set;}

	[JsonIgnore]
	public string? email {get; set;}

	[JsonIgnore]
	public string? code {get; set;}

	public userState userState {get; set;}
	public userRole userRole {get; set;}

	public string? bio {get; set;}
}

public enum userAction {
	REGISTER,
	REGISTER_CONFIRM,
	PASS_FORGOT,
	PASS_RESET,
	BAN,
	UNBAN
}

public class ForumUserAudit {
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int auditID {get; set;}

	public DateTime date {get; set;}

	public ForumUser user {get; set;}

	public userAction action {get; set;}

	public string? info {get; set;}
}