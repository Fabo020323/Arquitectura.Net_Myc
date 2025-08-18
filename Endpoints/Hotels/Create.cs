using FastEndpoints;
using FluentValidation;
using ProyectTemplate.Data;
using ProjectTemplate.Data.Entities;

namespace ProjectTemplate.Endpoints.Hotels;

public sealed class CreateHotelRequest
{
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CantidadHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public sealed class CreateHotelResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CantidadHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public class CreateHotelValidator : Validator<CreateHotelRequest>
{
    public CreateHotelValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Ubicacion).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CantidadHabitaciones).GreaterThan(0);
        RuleFor(x => x.PrecioPorNoche).GreaterThan(0);
    }
}

public class CreateHotelEndpoint(ProjectTemplateDbContext db)
    : Endpoint<CreateHotelRequest, CreateHotelResponse>
{
    public override void Configure()
    {
        Post("/hotels");
        //AllowAnonymous();
        Roles("ADMIN"); 
        Summary(s =>
        {
            s.Summary = "Crea un hotel";
            s.Description = "Crea un nuevo hotel (solo ADMIN).";
        });
    }

    public override async Task HandleAsync(CreateHotelRequest req, CancellationToken ct)
    {
        var entity = new Hotel
        {
            Nombre = req.Nombre.Trim(),
            Ubicacion = req.Ubicacion.Trim(),
            CantidadHabitaciones = req.CantidadHabitaciones,
            PrecioPorNoche = req.PrecioPorNoche
        };

        db.Hoteles.Add(entity);
        await db.SaveChangesAsync(ct);

        await Send.OkAsync(new CreateHotelResponse
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Ubicacion = entity.Ubicacion,
            CantidadHabitaciones = entity.CantidadHabitaciones,
            PrecioPorNoche = entity.PrecioPorNoche
        }, ct);
    }
}
