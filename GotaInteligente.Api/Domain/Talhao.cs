namespace GotaInteligente.Api.Domain;

/// <summary>Área da fazenda com uma cultura e um ou mais sensores de umidade.</summary>
public class Talhao
{
    public int Id { get; set; }
    public int FazendaId { get; set; }
    public Fazenda? Fazenda { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Cultura { get; set; } = string.Empty;
    public double AreaHectares { get; set; }

    /// <summary>Umidade do solo (%) abaixo da qual a cultura começa a sofrer.</summary>
    public double UmidadeIdealMinima { get; set; } = 40;

    public List<Sensor> Sensores { get; set; } = new();
    public List<RecomendacaoIrrigacao> Recomendacoes { get; set; } = new();
}
