using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// La base se crea y actualiza exclusivamente con las migraciones Code First.
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventosContext>();
    await db.Database.MigrateAsync();
    if (!await db.Categorias.AnyAsync())
    {
        db.Categorias.AddRange(
            new Categoria { Nombre = "Música" },
            new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Cultura" });
        await db.SaveChangesAsync();
    }
    if (!await db.Eventos.AnyAsync())
    {
        var categorias = await db.Categorias.OrderBy(c => c.Id).Take(3).ToListAsync();
        int CategoriaPara(int indice) => categorias[indice % categorias.Count].Id;
        db.Eventos.AddRange(
            new Evento { Titulo = "Noche de jazz", Fecha = DateTime.Today.AddDays(14).AddHours(19), Lugar = "Teatro Municipal", Aforo = 180, Precio = 35m, CategoriaId = CategoriaPara(0), Modalidad = "Presencial" },
            new Evento { Titulo = "Innovación digital", Fecha = DateTime.Today.AddDays(21).AddHours(10), Lugar = "Centro de Convenciones", Aforo = 300, Precio = 25m, CategoriaId = CategoriaPara(1), Modalidad = "Presencial" },
            new Evento { Titulo = "Cine al aire libre", Fecha = DateTime.Today.AddDays(28).AddHours(20), Lugar = "Parque Central", Aforo = 250, Precio = 15m, CategoriaId = CategoriaPara(2), Modalidad = "Presencial" },
            new Evento { Titulo = "Concierto acústico", Fecha = DateTime.Today.AddDays(35).AddHours(18), Lugar = "Auditorio Norte", Aforo = 120, Precio = 40m, CategoriaId = CategoriaPara(0), Modalidad = "Virtual" });
        await db.SaveChangesAsync();
    }
    if (!await db.Asistentes.AnyAsync())
    {
        db.Asistentes.AddRange(
            new Asistente { Nombres = "Lucía Herrera", Dni = "70451231", Correo = "lucia.herrera@example.com", Telefono = "987654321" },
            new Asistente { Nombres = "Mateo Salazar", Dni = "71826345", Correo = "mateo.salazar@example.com", Telefono = "986123456" },
            new Asistente { Nombres = "Camila Rojas", Dni = "73214568", Correo = "camila.rojas@example.com", Telefono = "985234567" },
            new Asistente { Nombres = "Diego Paredes", Dni = "74561239", Correo = "diego.paredes@example.com", Telefono = "984345678" },
            new Asistente { Nombres = "Valeria Torres", Dni = "75632148", Correo = "valeria.torres@example.com", Telefono = "983456789" });
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Eventos}/{action=Index}/{id?}");
app.Run();
