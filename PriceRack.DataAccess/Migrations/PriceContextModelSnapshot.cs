using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PriceRack.DataAccess.DBContexts;

#nullable disable

namespace PriceRack.DataAccess.Migrations
{
    [DbContext(typeof(PriceContext))]
    partial class PriceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("PriceRack.DataAccess.Entities.Price", b =>
                {
                    b.Property<string>("Instrument")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Instrument", "Time");

                    b.ToTable("Prices");
                });
#pragma warning restore 612, 618
        }
    }
}
