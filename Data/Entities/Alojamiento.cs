namespace ProjectTemplate.Data.Entities;

public class Alojamiento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
}
