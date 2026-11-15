using GotaInteligente.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace GotaInteligente.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Fazenda> Fazendas => Set<Fazenda>();
    public DbSet<Talhao> Talhoes => Set<Talhao>();
    public DbSet<Sensor> Sensores => Set<Sensor>();
    public DbSet<LeituraUmidade> Leituras => Set<LeituraUmidade>();
    public DbSet<RecomendacaoIrrigacao> Recomendacoes => Set<RecomendacaoIrrigacao>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Fazenda>(e =>
        {
            e.ToTable("Fazendas");
            e.Property(f => f.Nome).IsRequired().HasMaxLength(120);
            e.Property(f => f.Proprietario).IsRequired().HasMaxLength(120);
            e.Property(f => f.Municipio).IsRequired().HasMaxLength(80);
            e.Property(f => f.Estado).IsRequired().HasMaxLength(2);
            e.HasMany(f => f.Talhoes).WithOne(t => t.Fazenda!)
                .HasForeignKey(t => t.FazendaId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Talhao>(e =>
        {
            e.ToTable("Talhoes");
            e.Property(t => t.Nome).IsRequired().HasMaxLength(80);
            e.Property(t => t.Cultura).IsRequired().HasMaxLength(60);
            e.HasMany(t => t.Sensores).WithOne(s => s.Talhao!)
                .HasForeignKey(s => s.TalhaoId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(t => t.Recomendacoes).WithOne(r => r.Talhao!)
                .HasForeignKey(r => r.TalhaoId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Sensor>(e =>
        {
            e.ToTable("Sensores");
            e.Property(s => s.Codigo).IsRequired().HasMaxLength(40);
            e.HasIndex(s => s.Codigo).IsUnique();
            e.HasMany(s => s.Leituras).WithOne(l => l.Sensor!)
                .HasForeignKey(l => l.SensorId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<LeituraUmidade>(e =>
        {
            e.ToTable("Leituras");
            e.HasIndex(l => new { l.SensorId, l.DataHora });
        });

        mb.Entity<RecomendacaoIrrigacao>(e =>
        {
            e.ToTable("Recomendacoes");
            e.Property(r => r.Acao).HasConversion<string>().HasMaxLength(20);
            e.Property(r => r.Motivo).IsRequired().HasMaxLength(300);
            e.Property(r => r.Origem).IsRequired().HasMaxLength(60);
            e.HasIndex(r => new { r.TalhaoId, r.CriadaEm });
        });
    }
}
