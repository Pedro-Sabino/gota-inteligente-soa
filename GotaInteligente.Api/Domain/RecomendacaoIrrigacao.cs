namespace GotaInteligente.Api.Domain;

/// <summary>Decisão produzida pelo modelo de ML (Python) e consumida pelo app mobile.</summary>
public class RecomendacaoIrrigacao
{
    public long Id { get; set; }
    public int TalhaoId { get; set; }
    public Talhao? Talhao { get; set; }

    public AcaoIrrigacao Acao { get; set; }

    /// <summary>Texto exibido ao agricultor. Ex.: "Bomba desligada: 90% de chance de chuva em 2h".</summary>
    public string Motivo { get; set; } = string.Empty;

    public double ProbabilidadeChuvaPercentual { get; set; }
    public double UmidadeSoloPercentual { get; set; }
    public string Origem { get; set; } = "modelo-ml";
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
}
