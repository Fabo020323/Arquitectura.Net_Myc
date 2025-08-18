using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;

namespace ProjectTemplate.Endpoints.Lodgings;

public sealed class GetLodgingResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public class GetLodgingByIdEndpoint(ProjectTemplateDbContext db)
    : EndpointWithoutRequest<GetLodgingResponse>
{
    public override void Configure()
    {
        Get("/lodgings/{id:int}");    
        Roles("ADMIN");
        Summary(s =>
        {
            s.Summary = "Obtiene un alojamiento por Id";
            s.Description = "Detalle del alojamiento (solo ADMIN).";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        if (id <= 0)
        {
            AddError("id", "El parámetro de ruta 'id' es requerido y debe ser mayor que 0.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var a = await db.Alojamientos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new GetLodgingResponse
        {
            Id = a.Id,
            Nombre = a.Nombre,
            Ubicacion = a.Ubicacion,
            CapacidadMaxima = a.CapacidadMaxima,
            PrecioPorNoche = a.PrecioPorNoche
        }, ct);
    }
}
