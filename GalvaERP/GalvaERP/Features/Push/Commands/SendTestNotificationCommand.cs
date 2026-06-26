using System.Text.Json;
using GalvaERP.Common.Exceptions;
using GalvaERP.Common.Push;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Push.Commands;

public record SendTestNotificationCommand(int UserId, string? Message) : IRequest;

public class SendTestNotificationCommandHandler : IRequestHandler<SendTestNotificationCommand>
{
    private readonly AppDbContext _context;
    private readonly IPushService _pushService;

    public SendTestNotificationCommandHandler(AppDbContext context, IPushService pushService)
    {
        _context = context;
        _pushService = pushService;
    }

    public async Task Handle(SendTestNotificationCommand request, CancellationToken cancellationToken)
    {
        var subscriptions = await _context.Tx_PushSubscriptions
            .Where(s => s.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        if (subscriptions.Count == 0)
        {
            throw new NotFoundException("No push subscriptions found for this user.");
        }

        var payload = JsonSerializer.Serialize(new
        {
            title = "Galva ERP",
            body = request.Message ?? "Test notification from Galva ERP",
            icon = "/icon-192.png"
        });

        foreach (var sub in subscriptions)
        {
            try
            {
                await _pushService.SendNotificationAsync(sub.Endpoint, sub.P256dh, sub.AuthKey, payload, cancellationToken);
            }
            catch (NotFoundException)
            {
                // expired subscription, remove it
                _context.Tx_PushSubscriptions.Remove(sub);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
