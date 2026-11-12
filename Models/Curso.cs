using System.ComponentModel.DataAnnotations;

namespace PortalMicrocursos.Models;

public enum NivelCurso
{
    [Display(Name = "Básico")] Basico = 1,
    [Display(Name = "Intermedio")] Intermedio = 2,
    [Display(Name = "Avanzado")] Avanzado = 3
}

public static class NivelCursoExtensions
{
    public static string Nombre(this NivelCurso nivel) => nivel switch
    {
        NivelCurso.Basico => "Básico",
        NivelCurso.Intermedio => "Intermedio",
        NivelCurso.Avanzado => "Avanzado",
        _ => nivel.ToString()
    };
}

public class Curso
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(2000, ErrorMessage = "Máximo 2000 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Display(Name = "Nivel")]
    public NivelCurso Nivel { get; set; } = NivelCurso.Basico;

    [Range(1, 500, ErrorMessage = "La duración debe estar entre 1 y 500 horas.")]
    [Display(Name = "Duración (horas)")]
    public int DuracionHoras { get; set; } = 1;

    public bool Publicado { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Leccion> Lecciones { get; set; } = new List<Leccion>();
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
