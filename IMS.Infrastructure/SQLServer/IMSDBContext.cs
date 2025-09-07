using IMS.Infrastructure.SQLServer.DAOs;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.SQLServer;

public class IMSDBContext(DbContextOptions<IMSDBContext> options) : DbContext(options)
{
    public DbSet<ProductDao> Products => Set<ProductDao>();
    public DbSet<CategoryDao> Categories => Set<CategoryDao>();
    public DbSet<ProductStatusDao> ProductStatuses => Set<ProductStatusDao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductStatusDao>()
            .Property(p => p.Id)
            .ValueGeneratedNever();

        base.OnModelCreating(modelBuilder);

        const int InStockId = 0;
        const int SoldId = 1;
        const int DamagedId = 2;
        modelBuilder.Entity<ProductStatusDao>().HasData(
            new ProductStatusDao { Id = InStockId, Name = "InStock" },
            new ProductStatusDao { Id = SoldId, Name = "Sold" },
            new ProductStatusDao { Id = DamagedId, Name = "Damaged" }
        );
    }

}