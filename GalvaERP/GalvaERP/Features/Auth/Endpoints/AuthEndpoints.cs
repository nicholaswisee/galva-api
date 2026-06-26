using GalvaERP.Common.Exceptions;
using GalvaERP.Common.Security;
using GalvaERP.Features.Auth.Commands;
using GalvaERP.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GalvaERP.Features.Auth;

public static class AuthEndpoints
{
    private const string RefreshTokenCookieName = "refreshToken";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth").WithOpenApi();

        group.MapPost("/login", async (IMediator mediator, LoginRequest request, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new LoginCommand(request.Username, request.Password), ct);
            SetRefreshTokenCookie(ctx, result.RefreshToken);
            return Results.Ok(new AuthResponse(result.AccessToken, result.ExpiresIn));
        }).WithName("Login").AllowAnonymous();

        group.MapPost("/refresh", async (IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            if (!ctx.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken)
                || string.IsNullOrEmpty(refreshToken))
            {
                return Results.Unauthorized();
            }

            try
            {
                var result = await mediator.Send(new RefreshCommand(refreshToken), ct);
                SetRefreshTokenCookie(ctx, result.RefreshToken);
                return Results.Ok(new AuthResponse(result.AccessToken, result.ExpiresIn));
            }
            catch (NotFoundException)
            {
                return Results.Unauthorized();
            }
        }).WithName("Refresh").AllowAnonymous();

        group.MapPost("/logout", (HttpContext ctx) =>
        {
            // Clear the cookie regardless of whether the caller is authenticated.
            // We don't require a valid access token to clear the cookie.
            ctx.Response.Cookies.Delete(RefreshTokenCookieName);
            return Results.Ok();
        }).WithName("Logout").AllowAnonymous();

        return app;
    }

    private static void SetRefreshTokenCookie(HttpContext ctx, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = ctx.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(7),
        };

        ctx.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }
}
