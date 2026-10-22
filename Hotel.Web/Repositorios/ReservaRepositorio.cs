using System.Data;
using Hotel.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Repositorios;

public class ReservaRepositorio(HotelContext context)
{
    public async Task<List<Habitacion>> DisponiblesAsync(DateOnly ingreso, DateOnly salida) =>
        await context.Habitacions.AsNoTracking().Include(h => h.IdTipoHabitacionNavigation)
            .Where(h => h.Activo && !context.Reservas.Any(r => r.IdHabitacion == h.IdHabitacion
                && r.Estado != "Cancelada" && r.FechaIngreso < salida && r.FechaSalida > ingreso))
            .OrderBy(h => h.Numero).ToListAsync();

    public async Task<List<Huesped>> HuespedesAsync() =>
        await context.Huespeds.AsNoTracking().Where(h => h.Activo).OrderBy(h => h.Nombres).ToListAsync();

    public async Task<List<Habitacion>> HabitacionesAsync() =>
        await context.Habitacions.AsNoTracking().Where(h => h.Activo).OrderBy(h => h.Numero).ToListAsync();

    public async Task<bool> HuespedActivoAsync(int id) =>
        await context.Huespeds.AsNoTracking().AnyAsync(h => h.IdHuesped == id && h.Activo);

    public async Task<bool> HabitacionActivaAsync(int id) =>
        await context.Habitacions.AsNoTracking().AnyAsync(h => h.IdHabitacion == id && h.Activo);

    public async Task<bool> CruceAsync(int habitacion, DateOnly ingreso, DateOnly salida) =>
        await context.Reservas.AsNoTracking().AnyAsync(r => r.IdHabitacion == habitacion && r.Estado != "Cancelada"
            && r.FechaIngreso < salida && r.FechaSalida > ingreso);

    public async Task<bool> CrearAsync(Reserva reserva)
    {
        // Repetir la comprobacion dentro de la transaccion evita reservas simultaneas del mismo rango.
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if (!await HuespedActivoAsync(reserva.IdHuesped) || !await HabitacionActivaAsync(reserva.IdHabitacion)
            || await CruceAsync(reserva.IdHabitacion, reserva.FechaIngreso, reserva.FechaSalida)) return false;
        reserva.Estado = "Pendiente";
        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }

    public async Task<List<Reserva>> ListarAsync() =>
        await context.Reservas.AsNoTracking().Include(r => r.IdHuespedNavigation)
            .Include(r => r.IdHabitacionNavigation).OrderByDescending(r => r.IdReserva).ToListAsync();

    public async Task<bool> CancelarAsync(int id)
    {
        var reserva = await context.Reservas.FirstOrDefaultAsync(r => r.IdReserva == id);
        if (reserva is null || (reserva.Estado != "Pendiente" && reserva.Estado != "Confirmada")) return false;
        reserva.Estado = "Cancelada";
        await context.SaveChangesAsync();
        return true;
    }
}
