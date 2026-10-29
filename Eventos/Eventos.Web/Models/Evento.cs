using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Evento
{
    public int Id { get; set; }

    [Required, StringLength(100), Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required, DataType(DataType.DateTime), Display(Name = "Fecha y hora")]
    public DateTime Fecha { get; set; }

    [Required, StringLength(100)]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000)]
    public int Aforo { get; set; }

    [Column(TypeName = "decimal(10,2)"), Range(typeof(decimal), "0", "99999999.99")]
    public decimal Precio { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría.")]
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public bool Activo { get; set; } = true;

    [Required, StringLength(20), RegularExpression("^(Presencial|Virtual)$", ErrorMessage = "Selecciona Presencial o Virtual.")]
    public string Modalidad { get; set; } = "Presencial";

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
