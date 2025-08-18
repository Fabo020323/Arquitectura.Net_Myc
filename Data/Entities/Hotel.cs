namespace ProjectTemplate.Data.Entities;

public class Hotel
{
    public int Id { get; set; }

    public string Nombre { get; set; } = default!;
    public string Direccion { get; set; } = default!;
    public string Ciudad { get; set; } = default!;
    public string Pais { get; set; } = default!;
    public int Estrellas { get; set; }             
    public string? Descripcion { get; set; }        
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
