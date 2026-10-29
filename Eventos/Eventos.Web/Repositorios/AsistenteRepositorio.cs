using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio(EventosContext context)
{
    public Task<List<Asistente>> ListarAsync() => context.Asistentes.AsNoTracking().Where(a => a.Activo).OrderBy(a => a.Nombres).ToListAsync();
    public async Task<bool> ExisteDniOCorreoAsync(string dni, string correo) =>
        await context.Asistentes.AnyAsync(a => a.Dni == dni) || await context.Asistentes.AnyAsync(a => a.Correo == correo);
    public async Task CrearAsync(Asistente asistente) { asistente.Activo = true; context.Asistentes.Add(asistente); await context.SaveChangesAsync(); }
}
