using System;
using System.Threading;
using GalvaERP.Common.Web;
using GalvaERP.Features.POConfirmations.Commands;
using GalvaERP.Features.POConfirmations.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GalvaERP.Features.POConfirmations;

public static class POConfirmationEndpoints
{
    public static IEndpointRouteBuilder MapPOConfirmationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/po-confirmations").WithTags("POConfirmations").WithOpenApi();

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var list = await mediator.Send(new GetPOConfirmationsQuery(), ct);
            return Results.Ok(list);
        }).WithName("GetPOConfirmations");

        group.MapGet("/{*doku}", async (string doku, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var detail = await mediator.Send(new GetPOConfirmationByIdQuery(RouteParams.Decode(doku)), ct);
            if (detail is null)
            {
                return Results.NotFound();
            }
            if (detail.ETag is not null)
            {
                ctx.Response.Headers["ETag"] = $"\"{detail.ETag}\"";
            }
            return Results.Ok(detail);
        }).WithName("GetPOConfirmationById");

        group.MapPost("/", async (CreatePOConfirmationCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var detail = await mediator.Send(command, ct);
            return Results.Created($"/api/po-confirmations/{detail.Doku}", detail);
        }).WithName("CreatePOConfirmation");

        return app;
    }
}
