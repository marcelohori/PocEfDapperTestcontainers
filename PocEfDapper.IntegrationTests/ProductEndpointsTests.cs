using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PocEfDapper.Api.Endpoints;
using PocEfDapper.Application.Products.GetProductById;
using Xunit;

namespace PocEfDapper.IntegrationTests;

public class ProductEndpointsTests : IClassFixture<ProductApiFactory>
{
    private readonly HttpClient _client;

    public ProductEndpointsTests(ProductApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_And_GetById_ShouldReturnExactProductFromDapper()
    {
        // Arrange
        var request = new CreateProductRequest(
            Sku: "DELL-XPS-15",
            Name: "Notebook Dell XPS 15",
            Price: 12500.50m,
            StockQuantity: 10
        );

        // Act 1: Criar Produto (Persistido via EF Core)
        var createResponse = await _client.PostAsJsonAsync("/api/products", request);

        // Assert 1
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdPayload = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        createdPayload.Should().NotBeNull();
        createdPayload!.Id.Should().NotBeEmpty();

        // Act 2: Consultar Produto (Lido via Dapper com SQL cru)
        var getResponse = await _client.GetAsync($"/api/products/{createdPayload.Id}");

        // Assert 2
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        product.Should().NotBeNull();
        product!.Id.Should().Be(createdPayload.Id);
        product.Sku.Should().Be("DELL-XPS-15");
        product.Name.Should().Be("Notebook Dell XPS 15");
        product.Price.Should().Be(12500.50m);
        product.StockQuantity.Should().Be(10);
    }

    [Fact]
    public async Task CreateProduct_WhenSkuAlreadyExists_ShouldReturn409Conflict()
    {
        // Arrange
        var request = new CreateProductRequest(
            Sku: "IPHONE-15-PRO",
            Name: "iPhone 15 Pro",
            Price: 8999.00m,
            StockQuantity: 5
        );

        // Primeiro cadastro
        var firstResponse = await _client.PostAsJsonAsync("/api/products", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act: Segundo cadastro com mesmo SKU
        var secondResponse = await _client.PostAsJsonAsync("/api/products", request);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private record CreatedResponse(Guid Id);
}