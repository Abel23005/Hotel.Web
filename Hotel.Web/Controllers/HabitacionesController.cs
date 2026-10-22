using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Controllers;

public class HabitacionesController(HabitacionRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index(int? tipo)
    {
        await CargarTiposAsync(tipo);
        return View(await repositorio.ListarAsync(tipo));
    }

    public async Task<IActionResult> Details(int id)
    {
        var habitacion = await repositorio.ObtenerAsync(id);
        return habitacion is null ? NotFound() : View(habitacion);
    }

    public async Task<IActionResult> Create()
    {
        await CargarTiposAsync();
        return View(new Habitacion { Activo = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Numero,IdTipoHabitacion,PrecioPorNoche")] Habitacion habitacion)
    {
        await ValidarAsync(habitacion);
        if (ModelState.IsValid)
        {
            try
            {
                await repositorio.CrearAsync(habitacion);
                TempData["Mensaje"] = "Habitación creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException) { ModelState.AddModelError("Numero", "No se pudo guardar. Comprueba que el número no esté repetido."); }
        }
        await CargarTiposAsync(habitacion.IdTipoHabitacion);
        return View(habitacion);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var habitacion = await repositorio.ObtenerAsync(id);
        if (habitacion is null) return NotFound();
        await CargarTiposAsync(habitacion.IdTipoHabitacion);
        return View(habitacion);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdHabitacion,Numero,IdTipoHabitacion,PrecioPorNoche")] Habitacion habitacion)
    {
        if (id != habitacion.IdHabitacion) return BadRequest();
        await ValidarAsync(habitacion);
        if (ModelState.IsValid)
        {
            try
            {
                if (!await repositorio.EditarAsync(habitacion)) return NotFound();
                TempData["Mensaje"] = "Habitación actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException) { ModelState.AddModelError("Numero", "No se pudo guardar. Comprueba que el número no esté repetido."); }
        }
        await CargarTiposAsync(habitacion.IdTipoHabitacion);
        return View(habitacion);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var habitacion = await repositorio.ObtenerAsync(id);
        return habitacion is null ? NotFound() : View(habitacion);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await repositorio.EliminarAsync(id)) return NotFound();
        TempData["Mensaje"] = "Habitación desactivada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarTiposAsync(int? seleccionado = null) =>
        ViewBag.Tipos = new SelectList(await repositorio.TiposAsync(), "IdTipoHabitacion", "Nombre", seleccionado);

    private async Task ValidarAsync(Habitacion habitacion)
    {
        ModelState.Remove(nameof(Habitacion.IdTipoHabitacionNavigation));
        habitacion.Numero = habitacion.Numero?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(habitacion.Numero) || habitacion.Numero.Length > 10)
            ModelState.AddModelError("Numero", "Escribe un número de habitación de hasta 10 caracteres.");
        else if (await repositorio.ExisteNumeroAsync(habitacion.Numero, habitacion.IdHabitacion))
            ModelState.AddModelError("Numero", "Ese número de habitación ya existe, incluso si está desactivada.");
        if (!await repositorio.TipoExisteAsync(habitacion.IdTipoHabitacion))
            ModelState.AddModelError("IdTipoHabitacion", "Selecciona un tipo válido.");
        if (habitacion.PrecioPorNoche <= 0 || habitacion.PrecioPorNoche > 99999999.99m)
            ModelState.AddModelError("PrecioPorNoche", "El precio debe ser mayor que cero y no superar 99999999.99.");
    }
}
