# 💧 Gota Inteligente — API (Backend SOA)

> Global Solution 2026.2 · FIAP · Engenharia de Software · CitizenTech
> Disciplina: **Arquitetura Orientada a Serviços (SOA) e Web Services**

## Ficha da CitizenTech

| Campo | Resposta |
|---|---|
| **Nome da CitizenTech** | Gota Inteligente |
| **Cidade e bairro** | São Paulo (SP) – Parelheiros, zona rural do extremo sul |
| **Cidadão beneficiado** | Agricultor familiar / pequeno produtor |
| **Problema, em uma frase** | O pequeno produtor desperdiça água e energia regando "no olhômetro" ou perde a plantação numa seca repentina. |
| **Como funciona** | Um ESP32 mede a umidade do solo e envia à API; um modelo de ML cruza essa leitura com a previsão do tempo e grava uma recomendação (irrigar, aguardar ou não irrigar); o app do agricultor exibe o alerta e o painel do Sindicato/Prefeitura acompanha o estresse hídrico da região. |
| **O que cada parte faz** | **IoT (Wokwi/ESP32):** mede e envia umidade · **IA (Python):** decide se liga a bomba · **Mobile (React Native):** alerta o agricultor · **C# WinForms:** painel regional · **SOA (esta API):** centraliza dados e regras · **Cybersecurity / OS / Testing:** segurança, hospedagem no IIS e arquitetura TOGAF |
| **Link do repositório** | https://github.com/Pedro-Sabino/gota-inteligente-soa |
| **Indicador de impacto** | Irrigações evitadas nos últimos 30 dias (`GET /api/painel/resumo`) |
| **ODS conectados** | **ODS 2** (Fome zero e agricultura sustentável), **ODS 9** (Indústria, inovação e infraestrutura) e **ODS 13** (Ação contra a mudança global do clima) |

### Integrantes

| Nome | RM |
|---|---|
| Henrique Maciel Rodrigues | RM559628 |
| Igor Pereira Nociti | RM560225 |
| João Pedro Vieira de Moais | RM560527 |
| Luigi Bonuccelli | RM560950 |
| Pedro Paulo Sabino Botelho | RM559578 |

---

## O papel da API na solução

A API é a **camada de backend** que liga todas as partes da CitizenTech:

```
 ESP32 (Wokwi) ──POST /api/leituras──────────┐
                                             ▼
 Modelo de IA (Python) ──POST /api/recomendacoes──▶  [ Gota Inteligente API ] ──▶ SQLite
                                             ▲            │          │
 App Mobile ◀──GET /api/talhoes/{id}/recomendacoes/ultima │          │
 Painel WinForms ◀──GET /api/painel/estresse-hidrico ─────┘          │
                                                  Insomnia (testes manuais)
```

## Tecnologias

- C# / **.NET 8** — ASP.NET Core Web API
- **Entity Framework Core 8** (Code First) + **Migrations**
- **SQLite** (arquivo `gota-inteligente.db`, zero configuração)
- **Swagger** (documentação interativa) e **Insomnia** (testes manuais)

## Estrutura do projeto

```
GotaInteligente.Api/
├── Domain/        Entidades e regra de negócio (EstresseHidricoCalculator)
├── Data/          AppDbContext (mapeamento) e DbSeeder (dados simulados)
├── Dtos/          Contratos de entrada/saída com validação (DataAnnotations)
├── Controllers/   Endpoints REST
├── Migrations/    CriacaoInicial + AdicionaCoordenadasFazenda
└── Program.cs     Configuração, migrate automático e seed
insomnia/          Coleção do Insomnia com 33 requisições de teste
TESTES_MANUAIS.md  Roteiro e tabela de evidências
```

## Modelo de domínio

```
Fazenda 1 ──< Talhao 1 ──< Sensor 1 ──< LeituraUmidade
                  └──────< RecomendacaoIrrigacao
```

- **Fazenda:** propriedade do agricultor.
- **Talhao:** área com uma cultura e uma `UmidadeIdealMinima`.
- **Sensor:** um ESP32 (`Codigo` único, ex.: `ESP32-001`).
- **LeituraUmidade:** medição de umidade do solo (0–100%).
- **RecomendacaoIrrigacao:** decisão da IA (`Irrigar`, `Aguardar`, `NaoIrrigar`) com motivo e probabilidade de chuva.

**Regra de negócio central** (`EstresseHidricoCalculator`): compara a umidade atual com o ideal do talhão.

