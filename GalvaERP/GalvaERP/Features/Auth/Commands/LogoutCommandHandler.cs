using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Features.Auth.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly AppDbContext _context;

    public LogoutCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Master_Users
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        user.RefreshTokenHash = null;
        user.RefreshTokenExpiry = null;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
