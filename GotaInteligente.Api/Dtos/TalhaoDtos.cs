using System.ComponentModel.DataAnnotations;
using GotaInteligente.Api.Domain;

namespace GotaInteligente.Api.Dtos;

public class TalhaoRequest
{
    [Range(1, int.MaxValue)] public int FazendaId { get; set; }
    [Required, StringLength(80)] public string Nome { get; set; } = string.Empty;
    [Required, StringLength(60)] public string Cultura { get; set; } = string.Empty;
    [Range(0.01, 100000)] public double AreaHectares { get; set; }
    [Range(5, 95)] public double UmidadeIdealMinima { get; set; } = 40;
}

public class TalhaoResponse
{
    public int Id { get; set; }
    public int FazendaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cultura { get; set; } = string.Empty;
    public double AreaHectares { get; set; }
    public double UmidadeIdealMinima { get; set; }
    public double? UltimaUmidade { get; set; }
    public NivelEstresseHidrico NivelEstresseHidrico { get; set; }
}
