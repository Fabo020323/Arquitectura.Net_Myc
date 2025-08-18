using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;

namespace ProjectTemplate.Endpoints.Hotels;

public class DeleteHotelEndpoint(ProjectTemplateDbContext db)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/hotels/{id:int}");
        Roles("ADMIN");
        Summary(s =>
        {
            s.Summary = "Elimina un hotel";
            s.Description = "Elimina el hotel por Id (solo ADMIN).";
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

        var h = await db.Hoteles.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (h is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        db.Hoteles.Remove(h);
        await db.SaveChangesAsync(ct);

        // No content
        await Send.NoContentAsync(ct);
    }
}
