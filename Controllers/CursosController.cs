using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;
using PortalMicrocursos.ViewModels;

namespace PortalMicrocursos.Controllers;

/// <summary>Catálogo público: cualquier visitante puede verlo.</summary>
public class CursosController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public CursosController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Cursos.AsNoTracking().Where(c => c.Publicado);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Titulo.Contains(q) || c.Descripcion.Contains(q));

        ViewBag.Q = q;
        return View(await query.OrderBy(c => c.Titulo).ToListAsync());
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var curso = await _db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Publicado);
        if (curso == null) return NotFound();

        // Solo se cargan los títulos; el contenido nunca se envía desde aquí
        var lecciones = await _db.Lecciones.AsNoTracking()
            .Where(l => l.CursoId == id)
            .OrderBy(l => l.Orden)
            .Select(l => new Leccion { Id = l.Id, Titulo = l.Titulo, Orden = l.Orden, CursoId = l.CursoId })
            .ToListAsync();

        var userId = _userManager.GetUserId(User);
        var inscrito = userId != null &&
            await _db.Inscripciones.AnyAsync(i => i.CursoId == id && i.UsuarioId == userId);

        return View(new CursoDetalleViewModel { Curso = curso, Lecciones = lecciones, Inscrito = inscrito });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RolesApp.Suscriptor)]
    public async Task<IActionResult> Inscribirse(int id)
    {
        var curso = await _db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.Publicado);
        if (curso == null) return NotFound();

        var userId = _userManager.GetUserId(User)!;

        var yaInscrito = await _db.Inscripciones.AnyAsync(i => i.CursoId == id && i.UsuarioId == userId);
        if (!yaInscrito)
        {
            _db.Inscripciones.Add(new Inscripcion { CursoId = id, UsuarioId = userId });
            try
            {
                await _db.SaveChangesAsync();
                TempData["Ok"] = $"Te inscribiste en «{curso.Titulo}».";
            }
            catch (DbUpdateException)
            {
                // El índice único evita duplicados si se envía dos veces
            }
        }

        return RedirectToAction("Detalle", "MisCursos", new { id });
    }
}
