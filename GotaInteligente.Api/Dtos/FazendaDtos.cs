using System.ComponentModel.DataAnnotations;

namespace GotaInteligente.Api.Dtos;

public class FazendaRequest
{
    [Required, StringLength(120)] public string Nome { get; set; } = string.Empty;
    [Required, StringLength(120)] public string Proprietario { get; set; } = string.Empty;
    [Required, StringLength(80)] public string Municipio { get; set; } = string.Empty;
    [Required, StringLength(2, MinimumLength = 2)] public string Estado { get; set; } = string.Empty;
    [Range(0.01, 100000)] public double AreaHectares { get; set; }
    [Range(-90, 90)] public double? Latitude { get; set; }
    [Range(-180, 180)] public double? Longitude { get; set; }
}

public class FazendaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Proprietario { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public double AreaHectares { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int QuantidadeTalhoes { get; set; }
}
