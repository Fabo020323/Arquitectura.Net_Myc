namespace ProjectTemplate.Data.Entities;

public class Hotel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CantidadHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
}
