using GallifreyPlanet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Data;

public class GallifreyPlanetContext(DbContextOptions<GallifreyPlanetContext> options)
    : IdentityDbContext<User>(options: options)
{
    public DbSet<User> User { get; set; } = default!;
    public DbSet<LoginHistory> LoginHistory { get; set; } = default!;
    public DbSet<UserSession> UserSession { get; set; } = default!;
    public DbSet<Blog> Blog { get; set; } = default!;
    public DbSet<Friend> Friend { get; set; } = default!;
    public DbSet<Comment> Comment { get; set; } = default!;
    public DbSet<Notification> Notification { get; set; } = default!;
    public DbSet<Message> ChatMessage { get; set; } = default!;
    public DbSet<Conversation> Conversation { get; set; } = default!;
    public DbSet<Message> Message { get; set; } = default!;
}