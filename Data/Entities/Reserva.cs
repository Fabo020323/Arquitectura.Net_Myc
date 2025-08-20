namespace ProjectTemplate.Data.Entities;

public class Reserva
{
    public int Id { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    // 'pendiente', 'confirmada', 'cancelada'
    public string Estado { get; set; } = "pendiente";
}
