using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController(EventoRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = new SelectList(await repositorio.CategoriasActivasAsync(), "Id", "Nombre", categoriaId);
        ViewBag.CategoriaId = categoriaId;
        return View(await repositorio.ListarAsync(categoriaId));
    }
    public async Task<IActionResult> Details(int id) { var item = await repositorio.ObtenerAsync(id); return item is null ? NotFound() : View(item); }
    public async Task<IActionResult> Create() { await CargarCategorias(); return View(new Evento { Fecha = DateTime.Today.AddDays(1), Modalidad = "Presencial" }); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (!ModelState.IsValid) { await CargarCategorias(evento.CategoriaId); return View(evento); }
        await repositorio.CrearAsync(evento); TempData["Mensaje"] = "Evento creado correctamente."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id) { var evento = await repositorio.ObtenerParaEditarAsync(id); if (evento is null) return NotFound(); await CargarCategorias(evento.CategoriaId); return View(evento); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento entrada)
    {
        if (id != entrada.Id) return BadRequest();
        if (!ModelState.IsValid) { await CargarCategorias(entrada.CategoriaId); return View(entrada); }
        var evento = await repositorio.ObtenerParaEditarAsync(id); if (evento is null) return NotFound();
        evento.Titulo = entrada.Titulo; evento.Fecha = entrada.Fecha; evento.Lugar = entrada.Lugar; evento.Aforo = entrada.Aforo;
        evento.Precio = entrada.Precio; evento.CategoriaId = entrada.CategoriaId; evento.Modalidad = entrada.Modalidad;
        await repositorio.GuardarAsync(); TempData["Mensaje"] = "Evento actualizado correctamente."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id) { var evento = await repositorio.ObtenerAsync(id); return evento is null ? NotFound() : View(evento); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await repositorio.DesactivarAsync(id)) return NotFound();
        TempData["Mensaje"] = "Evento desactivado correctamente."; return RedirectToAction(nameof(Index));
    }
    private async Task CargarCategorias(int? seleccionada = null) => ViewBag.CategoriaId = new SelectList(await repositorio.CategoriasActivasAsync(), "Id", "Nombre", seleccionada);
}
