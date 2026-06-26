using System;
using System.Threading;
using GalvaERP.Features.PurchaseOrders.Commands;
using GalvaERP.Features.PurchaseOrders.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GalvaERP.Features.PurchaseOrders;

public static class PurchaseOrderEndpoints
{
    public static IEndpointRouteBuilder MapPurchaseOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/purchase-orders").WithTags("PurchaseOrders").WithOpenApi();

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var list = await mediator.Send(new GetPurchaseOrdersQuery(), ct);
            return Results.Ok(list);
        }).WithName("GetPurchaseOrders");

        group.MapGet("/{doku}", async (string doku, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var detail = await mediator.Send(new GetPurchaseOrderByIdQuery(doku), ct);
            if (detail is null)
            {
                return Results.NotFound();
            }
            if (detail.ETag is not null)
            {
                ctx.Response.Headers["ETag"] = $"\"{detail.ETag}\"";
            }
            return Results.Ok(detail);
        }).WithName("GetPurchaseOrderById");

        group.MapPost("/", async (CreatePurchaseOrderCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var doku = await mediator.Send(command, ct);
            return Results.Created($"/api/purchase-orders/{doku}", new { Doku = doku });
        }).WithName("CreatePurchaseOrder");

        group.MapPut("/{doku}", async (string doku, HttpContext ctx, IMediator mediator, CancellationToken ct) =>
        {
            if (!TryReadIfMatch(ctx, out var ifMatch, out var error))
            {
                return error!;
            }

            var body = await ctx.Request.ReadFromJsonAsync<UpdatePurchaseOrderCommand>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty request body.");

            try
            {
                var updated = await mediator.Send(
                    body with { Doku = doku, IfMatchRowVersion = ifMatch }, ct);
                if (updated.ETag is not null)
                {
                    ctx.Response.Headers["ETag"] = $"\"{updated.ETag}\"";
                }
                return Results.Ok(updated);
            }
            catch (Common.Exceptions.ConcurrencyException)
            {
                return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
            }
            catch (Common.Exceptions.NotFoundException)
            {
                return Results.NotFound();
            }
            catch (Common.Exceptions.DomainException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }).WithName("UpdatePurchaseOrder");

        return app;
    }

    private static bool TryReadIfMatch(HttpContext ctx, out byte[] rowVersion, out IResult? error)
    {
        rowVersion = Array.Empty<byte>();
        error = null;

        if (!ctx.Request.Headers.TryGetValue("If-Match", out var ifMatchHeader) || string.IsNullOrWhiteSpace(ifMatchHeader))
        {
            error = Results.Json(
                new { error = "If-Match header with the ETag (base64 RowVersion) is required." },
                statusCode: StatusCodes.Status428PreconditionRequired);
            return false;
        }

        var raw = ifMatchHeader.ToString().Trim();
        if (raw.StartsWith("\"") && raw.EndsWith("\"") && raw.Length >= 2)
        {
            raw = raw.Substring(1, raw.Length - 2);
        }
        else if (raw.StartsWith("W/\"") && raw.EndsWith("\"") && raw.Length >= 4)
        {
            raw = raw.Substring(3, raw.Length - 4);
        }

        try
        {
            rowVersion = Convert.FromBase64String(raw);
            return true;
        }
        catch (FormatException)
        {
            error = Results.BadRequest(new { error = "If-Match header is not a valid base64 ETag." });
            return false;
        }
    }
}