| Umidade ÷ ideal | Nível |
|---|---|
| ≥ 100% | Nenhum |
| 75% a 99% | Leve |
| 50% a 74% | Moderado |
| < 50% | Severo |
| sem leitura | SemDados |

## Como executar

**Pré-requisitos:** [.NET 8 SDK](https://dotnet.microsoft.com/download) e a ferramenta do EF Core:

```bash
dotnet tool install --global dotnet-ef
```

**1. Restaurar pacotes**
```bash
cd GotaInteligente.Api
dotnet restore
```

**2. Gerar as migrations** (uma vez, e commitar a pasta `Migrations/`)
```bash
dotnet ef migrations add CriacaoInicial
```

**3. Rodar** (aplica as migrations sozinha e popula dados simulados)
```bash
dotnet run
```

A API sobe em **http://localhost:5080** e o Swagger em **http://localhost:5080/swagger**.

### Hospedagem no IIS (integração com Operating Systems)
```bash
dotnet publish -c Release -o publish
```
No Windows Server, instale o **.NET 8 Hosting Bundle**, crie o site apontando para a pasta `publish` e dê permissão de **escrita** ao Application Pool na pasta (o SQLite cria o arquivo `.db` lá).

### Histórico de migrations

| Migration | O que muda |
|---|---|
| `CriacaoInicial` | Cria as tabelas Fazendas, Talhoes, Sensores, Leituras e Recomendacoes, com FKs em cascata e índice único em `Sensores.Codigo` |
| `AdicionaCoordenadasFazenda` | Adiciona `Latitude` e `Longitude` (opcionais) em Fazendas, para a IA consultar a previsão do tempo do local |

## Endpoints

| Método | Rota | Quem usa | Descrição |
|---|---|---|---|
| GET/POST | `/api/fazendas` | Painel / Admin | Lista (filtro `?municipio=`) e cria fazendas |
| GET/PUT/DELETE | `/api/fazendas/{id}` | Painel / Admin | Consulta, atualiza e remove |
| GET/POST | `/api/talhoes` | Mobile / Admin | Lista (`?fazendaId=`) e cria talhões, já com nível de estresse |
| GET/PUT/DELETE | `/api/talhoes/{id}` | Mobile / Admin | Consulta, atualiza e remove |
| GET/POST | `/api/sensores` | Admin | Lista (`?talhaoId=`) e cadastra sensores |
| GET/PUT/DELETE | `/api/sensores/{id}` | Admin | Consulta, atualiza e remove |
| GET | `/api/sensores/{id}/leituras` | Mobile / IA | Histórico de leituras (`?limite=`) |
| **POST** | `/api/leituras` | **ESP32** | Ingestão de leitura de umidade |
| **POST** | `/api/recomendacoes` | **IA (Python)** | Grava a decisão do modelo |
| GET | `/api/talhoes/{id}/recomendacoes` | Mobile | Histórico de recomendações |
| GET | `/api/talhoes/{id}/recomendacoes/ultima` | **Mobile** | Alerta mais recente do talhão |
| GET | `/api/painel/estresse-hidrico` | **WinForms** | Estresse hídrico por fazenda |
| GET | `/api/painel/resumo` | WinForms | Totais e indicador de impacto |

### Códigos de resposta
`200 OK` · `201 Created` · `204 No Content` · `400 Bad Request` (validação) · `404 Not Found` · `409 Conflict` (sensor duplicado) · `422 Unprocessable Entity` (sensor desativado)

### Exemplos de payload

**ESP32 → `POST /api/leituras`**
```json
{ "sensorCodigo": "ESP32-001", "umidadePercentual": 28.5 }
```

**IA → `POST /api/recomendacoes`**
```json
{
  "talhaoId": 1,
  "acao": "NaoIrrigar",
  "motivo": "Bomba desligada: 90% de chance de chuva nas próximas 2 horas.",
  "probabilidadeChuvaPercentual": 90,
  "umidadeSoloPercentual": 28.5,
  "origem": "modelo-ml"
}
```

## Testes manuais (Insomnia)

1. No Insomnia: **Import** → `insomnia/GotaInteligente.insomnia.json`.
2. A variável `base_url` já aponta para `http://localhost:5080`.
3. Execute as requisições na ordem das pastas e registre as evidências em [`TESTES_MANUAIS.md`](TESTES_MANUAIS.md).

## Dados e privacidade

Todos os dados do seed são **simulados**. Nenhum dado pessoal real é utilizado.

## Uso de IA

Os prompts utilizados no desenvolvimento estão registrados em `prompts.md`.
