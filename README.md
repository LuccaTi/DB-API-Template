# Web API Template — .NET 8

## Visão geral
Este repositório contém um template de API REST construída com ASP.NET Core (.NET 8). O projeto já vem estruturado com injeção de dependência, logging com Serilog, configuração via `appsettings.json` e Swagger opcional para documentação e testes.

O propósito é criar um "esqueleto"de Web APIs que sirva como um padrão de implementação, configuração e logging.

Endpoint de exemplo disponível:
- `GET /api/v1/test` → retorna uma lista de usuários da API de teste `jsonplaceholder.typicode.com`.
- `GET /api/v1/test/{id}` → retorna um usuário específico da lista de teste.

## Tecnologias e bibliotecas essenciais
- .NET 8 (ASP.NET Core)
- HttpClient: Biblioteca que faz as requisições e os tratamentos Http.
- Swashbuckle.AspNetCore (Swagger/OpenAPI)
- AutoMapper: Biblioteca usada para mapear os objetos consumidos em objetos criados pela API.
- Microsoft.Extensions.Configuration: Responsável por fazer leitura e escrita de arquivos de configuração.
- Serilog: Responsável por fazer a escrita em arquivos de Log.

## Estrutura do projeto
O projeto divide suas responsabilidades da seguinte forma:

- `src/WebAPITemplate.Api`:
  - **Responsabilidade**: Camada de Apresentação e ponto de entrada (`Program.cs`, `.exe`). Centraliza a injeção de dependências e expõe os endpoints via Controllers. Usa os projetos Application e Infrastructure como referência.

- `src/WebAPITemplate.Application`:
  - **Responsabilidade**: Casos de uso da aplicação. Contém abstrações de serviços, clientes externos, e os Mappers e DTOs de transporte que fluem de e para a API. Usa o projeto Domain como referência.

- `src/WebAPITemplate.Domain`:
  - **Responsabilidade**: Entidades de núcleo e regras de negócio absolutas, além das exceções de uso geral (`NotFoundException`, `ConflictException`, etc).

- `src/WebAPITemplate.Infrastructure`:
  - **Responsabilidade**: Implementação dos contratos definidos em Application/Domain (Ex: banco de dados, Clients, etc). Usa o projeto Application como referência.


## Endpoints
- `GET /api/v1/Test`
    - Resposta 200: `Lista de users`
    - Resposta 400: detalhes do erro em caso de falha, com log registrado.
- `GET /api/v1/Test/{id}`
    - Resposta 200: `User específico`
    - Resposta 400: detalhes do erro em caso de falha, com log registrado.

## Configuração

### appsettings.json
Configurações principais da aplicação (seção `AppConfig`):
- `UseSwaggerProduction` ("true" | "false"): habilita/desabilita o uso do swagger em produção (conveniência para testes).
- `ClientUrl` (string): URL base que será consumida pela API.
- `Timeout` (int): Tempo de timeout das conexões.

### appsettings.Production.json
Configuração específica para ambiente de produção:
- Define o Kestrel para escutar HTTPS na porta 443 (padrão web).
- Usado quando `ASPNETCORE_ENVIRONMENT=Production`.

### Perfis de execução (local)
Definidos em `src/WebAPITemplate.Api/Properties/launchSettings.json`:
- HTTPS: `https://localhost:5001`
- Ambiente padrão: `ASPNETCORE_ENVIRONMENT=Development`

## Uso da API
A API pode ser usada via console ao compilar o código e usar o .exe dentro do terminal, vale tanto para uso em Debug quanto Release.
