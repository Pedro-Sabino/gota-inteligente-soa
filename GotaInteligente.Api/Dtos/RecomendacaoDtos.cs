using System.ComponentModel.DataAnnotations;
using GotaInteligente.Api.Domain;

namespace GotaInteligente.Api.Dtos;

/// <summary>Payload que o script de IA (Python) envia depois de decidir.</summary>
public class RecomendacaoRequest
{
    [Range(1, int.MaxValue)] public int TalhaoId { get; set; }
    [EnumDataType(typeof(AcaoIrrigacao))] public AcaoIrrigacao Acao { get; set; }
    [Required, StringLength(300)] public string Motivo { get; set; } = string.Empty;
    [Range(0, 100)] public double ProbabilidadeChuvaPercentual { get; set; }
    [Range(0, 100)] public double UmidadeSoloPercentual { get; set; }
    [StringLength(60)] public string Origem { get; set; } = "modelo-ml";
}

public class RecomendacaoResponse
{
    public long Id { get; set; }
    public int TalhaoId { get; set; }
    public AcaoIrrigacao Acao { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public double ProbabilidadeChuvaPercentual { get; set; }
    public double UmidadeSoloPercentual { get; set; }
    public string Origem { get; set; } = string.Empty;
    public DateTime CriadaEm { get; set; }
}
