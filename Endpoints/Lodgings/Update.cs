// Endpoints/Lodgings/Update.cs
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;

namespace ProjectTemplate.Endpoints.Lodgings;

public sealed class UpdateLodgingRequest
{
    public int Id { get; set; } 
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public sealed class UpdateLodgingResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public class UpdateLodgingValidator : Validator<UpdateLodgingRequest>
{
    public UpdateLodgingValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Ubicacion).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CapacidadMaxima).GreaterThan(0);
        RuleFor(x => x.PrecioPorNoche).GreaterThan(0);
    }
}

public class UpdateLodgingEndpoint(ProjectTemplateDbContext db)
    : Endpoint<UpdateLodgingRequest, UpdateLodgingResponse>
{
    public override void Configure()
    {
        Put("/lodgings/{id:int}");   
        Roles("ADMIN");
        Summary(s =>
        {
            s.Summary = "Actualiza un alojamiento";
            s.Description = "Actualiza los datos del alojamiento (solo ADMIN).";
        });
    }

    public override async Task HandleAsync(UpdateLodgingRequest req, CancellationToken ct)
    {
        var routeId = Route<int>("id");
        if (routeId <= 0 || routeId != req.Id)
        {
            AddError(r => r.Id, "El Id de la ruta y del cuerpo deben coincidir.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var a = await db.Alojamientos.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (a is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        a.Nombre = req.Nombre.Trim();
        a.Ubicacion = req.Ubicacion.Trim();
        a.CapacidadMaxima = req.CapacidadMaxima;
        a.PrecioPorNoche = req.PrecioPorNoche;

        await db.SaveChangesAsync(ct);

        await Send.OkAsync(new UpdateLodgingResponse
        {
            Id = a.Id,
            Nombre = a.Nombre,
            Ubicacion = a.Ubicacion,
            CapacidadMaxima = a.CapacidadMaxima,
            PrecioPorNoche = a.PrecioPorNoche
        }, ct);
    }
}
