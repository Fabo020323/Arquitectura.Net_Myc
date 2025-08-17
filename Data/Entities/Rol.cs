namespace ProjectTemplate.Data.Entities;

public class Rol
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Normalizado { get; set; } = null!; 

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
