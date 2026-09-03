using ErrorOr;

namespace PocEfDapper.Domain.Products;

public static class ProductErrors
{
    public static Error DuplicateSku => Error.Conflict(
        code: "Product.DuplicateSku",
        description: "Já existe um produto cadastrado com este SKU.");

    public static Error NotFound => Error.NotFound(
        code: "Product.NotFound",
        description: "Produto não encontrado.");
}