namespace PocEfDapper.Domain.Products;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Product() { } // Construtor para o EF Core

    public static Product Create(string sku, string name, decimal price, int stockQuantity)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Sku = sku.ToUpperInvariant(),
            Name = name,
            Price = price,
            StockQuantity = stockQuantity,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}