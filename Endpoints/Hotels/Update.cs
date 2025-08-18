using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;

namespace ProjectTemplate.Endpoints.Hotels;

public sealed class UpdateHotelRequest
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CantidadHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public sealed class UpdateHotelResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CantidadHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public class UpdateHotelValidator : Validator<UpdateHotelRequest>
{
    public UpdateHotelValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Ubicacion).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CantidadHabitaciones).GreaterThan(0);
        RuleFor(x => x.PrecioPorNoche).GreaterThan(0);
    }
}

public class UpdateHotelEndpoint(ProjectTemplateDbContext db)
    : Endpoint<UpdateHotelRequest, UpdateHotelResponse>
{
    public override void Configure()
    {
        Put("/hotels/{id:int}");
        Roles("ADMIN");
        Summary(s =>
        {
            s.Summary = "Actualiza un hotel";
            s.Description = "Actualiza los datos del hotel (solo ADMIN).";
        });
    }

    public override async Task HandleAsync(UpdateHotelRequest req, CancellationToken ct)
    {
        var routeId = Route<int>("id");
        if (routeId <= 0 || routeId != req.Id)
        {
            AddError(r => r.Id, "El Id de la ruta y del cuerpo deben coincidir.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var h = await db.Hoteles.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (h is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        h.Nombre = req.Nombre.Trim();
        h.Ubicacion = req.Ubicacion.Trim();
        h.CantidadHabitaciones = req.CantidadHabitaciones;
        h.PrecioPorNoche = req.PrecioPorNoche;

        await db.SaveChangesAsync(ct);

        await Send.OkAsync(new UpdateHotelResponse
        {
            Id = h.Id,
            Nombre = h.Nombre,
            Ubicacion = h.Ubicacion,
            CantidadHabitaciones = h.CantidadHabitaciones,
            PrecioPorNoche = h.PrecioPorNoche
        }, ct);
    }
}
