using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio(EventosContext context)
{
    public async Task<List<EventoListado>> ListarAsync(int? categoriaId) => await context.Eventos
        .AsNoTracking().Where(e => e.Activo && (!categoriaId.HasValue || e.CategoriaId == categoriaId))
        .Include(e => e.Categoria)
        .OrderBy(e => e.Fecha)
        .Select(e => new EventoListado { Evento = e, InscritosConfirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada") })
        .ToListAsync();

    public Task<Evento?> ObtenerAsync(int id) => context.Eventos.AsNoTracking().Include(e => e.Categoria).FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    public Task<List<Categoria>> CategoriasActivasAsync() => context.Categorias.AsNoTracking().Where(c => c.Activo).OrderBy(c => c.Nombre).ToListAsync();
    public Task<Evento?> ObtenerParaEditarAsync(int id) => context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    public async Task CrearAsync(Evento evento) { evento.Activo = true; context.Eventos.Add(evento); await context.SaveChangesAsync(); }
    public Task GuardarAsync() => context.SaveChangesAsync();
    public async Task<bool> DesactivarAsync(int id)
    {
        var evento = await context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento is null) return false;
        evento.Activo = false;
        await context.SaveChangesAsync();
        return true;
    }
}
