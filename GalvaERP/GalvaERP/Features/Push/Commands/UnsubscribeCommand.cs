using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Push.Commands;

public record UnsubscribeCommand(int UserId, string? Endpoint) : IRequest;

public class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand>
{
    private readonly AppDbContext _context;

    public UnsubscribeCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Tx_PushSubscription> query = _context.Tx_PushSubscriptions
            .Where(s => s.UserId == request.UserId);

        if (request.Endpoint is not null)
        {
            query = query.Where(s => s.Endpoint == request.Endpoint);
        }

        var subs = await query.ToListAsync(cancellationToken);
        _context.Tx_PushSubscriptions.RemoveRange(subs);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
