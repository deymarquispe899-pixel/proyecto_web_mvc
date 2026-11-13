using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;
using PortalMicrocursos.ViewModels;

namespace PortalMicrocursos.Controllers;

[Authorize(Roles = RolesApp.Admin)]
public class AdminCursosController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminCursosController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var cursos = await _db.Cursos.AsNoTracking()
            .Include(c => c.Lecciones)
            .Include(c => c.Inscripciones)
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync();
        return View(cursos);
    }

    [HttpGet]
    public IActionResult Crear() => View(new Curso());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear([Bind("Titulo,Descripcion,Nivel,DuracionHoras")] Curso curso)
    {
        if (!ModelState.IsValid) return View(curso);

        curso.Publicado = false;
        curso.FechaCreacion = DateTime.UtcNow;
        _db.Cursos.Add(curso);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Curso creado. Agrega sus lecciones y luego publícalo.";
        return RedirectToAction("Index", "AdminLecciones", new { cursoId = curso.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var curso = await _db.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        return View(curso);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, [Bind("Titulo,Descripcion,Nivel,DuracionHoras")] Curso input)
    {
        var curso = await _db.Cursos.FindAsync(id);
        if (curso == null) return NotFound();

        if (!ModelState.IsValid)
        {
            input.Id = id;
            return View(input);
        }

        curso.Titulo = input.Titulo;
        curso.Descripcion = input.Descripcion;
        curso.Nivel = input.Nivel;
        curso.DuracionHoras = input.DuracionHoras;
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Curso actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var curso = await _db.Cursos.AsNoTracking()
            .Include(c => c.Lecciones)
            .Include(c => c.Inscripciones)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();
        return View(curso);
    }

    [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var curso = await _db.Cursos.FindAsync(id);
        if (curso == null) return NotFound();

        _db.Cursos.Remove(curso);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Curso eliminado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Publicar(int id)
    {
        var curso = await _db.Cursos.Include(c => c.Lecciones).FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();

        if (!curso.Publicado && curso.Lecciones.Count == 0)
        {
            TempData["Error"] = "Agrega al menos una lección antes de publicar el curso.";
            return RedirectToAction(nameof(Index));
        }

        curso.Publicado = !curso.Publicado;
        await _db.SaveChangesAsync();

        TempData["Ok"] = curso.Publicado ? "Curso publicado." : "Curso retirado del catálogo.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Suscriptores(int id)
    {
        var curso = await _db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return NotFound();

        var totalLecciones = await _db.Lecciones.CountAsync(l => l.CursoId == id);

        var progreso = await _db.LeccionesCompletadas.AsNoTracking()
            .Where(lc => lc.Leccion!.CursoId == id)
            .GroupBy(lc => lc.UsuarioId)
            .Select(g => new { UsuarioId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.UsuarioId, x => x.Total);

        var inscripciones = await _db.Inscripciones.AsNoTracking()
            .Where(i => i.CursoId == id)
            .Include(i => i.Usuario)
            .OrderBy(i => i.FechaInscripcion)
            .ToListAsync();

        var vm = new SuscriptoresViewModel
        {
            Curso = curso,
            TotalLecciones = totalLecciones,
            Suscriptores = inscripciones.Select(i => new SuscriptorItemViewModel
            {
                Email = i.Usuario?.Email ?? "(sin correo)",
                FechaInscripcion = i.FechaInscripcion,
                Completadas = progreso.TryGetValue(i.UsuarioId, out var n) ? n : 0
            }).ToList()
        };

        return View(vm);
    }
}
