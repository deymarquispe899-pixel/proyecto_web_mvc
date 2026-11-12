using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;
using PortalMicrocursos.ViewModels;

namespace PortalMicrocursos.Controllers;

[Authorize(Roles = RolesApp.Suscriptor)]
public class MisCursosController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public MisCursosController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var cursos = await _db.Inscripciones
            .AsNoTracking()
            .Where(i => i.UsuarioId == userId && i.Curso!.Publicado)
            .OrderByDescending(i => i.FechaInscripcion)
            .Select(i => new MiCursoViewModel
            {
                CursoId = i.CursoId,
                Titulo = i.Curso!.Titulo,
                Nivel = i.Curso.Nivel,
                DuracionHoras = i.Curso.DuracionHoras,
                TotalLecciones = i.Curso.Lecciones.Count,
                Completadas = i.Curso.Lecciones.Count(l => l.Completadas.Any(lc => lc.UsuarioId == userId))
            })
            .ToListAsync();

        return View(cursos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var userId = _userManager.GetUserId(User)!;

        var inscrito = await _db.Inscripciones.AnyAsync(i => i.CursoId == id && i.UsuarioId == userId);
        if (!inscrito)
        {
            TempData["Error"] = "No estás inscrito en ese curso.";
            return RedirectToAction("Detalles", "Cursos", new { id });
        }

        var curso = await _db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.Publicado);
        if (curso == null) return NotFound();

        var lecciones = await _db.Lecciones.AsNoTracking()
            .Where(l => l.CursoId == id)
            .OrderBy(l => l.Orden)
            .Select(l => new Leccion { Id = l.Id, Titulo = l.Titulo, Orden = l.Orden, CursoId = l.CursoId })
            .ToListAsync();

        var completadas = await _db.LeccionesCompletadas.AsNoTracking()
            .Where(lc => lc.UsuarioId == userId && lc.Leccion!.CursoId == id)
            .Select(lc => lc.LeccionId)
            .ToListAsync();

        return View(new MiCursoDetalleViewModel
        {
            Curso = curso,
            Lecciones = lecciones,
            CompletadasIds = completadas.ToHashSet()
        });
    }
}
