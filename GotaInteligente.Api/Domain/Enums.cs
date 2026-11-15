namespace GotaInteligente.Api.Domain;

/// <summary>Decisão do modelo de IA para um talhão.</summary>
public enum AcaoIrrigacao
{
    Irrigar = 1,
    Aguardar = 2,
    NaoIrrigar = 3
}

/// <summary>Quanto menor a umidade em relação ao ideal, maior o estresse. A ordem importa (Max = pior caso).</summary>
public enum NivelEstresseHidrico
{
    SemDados = 0,
    Nenhum = 1,
    Leve = 2,
    Moderado = 3,
    Severo = 4
}
