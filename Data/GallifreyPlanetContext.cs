using GallifreyPlanet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Data
{
    public class GallifreyPlanetContext : IdentityDbContext<User>
    {
        public GallifreyPlanetContext(DbContextOptions<GallifreyPlanetContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; } = default!;
        public DbSet<LoginHistory> LoginHistory { get; set; } = default!;
        public DbSet<UserSession> UserSession { get; set; } = default!;
        public DbSet<Blog> Blog { get; set; } = default!;
        public DbSet<Friend> Friend { get; set; } = default!;
        public DbSet<Comment> Comment { get; set; } = default!;
        public DbSet<Notification> Notification { get; set; } = default!;
        public DbSet<MessageViewModel> ChatMessage { get; set; } = default!;
    }
}
