namespace Eventos.Web.Models;

public class EventoListado
{
    public Evento Evento { get; set; } = new();
    public int InscritosConfirmados { get; set; }
    public int CuposDisponibles => Math.Max(0, Evento.Aforo - InscritosConfirmados);
}
