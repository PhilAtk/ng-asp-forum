using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class ForumContext : DbContext
{
	public DbSet<ForumThread> Threads { get; set; }
	public DbSet<ForumPost> Posts { get; set; }
	public DbSet<ForumUser> Users { get; set; }

	public string DbPath { get; }

	public ForumContext()
	{
		var folder = Environment.SpecialFolder.LocalApplicationData;
		var path = Environment.GetFolderPath(folder);
		DbPath = Path.Join(path, "forum.db");
	}

	// TODO: Add MySQL option
	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={DbPath}");
}

public class ForumThread {
	[Key]
	public int threadID {get; set;}

	public ForumUser author {get; set;}

	public DateTime date {get; set;}
	public DateTime? dateModified {get; set;}
	public bool edited {get; set;}

	public string? topic {get; set;}

	public ICollection<ForumPost> posts {get; set;}
}

public class ForumPost {
	[Key]
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
	[Key]
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