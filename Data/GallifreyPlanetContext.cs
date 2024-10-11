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

        public DbSet<User> User { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
    }
}
