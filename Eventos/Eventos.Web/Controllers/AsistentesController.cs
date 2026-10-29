using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController(AsistenteRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());
    public IActionResult Create() => View(new Asistente());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        if (await repositorio.ExisteDniOCorreoAsync(asistente.Dni, asistente.Correo))
            ModelState.AddModelError(string.Empty, "El DNI o el correo ya está registrado.");
        if (!ModelState.IsValid) return View(asistente);
        await repositorio.CrearAsync(asistente); TempData["Mensaje"] = "Asistente registrado correctamente."; return RedirectToAction(nameof(Index));
    }
}
