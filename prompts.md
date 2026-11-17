# Prompts utilizados (IA como revisora)

**Ferramenta:** Claude (Anthropic)
**Disciplina:** Arquitetura Orientada a Serviços (SOA) e Web Services
**Uso:** a ideia da solução foi definida em equipe. O código da API foi escrito por **Pedro Paulo**, que também o executou, depurou e testou. O Claude foi usado como **revisor**: revisou o código enviado por Pedro Paulo, apontou problemas, fez pequenas alterações e ajudou na documentação. Cada integrante sabe explicar o que foi entregue.

> **Observação:** o histórico original da conversa com a ferramenta foi perdido e não pôde ser recuperado. Os prompts abaixo foram **reconstruídos de memória** por Pedro Paulo e redigidos de forma estruturada: representam o pedido feito em cada etapa, mas não são a transcrição literal.

---

## 1. Revisão do modelo de domínio

> Modelei o domínio da API da **Gota Inteligente** (Fazenda → Talhão → Sensor → LeituraUmidade e RecomendacaoIrrigacao) para centralizar os dados do ESP32 e do modelo de IA que decide a irrigação. Revise as entidades, os relacionamentos e as cardinalidades, e aponte se algo não se justifica pelo problema do agricultor.

**Resultado:** modelo de domínio validado, com pequenos ajustes.

## 2. Revisão do contrato da API REST

> Desenhei os endpoints da API em ASP.NET Core (.NET 8) para quatro consumidores: o ESP32, o modelo de IA em Python, o app React Native e o painel WinForms. Revise os métodos HTTP, as rotas, os payloads e os códigos de resposta (200, 201, 204, 400, 404, 409 e 422).

**Resultado:** ajustes na tabela de endpoints e na separação por controllers.

## 3. Revisão da persistência com EF Core

> Segue a camada de dados com Entity Framework Core 8 e SQLite (Code First), com o `AppDbContext`, o seed de dados simulados e a migration inicial. Revise a configuração do `OnModelCreating` (tamanhos de campo, exclusão em cascata, índice único do código do sensor e índices compostos por data).

**Resultado:** revisão de `AppDbContext`, `DbSeeder` e da migration `CriacaoInicial`.

## 4. Revisão de DTOs, validação e erros

> Revise os DTOs de entrada e saída e as validações por DataAnnotations (umidade de 0 a 100%, UF com 2 letras, enum de ação). Confirme se estou usando 404, 409 e 422 nos casos certos e se as respostas de erro seguem o `ProblemDetails`.

**Resultado:** correções pontuais na pasta `Dtos/` e nas validações dos controllers.

## 5. Revisão da regra de estresse hídrico

> Implementei a classificação do estresse hídrico de um talhão a partir da umidade atual e da umidade ideal mínima da cultura (Nenhum, Leve, Moderado e Severo). Revise se a regra está correta e se é fácil de explicar para o Sindicato Rural.

**Resultado:** `EstresseHidricoCalculator` revisado.

## 6. Revisão dos endpoints agregados

> Revise os endpoints do painel (estresse hídrico por fazenda com filtro por município e resumo com o indicador de impacto). Verifique se há consultas N+1 ou cálculos incorretos.

**Resultado:** ajustes no `PainelController` e no indicador "irrigações evitadas nos últimos 30 dias".

## 7. Revisão do plano de testes

> Revise o meu plano de testes manuais e a coleção do Insomnia. Aponte caminhos de sucesso ou de erro que ficaram sem cobertura.

**Resultado:** `insomnia/GotaInteligente.insomnia.json` e `TESTES_MANUAIS.md` revisados.

## 8. Evolução do esquema

> Adicionei latitude e longitude opcionais à fazenda, para o modelo de IA consultar a previsão do tempo do local. Revise a validação de faixa e a nova migration, garantindo que os dados existentes não se percam.

**Resultado:** migration `AdicionaCoordenadasFazenda` revisada.

## 9. Ambiente e ferramentas

> Ao instalar o `dotnet-ef` com o .NET SDK 8 aparece o erro "DotnetToolSettings.xml não encontrado". Explique a causa e como instalar uma versão compatível com o SDK.

**Resultado:** `dotnet-ef` fixado na versão 8.0.11.

## 10. Revisão da documentação

> Revise o README (ficha da CitizenTech, papel da API, diagrama de integração, estrutura de pastas, modelo de domínio, execução, migrations e endpoints) e aponte o que estiver faltando ou pouco claro.

**Resultado:** `README.md` revisado.

---

## Reflexão da equipe

- **Onde a revisão da IA ajudou:** apontou ajustes pontuais na configuração do EF Core, nas validações e nos códigos HTTP de erro.
- **O que validamos à mão:** a regra de estresse hídrico (ex.: 31,5% ÷ 45% = 70% → Moderado), os códigos HTTP de cada caso de erro e os logs das migrations.
- **O que aprendemos:** a versão das ferramentas importa (a versão mais recente do `dotnet-ef` não funciona com o SDK 8), e dados de localização na API são essenciais para integrar com a previsão do tempo da IA.
