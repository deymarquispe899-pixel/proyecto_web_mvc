
## ¿Razor Pages o MVC? Usamos **MVC**

*(Reescribe esta sección con tus propias palabras; aquí va una base.)*

Elegimos ASP.NET Core **MVC** porque el proyecto tiene varios roles (Invitado, Suscriptor y Administrador) y muchas acciones que se reparten naturalmente en controladores: catálogo, inscripción, lecciones protegidas y administración. Con MVC separamos con claridad el modelo (entidades de EF Core), las vistas y los controladores, y podemos aplicar `[Authorize(Roles = "...")]` a cada controlador o acción. Además, las rutas por convención (`/Lecciones/Ver/5`) hacen fácil probar que el servidor bloquea el acceso directo a una lección aunque se conozca la URL.
s
integrantes: Deymar Denilzon Quispe Choque
            Juan Gabriel Ventura Flores







