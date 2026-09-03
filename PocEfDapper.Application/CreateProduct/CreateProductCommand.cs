using ErrorOr;
using MediatR;

namespace PocEfDapper.Application.Products.CreateProduct;

public record CreateProductCommand(
    string Sku,
    string Name,
    decimal Price,
    int StockQuantity
) : IRequest<ErrorOr<Guid>>;