using GotaInteligente.Api.Data;
using GotaInteligente.Api.Domain;
using GotaInteligente.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GotaInteligente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensoresController : ControllerBase
{
    private readonly AppDbContext _db;
    public SensoresController(AppDbContext db) => _db = db;

    private static SensorResponse Mapear(Sensor s, int totalLeituras) => new()
    {
        Id = s.Id, TalhaoId = s.TalhaoId, Codigo = s.Codigo, Ativo = s.Ativo, TotalLeituras = totalLeituras
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SensorResponse>>> Listar([FromQuery] int? talhaoId)
    {
        var query = _db.Sensores.AsNoTracking().AsQueryable();
        if (talhaoId is not null) query = query.Where(s => s.TalhaoId == talhaoId);

        var lista = await query.OrderBy(s => s.Codigo)
            .Select(s => new { Sensor = s, Total = s.Leituras.Count })
            .ToListAsync();

        return Ok(lista.Select(x => Mapear(x.Sensor, x.Total)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SensorResponse>> Obter(int id)
    {
        var item = await _db.Sensores.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new { Sensor = s, Total = s.Leituras.Count })
            .FirstOrDefaultAsync();

        return item is null ? NotFound() : Ok(Mapear(item.Sensor, item.Total));
    }

    /// <summary>Histórico de leituras do sensor (mais recentes primeiro).</summary>
    [HttpGet("{id:int}/leituras")]
    public async Task<ActionResult<IEnumerable<LeituraResponse>>> ListarLeituras(int id, [FromQuery] int limite = 50)
    {
        var sensor = await _db.Sensores.AsNoTracking()
            .Include(s => s.Talhao)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sensor is null) return NotFound();

        limite = Math.Clamp(limite, 1, 500);

        var leituras = await _db.Leituras.AsNoTracking()
            .Where(l => l.SensorId == id)
            .OrderByDescending(l => l.DataHora)
            .Take(limite)
            .ToListAsync();

        var ideal = sensor.Talhao!.UmidadeIdealMinima;
        return Ok(leituras.Select(l => LeiturasController.Mapear(l, sensor, ideal)));
    }

    [HttpPost]
    public async Task<ActionResult<SensorResponse>> Criar(SensorRequest dto)
    {
        if (!await _db.Talhoes.AnyAsync(t => t.Id == dto.TalhaoId))
            return NotFound(new ProblemDetails { Title = "Talhão não encontrado", Status = 404 });

        if (await _db.Sensores.AnyAsync(s => s.Codigo == dto.Codigo))
            return Conflict(new ProblemDetails { Title = "Já existe um sensor com esse código", Status = 409 });

        var sensor = new Sensor { TalhaoId = dto.TalhaoId, Codigo = dto.Codigo, Ativo = dto.Ativo };
        _db.Sensores.Add(sensor);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = sensor.Id }, Mapear(sensor, 0));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, SensorRequest dto)
    {
        var sensor = await _db.Sensores.FindAsync(id);
        if (sensor is null) return NotFound();

        if (!await _db.Talhoes.AnyAsync(t => t.Id == dto.TalhaoId))
            return NotFound(new ProblemDetails { Title = "Talhão não encontrado", Status = 404 });

        if (await _db.Sensores.AnyAsync(s => s.Codigo == dto.Codigo && s.Id != id))
            return Conflict(new ProblemDetails { Title = "Já existe um sensor com esse código", Status = 409 });

        sensor.TalhaoId = dto.TalhaoId;
        sensor.Codigo = dto.Codigo;
        sensor.Ativo = dto.Ativo;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var sensor = await _db.Sensores.FindAsync(id);
        if (sensor is null) return NotFound();

        _db.Sensores.Remove(sensor);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
