namespace ProjectTemplate.Data.Entities;

public class Usuario
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;

    // Hash + Salt (PBKDF2)
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // FK a Rol
    public Guid RolId { get; set; }
    public Rol Rol { get; set; } = null!;
}
