using GotaInteligente.Api.Domain;

namespace GotaInteligente.Api.Dtos;

/// <summary>Consumido pelo painel WinForms do Sindicato Rural / Prefeitura.</summary>
public class PainelFazendaResponse
{
    public int FazendaId { get; set; }
    public string FazendaNome { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public int TotalTalhoes { get; set; }
    public int TalhoesEmEstresse { get; set; }
    public NivelEstresseHidrico PiorNivel { get; set; }
    public List<PainelTalhaoResumo> Talhoes { get; set; } = new();
}

public class PainelTalhaoResumo
{
    public int TalhaoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cultura { get; set; } = string.Empty;
    public double? UltimaUmidade { get; set; }
    public NivelEstresseHidrico Nivel { get; set; }
}

public class PainelResumoResponse
{
    public int TotalFazendas { get; set; }
    public int TotalTalhoes { get; set; }
    public Dictionary<string, int> TalhoesPorNivel { get; set; } = new();

    /// <summary>Indicador de impacto: recomendações "Aguardar"/"NaoIrrigar" nos últimos 30 dias.</summary>
    public int IrrigacoesEvitadasUltimos30Dias { get; set; }
}
