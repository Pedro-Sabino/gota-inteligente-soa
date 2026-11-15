namespace GotaInteligente.Api.Domain;

public class Fazenda
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Proprietario { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public double AreaHectares { get; set; }

    /// <summary>Coordenadas da propriedade, usadas pela IA para buscar a previsão do tempo do local (migration 2).</summary>
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public List<Talhao> Talhoes { get; set; } = new();
}
