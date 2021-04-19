using Microsoft.EntityFrameworkCore;

namespace Prophet.Logging
{
    public class ProphetContext : DbContext
    {
        public ProphetContext(DbContextOptions<ProphetContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Logg> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Logg>().OwnsOne(x => x.Error);
        }
    }
}