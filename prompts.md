# Prompts utilizados (IA como copiloto)

Registro dos prompts usados com o Claude (Anthropic) durante o desenvolvimento da API de SOA. Todo o código gerado foi revisado, executado e testado pela equipe.

| # | Prompt (resumo) | Como usamos a resposta |
|---|---|---|
| 1 | "Segue a proposta da GS e a nossa ideia (Gota Inteligente: ESP32 + IA + app + painel WinForms). Desenvolva a solução de SOA." | Base da API: modelagem Fazenda → Talhão → Sensor → Leitura → Recomendação, controllers REST, EF Core + SQLite e coleção do Insomnia |
| 2 | "O comando `dotnet` não é reconhecido no PowerShell." | Instalação do .NET 8 SDK e ajuste do PATH |
| 3 | "Erro ao instalar o dotnet-ef: DotnetToolSettings.xml não encontrado." | Instalação do `dotnet-ef` fixado na versão 8.0.11, compatível com o SDK 8 |
| 4 | "Está certo esse log do `dotnet run`?" | Validação da migration `CriacaoInicial` e do seed |
| 5 | "Faça os próximos passos: segunda migration, README e GitHub." | Migration `AdicionaCoordenadasFazenda` (Latitude/Longitude), atualização do README e publicação do repositório |

## O que aprendemos / alteramos
- A regra de estresse hídrico (`EstresseHidricoCalculator`) foi conferida à mão com os dados do seed (ex.: 31,5% ÷ 45% = 70% → Moderado).
- Versões das ferramentas importam: a última versão do `dotnet-ef` não é compatível com o SDK 8.
- As coordenadas da fazenda foram adicionadas porque a IA (Python) precisa delas para consultar a previsão do tempo.
