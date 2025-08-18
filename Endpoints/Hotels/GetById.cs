using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;

namespace ProjectTemplate.Endpoints.Hotels;

public sealed class GetHotelResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CantidadHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public class GetHotelByIdEndpoint(ProjectTemplateDbContext db)
    : EndpointWithoutRequest<GetHotelResponse>
{
    public override void Configure()
    {
        Get("/hotels/{id:int}");
        Roles("ADMIN"); 
        Summary(s =>
        {
            s.Summary = "Obtiene un hotel por Id";
            s.Description = "Detalle del hotel (solo ADMIN por defecto).";
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

        var h = await db.Hoteles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (h is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new GetHotelResponse
        {
            Id = h.Id,
            Nombre = h.Nombre,
            Ubicacion = h.Ubicacion,
            CantidadHabitaciones = h.CantidadHabitaciones,
            PrecioPorNoche = h.PrecioPorNoche
        }, ct);
    }
}
