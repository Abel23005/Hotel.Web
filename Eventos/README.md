# Eventos Web

Aplicación ASP.NET Core MVC para administrar eventos, asistentes e inscripciones con EF Core Code First y SQL Server. Se preparó a partir del proyecto Hotel, manteniendo los recursos de interfaz útiles y reemplazando su dominio y acceso Database First.

## Requisitos

- Windows, .NET SDK 10 y SQL Server LocalDB o una instancia de SQL Server accesible.
- Visual Studio con ASP.NET y desarrollo web, o la CLI de .NET.

## Ejecutar

Abrir `Eventos.sln` en Visual Studio o ejecutar desde esta carpeta:

```powershell
dotnet tool restore
dotnet restore Eventos.sln
dotnet run --project Eventos.Web --launch-profile http
```

La cadena de conexión `EventosDB` se configura en `Eventos.Web/appsettings.json`. Al iniciar, la aplicación aplica las migraciones y agrega las categorías, eventos y asistentes iniciales si cada tabla está vacía.

## Migraciones

La carpeta `Eventos.Web/Migrations` contiene tres migraciones: `InitialCreate`, `AgregarModalidadEvento` y `AgregarTelefonoAsistente`. Estos son los comandos utilizados desde la raíz `Eventos`:

```powershell
dotnet tool restore
dotnet ef migrations add InitialCreate --project Eventos.Web/Eventos.Web.csproj --startup-project Eventos.Web/Eventos.Web.csproj --output-dir Migrations
dotnet ef database update --project Eventos.Web/Eventos.Web.csproj --startup-project Eventos.Web/Eventos.Web.csproj
dotnet ef migrations add AgregarModalidadEvento --project Eventos.Web/Eventos.Web.csproj --startup-project Eventos.Web/Eventos.Web.csproj --output-dir Migrations
dotnet ef database update --project Eventos.Web/Eventos.Web.csproj --startup-project Eventos.Web/Eventos.Web.csproj
dotnet ef migrations add AgregarTelefonoAsistente --project Eventos.Web/Eventos.Web.csproj --startup-project Eventos.Web/Eventos.Web.csproj --output-dir Migrations
dotnet ef database update --project Eventos.Web/Eventos.Web.csproj --startup-project Eventos.Web/Eventos.Web.csproj
```

La migración `AgregarModalidadEvento` establece el valor predeterminado `Presencial` para los eventos que ya existan. El modelo usa solo Data Annotations para configurar entidades y relaciones; no contiene Fluent API, SQL manual, `EnsureCreated` ni `FromSqlRaw`.

## Funciones

- Eventos activos, filtro por categoría, orden por fecha, detalles, alta, edición y desactivación lógica.
- Asistentes activos ordenados por nombre, registro y validación de DNI y correo duplicados.
- Inscripciones con comprobación de fecha, duplicidad confirmada y aforo; cancelación mediante cambio de estado.
- Repositorios asíncronos con LINQ y `AsNoTracking()` en consultas de lectura. Los controladores no acceden al contexto.

## Observaciones y conclusiones

- La base se crea y se actualiza a partir de las clases del modelo; `__EFMigrationsHistory` conserva el registro de los cambios aplicados.
- `Modalidad` se incorpora en una migración separada con el valor inicial `Presencial`, y `Telefono` se agrega después como dato opcional.
- Los repositorios concentran las consultas LINQ y el guardado asíncrono. Los controladores aplican las validaciones de negocio para duplicados, fechas y aforo.
- La desactivación lógica de eventos y la cancelación por estado conservan los registros históricos.

## Evidencia académica por adjuntar

Capturar las vistas de eventos, asistentes e inscripciones en el navegador. En SQL Server, ejecutar `SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;` en `EventosDB` y capturar las tres filas junto con la carpeta `Migrations` expandida. Los archivos de las tres migraciones y `EventosContextModelSnapshot.cs` están en `Eventos.Web/Migrations`.
