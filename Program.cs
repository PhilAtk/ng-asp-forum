using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<ForumContext>();
builder.Services.AddTransient<ForumAuthenticator>();
builder.Services.AddTransient<ForumEmail>();

var app = builder.Build();

// Make sure the database is created and updated
using (var scope = app.Services.CreateScope())
{
    var forumContext = scope.ServiceProvider.GetRequiredService<ForumContext>();
    forumContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

else {
	app.UseSwagger();
	app.UseSwaggerUI();
	Console.WriteLine("We're dev");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
