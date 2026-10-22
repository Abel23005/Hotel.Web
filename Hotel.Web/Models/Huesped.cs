using System;
using System.Collections.Generic;

namespace Hotel.Web.Models;

public partial class Huesped
{
    public int IdHuesped { get; set; }

    public string Nombres { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
