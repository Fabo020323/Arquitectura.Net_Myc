namespace ProjectTemplate.Data.Entities;

public class ReservaAlojamiento : Reserva
{
    public int AlojamientoId { get; set; }
    public Alojamiento Alojamiento { get; set; } = default!;

    public int CantidadPersonas { get; set; }
}
