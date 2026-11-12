using Microsoft.AspNetCore.Identity;

namespace PortalMicrocursos.Models;

public class Inscripcion
{
    public int Id { get; set; }

    public int CursoId { get; set; }
    public Curso? Curso { get; set; }

    public string UsuarioId { get; set; } = string.Empty;
    public IdentityUser? Usuario { get; set; }

    public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
}
