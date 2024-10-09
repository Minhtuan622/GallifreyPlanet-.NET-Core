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

        public DbSet<GallifreyPlanet.Models.User> User { get; set; }
    }
}
