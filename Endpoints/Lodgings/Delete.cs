using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;

namespace ProjectTemplate.Endpoints.Lodgings;

public class DeleteLodgingEndpoint(ProjectTemplateDbContext db)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/lodgings/{id:int}");   
        Roles("ADMIN");
        Summary(s =>
        {
            s.Summary = "Elimina un alojamiento";
            s.Description = "Elimina el alojamiento por Id (solo ADMIN).";
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

        var a = await db.Alojamientos.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        db.Alojamientos.Remove(a);
        await db.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}
