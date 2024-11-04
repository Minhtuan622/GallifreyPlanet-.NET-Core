using GallifreyPlanet.Data;
using GallifreyPlanet.Hubs;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string context = builder.Configuration.GetConnectionString(name: "GallifreyPlanetContext")
                 ?? throw new InvalidOperationException(message: "Connection string 'GallifreyPlanetContext' not found.");
builder.Services
    .AddDbContext<GallifreyPlanetContext>(options => options.UseSqlServer(context));
builder.Services
    .AddDefaultIdentity<User>(option => option.SignIn.RequireConfirmedAccount = true)
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
    .AddFacebook(opt =>
    {
        opt.ClientId = builder.Configuration[key: "Authentication:Facebook:ClientId"]!;
        opt.ClientSecret = builder.Configuration[key: "Authentication:Facebook:ClientSecret"]!;
    })
    .AddGoogle(opt =>
    {
        opt.ClientId = builder.Configuration[key: "Authentication:Google:ClientId"]!;
        opt.ClientSecret = builder.Configuration[key: "Authentication:Google:ClientSecret"]!;
    });

WebApplication app = builder.Build();

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

app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
