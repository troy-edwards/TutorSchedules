using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ScheduleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(
        builder.Environment.IsDevelopment() ? 
        "DefaultConnection" : 
        Environment.GetEnvironmentVariable("SQLAZURECONNSTR_AzureDbConnection")
        )));


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
app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.Run();