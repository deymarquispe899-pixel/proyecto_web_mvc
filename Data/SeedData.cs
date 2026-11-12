using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Data;

public static class SeedData
{
    public const string AdminEmail = "admin@univalle.edu";
    public const string AdminPassword = "Admin123";

    public static async Task InicializarAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        // Si hay migraciones se aplican; si no, se crea el esquema directamente.
        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync();
        else
            await db.Database.EnsureCreatedAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var rol in new[] { RolesApp.Admin, RolesApp.Suscriptor })
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, AdminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, RolesApp.Admin);
        }

        if (!await db.Cursos.AnyAsync())
        {
            var csharp = new Curso
            {
                Titulo = "Introducción a C#",
                Descripcion = "Aprende los fundamentos del lenguaje C#: variables, control de flujo, métodos y clases.",
                Nivel = NivelCurso.Basico,
                DuracionHoras = 6,
                Publicado = true,
                Lecciones =
                {
                    new Leccion { Orden = 1, Titulo = "¿Qué es C#?", Contenido = "<p>C# es un lenguaje moderno, orientado a objetos y fuertemente tipado desarrollado por Microsoft.</p><p>Se ejecuta sobre la plataforma <strong>.NET</strong>.</p>" },
                    new Leccion { Orden = 2, Titulo = "Variables y tipos de datos", Contenido = "<p>Ejemplos de declaración:</p><pre>int edad = 20;\nstring nombre = \"Ana\";\nbool activo = true;</pre>" },
                    new Leccion { Orden = 3, Titulo = "Estructuras de control", Contenido = "<p>Las estructuras principales son <code>if</code>, <code>switch</code>, <code>for</code>, <code>foreach</code> y <code>while</code>.</p>" }
                }
            };

            var sql = new Curso
            {
                Titulo = "SQL Server desde cero",
                Descripcion = "Consultas básicas, filtros, joins y buenas prácticas con SQL Server.",
                Nivel = NivelCurso.Intermedio,
                DuracionHoras = 8,
                Publicado = true,
                Lecciones =
                {
                    new Leccion { Orden = 1, Titulo = "SELECT y WHERE", Contenido = "<p>La sentencia básica para consultar datos:</p><pre>SELECT * FROM Cursos WHERE Publicado = 1;</pre>" },
                    new Leccion { Orden = 2, Titulo = "JOIN entre tablas", Contenido = "<p>Un <code>INNER JOIN</code> combina filas de dos tablas relacionadas.</p>" }
                }
            };

            var borrador = new Curso
            {
                Titulo = "ASP.NET Core Avanzado (borrador)",
                Descripcion = "Curso en preparación. Aún no está publicado.",
                Nivel = NivelCurso.Avanzado,
                DuracionHoras = 12,
                Publicado = false,
                Lecciones =
                {
                    new Leccion { Orden = 1, Titulo = "Middleware", Contenido = "<p>Contenido en construcción.</p>" }
                }
            };

            db.Cursos.AddRange(csharp, sql, borrador);
            await db.SaveChangesAsync();
        }
    }
}
