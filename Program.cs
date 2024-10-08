using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GallifreyPlanetContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("GallifreyPlanetContext") ?? throw new InvalidOperationException("Connection string 'GallifreyPlanetContext' not found.")));
builder.Services.AddDefaultIdentity<User>(option => option.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<GallifreyPlanetContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
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

app.Run();
