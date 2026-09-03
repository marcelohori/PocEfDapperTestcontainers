namespace PocEfDapper.Application.Products.GetProductById;

public record ProductResponse(
    Guid Id,
    string Sku,
    string Name,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAtUtc
);