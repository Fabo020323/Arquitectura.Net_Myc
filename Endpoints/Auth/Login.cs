using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProyectTemplate.Data;
using ProjectTemplate.Utils;

namespace ProjectTemplate.Endpoints.Auth;

public sealed class LoginRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAtUtc { get; set; }
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class LoginEndpoint(ProjectTemplateDbContext db, IConfiguration cfg)
    : Endpoint<LoginRequest, AuthResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Autentica un usuario y devuelve un JWT";
            s.Description = "Valida credenciales y emite un token JWT con claim de rol.";
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UsernameOrEmail) ||
            string.IsNullOrWhiteSpace(req.Password))
        {
            AddError("Credenciales inválidas.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var user = await db.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u =>
                u.Username == req.UsernameOrEmail || u.Email == req.UsernameOrEmail, ct);

        if (user is null)
        {
            AddError("Usuario o contraseña incorrectos.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var valid = Password.Verify(req.Password, user.PasswordSalt, user.PasswordHash);
        if (!valid)
        {
            AddError("Usuario o contraseña incorrectos.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var auth = cfg.GetSection("Auth");
        var key     = auth["Key"] ?? throw new InvalidOperationException("Auth.Key no configurada.");
        var issuer  = auth["Issuer"];
        var audience= auth["Audience"];
        var minutes = int.TryParse(auth["AccessTokenMinutes"], out var m) ? m : 60;

        var token = JwtToken.CreateToken(user, key, issuer, audience, minutes);

        await Send.OkAsync(new AuthResponse
        {
            AccessToken = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(minutes),
            Username = user.Username,
            Role = user.Rol.Normalizado
        }, ct);
    }
}
