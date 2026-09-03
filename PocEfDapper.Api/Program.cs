using Microsoft.EntityFrameworkCore;
using PocEfDapper.Api.Endpoints;
using PocEfDapper.Application;
using PocEfDapper.Infrastructure;
using PocEfDapper.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Assegurar criação do schema (garante que o container de teste tenha as tabelas prontas)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapProductEndpoints();

app.Run();

// Necessário para o WebApplicationFactory nos testes de integração
public partial class Program { }