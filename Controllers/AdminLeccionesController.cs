using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;
using PortalMicrocursos.ViewModels;

namespace PortalMicrocursos.Controllers;

[Authorize(Roles = RolesApp.Admin)]
public class AdminLeccionesController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminLeccionesController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int cursoId)
    {
        var curso = await _db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return NotFound();

        var lecciones = await _db.Lecciones.AsNoTracking()
            .Where(l => l.CursoId == cursoId)
            .OrderBy(l => l.Orden)
            .ToListAsync();

        return View(new LeccionesAdminViewModel { Curso = curso, Lecciones = lecciones });
    }

    [HttpGet]
    public async Task<IActionResult> Crear(int cursoId)
    {
        var curso = await _db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return NotFound();

        ViewBag.CursoTitulo = curso.Titulo;
        return View(new Leccion { CursoId = cursoId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(int cursoId, [Bind("Titulo,Contenido")] Leccion leccion)
    {
        var curso = await _db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return NotFound();

        if (!ModelState.IsValid)
        {
            leccion.CursoId = cursoId;
            ViewBag.CursoTitulo = curso.Titulo;
            return View(leccion);
        }

        var maxOrden = await _db.Lecciones.Where(l => l.CursoId == cursoId)
            .Select(l => (int?)l.Orden).MaxAsync() ?? 0;

        leccion.CursoId = cursoId;
        leccion.Orden = maxOrden + 1;
        _db.Lecciones.Add(leccion);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Lección agregada.";
        return RedirectToAction(nameof(Index), new { cursoId });
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var leccion = await _db.Lecciones.Include(l => l.Curso).FirstOrDefaultAsync(l => l.Id == id);
        if (leccion == null) return NotFound();

        ViewBag.CursoTitulo = leccion.Curso!.Titulo;
        return View(leccion);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, [Bind("Titulo,Contenido")] Leccion input)
    {
        var leccion = await _db.Lecciones.Include(l => l.Curso).FirstOrDefaultAsync(l => l.Id == id);
        if (leccion == null) return NotFound();

        if (!ModelState.IsValid)
        {
            input.Id = id;
            input.CursoId = leccion.CursoId;
            ViewBag.CursoTitulo = leccion.Curso!.Titulo;
            return View(input);
        }

        leccion.Titulo = input.Titulo;
        leccion.Contenido = input.Contenido;
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Lección actualizada.";
        return RedirectToAction(nameof(Index), new { cursoId = leccion.CursoId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var leccion = await _db.Lecciones.FindAsync(id);
        if (leccion == null) return NotFound();

        var cursoId = leccion.CursoId;
        _db.Lecciones.Remove(leccion);
        await _db.SaveChangesAsync();
        await ReordenarAsync(cursoId);

        TempData["Ok"] = "Lección eliminada.";
        return RedirectToAction(nameof(Index), new { cursoId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Mover(int id, string direccion)
    {
        var leccion = await _db.Lecciones.FindAsync(id);
        if (leccion == null) return NotFound();

        var lista = await _db.Lecciones
            .Where(l => l.CursoId == leccion.CursoId)
            .OrderBy(l => l.Orden).ThenBy(l => l.Id)
            .ToListAsync();

        var idx = lista.FindIndex(l => l.Id == id);
        var destino = direccion == "subir" ? idx - 1 : idx + 1;

        if (idx >= 0 && destino >= 0 && destino < lista.Count)
        {
            (lista[idx], lista[destino]) = (lista[destino], lista[idx]);
            for (var i = 0; i < lista.Count; i++)
                lista[i].Orden = i + 1;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index), new { cursoId = leccion.CursoId });
    }

    private async Task ReordenarAsync(int cursoId)
    {
        var lista = await _db.Lecciones
            .Where(l => l.CursoId == cursoId)
            .OrderBy(l => l.Orden).ThenBy(l => l.Id)
            .ToListAsync();

        for (var i = 0; i < lista.Count; i++)
            lista[i].Orden = i + 1;

        await _db.SaveChangesAsync();
    }
}
