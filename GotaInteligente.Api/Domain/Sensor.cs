namespace GotaInteligente.Api.Domain;

/// <summary>Representa um ESP32 com sensor de umidade de solo instalado em um talhão.</summary>
public class Sensor
{
    public int Id { get; set; }
    public int TalhaoId { get; set; }
    public Talhao? Talhao { get; set; }

    /// <summary>Identificador único do dispositivo (ex.: ESP32-001). É o que o firmware envia.</summary>
    public string Codigo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public List<LeituraUmidade> Leituras { get; set; } = new();
}
