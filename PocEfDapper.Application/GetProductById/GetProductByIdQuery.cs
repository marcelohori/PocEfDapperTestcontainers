using ErrorOr;
using MediatR;

namespace PocEfDapper.Application.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ErrorOr<ProductResponse>>;