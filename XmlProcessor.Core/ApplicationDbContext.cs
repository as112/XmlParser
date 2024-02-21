using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using XmlProcessor.Core.Models;

namespace XmlProcessor.Core
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;

        public ApplicationDbContext(string connectionString) : base()
        {
            _connectionString = connectionString;

            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        public DbSet<FinalData> Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FinalData>()
                .HasKey(k => k.ModuleCategoryID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
