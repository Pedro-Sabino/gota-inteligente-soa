# Prompts utilizados (IA como copiloto)

**Ferramenta:** Claude (Anthropic)
**Disciplina:** Arquitetura Orientada a Serviços (SOA) e Web Services
**Uso:** a ideia da solução foi definida em equipe. O código da API foi escrito por **Pedro Paulo**, que também o executou, depurou e testou. O Claude foi usado como **revisor**: revisou o código enviado por Pedro Paulo, apontou problemas, fez pequenas alterações e ajudou na documentação. Cada integrante sabe explicar o que foi entregue.

Os prompts abaixo estão organizados pelas etapas do desenvolvimento e escritos de forma estruturada.

---

## 1. Modelagem do domínio

> Atue como arquiteto de software. Nossa CitizenTech, **Gota Inteligente**, ajuda o agricultor familiar a irrigar no momento certo: um ESP32 mede a umidade do solo e um modelo de ML cruza essa leitura com a previsão do tempo para decidir se a bomba deve ser ligada. Proponha um modelo de domínio para a API que centraliza esses dados, com entidades, atributos, relacionamentos e cardinalidades. Justifique cada entidade a partir do problema do cidadão.

**Resultado:** entidades Fazenda → Talhão → Sensor → LeituraUmidade e RecomendacaoIrrigacao.

## 2. Contrato da API REST

> A partir desse modelo de domínio, desenhe os endpoints REST da API em ASP.NET Core (.NET 8). A API terá quatro consumidores: o ESP32 (envia leituras), o modelo de IA em Python (grava recomendações), o app React Native (consulta o último alerta) e um painel WinForms (monitora o estresse hídrico regional). Para cada endpoint, indique método HTTP, rota, consumidor, payload e códigos de resposta (200, 201, 204, 400, 404, 409, 422).

**Resultado:** tabela de endpoints do README e separação por controllers.

## 3. Persistência com EF Core e migrations

> Implemente a camada de dados com Entity Framework Core 8 e SQLite usando Code First. Configure no `OnModelCreating` os tamanhos de campo, a exclusão em cascata, o índice único para o código do sensor e índices compostos para consultas por data. Explique como gerar e aplicar as migrations e como popular o banco apenas com dados simulados na primeira execução.

**Resultado:** `AppDbContext`, `DbSeeder` e a migration `CriacaoInicial`.

## 4. DTOs, validação e tratamento de erros

> Crie DTOs de entrada e saída separados das entidades, com validação por DataAnnotations (faixas de umidade de 0 a 100%, UF com 2 letras, enum de ação válido). Padronize as respostas de erro com `ProblemDetails` e explique quando usar 404, 409 e 422 nesta API.

**Resultado:** pasta `Dtos/` e validações nos controllers.

## 5. Regra de negócio: estresse hídrico

> Proponha uma regra simples e explicável para classificar o estresse hídrico de um talhão a partir da umidade atual e da umidade ideal mínima da cultura. Ela deve ter níveis claros para que o Sindicato Rural entenda o painel sem conhecimento técnico.

**Resultado:** `EstresseHidricoCalculator` com os níveis Nenhum, Leve, Moderado e Severo.

## 6. Endpoints agregados para o painel

> Implemente endpoints de leitura agregada para o painel do Sindicato Rural: estresse hídrico por fazenda, com o pior nível e os talhões em estresse, com filtro por município, e um resumo com um indicador de impacto mensurável da solução. Evite consultas N+1.

**Resultado:** `PainelController` e o indicador "irrigações evitadas nos últimos 30 dias".

## 7. Plano de testes manuais

> Monte um plano de testes manuais no Insomnia que cubra os caminhos de sucesso e de erro de cada endpoint. Para cada caso, informe o status esperado e o que validar na resposta. Gere a coleção em formato de importação do Insomnia.

**Resultado:** `insomnia/GotaInteligente.insomnia.json` e `TESTES_MANUAIS.md`.

## 8. Evolução do esquema

> Precisamos guardar a localização da fazenda para que o modelo de IA consulte a previsão do tempo do local. Adicione latitude e longitude opcionais, com validação de faixa, e gere uma nova migration sem perder os dados existentes.

**Resultado:** migration `AdicionaCoordenadasFazenda`.

## 9. Ambiente e ferramentas

> Ao instalar o `dotnet-ef` com o .NET SDK 8 aparece o erro "DotnetToolSettings.xml não encontrado". Explique a causa e como instalar uma versão compatível com o SDK.

**Resultado:** `dotnet-ef` fixado na versão 8.0.11.

## 10. Documentação

> Escreva o README do repositório com a ficha da CitizenTech, o papel da API na solução, o diagrama de integração, a estrutura de pastas, o modelo de domínio, como executar, o histórico de migrations e a tabela de endpoints.

**Resultado:** `README.md`.

---

## Reflexão da equipe

- **O que a IA acelerou:** a estrutura inicial do projeto, a configuração do EF Core e a coleção de testes.
- **O que validamos à mão:** a regra de estresse hídrico (ex.: 31,5% ÷ 45% = 70% → Moderado), os códigos HTTP de cada caso de erro e os logs das migrations.
- **O que aprendemos:** a versão das ferramentas importa (a versão mais recente do `dotnet-ef` não funciona com o SDK 8), e dados de localização na API são essenciais para integrar com a previsão do tempo da IA.
