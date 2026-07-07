using GalvaERP.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.MasterData;

public static class MasterDataEndpoints
{
    public static IEndpointRouteBuilder MapMasterDataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/master-data").WithTags("MasterData").WithOpenApi();

        group.MapGet("/vendors", async (AppDbContext context, CancellationToken ct) =>
            await context.Suppliers
                .AsNoTracking()
                .Select(s => new { s.Kode, s.Nama, s.MTU, s.Syarat })
                .ToListAsync(ct))
            .WithName("GetVendors");

        group.MapGet("/departments", async (AppDbContext context, CancellationToken ct) =>
            await context.Departments
                .AsNoTracking()
                .Select(d => new { d.Kode, d.Nama })
                .ToListAsync(ct))
            .WithName("GetDepartments");

        group.MapGet("/inventory", async (AppDbContext context, CancellationToken ct) =>
            await context.Barangs
                .AsNoTracking()
                .Select(b => new { b.Kode, b.Nama, b.Merk, b.Satuan, b.Harga })
                .ToListAsync(ct))
            .WithName("GetInventory");

        group.MapGet("/warehouses", async (AppDbContext context, CancellationToken ct) =>
            await context.Gudangs
                .AsNoTracking()
                .Where(g => g.Aktif == true)
                .Select(g => new { g.Kode, g.Nama })
                .ToListAsync(ct))
            .WithName("GetWarehouses");

        group.MapGet("/banks", async (AppDbContext context, CancellationToken ct) =>
            await context.Banks
                .AsNoTracking()
                .Select(b => new { b.Kode, b.Nama })
                .ToListAsync(ct))
            .WithName("GetBanks");

        return app;
    }
}
