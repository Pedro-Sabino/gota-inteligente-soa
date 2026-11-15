# Testes manuais — Gota Inteligente API

Ferramenta: **Insomnia** · Coleção: `insomnia/GotaInteligente.insomnia.json`
Pré-condição: banco recém-criado (apague `gota-inteligente.db` e rode `dotnet run` para voltar ao estado inicial).

> Execute na ordem. Cole o print de cada requisição na coluna **Evidência** (pasta `evidencias/`).

| # | Requisição | Status esperado | O que validar | Resultado | Evidência |
|---|---|---|---|---|---|
| 1 | GET /api/fazendas | 200 | 2 fazendas do seed | ✅ 200 | [01-listar-fazendas.txt](evidencias/01-listar-fazendas.txt) |
| 2 | GET /api/fazendas?municipio=São Paulo | 200 | Filtro aplicado | ✅ 200 | [02-listar-por-municipio.txt](evidencias/02-listar-por-municipio.txt) |
| 3 | GET /api/fazendas/1 | 200 | `quantidadeTalhoes` = 2 | ✅ 200 | [03-obter-fazenda-1.txt](evidencias/03-obter-fazenda-1.txt) |
| 4 | GET /api/fazendas/999 | 404 | Fazenda inexistente | ✅ 404 | [04-obter-fazenda-inexistente.txt](evidencias/04-obter-fazenda-inexistente.txt) |
| 5 | POST /api/fazendas | 201 | Header `Location` e id 3 | ✅ 201 | [05-criar-fazenda.txt](evidencias/05-criar-fazenda.txt) |
| 6 | POST /api/fazendas (inválida) | 400 | Erros de validação de `nome` e `estado` | ✅ 400 | [06-criar-fazenda-invalida-sem-nome-uf-com-3-letras.txt](evidencias/06-criar-fazenda-invalida-sem-nome-uf-com-3-letras.txt) |
| 7 | PUT /api/fazendas/1 | 204 | Sem corpo; GET seguinte mostra 5.0 ha | ✅ 204 | [07-atualizar-fazenda-1.txt](evidencias/07-atualizar-fazenda-1.txt) |
| 8 | DELETE /api/fazendas/3 | 204 | GET /api/fazendas/3 passa a dar 404 | ✅ 204 | [08-remover-fazenda-3-criada-no-teste-de-criacao.txt](evidencias/08-remover-fazenda-3-criada-no-teste-de-criacao.txt) |
| 9 | GET /api/talhoes | 200 | `nivelEstresseHidrico` presente | ✅ 200 | [09-listar-talhoes.txt](evidencias/09-listar-talhoes.txt) |
| 10 | GET /api/talhoes?fazendaId=1 | 200 | Só talhões da fazenda 1 | ✅ 200 | [10-listar-talhoes-da-fazenda-1.txt](evidencias/10-listar-talhoes-da-fazenda-1.txt) |
| 11 | GET /api/talhoes/1 | 200 | `ultimaUmidade` = 31.5, nível Moderado (31,5 ÷ 45 = 70%) | ✅ 200 | [11-obter-talhao-1-com-nivel-de-estresse.txt](evidencias/11-obter-talhao-1-com-nivel-de-estresse.txt) |
| 12 | POST /api/talhoes | 201 | Talhão criado, nível SemDados | ✅ 201 | [12-criar-talhao.txt](evidencias/12-criar-talhao.txt) |
| 13 | POST /api/talhoes (fazenda 999) | 404 | Problem Details | ✅ 404 | [13-criar-talhao-em-fazenda-inexistente.txt](evidencias/13-criar-talhao-em-fazenda-inexistente.txt) |
| 14 | POST /api/talhoes (umidade 150) | 400 | Erro em `umidadeIdealMinima` | ✅ 400 | [14-criar-talhao-com-umidade-ideal-fora-da-faixa.txt](evidencias/14-criar-talhao-com-umidade-ideal-fora-da-faixa.txt) |
| 15 | PUT /api/talhoes/1 | 204 | Atualizado | ✅ 204 | [15-atualizar-talhao-1.txt](evidencias/15-atualizar-talhao-1.txt) |
| 16 | GET /api/sensores | 200 | 3 sensores do seed (+ novos) | ✅ 200 | [16-listar-sensores.txt](evidencias/16-listar-sensores.txt) |
| 17 | GET /api/sensores/1 | 200 | `totalLeituras` = 5 | ✅ 200 | [17-obter-sensor-1.txt](evidencias/17-obter-sensor-1.txt) |
| 18 | GET /api/sensores/1/leituras?limite=10 | 200 | Ordem da mais recente para a mais antiga | ✅ 200 | [18-historico-de-leituras-do-sensor-1.txt](evidencias/18-historico-de-leituras-do-sensor-1.txt) |
| 19 | POST /api/sensores | 201 | Sensor ESP32-010 criado | ✅ 201 | [19-cadastrar-sensor.txt](evidencias/19-cadastrar-sensor.txt) |
| 20 | POST /api/sensores (código duplicado) | 409 | Conflito | ✅ 409 | [20-cadastrar-sensor-com-codigo-duplicado.txt](evidencias/20-cadastrar-sensor-com-codigo-duplicado.txt) |
| 21 | POST /api/sensores (talhão 999) | 404 | Talhão inexistente | ✅ 404 | [21-cadastrar-sensor-em-talhao-inexistente.txt](evidencias/21-cadastrar-sensor-em-talhao-inexistente.txt) |
| 22 | POST /api/leituras | 201 | Nível de estresse calculado na resposta | ✅ 201 | [22-enviar-leitura-de-umidade.txt](evidencias/22-enviar-leitura-de-umidade.txt) |
| 23 | POST /api/leituras (130%) | 400 | Fora de 0–100 | ✅ 400 | [23-umidade-fora-de-0-100.txt](evidencias/23-umidade-fora-de-0-100.txt) |
| 24 | POST /api/leituras (sensor desconhecido) | 404 | Sensor não cadastrado | ✅ 404 | [24-sensor-nao-cadastrado.txt](evidencias/24-sensor-nao-cadastrado.txt) |
| 25 | POST /api/recomendacoes | 201 | Recomendação gravada | ✅ 201 | [25-ia-envia-recomendacao-nao-irrigar.txt](evidencias/25-ia-envia-recomendacao-nao-irrigar.txt) |
| 26 | POST /api/recomendacoes (talhão 999) | 404 | Talhão inexistente | ✅ 404 | [26-recomendacao-para-talhao-inexistente.txt](evidencias/26-recomendacao-para-talhao-inexistente.txt) |
| 27 | POST /api/recomendacoes (ação inválida) | 400 | Enum inválido | ✅ 400 | [27-acao-invalida.txt](evidencias/27-acao-invalida.txt) |
| 28 | GET /api/talhoes/1/recomendacoes | 200 | Mais recente primeiro | ✅ 200 | [28-historico-do-talhao-1.txt](evidencias/28-historico-do-talhao-1.txt) |
| 29 | GET /api/talhoes/1/recomendacoes/ultima | 200 | Alerta exibido pelo app mobile | ✅ 200 | [29-ultimo-alerta-do-talhao-1-app-mobile.txt](evidencias/29-ultimo-alerta-do-talhao-1-app-mobile.txt) |
| 30 | GET /api/talhoes/999/recomendacoes/ultima | 404 | Sem recomendações | ✅ 404 | [30-ultimo-alerta-de-talhao-sem-recomendacoes.txt](evidencias/30-ultimo-alerta-de-talhao-sem-recomendacoes.txt) |
| 31 | GET /api/painel/estresse-hidrico | 200 | Fazendas ordenadas pelo pior nível | ✅ 200 | [31-estresse-hidrico-por-fazenda.txt](evidencias/31-estresse-hidrico-por-fazenda.txt) |
| 32 | GET /api/painel/estresse-hidrico?municipio=São Paulo | 200 | Filtro por município | ✅ 200 | [32-estresse-hidrico-filtrado-por-municipio.txt](evidencias/32-estresse-hidrico-filtrado-por-municipio.txt) |
| 33 | GET /api/painel/resumo | 200 | `irrigacoesEvitadasUltimos30Dias` ≥ 1 | ✅ 200 | [33-resumo-e-indicador-de-impacto.txt](evidencias/33-resumo-e-indicador-de-impacto.txt) |

> Os valores esperados dos itens 11 e 17 vêm do seed. Se você mudar o `DbSeeder`, ajuste a tabela.
