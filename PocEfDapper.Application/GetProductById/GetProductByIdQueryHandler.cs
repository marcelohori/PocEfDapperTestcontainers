using Dapper;
using ErrorOr;
using MediatR;
using PocEfDapper.Application.Common;
using PocEfDapper.Domain.Products;

namespace PocEfDapper.Application.Products.GetProductById;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                id AS Id, 
                sku AS Sku, 
                name AS Name, 
                price AS Price, 
                stock_quantity AS StockQuantity, 
                created_at_utc AS CreatedAtUtc
            FROM products
            WHERE id = @Id;
            """;

        var product = await connection.QuerySingleOrDefaultAsync<ProductResponse>(
            new CommandDefinition(sql, new { request.Id }, cancellationToken: cancellationToken)
        );

        if (product is null)
        {
            return ProductErrors.NotFound;
        }

        return product;
    }
}