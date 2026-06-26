using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Common.Exceptions;
using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, string>
{
    private readonly AppDbContext _context;

    public CreatePurchaseOrderCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Kode == request.Kode_Supplier, cancellationToken);

        if (supplier is null)
        {
            throw new DomainException($"Supplier '{request.Kode_Supplier}' does not exist.");
        }

        var prefix = $"PO-{request.Tgl:yyyyMMdd}-";
        var todayCount = await _context.POs
            .Where(p => p.Doku != null && p.Doku.StartsWith(prefix))
            .CountAsync(cancellationToken);

        var doku = $"{prefix}{(todayCount + 1):000}";

        var po = new PO
        {
            Doku = doku,
            Tgl = request.Tgl,
            Kode_Supplier = request.Kode_Supplier,
            Kode_dept = request.Kode_dept,
            Memo = request.Memo,
            STS = "0",
            PPN = 0d,
            Diskon = 0d,
            EntryDate = DateTime.Now,
            Wkt = DateTime.Now
        };

        double nilai = 0d;
        var subPos = new List<SubPO>();

        foreach (var line in request.LineItems)
        {
            var lineTotal = line.Jumlah * line.Harga;
            nilai += lineTotal;

            subPos.Add(new SubPO
            {
                Doku = doku,
                Kode_Brg = line.Kode_Brg,
                Jumlah = line.Jumlah,
                Harga = line.Harga,
                Total = lineTotal,
                Kode_Gudang = line.Kode_Gudang,
                Alias = line.Alias,
                Kode_Dept = request.Kode_dept,
                Tgl = request.Tgl,
                EntryDate = DateTime.Now
            });
        }

        po.Nilai = nilai;

        _context.POs.Add(po);
        _context.SubPOs.AddRange(subPos);

        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
