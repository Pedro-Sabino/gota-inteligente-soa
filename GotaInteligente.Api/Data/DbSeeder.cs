using GotaInteligente.Api.Domain;

namespace GotaInteligente.Api.Data;

/// <summary>
/// Dados 100% SIMULADOS (nenhum dado pessoal real), conforme o combinado da GS.
/// </summary>
public static class DbSeeder
{
    public static void Popular(AppDbContext db)
    {
        if (db.Fazendas.Any()) return;

        var agora = DateTime.UtcNow;

        var sitio = new Fazenda
        {
            Nome = "Sítio Boa Esperança",
            Proprietario = "Agricultor Exemplo 1",
            Municipio = "São Paulo",
            Estado = "SP",
            AreaHectares = 4.5,
            Latitude = -23.8270,
            Longitude = -46.7290,
            Talhoes =
            {
                NovoTalhao("Horta Norte", "Alface", 0.8, 45, "ESP32-001", agora, new[] { 52.0, 48.5, 44.0, 38.0, 31.5 }),
                NovoTalhao("Horta Sul", "Tomate", 1.2, 40, "ESP32-002", agora, new[] { 55.0, 54.0, 52.5, 50.0, 49.0 })
            }
        };

        var chacara = new Fazenda
        {
            Nome = "Chácara Água Viva",
            Proprietario = "Agricultor Exemplo 2",
            Municipio = "São Paulo",
            Estado = "SP",
            AreaHectares = 2.0,
            Latitude = -23.8650,
            Longitude = -46.6780,
            Talhoes =
            {
                NovoTalhao("Canteiro Principal", "Couve", 0.5, 42, "ESP32-003", agora, new[] { 40.0, 33.0, 27.5, 21.0, 16.0 })
            }
        };

        db.Fazendas.AddRange(sitio, chacara);
        db.SaveChanges();

        // Uma recomendação de exemplo, como se tivesse vindo do modelo de IA.
        var primeiroTalhao = db.Talhoes.OrderBy(t => t.Id).First();
        db.Recomendacoes.Add(new RecomendacaoIrrigacao
        {
            TalhaoId = primeiroTalhao.Id,
            Acao = AcaoIrrigacao.NaoIrrigar,
            Motivo = "Bomba desligada: 90% de chance de chuva nas próximas 2 horas.",
            ProbabilidadeChuvaPercentual = 90,
            UmidadeSoloPercentual = 31.5,
            Origem = "seed-simulado",
            CriadaEm = agora
        });
        db.SaveChanges();
    }

    private static Talhao NovoTalhao(string nome, string cultura, double area, double umidadeIdeal,
        string codigoSensor, DateTime agora, double[] umidades)
    {
        var sensor = new Sensor { Codigo = codigoSensor, Ativo = true };

        // Leituras de hora em hora, da mais antiga para a mais recente.
        for (var i = 0; i < umidades.Length; i++)
        {
            sensor.Leituras.Add(new LeituraUmidade
            {
                UmidadePercentual = umidades[i],
                DataHora = agora.AddHours(-(umidades.Length - 1 - i))
            });
        }

        return new Talhao
        {
            Nome = nome,
            Cultura = cultura,
            AreaHectares = area,
            UmidadeIdealMinima = umidadeIdeal,
            Sensores = { sensor }
        };
    }
}
