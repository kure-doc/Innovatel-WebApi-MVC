Innovatel WebApi MVC

Proyecto de API web construido con ASP.NET Core MVC que proporciona servicios para gestionar información de clientes, productos, inventario y ventas.
  Características

    ASP.NET Core 7.0 - Framework moderno y de alto rendimiento

    Entity Framework Core - ORM para acceso a datos

    SQL Server - Base de datos relacional

    Patrón MVC - Arquitectura Model-View-Controller

    Swagger/OpenAPI - Documentación interactiva de la API

    JWT Authentication - Autenticación segura con tokens

    Repository Pattern - Patrón de repositorio para acceso a datos

  Requisitos Previos

    .NET 7.0 SDK

    SQL Server (LocalDB o Express)

    Visual Studio 2022 o Visual Studio Code

  Instalación y Configuración

    Clonar el repositorio
    bash

git clone https://github.com/kure-doc/Innovatel-WebApi-MVC.git
cd Innovatel-WebApi-MVC

Configurar la base de datos

    Asegúrate de que SQL Server esté ejecutándose

    Actualiza la cadena de conexión en appsettings.json:

json

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InnovatelDB;Trusted_Connection=true;"
}

Aplicar migraciones de la base de datos
bash

dotnet ef database update

Ejecutar la aplicación
bash

    dotnet run

    Acceder a la aplicación

        API: https://localhost:7000

        Swagger UI: https://localhost:7000/swagger

  Estructura del Proyecto
text

Innovatel-WebApi-MVC/
├── Controllers/          # Controladores de la API
├── Models/              # Modelos de datos y entidades
├── Data/                # Contexto de base de datos y configuraciones
├── Repository/          # Implementaciones del patrón repositorio
├── Services/            # Lógica de negocio y servicios
├── DTOs/                # Objetos de transferencia de datos
├── Migrations/          # Migraciones de Entity Framework
└── appsettings.json     # Configuración de la aplicación

  Endpoints Principales
Clientes

    GET /api/clientes - Obtener todos los clientes

    GET /api/clientes/{id} - Obtener cliente por ID

    POST /api/clientes - Crear nuevo cliente

    PUT /api/clientes/{id} - Actualizar cliente

    DELETE /api/clientes/{id} - Eliminar cliente

Productos

    GET /api/productos - Obtener todos los productos

    GET /api/productos/{id} - Obtener producto por ID

    POST /api/productos - Crear nuevo producto

    PUT /api/productos/{id} - Actualizar producto

    DELETE /api/productos/{id} - Eliminar producto

Inventario

    GET /api/inventario - Consultar estado del inventario

    POST /api/inventario/ajustar - Ajustar niveles de inventario

Ventas

    GET /api/ventas - Obtener historial de ventas

    POST /api/ventas - Registrar nueva venta

  Autenticación

La API utiliza autenticación JWT. Para acceder a endpoints protegidos:

    Obtener token de autenticación

    Incluir en las cabeceras: Authorization: Bearer {token}

  Testing

Ejecutar las pruebas unitarias:
bash

dotnet test

  Dependencias Principales

    Microsoft.EntityFrameworkCore.SqlServer - EF Core para SQL Server

    Microsoft.EntityFrameworkCore.Tools - Herramientas de EF Core

    Swashbuckle.AspNetCore - Swagger/OpenAPI

    Microsoft.AspNetCore.Authentication.JwtBearer - Autenticación JWT

    AutoMapper - Mapeo de objetos

  Contribución

    Fork el proyecto

    Crear una rama para tu feature (git checkout -b feature/AmazingFeature)

    Commit tus cambios (git commit -m 'Add some AmazingFeature')

    Push a la rama (git push origin feature/AmazingFeature)

    Abrir un Pull Request

  Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo LICENSE para más detalles.
  Soporte

Si tienes preguntas o problemas, abre un issue en el repositorio o contacta al equipo de desarrollo.
  Despliegue
Publicar en IIS
bash

dotnet publish -c Release

Publicar en Docker
bash

docker build -t innovatel-api .
docker run -p 8080:80 innovatel-api

¡Dale una estrella al repositorio si te resulta útil!
