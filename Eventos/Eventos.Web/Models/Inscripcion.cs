using System.ComponentModel.DataAnnotations;

namespace Eventos.Web.Models;

public class Inscripcion
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un evento.")]
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un asistente.")]
    public int AsistenteId { get; set; }
    public Asistente? Asistente { get; set; }

    [Required]
    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required, StringLength(20)]
    public string Estado { get; set; } = "Confirmada";
}
