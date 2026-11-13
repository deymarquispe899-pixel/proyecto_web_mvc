using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;
using PortalMicrocursos.ViewModels;

namespace PortalMicrocursos.Controllers;

[Authorize]
public class LeccionesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public LeccionesController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Ver(int id)
    {
        var leccion = await _db.Lecciones.AsNoTracking()
            .Include(l => l.Curso)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (leccion == null) return NotFound();

        var userId = _userManager.GetUserId(User)!;
        var esAdmin = User.IsInRole(RolesApp.Admin);

        if (!esAdmin)
        {
            if (!leccion.Curso!.Publicado) return NotFound();

            var inscrito = await _db.Inscripciones
                .AnyAsync(i => i.CursoId == leccion.CursoId && i.UsuarioId == userId);
            if (!inscrito)
            {
                TempData["Error"] = "Debes inscribirte en el curso para acceder a sus lecciones.";
                return RedirectToAction("Detalles", "Cursos", new { id = leccion.CursoId });
            }
        }

        var ids = await _db.Lecciones.AsNoTracking()
            .Where(l => l.CursoId == leccion.CursoId)
            .OrderBy(l => l.Orden)
            .Select(l => l.Id)
            .ToListAsync();
        var idx = ids.IndexOf(id);

        var completada = !esAdmin &&
            await _db.LeccionesCompletadas.AnyAsync(lc => lc.LeccionId == id && lc.UsuarioId == userId);

        return View(new LeccionVerViewModel
        {
            Leccion = leccion,
            AnteriorId = idx > 0 ? ids[idx - 1] : null,
            SiguienteId = idx >= 0 && idx < ids.Count - 1 ? ids[idx + 1] : null,
            Posicion = idx + 1,
            Total = ids.Count,
            Completada = completada,
            PuedeCompletar = !esAdmin
        });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RolesApp.Suscriptor)]
    public async Task<IActionResult> Completar(int id)
    {
        var leccion = await _db.Lecciones.AsNoTracking()
            .Include(l => l.Curso)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (leccion == null || !leccion.Curso!.Publicado) return NotFound();

        var userId = _userManager.GetUserId(User)!;

        var inscrito = await _db.Inscripciones
            .AnyAsync(i => i.CursoId == leccion.CursoId && i.UsuarioId == userId);
        if (!inscrito) return Forbid();

        var yaCompletada = await _db.LeccionesCompletadas
            .AnyAsync(lc => lc.LeccionId == id && lc.UsuarioId == userId);
        if (!yaCompletada)
        {
            _db.LeccionesCompletadas.Add(new LeccionCompletada { LeccionId = id, UsuarioId = userId });
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
            }
        }

        TempData["Ok"] = "Lección marcada como completada.";
        return RedirectToAction(nameof(Ver), new { id });
    }
}
