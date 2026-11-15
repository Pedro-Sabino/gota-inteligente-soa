using GotaInteligente.Api.Data;
using GotaInteligente.Api.Domain;
using GotaInteligente.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GotaInteligente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TalhoesController : ControllerBase
{
    private readonly AppDbContext _db;
    public TalhoesController(AppDbContext db) => _db = db;

    /// <summary>Projeta talhões já com a última umidade lida (subconsulta traduzida para SQL).</summary>
    private IQueryable<TalhaoResponse> Projetar(IQueryable<Talhao> query) =>
        query.Select(t => new TalhaoResponse
        {
            Id = t.Id,
            FazendaId = t.FazendaId,
            Nome = t.Nome,
            Cultura = t.Cultura,
            AreaHectares = t.AreaHectares,
            UmidadeIdealMinima = t.UmidadeIdealMinima,
            UltimaUmidade = _db.Leituras
                .Where(l => l.Sensor!.TalhaoId == t.Id)
                .OrderByDescending(l => l.DataHora)
                .Select(l => (double?)l.UmidadePercentual)
                .FirstOrDefault()
        });

    private static TalhaoResponse ComNivel(TalhaoResponse r)
    {
        r.NivelEstresseHidrico = EstresseHidricoCalculator.Classificar(r.UltimaUmidade, r.UmidadeIdealMinima);
        return r;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TalhaoResponse>>> Listar([FromQuery] int? fazendaId)
    {
        var query = _db.Talhoes.AsNoTracking().AsQueryable();
        if (fazendaId is not null) query = query.Where(t => t.FazendaId == fazendaId);

        var lista = await Projetar(query.OrderBy(t => t.Nome)).ToListAsync();
        return Ok(lista.Select(ComNivel));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TalhaoResponse>> Obter(int id)
    {
        var item = await Projetar(_db.Talhoes.AsNoTracking().Where(t => t.Id == id)).FirstOrDefaultAsync();
        return item is null ? NotFound() : Ok(ComNivel(item));
    }

    [HttpPost]
    public async Task<ActionResult<TalhaoResponse>> Criar(TalhaoRequest dto)
    {
        if (!await _db.Fazendas.AnyAsync(f => f.Id == dto.FazendaId))
            return NotFound(new ProblemDetails { Title = "Fazenda não encontrada", Status = 404 });

        var talhao = new Talhao
        {
            FazendaId = dto.FazendaId, Nome = dto.Nome, Cultura = dto.Cultura,
            AreaHectares = dto.AreaHectares, UmidadeIdealMinima = dto.UmidadeIdealMinima
        };
        _db.Talhoes.Add(talhao);
        await _db.SaveChangesAsync();

        var resposta = await Projetar(_db.Talhoes.AsNoTracking().Where(t => t.Id == talhao.Id)).FirstAsync();
        return CreatedAtAction(nameof(Obter), new { id = talhao.Id }, ComNivel(resposta));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, TalhaoRequest dto)
    {
        var talhao = await _db.Talhoes.FindAsync(id);
        if (talhao is null) return NotFound();

        if (!await _db.Fazendas.AnyAsync(f => f.Id == dto.FazendaId))
            return NotFound(new ProblemDetails { Title = "Fazenda não encontrada", Status = 404 });

        talhao.FazendaId = dto.FazendaId;
        talhao.Nome = dto.Nome;
        talhao.Cultura = dto.Cultura;
        talhao.AreaHectares = dto.AreaHectares;
        talhao.UmidadeIdealMinima = dto.UmidadeIdealMinima;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var talhao = await _db.Talhoes.FindAsync(id);
        if (talhao is null) return NotFound();

        _db.Talhoes.Remove(talhao);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
