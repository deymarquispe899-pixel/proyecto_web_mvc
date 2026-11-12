using Microsoft.AspNetCore.Identity;

namespace PortalMicrocursos.Models;

public class LeccionCompletada
{
    public int Id { get; set; }

    public int LeccionId { get; set; }
    public Leccion? Leccion { get; set; }

    public string UsuarioId { get; set; } = string.Empty;
    public IdentityUser? Usuario { get; set; }

    public DateTime FechaCompletada { get; set; } = DateTime.UtcNow;
}
