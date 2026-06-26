using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Push.Commands;

public record SubscribeCommand(int UserId, string Endpoint, string P256dh, string Auth) : IRequest;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand>
{
    private readonly AppDbContext _context;

    public SubscribeCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _context.Tx_PushSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == request.UserId && s.Endpoint == request.Endpoint, cancellationToken);

        if (existing is not null)
        {
            return; // already subscribed, idempotent
        }

        _context.Tx_PushSubscriptions.Add(new Tx_PushSubscription
        {
            UserId = request.UserId,
            Endpoint = request.Endpoint,
            P256dh = request.P256dh,
            AuthKey = request.Auth,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
