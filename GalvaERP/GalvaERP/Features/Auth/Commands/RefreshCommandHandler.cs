using System.Security.Cryptography;
using System.Text;
using GalvaERP.Common.Exceptions;
using GalvaERP.Common.Security;
using GalvaERP.Features.Auth.DTOs;
using GalvaERP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GalvaERP.Features.Auth.Commands;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, LoginResult>
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public RefreshCommandHandler(
        AppDbContext context,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _context = context;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));

        var user = await _context.Master_Users
            .FirstOrDefaultAsync(u => u.RefreshTokenHash == tokenHash, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("Invalid refresh token");
        }

        if (user.RefreshTokenExpiry is null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new NotFoundException("Refresh token expired");
        }

        // Rotate: issue a fresh access token + refresh token, invalidate the old refresh token.
        var accessToken = _tokenService.GenerateAccessToken(user.UserId, user.Username, user.Role);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(newRefreshToken)));
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, _jwtOptions.AccessTokenExpirationMinutes * 60, newRefreshToken);
    }
}
