using FastEndpoints;
using FluentValidation;
using ProyectTemplate.Data;
using ProjectTemplate.Data.Entities;

namespace ProjectTemplate.Endpoints.Lodgings;

public sealed class CreateLodgingRequest
{
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public sealed class CreateLodgingResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public class CreateLodgingValidator : Validator<CreateLodgingRequest>
{
    public CreateLodgingValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Ubicacion).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CapacidadMaxima).GreaterThan(0);
        RuleFor(x => x.PrecioPorNoche).GreaterThan(0);
    }
}

public class CreateLodgingEndpoint(ProjectTemplateDbContext db)
    : Endpoint<CreateLodgingRequest, CreateLodgingResponse>
{
    public override void Configure()
    {
        Post("/lodgings");
        AllowAnonymous();
        //Roles("ADMIN");           
        Summary(s =>
        {
            s.Summary = "Crea un alojamiento";
            s.Description = "Crea un nuevo alojamiento (solo ADMIN).";
        });
    }

    public override async Task HandleAsync(CreateLodgingRequest req, CancellationToken ct)
    {
        var entity = new Alojamiento
        {
            Nombre = req.Nombre.Trim(),
            Ubicacion = req.Ubicacion.Trim(),
            CapacidadMaxima = req.CapacidadMaxima,
            PrecioPorNoche = req.PrecioPorNoche
        };

        db.Alojamientos.Add(entity);
        await db.SaveChangesAsync(ct);

        await Send.OkAsync(new CreateLodgingResponse
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Ubicacion = entity.Ubicacion,
            CapacidadMaxima = entity.CapacidadMaxima,
            PrecioPorNoche = entity.PrecioPorNoche
        }, ct);
    }
}
