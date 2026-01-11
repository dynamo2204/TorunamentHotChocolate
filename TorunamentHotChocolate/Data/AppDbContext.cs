using Microsoft.EntityFrameworkCore;
using TorunamentHotChocolate.Models;

namespace TorunamentHotChocolate.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Bracket> Brackets { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tournament>()
                .HasMany(t => t.Participants)
                .WithMany(u => u.Tournaments);
        }
    }
}
