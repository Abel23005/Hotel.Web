using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers;

public class ReservasController(ReservaRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());

    public async Task<IActionResult> Disponibles(DateOnly? ingreso, DateOnly? salida)
    {
        ViewBag.Ingreso = ingreso;
        ViewBag.Salida = salida;
        if (ingreso is null && salida is null) return View(new List<Habitacion>());
        if (ingreso is null || salida is null || salida <= ingreso)
            ModelState.AddModelError("", "Indica ambas fechas; la salida debe ser posterior al ingreso.");
        if (ingreso < DateOnly.FromDateTime(DateTime.Today))
            ModelState.AddModelError("", "El ingreso no puede ser anterior a hoy.");
        if (!ModelState.IsValid) return View(new List<Habitacion>());
        ViewBag.Noches = salida!.Value.DayNumber - ingreso!.Value.DayNumber;
        ViewBag.Busqueda = true;
        return View(await repositorio.DisponiblesAsync(ingreso.Value, salida.Value));
    }

    public async Task<IActionResult> Create(int? habitacion, DateOnly? ingreso, DateOnly? salida)
    {
        await CargarListasAsync();
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        return View(new Reserva { IdHabitacion = habitacion ?? 0, FechaIngreso = ingreso ?? hoy,
            FechaSalida = salida ?? hoy.AddDays(1), Estado = "Pendiente" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdHuesped,IdHabitacion,FechaIngreso,FechaSalida,Observaciones")] Reserva reserva)
    {
        ModelState.Remove(nameof(Reserva.IdHuespedNavigation));
        ModelState.Remove(nameof(Reserva.IdHabitacionNavigation));
        ModelState.Remove(nameof(Reserva.Estado));
        reserva.Estado = "Pendiente";
        if (reserva.FechaSalida <= reserva.FechaIngreso)
            ModelState.AddModelError("FechaSalida", "La salida debe ser posterior al ingreso.");
        if (reserva.FechaIngreso < DateOnly.FromDateTime(DateTime.Today))
            ModelState.AddModelError("FechaIngreso", "El ingreso no puede ser anterior a hoy.");
        if (reserva.Observaciones?.Length > 200)
            ModelState.AddModelError("Observaciones", "Máximo 200 caracteres.");
        if (!await repositorio.HuespedActivoAsync(reserva.IdHuesped))
            ModelState.AddModelError("IdHuesped", "Selecciona un huésped activo.");
        if (!await repositorio.HabitacionActivaAsync(reserva.IdHabitacion))
            ModelState.AddModelError("IdHabitacion", "Selecciona una habitación activa.");
        if (ModelState.IsValid && await repositorio.CruceAsync(reserva.IdHabitacion, reserva.FechaIngreso, reserva.FechaSalida))
            ModelState.AddModelError("IdHabitacion", "La habitación ya está reservada en esas fechas.");
        if (ModelState.IsValid)
        {
            if (await repositorio.CrearAsync(reserva))
            {
                TempData["Mensaje"] = "Reserva registrada en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "La disponibilidad cambió. Selecciona otra habitación o fechas.");
        }
        await CargarListasAsync();
        return View(reserva);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        TempData["Mensaje"] = await repositorio.CancelarAsync(id)
            ? "Reserva cancelada correctamente." : "Solo se pueden cancelar reservas pendientes o confirmadas.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        ViewBag.Huespedes = new SelectList(await repositorio.HuespedesAsync(), "IdHuesped", "Nombres");
        ViewBag.Habitaciones = new SelectList(await repositorio.HabitacionesAsync(), "IdHabitacion", "Numero");
    }
}
