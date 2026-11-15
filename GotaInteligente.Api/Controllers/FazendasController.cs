using GotaInteligente.Api.Data;
using GotaInteligente.Api.Domain;
using GotaInteligente.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GotaInteligente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FazendasController : ControllerBase
{
    private readonly AppDbContext _db;
    public FazendasController(AppDbContext db) => _db = db;

    private static FazendaResponse Mapear(Fazenda f, int qtdTalhoes) => new()
    {
        Id = f.Id, Nome = f.Nome, Proprietario = f.Proprietario, Municipio = f.Municipio,
        Estado = f.Estado, AreaHectares = f.AreaHectares,
        Latitude = f.Latitude, Longitude = f.Longitude, QuantidadeTalhoes = qtdTalhoes
    };

    /// <summary>Lista fazendas. Filtro opcional por município.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FazendaResponse>>> Listar([FromQuery] string? municipio)
    {
        var query = _db.Fazendas.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(municipio))
            query = query.Where(f => f.Municipio == municipio);

        var fazendas = await query
            .OrderBy(f => f.Nome)
            .Select(f => new { Fazenda = f, Qtd = f.Talhoes.Count })
            .ToListAsync();

        return Ok(fazendas.Select(x => Mapear(x.Fazenda, x.Qtd)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FazendaResponse>> Obter(int id)
    {
        var item = await _db.Fazendas.AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new { Fazenda = f, Qtd = f.Talhoes.Count })
            .FirstOrDefaultAsync();

        return item is null ? NotFound() : Ok(Mapear(item.Fazenda, item.Qtd));
    }

    [HttpPost]
    public async Task<ActionResult<FazendaResponse>> Criar(FazendaRequest dto)
    {
        var fazenda = new Fazenda
        {
            Nome = dto.Nome, Proprietario = dto.Proprietario, Municipio = dto.Municipio,
            Estado = dto.Estado.ToUpperInvariant(), AreaHectares = dto.AreaHectares,
            Latitude = dto.Latitude, Longitude = dto.Longitude
        };
        _db.Fazendas.Add(fazenda);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = fazenda.Id }, Mapear(fazenda, 0));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, FazendaRequest dto)
    {
        var fazenda = await _db.Fazendas.FindAsync(id);
        if (fazenda is null) return NotFound();

        fazenda.Nome = dto.Nome;
        fazenda.Proprietario = dto.Proprietario;
        fazenda.Municipio = dto.Municipio;
        fazenda.Estado = dto.Estado.ToUpperInvariant();
        fazenda.AreaHectares = dto.AreaHectares;
        fazenda.Latitude = dto.Latitude;
        fazenda.Longitude = dto.Longitude;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var fazenda = await _db.Fazendas.FindAsync(id);
        if (fazenda is null) return NotFound();

        _db.Fazendas.Remove(fazenda); // cascata remove talhões, sensores, leituras e recomendações
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
