using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio(EventosContext context)
{
    public Task<List<Inscripcion>> ListarAsync() => context.Inscripciones.AsNoTracking()
        .Include(i => i.Evento).Include(i => i.Asistente).OrderByDescending(i => i.FechaInscripcion).ToListAsync();
    public Task<List<Evento>> EventosActivosAsync() => context.Eventos.AsNoTracking().Where(e => e.Activo && e.Fecha >= DateTime.Now).OrderBy(e => e.Fecha).ToListAsync();
    public Task<List<Asistente>> AsistentesActivosAsync() => context.Asistentes.AsNoTracking().Where(a => a.Activo).OrderBy(a => a.Nombres).ToListAsync();
    public async Task<List<string>> ValidarAsync(int eventoId, int asistenteId)
    {
        var errores = new List<string>();
        var evento = await context.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);
        if (evento is null) { errores.Add("Selecciona un evento activo."); return errores; }
        if (evento.Fecha < DateTime.Now) errores.Add("No puedes inscribirte en un evento que ya pasó.");
        if (!await context.Asistentes.AnyAsync(a => a.Id == asistenteId && a.Activo)) errores.Add("Selecciona un asistente activo.");
        if (await context.Inscripciones.AnyAsync(i => i.EventoId == eventoId && i.AsistenteId == asistenteId && i.Estado == "Confirmada"))
            errores.Add("Este asistente ya tiene una inscripción confirmada para el evento.");
        var confirmadas = await context.Inscripciones.CountAsync(i => i.EventoId == eventoId && i.Estado == "Confirmada");
        if (confirmadas >= evento.Aforo) errores.Add("El aforo del evento está completo.");
        return errores;
    }
    public async Task CrearAsync(Inscripcion inscripcion)
    {
        inscripcion.Estado = "Confirmada"; inscripcion.FechaInscripcion = DateTime.Now;
        context.Inscripciones.Add(inscripcion); await context.SaveChangesAsync();
    }
    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await context.Inscripciones.FirstOrDefaultAsync(i => i.Id == id && i.Estado == "Confirmada");
        if (inscripcion is null) return false;
        inscripcion.Estado = "Cancelada"; await context.SaveChangesAsync(); return true;
    }
}
