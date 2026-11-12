# Portal de Microcursos y Tutoriales

**Materia:** Programación Web — Univalle (SCR_2_2026)

**Integrantes:**
- Nombre del estudiante 1
- Nombre del estudiante 2

## ¿Razor Pages o MVC? Usamos **MVC**

*(Reescribe esta sección con tus propias palabras; aquí va una base.)*

Elegimos ASP.NET Core **MVC** porque el proyecto tiene varios roles (Invitado, Suscriptor y Administrador) y muchas acciones que se reparten naturalmente en controladores: catálogo, inscripción, lecciones protegidas y administración. Con MVC separamos con claridad el modelo (entidades de EF Core), las vistas y los controladores, y podemos aplicar `[Authorize(Roles = "...")]` a cada controlador o acción. Además, las rutas por convención (`/Lecciones/Ver/5`) hacen fácil probar que el servidor bloquea el acceso directo a una lección aunque se conozca la URL.

## Tecnologías

- ASP.NET Core MVC (.NET 8)
- ASP.NET Core Identity (usuarios y roles en las tablas `AspNet*`)
- Entity Framework Core + SQL Server (LocalDB por defecto)
- Bootstrap 5 (por CDN)

## Cómo ejecutar

1. Requisitos: SDK de .NET 8 (o superior) y SQL Server / LocalDB.
2. Revisa la cadena de conexión en `appsettings.json` (`DefaultConnection`).
3. Ejecuta:
   ```bash
   dotnet restore
   dotnet run
   ```
4. Al iniciar, la aplicación crea la base de datos, los roles `Admin` y `Suscriptor`, un administrador y cursos de ejemplo.

> Si usas otra versión de .NET, cambia `TargetFramework` y la versión de los paquetes `Microsoft.*` en el `.csproj`.







