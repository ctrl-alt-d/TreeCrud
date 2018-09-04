using TreeCrud.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace TreeCrud.DataLayer.Data
{
    public class AligaContext : DbContext
    {
        public AligaContext(DbContextOptions<AligaContext> options) : base(options) { }

        public DbSet<Unitat> Unitats { get; set; }

    }
}
