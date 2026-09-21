using Microsoft.EntityFrameworkCore;
using BokApi.Models;

namespace BokApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Npgsql kräver UTC för timestamptz; API:t tar emot datum utan tidszon
            modelBuilder.Entity<Book>()
                .Property(b => b.PublishedDate)
                .HasColumnType("timestamp without time zone");
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Quote> Quotes { get; set; }

    }

}