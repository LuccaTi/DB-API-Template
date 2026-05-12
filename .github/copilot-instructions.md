# Copilot Instructions

## Diretrizes de projeto
- O usuário se chama Lucca, tem 27 anos, é desenvolvedor Back-End (.NET, SQL) e está estudando Full-Stack JS (The Odin Project).
- Preferir arquiteturas onde APIs de CRUD de banco de dados são estritamente separadas de APIs que consomem serviços externos.
- Preferir Mapster em projetos .NET; o usuário já migrou com sucesso do AutoMapper para Mapster visando escalabilidade comercial em APIs .NET.
- Fornecer orientações didáticas e acionáveis que o próprio usuário possa executar; evitar sugerir "downgrades" tecnológicos ou soluções que gerem dívida técnica.

## Boas práticas adicionais para testes
- Separe testes por tipo: Unit, Integration, e E2E; organize pastas/classe com convenção clara.
- Nomeie testes de forma descritiva: MethodName_StateUnderTest_ExpectedBehavior.
- Mantenha Application e Domain fáceis de testar: evite dependências estáticas e use injeção de dependência.
- Prefira testar lógica de negócio em unit tests; reserve integração para verificar contratos entre camadas e middleware.
- Em projetos .NET, use CancellationToken de ponta a ponta (Controller -> Service -> Repository -> EF Core) como último parâmetro com valor padrão (CancellationToken cancellationToken = default). Nos testes unitários com Moq, valide o 'plumbing' passando um CancellationToken explícito em vez de It.IsAny<CancellationToken>().
- Em projetos .NET, utilizar sempre CancellationToken de ponta a ponta (Controller -> Service -> Repository -> EF Core) como último parâmetro (cancellationToken = default). Nos testes unitários com Moq, validar o 'plumbing' passando um token explícito em vez de It.IsAny<CancellationToken>().