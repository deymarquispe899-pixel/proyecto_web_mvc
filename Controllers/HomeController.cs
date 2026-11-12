using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;

namespace PortalMicrocursos.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var recientes = await _db.Cursos
            .AsNoTracking()
            .Where(c => c.Publicado)
            .OrderByDescending(c => c.FechaCreacion)
            .Take(3)
            .ToListAsync();

        return View(recientes);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
