using GotaInteligente.Api.Data;
using GotaInteligente.Api.Domain;
using GotaInteligente.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GotaInteligente.Api.Controllers;

/// <summary>
/// Recebe as decisões do modelo de IA (POST) e as entrega ao app do agricultor (GET).
/// </summary>
[ApiController]
[Route("api")]
public class RecomendacoesController : ControllerBase
{
    private readonly AppDbContext _db;
    public RecomendacoesController(AppDbContext db) => _db = db;

    private static RecomendacaoResponse Mapear(RecomendacaoIrrigacao r) => new()
    {
        Id = r.Id, TalhaoId = r.TalhaoId, Acao = r.Acao, Motivo = r.Motivo,
        ProbabilidadeChuvaPercentual = r.ProbabilidadeChuvaPercentual,
        UmidadeSoloPercentual = r.UmidadeSoloPercentual, Origem = r.Origem, CriadaEm = r.CriadaEm
    };

    [HttpPost("recomendacoes")]
    public async Task<ActionResult<RecomendacaoResponse>> Criar(RecomendacaoRequest dto)
    {
        if (!await _db.Talhoes.AnyAsync(t => t.Id == dto.TalhaoId))
            return NotFound(new ProblemDetails { Title = "Talhão não encontrado", Status = 404 });

        var rec = new RecomendacaoIrrigacao
        {
            TalhaoId = dto.TalhaoId,
            Acao = dto.Acao,
            Motivo = dto.Motivo,
            ProbabilidadeChuvaPercentual = dto.ProbabilidadeChuvaPercentual,
            UmidadeSoloPercentual = dto.UmidadeSoloPercentual,
            Origem = string.IsNullOrWhiteSpace(dto.Origem) ? "modelo-ml" : dto.Origem,
            CriadaEm = DateTime.UtcNow
        };
        _db.Recomendacoes.Add(rec);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterUltima), new { talhaoId = rec.TalhaoId }, Mapear(rec));
    }

    /// <summary>Histórico de recomendações do talhão (mais recentes primeiro).</summary>
    [HttpGet("talhoes/{talhaoId:int}/recomendacoes")]
    public async Task<ActionResult<IEnumerable<RecomendacaoResponse>>> Listar(int talhaoId, [FromQuery] int limite = 20)
    {
        if (!await _db.Talhoes.AnyAsync(t => t.Id == talhaoId)) return NotFound();

        limite = Math.Clamp(limite, 1, 200);
        var lista = await _db.Recomendacoes.AsNoTracking()
            .Where(r => r.TalhaoId == talhaoId)
            .OrderByDescending(r => r.CriadaEm)
            .Take(limite)
            .ToListAsync();

        return Ok(lista.Select(Mapear));
    }

    /// <summary>Alerta mais recente — é o que o app mobile exibe ao agricultor.</summary>
    [HttpGet("talhoes/{talhaoId:int}/recomendacoes/ultima")]
    public async Task<ActionResult<RecomendacaoResponse>> ObterUltima(int talhaoId)
    {
        var rec = await _db.Recomendacoes.AsNoTracking()
            .Where(r => r.TalhaoId == talhaoId)
            .OrderByDescending(r => r.CriadaEm)
            .FirstOrDefaultAsync();

        return rec is null ? NotFound() : Ok(Mapear(rec));
    }
}
