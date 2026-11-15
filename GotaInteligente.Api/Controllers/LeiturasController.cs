using GotaInteligente.Api.Data;
using GotaInteligente.Api.Domain;
using GotaInteligente.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GotaInteligente.Api.Controllers;

/// <summary>Endpoint de ingestão: é aqui que o ESP32 (Wokwi) envia as medições.</summary>
[ApiController]
[Route("api/[controller]")]
public class LeiturasController : ControllerBase
{
    private readonly AppDbContext _db;
    public LeiturasController(AppDbContext db) => _db = db;

    internal static LeituraResponse Mapear(LeituraUmidade l, Sensor s, double umidadeIdealMinima) => new()
    {
        Id = l.Id,
        SensorId = l.SensorId,
        SensorCodigo = s.Codigo,
        TalhaoId = s.TalhaoId,
        UmidadePercentual = l.UmidadePercentual,
        DataHora = l.DataHora,
        NivelEstresseHidrico = EstresseHidricoCalculator.Classificar(l.UmidadePercentual, umidadeIdealMinima)
    };

    [HttpPost]
    public async Task<ActionResult<LeituraResponse>> Registrar(LeituraRequest dto)
    {
        var sensor = await _db.Sensores
            .Include(s => s.Talhao)
            .FirstOrDefaultAsync(s => s.Codigo == dto.SensorCodigo);

        if (sensor is null)
            return NotFound(new ProblemDetails { Title = "Sensor não cadastrado", Status = 404 });

        if (!sensor.Ativo)
            return UnprocessableEntity(new ProblemDetails { Title = "Sensor desativado", Status = 422 });

        var leitura = new LeituraUmidade
        {
            SensorId = sensor.Id,
            UmidadePercentual = dto.UmidadePercentual,
            DataHora = (dto.DataHora ?? DateTime.UtcNow).ToUniversalTime()
        };
        _db.Leituras.Add(leitura);
        await _db.SaveChangesAsync();

        var resposta = Mapear(leitura, sensor, sensor.Talhao!.UmidadeIdealMinima);
        return Created($"/api/sensores/{sensor.Id}/leituras", resposta);
    }
}
