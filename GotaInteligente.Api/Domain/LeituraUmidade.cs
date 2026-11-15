namespace GotaInteligente.Api.Domain;

public class LeituraUmidade
{
    public long Id { get; set; }
    public int SensorId { get; set; }
    public Sensor? Sensor { get; set; }

    /// <summary>Umidade do solo em %, de 0 a 100.</summary>
    public double UmidadePercentual { get; set; }

    /// <summary>Sempre em UTC.</summary>
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
}
