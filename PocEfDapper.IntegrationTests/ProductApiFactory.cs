using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PocEfDapper.Application.Common;
using PocEfDapper.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace PocEfDapper.IntegrationTests;

public class ProductApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove o DbContext original
            var descriptorDbContext = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptorDbContext is not null) services.Remove(descriptorDbContext);

            // Remove a factory original do Dapper
            var descriptorDapper = services.SingleOrDefault(d =>
                d.ServiceType == typeof(ISqlConnectionFactory));
            if (descriptorDapper is not null) services.Remove(descriptorDapper);

            var connectionString = _dbContainer.GetConnectionString();

            // Registra apontando para o container dinâmico
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(connectionString));
        });
    }

    public async Task InitializeAsync()
    {
        // Sobe o container Docker do PostgreSQL antes dos testes rodarem
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        // Encerra e remove o container após os testes
        await _dbContainer.StopAsync();
    }
}