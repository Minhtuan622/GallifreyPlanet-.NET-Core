using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GallifreyPlanet.Models;

namespace GallifreyPlanet.Data
{
    public class GallifreyPlanetContext : DbContext
    {
        public GallifreyPlanetContext (DbContextOptions<GallifreyPlanetContext> options)
            : base(options)
        {
        }

        public DbSet<GallifreyPlanet.Models.User> User { get; set; } = default!;
    }
}
