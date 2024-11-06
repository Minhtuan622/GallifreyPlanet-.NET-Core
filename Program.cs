using GallifreyPlanet.Data;
using GallifreyPlanet.Hubs;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args: args);
//var connectionString = builder.Configuration.GetConnectionString("DEV_TESTContext");
var connectionString = builder.Configuration.GetConnectionString("GallifreyPlanetContext");

builder.Services
    .AddDbContext<GallifreyPlanetContext>(optionsAction: options =>
        options.UseSqlServer(connectionString: connectionString)
    )
    .AddDefaultIdentity<User>(configureOptions: option => option.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<GallifreyPlanetContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<FriendService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ChatService>();

builder.Services.AddSignalR();

builder.Services.AddSession();

builder.Services.AddAuthentication()
    .AddFacebook(configureOptions: opt =>
    {
        opt.ClientId = builder.Configuration[key: "Authentication:Facebook:ClientId"]!;
        opt.ClientSecret = builder.Configuration[key: "Authentication:Facebook:ClientSecret"]!;
    })
    .AddGoogle(configureOptions: opt =>
    {
        opt.ClientId = builder.Configuration[key: "Authentication:Google:ClientId"]!;
        opt.ClientSecret = builder.Configuration[key: "Authentication:Google:ClientSecret"]!;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorHandlingPath: "/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapHub<ChatHub>(pattern: "/chatHub");
app.MapHub<NotificationHub>(pattern: "/notificationHub");
app.MapHub<CommentHub>(pattern: "/commentHub");

app.Run();