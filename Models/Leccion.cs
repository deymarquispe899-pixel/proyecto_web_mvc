using System.ComponentModel.DataAnnotations;

namespace PortalMicrocursos.Models;

public class Leccion
{
    public int Id { get; set; }

    public int CursoId { get; set; }
    public Curso? Curso { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es obligatorio.")]
    [Display(Name = "Contenido (texto o HTML)")]
    public string Contenido { get; set; } = string.Empty;

    public int Orden { get; set; }

    public ICollection<LeccionCompletada> Completadas { get; set; } = new List<LeccionCompletada>();
}
