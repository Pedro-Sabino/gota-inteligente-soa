namespace GotaInteligente.Api.Domain;

/// <summary>Regra de negócio central: transforma umidade atual em nível de estresse hídrico.</summary>
public static class EstresseHidricoCalculator
{
    public static NivelEstresseHidrico Classificar(double? umidadeAtual, double umidadeIdealMinima)
    {
        if (umidadeAtual is null || umidadeIdealMinima <= 0)
            return NivelEstresseHidrico.SemDados;

        var razao = umidadeAtual.Value / umidadeIdealMinima;

        if (razao >= 1.0) return NivelEstresseHidrico.Nenhum;
        if (razao >= 0.75) return NivelEstresseHidrico.Leve;
        if (razao >= 0.5) return NivelEstresseHidrico.Moderado;
        return NivelEstresseHidrico.Severo;
    }
}
