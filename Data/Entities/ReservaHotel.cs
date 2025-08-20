namespace ProjectTemplate.Data.Entities;

public class ReservaHotel : Reserva
{
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = default!;

    public int CantidadHabitaciones { get; set; }
}
