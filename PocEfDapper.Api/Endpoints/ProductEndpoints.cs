using MediatR;
using PocEfDapper.Api.Common;
using PocEfDapper.Application.Products.CreateProduct;
using PocEfDapper.Application.Products.GetProductById;

namespace PocEfDapper.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products").WithTags("Products");

        // Write: EF Core via MediatR Command
        group.MapPost("/", async (CreateProductRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateProductCommand(request.Sku, request.Name, request.Price, request.StockQuantity);
            var result = await sender.Send(command, ct);

            return result.Match(
                id => TypedResults.Created($"/api/products/{id}", new { Id = id }),
                errors => CustomResults.Problem(errors)
            );
        })
        .WithName("CreateProduct")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // Read: Dapper via MediatR Query
        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var query = new GetProductByIdQuery(id);
            var result = await sender.Send(query, ct);

            return result.Match(
                product => TypedResults.Ok(product),
                errors => CustomResults.Problem(errors)
            );
        })
        .WithName("GetProductById")
        .Produces<ProductResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

public record CreateProductRequest(string Sku, string Name, decimal Price, int StockQuantity);