
# POC 02: Hybrid Persistence with EF Core, Dapper & Testcontainers (.NET 8)

[🇧🇷 Versão em Português](#-versão-em-português) | [🇺🇸 English Version](#-english-version)

---

## 🇧🇷 Versão em Português

### Sobre o Projeto
Esta Prova de Conceito (PoC) demonstra a implementação de uma arquitetura híbrida de persistência e acesso a dados com **.NET 8**, combinando o melhor de dois mundos:
1. **EF Core** no fluxo de escrita (Commands via CQRS) para garantir integridade transacional, encapsulamento de regras de domínio e mapeamento de entidades.
2. **Dapper** no fluxo de leitura (Queries via CQRS) para consultas de alto desempenho executadas via SQL nativo, sem o custo de alocação e processamento de *change tracking*.
3. **Testcontainers for .NET** para testes de integração end-to-end com uma instância real do **PostgreSQL** em container Docker, eliminando bancos em memória (In-Memory/SQLite) e *mocks* frágeis.

### Tecnologias e Padrões Utilizados
* **.NET 8 SDK** (C# 12)
* **ASP.NET Core Minimal APIs** com rotas mapeadas por extensão
* **EF Core (Npgsql)** para modelagem relacional e comandos de escrita
* **Dapper** para queries de leitura com mapeamento zero-tracking de alta velocidade
* **CQRS com MediatR** desacoplando Command Stack e Query Stack
* **Result Pattern (`ErrorOr`)** para controle de fluxo funcional e sem exceções
* **Testcontainers (PostgreSQL)** + **xUnit** + **FluentAssertions** para testes de integração reais
* **RFC 7807 (ProblemDetails)** para padronização de respostas de erro HTTP

---

### Estrutura da Solução

```text
PocEfDapperTestcontainers/
├── src/
│   ├── PocEfDapper.Domain/             # Entidades, regras de negócio e interfaces de escrita
│   ├── PocEfDapper.Application/        # Commands (EF Core), Queries (Dapper) e CQRS Handlers
│   ├── PocEfDapper.Infrastructure/     # DbContext EF, Dapper Connection Factory e Repositórios
│   └── PocEfDapper.Api/                # Endpoints Minimal API e configurações
├── tests/
│   └── PocEfDapper.IntegrationTests/   # Testes com Testcontainers (Postgres real em Docker)
└── PocEfDapperTestcontainers.sln
Como Executar
Pré-requisitos
.NET 8 SDK instalado.

Docker Desktop ou Docker Engine em execução (obrigatório para os testes via Testcontainers).

Executando a API Localmente
Certifique-se de que possui uma instância de PostgreSQL em execução localmente (ou use a connection string padrão no appsettings.json):

Bash
docker run --name poc-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=poc_ef_dapper -p 5432:5432 -d postgres:16-alpine
Compile e execute a API:

Bash
dotnet restore
dotnet build
dotnet run --project PocEfDapper.Api/PocEfDapper.Api.csproj
Acesse a documentação Swagger interativa:

Plaintext
https://localhost:<porta>/swagger
Executando os Testes de Integração (Testcontainers)
Com o Docker em execução, rode no terminal:

Bash
dotnet test
O Testcontainers inicializará automaticamente um container temporário do postgres:16-alpine, criará o schema, rodará os testes de integração HTTP e destruirá o container ao finalizar.

Cenários de Teste
1. Criação de Produto (Persistência via EF Core)
Endpoint: POST /api/products

Payload:

JSON
{
  "sku": "DELL-XPS-15",
  "name": "Notebook Dell XPS 15",
  "price": 12500.50,
  "stockQuantity": 10
}
Resposta Esperada: 201 Created contendo o GUID gerado:

JSON
{
  "id": "a90b4d45-6677-4c07-b2f7-ecde67645166"
}
2. Consulta de Produto (Leitura Otimizada via Dapper)
Endpoint: GET /api/products/{id}

Resposta Esperada: 200 OK

JSON
{
  "id": "a90b4d45-6677-4c07-b2f7-ecde67645166",
  "sku": "DELL-XPS-15",
  "name": "Notebook Dell XPS 15",
  "price": 12500.50,
  "stockQuantity": 10,
  "createdAtUtc": "2026-09-03T04:18:39Z"
}
3. Regra de Conflito de SKU Duplicado (RFC 7807)
Reenvie o mesmo payload de cadastro com o SKU DELL-XPS-15.

Resposta Esperada: 409 Conflict

JSON
{
  "type": "[https://tools.ietf.org/html/rfc9110#section-15.5.10](https://tools.ietf.org/html/rfc9110#section-15.5.10)",
  "title": "Product.DuplicateSku",
  "status": 409,
  "detail": "Já existe um produto cadastrado com este SKU."
}
🇺🇸 English Version
About the Project
This Proof of Concept (PoC) illustrates the implementation of a hybrid persistence and data-access architecture using .NET 8, leveraging the strengths of two distinct data access tools:

EF Core on the write side (CQRS Commands) to ensure aggregate encapsulation, transaction boundary safety, and entity lifecycle tracking.

Dapper on the read side (CQRS Queries) for low-latency, raw SQL queries without the memory overhead of entity change tracking.

Testcontainers for .NET for reliable integration tests against a real PostgreSQL Docker container, avoiding brittle mocks and inaccurate in-memory database providers.

Tech Stack & Architectural Patterns
.NET 8 SDK (C# 12)

ASP.NET Core Minimal APIs with route extension groupings

EF Core (Npgsql) for domain entity persistence and write commands

Dapper for high-throughput, read-only SQL queries

CQRS with MediatR separating Write and Read pipelines

Result Pattern (ErrorOr) for functional and predictable domain error handling

Testcontainers (PostgreSQL) + xUnit + FluentAssertions for automated containerized tests

RFC 7807 (ProblemDetails) standard error responses

Solution Layout
Plaintext
PocEfDapperTestcontainers/
├── src/
│   ├── PocEfDapper.Domain/             # Domain entities, business logic, and repository contracts
│   ├── PocEfDapper.Application/        # Commands (EF Core), Queries (Dapper), and MediatR handlers
│   ├── PocEfDapper.Infrastructure/     # DbContext, Dapper connection factory, and repository adapters
│   └── PocEfDapper.Api/                # Minimal API routes and application configuration
├── tests/
│   └── PocEfDapper.IntegrationTests/   # Integration tests using live PostgreSQL via Testcontainers
└── PocEfDapperTestcontainers.sln
Getting Started
Prerequisites
.NET 8 SDK or higher.

Docker Desktop or Docker Engine running (required for Testcontainers).

Running the API Locally
Start a local PostgreSQL instance (or use your preferred instance):

Bash
docker run --name poc-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=poc_ef_dapper -p 5432:5432 -d postgres:16-alpine
Build and run the API:

Bash
dotnet restore
dotnet build
dotnet run --project PocEfDapper.Api/PocEfDapper.Api.csproj
Open Swagger UI in your browser:

Plaintext
https://localhost:<porta>/swagger
Running Integration Tests (Testcontainers)
Make sure Docker is running, then run:

Bash
dotnet test
Testcontainers will automatically spin up a clean postgres:16-alpine container, initialize the schema, execute the API integration tests, and dispose of the container afterward.

Test Scenarios
1. Create Product (EF Core Persistence)
Endpoint: POST /api/products

Payload:

JSON
{
  "sku": "DELL-XPS-15",
  "name": "Notebook Dell XPS 15",
  "price": 12500.50,
  "stockQuantity": 10
}
Expected Response: 201 Created with the generated GUID:

JSON
{
  "id": "a90b4d45-6677-4c07-b2f7-ecde67645166"
}
2. Query Product (Dapper Read Model)
Endpoint: GET /api/products/{id}

Expected Response: 200 OK

JSON
{
  "id": "a90b4d45-6677-4c07-b2f7-ecde67645166",
  "sku": "DELL-XPS-15",
  "name": "Notebook Dell XPS 15",
  "price": 12500.50,
  "stockQuantity": 10,
  "createdAtUtc": "2026-09-03T04:18:39Z"
}
3. Duplicate SKU Conflict Rule (RFC 7807)
Re-send the exact same payload containing DELL-XPS-15.

Expected Response: 409 Conflict

JSON
{
  "type": "[https://tools.ietf.org/html/rfc9110#section-15.5.10](https://tools.ietf.org/html/rfc9110#section-15.5.10)",
  "title": "Product.DuplicateSku",
  "status": 409,
  "detail": "Já existe um produto cadastrado com este SKU."
}
