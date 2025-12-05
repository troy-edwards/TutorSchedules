using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use Heroku's PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "5253";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Add services to the container.
builder.Services.AddRazorPages();

// Get connection string - parse Heroku DATABASE_URL if in production
string? connectionString;
if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    // Heroku provides DATABASE_URL in format: postgres://user:password@host:port/database
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        var uri = new Uri(databaseUrl);
        var username = uri.UserInfo.Split(':')[0];
        var password = uri.UserInfo.Split(':')[1];
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
    else
    {
        throw new InvalidOperationException("DATABASE_URL environment variable is not set");
    }
}

builder.Services.AddDbContext<ScheduleContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.Run();