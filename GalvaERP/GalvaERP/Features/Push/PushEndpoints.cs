using GalvaERP.Common.Exceptions;
using GalvaERP.Common.Push;
using GalvaERP.Common.Security;
using GalvaERP.Features.Push.Commands;
using GalvaERP.Features.Push.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GalvaERP.Features.Push;

public static class PushEndpoints
{
    public static IEndpointRouteBuilder MapPushEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/push").WithTags("Push").WithOpenApi();

        group.MapGet("/vapid-public-key", (IOptions<VapidOptions> vapid) => Results.Ok(new { publicKey = vapid.Value.PublicKey }))
            .AllowAnonymous() // pre-login: client needs the public key to subscribe before auth
            .WithName("GetVapidPublicKey");

        group.MapPost("/subscribe", async (IMediator mediator, PushSubscriptionRequest request, HttpContext ctx, CancellationToken ct) =>
        {
            var userId = CurrentUser.GetUserId(ctx.User);
            if (userId is null) return Results.Unauthorized();
            await mediator.Send(new SubscribeCommand(userId.Value, request.Endpoint, request.P256dh, request.Auth), ct);
            return Results.Ok();
        }).WithName("SubscribeToPush");

        group.MapDelete("/subscribe", async (IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var userId = CurrentUser.GetUserId(ctx.User);
            if (userId is null) return Results.Unauthorized();
            await mediator.Send(new UnsubscribeCommand(userId.Value, null), ct);
            return Results.Ok();
        }).WithName("UnsubscribeFromPush");

        group.MapPost("/test", async (IMediator mediator, PushTestRequest? request, HttpContext ctx, CancellationToken ct) =>
        {
            var userId = CurrentUser.GetUserId(ctx.User);
            if (userId is null) return Results.Unauthorized();
            try
            {
                await mediator.Send(new SendTestNotificationCommand(userId.Value, request?.Message), ct);
                return Results.Ok();
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        }).WithName("SendTestPush");

        return app;
    }
}
