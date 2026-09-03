using ErrorOr;
using MediatR;
using PocEfDapper.Domain.Products;

namespace PocEfDapper.Application.Products.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ErrorOr<Guid>>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existing = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);
        if (existing is not null)
        {
            return ProductErrors.DuplicateSku;
        }

        var product = Product.Create(request.Sku, request.Name, request.Price, request.StockQuantity);

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}