# Testes manuais — Gota Inteligente API

Ferramenta: **Insomnia** · Coleção: `insomnia/GotaInteligente.insomnia.json`
Pré-condição: banco recém-criado (apague `gota-inteligente.db` e rode `dotnet run` para voltar ao estado inicial).

> Execute na ordem. Cole o print de cada requisição na coluna **Evidência** (pasta `evidencias/`).

| # | Requisição | Status esperado | O que validar | Resultado | Evidência |
|---|---|---|---|---|---|
| 1 | GET /api/fazendas | 200 | 2 fazendas do seed | | |
| 2 | GET /api/fazendas?municipio=São Paulo | 200 | Filtro aplicado | | |
| 3 | GET /api/fazendas/1 | 200 | `quantidadeTalhoes` = 2 | | |
| 4 | GET /api/fazendas/999 | 404 | Fazenda inexistente | | |
| 5 | POST /api/fazendas | 201 | Header `Location` e id 3 | | |
| 6 | POST /api/fazendas (inválida) | 400 | Erros de validação de `nome` e `estado` | | |
| 7 | PUT /api/fazendas/1 | 204 | Sem corpo; GET seguinte mostra 5.0 ha | | |
| 8 | DELETE /api/fazendas/3 | 204 | GET /api/fazendas/3 passa a dar 404 | | |
| 9 | GET /api/talhoes | 200 | `nivelEstresseHidrico` presente | | |
| 10 | GET /api/talhoes?fazendaId=1 | 200 | Só talhões da fazenda 1 | | |
| 11 | GET /api/talhoes/1 | 200 | `ultimaUmidade` = 31.5, nível Moderado (31,5 ÷ 45 = 70%) | | |
| 12 | POST /api/talhoes | 201 | Talhão criado, nível SemDados | | |
| 13 | POST /api/talhoes (fazenda 999) | 404 | Problem Details | | |
| 14 | POST /api/talhoes (umidade 150) | 400 | Erro em `umidadeIdealMinima` | | |
| 15 | PUT /api/talhoes/1 | 204 | Atualizado | | |
| 16 | GET /api/sensores | 200 | 3 sensores do seed (+ novos) | | |
| 17 | GET /api/sensores/1 | 200 | `totalLeituras` = 5 | | |
| 18 | GET /api/sensores/1/leituras?limite=10 | 200 | Ordem da mais recente para a mais antiga | | |
| 19 | POST /api/sensores | 201 | Sensor ESP32-010 criado | | |
| 20 | POST /api/sensores (código duplicado) | 409 | Conflito | | |
| 21 | POST /api/sensores (talhão 999) | 404 | Talhão inexistente | | |
| 22 | POST /api/leituras | 201 | Nível de estresse calculado na resposta | | |
| 23 | POST /api/leituras (130%) | 400 | Fora de 0–100 | | |
| 24 | POST /api/leituras (sensor desconhecido) | 404 | Sensor não cadastrado | | |
| 25 | POST /api/recomendacoes | 201 | Recomendação gravada | | |
| 26 | POST /api/recomendacoes (talhão 999) | 404 | Talhão inexistente | | |
| 27 | POST /api/recomendacoes (ação inválida) | 400 | Enum inválido | | |
| 28 | GET /api/talhoes/1/recomendacoes | 200 | Mais recente primeiro | | |
| 29 | GET /api/talhoes/1/recomendacoes/ultima | 200 | Alerta exibido pelo app mobile | | |
| 30 | GET /api/talhoes/999/recomendacoes/ultima | 404 | Sem recomendações | | |
| 31 | GET /api/painel/estresse-hidrico | 200 | Fazendas ordenadas pelo pior nível | | |
| 32 | GET /api/painel/estresse-hidrico?municipio=São Paulo | 200 | Filtro por município | | |
| 33 | GET /api/painel/resumo | 200 | `irrigacoesEvitadasUltimos30Dias` ≥ 1 | | |

> Os valores esperados dos itens 11 e 17 vêm do seed. Se você mudar o `DbSeeder`, ajuste a tabela.
