using Microsoft.EntityFrameworkCore;
using PocEfDapper.Domain.Products;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PocEfDapper.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(builder =>
        {
            builder.ToTable("products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.Sku).HasColumnName("sku").HasMaxLength(50).IsRequired();
            builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            builder.Property(p => p.Price).HasColumnName("price").HasPrecision(18, 2);
            builder.Property(p => p.StockQuantity).HasColumnName("stock_quantity");
            builder.Property(p => p.CreatedAtUtc).HasColumnName("created_at_utc");

            builder.HasIndex(p => p.Sku).IsUnique();
        });
    }
}