using Microsoft.EntityFrameworkCore;
using PriceRack.DataAccess.Entities;

namespace PriceRack.DataAccess.DBContexts
{
    public class PriceContext : DbContext
    {
        public PriceContext(DbContextOptions<PriceContext> options) : base(options) { }

        public DbSet<Price> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Price>().HasKey(p => new { p.Instrument, p.Time });
            modelBuilder.Entity<Price>().Property(p => p.Value).HasColumnType("decimal(18,2)");
        }
    }

}