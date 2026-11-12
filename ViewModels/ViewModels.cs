using System.ComponentModel.DataAnnotations;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo no válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo no válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}

public class CursoDetalleViewModel
{
    public Curso Curso { get; set; } = null!;
    public List<Leccion> Lecciones { get; set; } = new();
    public bool Inscrito { get; set; }
}

public class MiCursoViewModel
{
    public int CursoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public NivelCurso Nivel { get; set; }
    public int DuracionHoras { get; set; }
    public int TotalLecciones { get; set; }
    public int Completadas { get; set; }
    public int Porcentaje => TotalLecciones == 0 ? 0 : (int)Math.Round(Completadas * 100.0 / TotalLecciones);
}

public class MiCursoDetalleViewModel
{
    public Curso Curso { get; set; } = null!;
    public List<Leccion> Lecciones { get; set; } = new();
    public HashSet<int> CompletadasIds { get; set; } = new();
    public int Porcentaje => Lecciones.Count == 0 ? 0 : (int)Math.Round(CompletadasIds.Count * 100.0 / Lecciones.Count);
}

public class LeccionVerViewModel
{
    public Leccion Leccion { get; set; } = null!;
    public int? AnteriorId { get; set; }
    public int? SiguienteId { get; set; }
    public int Posicion { get; set; }
    public int Total { get; set; }
    public bool Completada { get; set; }
    public bool PuedeCompletar { get; set; }
}

public class LeccionesAdminViewModel
{
    public Curso Curso { get; set; } = null!;
    public List<Leccion> Lecciones { get; set; } = new();
}

public class SuscriptorItemViewModel
{
    public string Email { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public int Completadas { get; set; }
}

public class SuscriptoresViewModel
{
    public Curso Curso { get; set; } = null!;
    public int TotalLecciones { get; set; }
    public List<SuscriptorItemViewModel> Suscriptores { get; set; } = new();
}
