using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Data.Entities;
using ProyectTemplate.Data; 
using System.ComponentModel.DataAnnotations;

namespace ProjectTemplate.Endpoints.Hotels;

public sealed class ListHotelsRequest
{
    // Filtros
    public string? Q { get; set; }           // busca en Nombre/Ubicacion
    public string? Ubicacion { get; set; }
    public decimal? MinPrecio { get; set; }
    public decimal? MaxPrecio { get; set; }

    // Paginado
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 20;
}

public sealed class HotelItem
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CantidadHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public sealed class PagedHotelsResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public List<HotelItem> Items { get; set; } = new();
}

public class ListHotelsEndpoint(ProjectTemplateDbContext db)
    : Endpoint<ListHotelsRequest, PagedHotelsResponse>
{
    public override void Configure()
    {
        Get("/hotels");
        AllowAnonymous(); 
        Summary(s =>
        {
            s.Summary = "Lista hoteles con paginado y filtros";
            s.Description = "Listado público de hoteles con filtros por texto/ubicación/precio y paginado.";
        });
    }

    public override async Task HandleAsync(ListHotelsRequest req, CancellationToken ct)
    {
        var qry = db.Hoteles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Q))
        {
            var q = req.Q.Trim().ToLower();
            qry = qry.Where(h => EF.Functions.ILike(h.Nombre, $"%{q}%") ||
                                 EF.Functions.ILike(h.Ubicacion, $"%{q}%"));
        }

        if (!string.IsNullOrWhiteSpace(req.Ubicacion))
        {
            var u = req.Ubicacion.Trim().ToLower();
            qry = qry.Where(h => EF.Functions.ILike(h.Ubicacion, $"%{u}%"));
        }

        if (req.MinPrecio.HasValue) qry = qry.Where(h => h.PrecioPorNoche >= req.MinPrecio.Value);
        if (req.MaxPrecio.HasValue) qry = qry.Where(h => h.PrecioPorNoche <= req.MaxPrecio.Value);

        var total = await qry.CountAsync(ct);

        var items = await qry
            .OrderBy(h => h.Nombre)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(h => new HotelItem
            {
                Id = h.Id,
                Nombre = h.Nombre,
                Ubicacion = h.Ubicacion,
                CantidadHabitaciones = h.CantidadHabitaciones,
                PrecioPorNoche = h.PrecioPorNoche
            })
            .ToListAsync(ct);

        await Send.OkAsync(new PagedHotelsResponse
        {
            Page = req.Page,
            PageSize = req.PageSize,
            Total = total,
            Items = items
        }, ct);
    }
}
