using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Data;
using StudySpeech.Models;
// This is the main entry point for the StudySpeech application. It configures services, middleware, and routes for the application.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// The SQLite file's folder must exist before the provider can open it (it won't create missing directories itself).
var sqliteDataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
var sqliteDirectory = Path.GetDirectoryName(sqliteDataSource);
if (!string.IsNullOrEmpty(sqliteDirectory))
{
    Directory.CreateDirectory(sqliteDirectory);
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Add services for identity management and authentication.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AzureSpeechService>();

//
var app = builder.Build();

// Seed the database with sample notes if they do not already exist.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    SeedData.EnsureSampleNotes(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use HTTPS redirection and routing middleware.
app.UseHttpsRedirection();
app.UseRouting();

// Use authentication and authorization middleware to protect routes and resources.
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map Razor Pages routes for identity management and other pages.
app.MapRazorPages()
   .WithStaticAssets();

// Run the application and start listening for incoming HTTP requests.
app.Run();
