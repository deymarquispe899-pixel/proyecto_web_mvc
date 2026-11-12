using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Leccion> Lecciones => Set<Leccion>();
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();
    public DbSet<LeccionCompletada> LeccionesCompletadas => Set<LeccionCompletada>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Curso>(e =>
        {
            e.Property(c => c.Titulo).HasMaxLength(150).IsRequired();
            e.Property(c => c.Descripcion).HasMaxLength(2000).IsRequired();
            e.HasIndex(c => c.Publicado);
        });

        builder.Entity<Leccion>(e =>
        {
            e.Property(l => l.Titulo).HasMaxLength(150).IsRequired();
            e.HasOne(l => l.Curso)
             .WithMany(c => c.Lecciones)
             .HasForeignKey(l => l.CursoId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(l => new { l.CursoId, l.Orden });
        });

        // Un usuario solo puede inscribirse una vez en cada curso
        builder.Entity<Inscripcion>(e =>
        {
            e.HasOne(i => i.Curso)
             .WithMany(c => c.Inscripciones)
             .HasForeignKey(i => i.CursoId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(i => i.Usuario)
             .WithMany()
             .HasForeignKey(i => i.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(i => new { i.CursoId, i.UsuarioId }).IsUnique();
        });

        // Una lección solo se marca una vez como completada por usuario
        builder.Entity<LeccionCompletada>(e =>
        {
            e.HasOne(lc => lc.Leccion)
             .WithMany(l => l.Completadas)
             .HasForeignKey(lc => lc.LeccionId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(lc => lc.Usuario)
             .WithMany()
             .HasForeignKey(lc => lc.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(lc => new { lc.LeccionId, lc.UsuarioId }).IsUnique();
        });
    }
}
