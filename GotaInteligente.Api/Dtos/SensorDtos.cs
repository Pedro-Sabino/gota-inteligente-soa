using System.ComponentModel.DataAnnotations;

namespace GotaInteligente.Api.Dtos;

public class SensorRequest
{
    [Range(1, int.MaxValue)] public int TalhaoId { get; set; }
    [Required, StringLength(40)] public string Codigo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public class SensorResponse
{
    public int Id { get; set; }
    public int TalhaoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public int TotalLeituras { get; set; }
}
