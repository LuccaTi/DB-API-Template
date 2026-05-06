# DB API Template .NET 8

## Visão geral
Este repositório contém um template de API REST orientada a banco de dados construída com ASP.NET Core (.NET 8). O projeto já vem estruturado com injeção de dependência, logging com Serilog, configuração via appsettings.json, persistência de dados utilizando Entity Framework Core e Swagger opcional para documentação e testes.

O propósito é criar um "esqueleto" de Web APIs especializado em operações CRUD e acesso direto a banco de dados, servindo como padrão de implementação limpa e escalável.

Endpoints de exemplo disponíveis (Entidade Product):
- GET /api/v1/Products: Retorna uma lista de todos os produtos do banco de dados.
- GET /api/v1/Products/{id}: Retorna um produto específico pelo seu ID.
- POST /api/v1/Products: Insere um novo produto.
- PUT /api/v1/Products/{id}: Atualiza um produto existente.
- DELETE /api/v1/Products/{id}: Remove um produto pelo seu ID.

## Tecnologias e bibliotecas essenciais
- .NET 8 (ASP.NET Core)
- Entity Framework Core: ORM responsável pela persistência e consultas no banco de dados.
- Mapster: Biblioteca de alta performance para mapear objetos de domínio para DTOs (adotada no lugar do AutoMapper para maior escalabilidade comercial).
- Swashbuckle.AspNetCore (Swagger/OpenAPI)
- Microsoft.Extensions.Configuration: Leitura de configurações.
- Serilog: Escrita estruturada em arquivos e console de Log.

## Estrutura do projeto
O projeto divide suas responsabilidades da seguinte forma:

- src/DBAPITemplate.Api:
  - Responsabilidade: Camada de Apresentação e ponto de entrada (Program.cs, .exe). Centraliza a injeção de dependências e expõe os endpoints via Controllers. Usa os projetos Application e Infrastructure como referência.

- src/DBAPITemplate.Application:
  - Responsabilidade: Casos de uso da aplicação. Contém abstrações de serviços, Mappers (Mapster) e DTOs de transporte que fluem de e para a API. Usa o projeto Domain como referência.

- src/DBAPITemplate.Domain:
  - Responsabilidade: Entidades de núcleo (como Product), regras de negócio absolutas e exceções de uso geral (NotFoundException, ConflictException, etc).

- src/DBAPITemplate.Infrastructure:
  - Responsabilidade: Implementação dos contratos e persistência (AppDbContext, EF Core). Usa o projeto Application como referência.

## Endpoints (Products)
- GET /api/v1/Products
    - Resposta 200: Lista de produtos
- GET /api/v1/Products/{id}
    - Resposta 200: Produto específico
    - Resposta 404: Erro devidamente formatado se não encontrado
- POST /api/v1/Products
    - Corpo da Requisição: Dados do produto
    - Resposta 201: Produto criado e rota no cabeçalho Location
- PUT /api/v1/Products/{id}
    - Corpo da Requisição: Dados atualizados do produto
    - Resposta 204: No Content (Sucesso)
- DELETE /api/v1/Products/{id}
    - Resposta 204: No Content (Sucesso)

## Configuração

### appsettings.json
Configurações principais da aplicação e log:
- Serilog: Configuração de níveis mínimo e saídas de log.
- UseSwaggerProduction: ("true" | "false") habilita/desabilita o uso do swagger em produção.

### appsettings.Production.json
Configuração específica para ambiente de produção:
- Define o Kestrel para escutar HTTPS na porta 443 (padrão web).
- Usado quando ASPNETCORE_ENVIRONMENT=Production.

### Perfis de execução (local)
Definidos em src/DBAPITemplate.Api/Properties/launchSettings.json.

## Uso da API
A API pode ser usada via console ao compilar o código e rodar o .exe, ou diretamente no Visual Studio com F5.
