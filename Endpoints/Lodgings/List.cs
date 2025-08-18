using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ProyectTemplate.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectTemplate.Endpoints.Lodgings;

public sealed class ListLodgingsRequest
{
    public string? Q { get; set; }            
    public string? Ubicacion { get; set; }
    public int?  MinCapacidad { get; set; }
    public decimal? MinPrecio { get; set; }
    public decimal? MaxPrecio { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 20;
}

public sealed class LodgingItem
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Ubicacion { get; set; } = default!;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
}

public sealed class PagedLodgingsResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public List<LodgingItem> Items { get; set; } = new();
}

public class ListLodgingsEndpoint(ProjectTemplateDbContext db)
    : Endpoint<ListLodgingsRequest, PagedLodgingsResponse>
{
    public override void Configure()
    {
        Get("/lodgings");     
        AllowAnonymous();     
        Summary(s =>
        {
            s.Summary = "Lista alojamientos con paginado y filtros";
            s.Description = "Listado público de alojamientos con filtros por texto, ubicación, capacidad y precio.";
        });
    }

    public override async Task HandleAsync(ListLodgingsRequest req, CancellationToken ct)
    {
        var qry = db.Alojamientos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Q))
        {
            var q = req.Q.Trim().ToLower();
            qry = qry.Where(a => EF.Functions.ILike(a.Nombre, $"%{q}%") ||
                                 EF.Functions.ILike(a.Ubicacion, $"%{q}%"));
        }

        if (!string.IsNullOrWhiteSpace(req.Ubicacion))
        {
            var u = req.Ubicacion.Trim().ToLower();
            qry = qry.Where(a => EF.Functions.ILike(a.Ubicacion, $"%{u}%"));
        }

        if (req.MinCapacidad.HasValue) qry = qry.Where(a => a.CapacidadMaxima >= req.MinCapacidad.Value);
        if (req.MinPrecio.HasValue)    qry = qry.Where(a => a.PrecioPorNoche >= req.MinPrecio.Value);
        if (req.MaxPrecio.HasValue)    qry = qry.Where(a => a.PrecioPorNoche <= req.MaxPrecio.Value);

        var total = await qry.CountAsync(ct);

        var items = await qry
            .OrderBy(a => a.Nombre)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(a => new LodgingItem
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Ubicacion = a.Ubicacion,
                CapacidadMaxima = a.CapacidadMaxima,
                PrecioPorNoche = a.PrecioPorNoche
            })
            .ToListAsync(ct);

        await Send.OkAsync(new PagedLodgingsResponse
        {
            Page = req.Page,
            PageSize = req.PageSize,
            Total = total,
            Items = items
        }, ct);
    }
}
