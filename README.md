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

## 🗄️ Como alterar do Banco em Memória para o SQL Server:

Este template vem configurado por padrão com o Entity Framework Core InMemory, visando facilitar testes rápidos e estudos sem a necessidade de infraestrutura pesada. Quando for evoluir a API para um projeto real que exige persistência de dados no disco, siga os passos abaixo para conectar a um banco SQL Server:

1. Pré-Requisito: Ter um Servidor SQL rodando  
Para que essa conexão funcione, é necessário ter uma instância do SQL Server instalada:  
• SQL Server Developer / Express: O programa tradicional do SQL Server instalado na sua máquina (onde usa-se o SQL Server Management Studio).

2. Instalação dos Pacotes (NuGet)  
Será preciso instalar pacotes específicos em dois projetos diferentes:  
• No projeto DBAPITemplate.Infrastructure instale o pacote Microsoft.EntityFrameworkCore.SqlServer. (Lembre-se de remover o pacote InMemory).  
• No projeto DBAPITemplate.Api instale o pacote Microsoft.EntityFrameworkCore.Tools. (Esse pacote é quem permite que o console do Visual Studio entenda os comandos de Migrations).

3. Configurar a Connection String (Appsettings)  
A string de conexão define onde o banco está e qual usuário e senha devem ser usados. A melhor prática comercial é separar por ambiente:  
• No appsettings.json (Base / Default):  
Pode deixar vazio ou apontar para um banco local genérico:

```json
"ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DB_Template;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
```  
• No appsettings.Development.json (Ambiente de Desenvolvimento):  
Aqui geralmente fica o banco da própria máquina (LocalDB ou seu SQL Express). Os testes rodam mirando aqui.  
• No appsettings.Production.json (Ambiente de Produção):  
Geralmente não se coloca senhas cruas aqui por segurança! Em produção, a string vem através de variáveis de ambiente no Docker/Linux ou pelo Azure KeyVault.

4. Atualizar o Program.cs  
Vá no projeto da API, abra o Program.cs e localize aonde o contexto está sendo registrado em memória. Mude isto:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("DB_Template_Local"));
```  

Para isto:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```
5. Executar as Migrations  
Como o SQL Server é um banco de dados relacional e fixo, não basta o código existir; o banco físico precisa ter as tabelas criadas exatamente como suas entidades do projeto Domain. É aí que entram as Migrations.  

As migrations são arquivos de código que funcionam como um sistema de controle de versão para o esquema do banco de dados, permitindo criar, atualizar e reverter tabelas e colunas de forma automatizada e sincronizada com o código da aplicação. Elas traduzem as classes C# para comandos SQL, garantindo que o banco de dados evolua junto com o seu software sem a necessidade de intervenção manual direta no banco.

Cada detalhe da classe C# é mapeado para uma estrutura correspondente no SQL:
- A Classe: Vira a Tabela.
- As Propriedades: Viram as Colunas.
- O Tipo de Dado (int, string, bool): Vira o Tipo de Campo (INT, VARCHAR, BIT).
- A Propriedade Id: Geralmente é traduzida automaticamente como a Primary Key (Chave Primária).

### Como fazer:
Abra o Package Manager Console (Console do Gerenciador de Pacotes) no Visual Studio:

1. Certifique-se de que o Projeto de Inicialização lá em cima seja o DBAPITemplate.Api.

2. Mude o escopo da janela do console (Default project) para DBAPITemplate.Infrastructure (pois é lá que o AppDbContext mora).

3. Execute os comandos:  
Comando 1: Add-Migration InitialCreate  
• O que faz? O Entity Framework lê o AppDbContext e suas entidades (ex: Product), vê quais tabelas faltam e escreve um arquivo C# (Translation) contendo as instruções SQL necessárias para gerá-las. Esse comando não toca no banco ainda. É o planejamento.  

Comando 2: Update-Database  
• O que faz? Ele "aperta o botão verde". Ele pega todas as migrations que você gerou, conecta no seu SQL Server usando aquela ConnectionString que configuramos, cria o banco e cria as tabelas fisicamente.
---
