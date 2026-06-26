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

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public LoginCommandHandler(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Master_Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            // Use the same message for both failure modes to prevent user enumeration.
            throw new NotFoundException("Invalid username or password");
        }

        var accessToken = _tokenService.GenerateAccessToken(user.UserId, user.Username, user.Role);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, _jwtOptions.AccessTokenExpirationMinutes * 60, refreshToken);
    }
}
