using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;
using ProjectTemplate.Data.Entities;
using ProjectTemplate.Utils;

namespace ProjectTemplate.Endpoints.Auth;

public sealed class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public sealed class RegisterResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}

public class RegisterEndpoint(ProjectTemplateDbContext db) 
    : Endpoint<RegisterRequest, RegisterResponse>
{
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous(); 
        Summary(s =>
        {
            s.Summary = "Crea un nuevo usuario";
            s.Description = "Registra un usuario con rol (por defecto Cliente).";
        });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(req.Username) ||
            string.IsNullOrWhiteSpace(req.Email) ||
            string.IsNullOrWhiteSpace(req.Password))
        {
            AddError("Todos los campos son obligatorios.");
            await Send.ErrorsAsync(cancellation: ct);
        }

        var existsUser = await db.Usuarios
            .AnyAsync(u => u.Username == req.Username || u.Email == req.Email, ct);

        if (existsUser)
        {
            AddError("Username o Email ya están en uso.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var roleName = "Cliente";//string.IsNullOrWhiteSpace(req.RoleName) ? "Cliente" : req.RoleName.Trim();
        var roleNormalized = roleName.ToUpperInvariant();

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Normalizado == roleNormalized, ct);
        if (role is null)
        {
            role = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = roleName,
                Normalizado = roleNormalized
            };
            await db.Roles.AddAsync(role, ct);
        }

        var salt = Password.NewSalt();
        var hash = Password.Hash(req.Password, salt);

        var user = new Usuario
        {
            Id = Guid.NewGuid(),
            Username = req.Username.Trim(),
            Email = req.Email.Trim(),
            PasswordSalt = salt,
            PasswordHash = hash,
            RolId = role.Id,
            CreatedAtUtc = DateTime.UtcNow
        };

        await db.Usuarios.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
        
        await Send.OkAsync(new RegisterResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAtUtc = user.CreatedAtUtc
        }, ct);
    }
}
