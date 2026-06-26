using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.PurchaseRequisitions.Commands;

public class CreatePurchaseRequisitionCommandHandler : IRequestHandler<CreatePurchaseRequisitionCommand, string>
{
    private readonly AppDbContext _context;

    public CreatePurchaseRequisitionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreatePurchaseRequisitionCommand request, CancellationToken cancellationToken)
    {
        var prefix = $"SPB-{request.Tgl:yyyyMMdd}-";
        var todayCount = await _context.SPBs
            .Where(s => s.Doku != null && s.Doku.StartsWith(prefix))
            .CountAsync(cancellationToken);

        var doku = $"{prefix}{(todayCount + 1):000}";

        var spb = new SPB
        {
            Doku = doku,
            Tgl = request.Tgl,
            Kode_Dept = request.Kode_Dept,
            Kode_Sales = request.Kode_Sales,
            MEMO = request.Memo,
            Status = "Pending",
            Sts = "0",
            EntryDate = DateTime.Now,
            Waktu = DateTime.Now
        };

        double total = 0d;
        var subSpbs = new System.Collections.Generic.List<SubSPB>();

        foreach (var line in request.LineItems)
        {
            var lineTotal = line.Jumlah * line.Harga;
            total += lineTotal;

            subSpbs.Add(new SubSPB
            {
                Doku = doku,
                Kode_Brg = line.Kode_Brg,
                Jumlah = line.Jumlah,
                Harga = line.Harga,
                Kode_Gudang = line.Kode_Gudang,
                Alias = line.Alias,
                Tgl = request.Tgl,
                Nilai = lineTotal,
                EntryDate = DateTime.Now
            });
        }

        spb.Total = total;
        spb.Nilai = total;
        spb.GROSS = total;
        spb.GRANDTOTAL = total;

        _context.SPBs.Add(spb);
        _context.SubSPBs.AddRange(subSpbs);

        await _context.SaveChangesAsync(cancellationToken);

        return doku;
    }
}
