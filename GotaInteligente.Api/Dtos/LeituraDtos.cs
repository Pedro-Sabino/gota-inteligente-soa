using System.ComponentModel.DataAnnotations;
using GotaInteligente.Api.Domain;

namespace GotaInteligente.Api.Dtos;

/// <summary>Payload que o ESP32 envia a cada medição.</summary>
public class LeituraRequest
{
    [Required, StringLength(40)] public string SensorCodigo { get; set; } = string.Empty;
    [Range(0, 100)] public double UmidadePercentual { get; set; }

    /// <summary>Opcional. Se omitido, a API usa o horário atual (UTC).</summary>
    public DateTime? DataHora { get; set; }
}

public class LeituraResponse
{
    public long Id { get; set; }
    public int SensorId { get; set; }
    public string SensorCodigo { get; set; } = string.Empty;
    public int TalhaoId { get; set; }
    public double UmidadePercentual { get; set; }
    public DateTime DataHora { get; set; }
    public NivelEstresseHidrico NivelEstresseHidrico { get; set; }
}
