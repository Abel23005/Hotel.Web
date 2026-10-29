using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController(InscripcionRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());
    public async Task<IActionResult> Create() { await CargarListas(); return View(new Inscripcion()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        foreach (var error in await repositorio.ValidarAsync(inscripcion.EventoId, inscripcion.AsistenteId)) ModelState.AddModelError(string.Empty, error);
        if (!ModelState.IsValid) { await CargarListas(inscripcion.EventoId, inscripcion.AsistenteId); return View(inscripcion); }
        await repositorio.CrearAsync(inscripcion); TempData["Mensaje"] = "Inscripción confirmada."; return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (!await repositorio.CancelarAsync(id)) return NotFound();
        TempData["Mensaje"] = "Inscripción cancelada."; return RedirectToAction(nameof(Index));
    }
    private async Task CargarListas(int? eventoId = null, int? asistenteId = null)
    {
        ViewBag.EventoId = new SelectList(await repositorio.EventosActivosAsync(), "Id", "Titulo", eventoId);
        ViewBag.AsistenteId = new SelectList(await repositorio.AsistentesActivosAsync(), "Id", "Nombres", asistenteId);
    }
}
