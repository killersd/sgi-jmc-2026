using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SGI_JMC.Api.Models;

namespace SGI_JMC.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Declaracao> Declaracoes => Set<Declaracao>();
    public DbSet<Oficio> Oficios => Set<Oficio>();
    public DbSet<OficioFuncao> OficiosFuncao => Set<OficioFuncao>();
    public DbSet<PerfilPermissao> PerfilPermissoes => Set<PerfilPermissao>();
    public DbSet<HorarioProfessor> HorariosProfessor => Set<HorarioProfessor>();
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<ProfessorDisciplina> ProfessorDisciplinas => Set<ProfessorDisciplina>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Advertencia> Advertencias => Set<Advertencia>();
    public DbSet<Suspensao> Suspensoes => Set<Suspensao>();
    public DbSet<ConfiguracaoNotificacoes> ConfiguracoesNotificacoes => Set<ConfiguracaoNotificacoes>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Aluno>(entity =>
        {
            entity.HasIndex(a => a.CodigoSeed).IsUnique();
            entity.HasIndex(a => new { a.AnoLetivo, a.AnoSerie, a.Turma });
        });

        builder.Entity<HorarioProfessor>(entity =>
        {
            entity.Property(h => h.GradeJson).HasColumnType("jsonb");
        });

        builder.Entity<Disciplina>(entity =>
        {
            entity.HasIndex(d => d.Nome).IsUnique();
        });

        builder.Entity<Turma>(entity =>
        {
            entity.HasIndex(t => t.Nome).IsUnique();
        });

        builder.Entity<PerfilPermissao>(entity =>
        {
            entity.HasKey(p => new { p.Perfil, p.ModuloChave });
        });

        builder.Entity<ProfessorDisciplina>(entity =>
        {
            entity.HasKey(pd => new { pd.ProfessorId, pd.DisciplinaId });

            entity.HasOne(pd => pd.Professor)
                .WithMany(p => p.ProfessorDisciplinas)
                .HasForeignKey(pd => pd.ProfessorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pd => pd.Disciplina)
                .WithMany(d => d.ProfessorDisciplinas)
                .HasForeignKey(pd => pd.DisciplinaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
