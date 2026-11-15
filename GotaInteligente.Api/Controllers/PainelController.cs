using GotaInteligente.Api.Data;
using GotaInteligente.Api.Domain;
using GotaInteligente.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GotaInteligente.Api.Controllers;

/// <summary>Visões agregadas para o painel do Sindicato Rural / Prefeitura (C# WinForms).</summary>
[ApiController]
[Route("api/[controller]")]
public class PainelController : ControllerBase
{
    private readonly AppDbContext _db;
    public PainelController(AppDbContext db) => _db = db;

    private async Task<List<(int FazendaId, string FazendaNome, string Municipio, PainelTalhaoResumo Talhao)>>
        CarregarTalhoes(string? municipio)
    {
        var query = _db.Talhoes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(municipio))
            query = query.Where(t => t.Fazenda!.Municipio == municipio);

        var dados = await query.Select(t => new
        {
            t.Id, t.Nome, t.Cultura, t.UmidadeIdealMinima,
            t.FazendaId, FazendaNome = t.Fazenda!.Nome, t.Fazenda.Municipio,
            Ultima = _db.Leituras
                .Where(l => l.Sensor!.TalhaoId == t.Id)
                .OrderByDescending(l => l.DataHora)
                .Select(l => (double?)l.UmidadePercentual)
                .FirstOrDefault()
        }).ToListAsync();

        return dados.Select(d => (d.FazendaId, d.FazendaNome, d.Municipio, new PainelTalhaoResumo
        {
            TalhaoId = d.Id, Nome = d.Nome, Cultura = d.Cultura, UltimaUmidade = d.Ultima,
            Nivel = EstresseHidricoCalculator.Classificar(d.Ultima, d.UmidadeIdealMinima)
        })).ToList();
    }

    /// <summary>Estresse hídrico por fazenda, com o pior nível de cada uma. Filtro opcional por município.</summary>
    [HttpGet("estresse-hidrico")]
    public async Task<ActionResult<IEnumerable<PainelFazendaResponse>>> EstresseHidrico([FromQuery] string? municipio)
    {
        var talhoes = await CarregarTalhoes(municipio);

        var resposta = talhoes
            .GroupBy(x => new { x.FazendaId, x.FazendaNome, x.Municipio })
            .Select(g => new PainelFazendaResponse
            {
                FazendaId = g.Key.FazendaId,
                FazendaNome = g.Key.FazendaNome,
                Municipio = g.Key.Municipio,
                TotalTalhoes = g.Count(),
                TalhoesEmEstresse = g.Count(x => x.Talhao.Nivel >= NivelEstresseHidrico.Moderado),
                PiorNivel = g.Max(x => x.Talhao.Nivel),
                Talhoes = g.Select(x => x.Talhao).OrderByDescending(t => t.Nivel).ToList()
            })
            .OrderByDescending(f => f.PiorNivel)
            .ThenBy(f => f.FazendaNome)
            .ToList();

        return Ok(resposta);
    }

    /// <summary>Números gerais e indicador de impacto da solução.</summary>
    [HttpGet("resumo")]
    public async Task<ActionResult<PainelResumoResponse>> Resumo()
    {
        var talhoes = await CarregarTalhoes(null);
        var limite = DateTime.UtcNow.AddDays(-30);

        var evitadas = await _db.Recomendacoes.CountAsync(r =>
            r.CriadaEm >= limite && (r.Acao == AcaoIrrigacao.Aguardar || r.Acao == AcaoIrrigacao.NaoIrrigar));

        var resposta = new PainelResumoResponse
        {
            TotalFazendas = await _db.Fazendas.CountAsync(),
            TotalTalhoes = talhoes.Count,
            IrrigacoesEvitadasUltimos30Dias = evitadas,
            TalhoesPorNivel = Enum.GetValues<NivelEstresseHidrico>()
                .ToDictionary(n => n.ToString(), n => talhoes.Count(x => x.Talhao.Nivel == n))
        };

        return Ok(resposta);
    }
}
