using System;
using GalvaERP.Common.Web;
using GalvaERP.Features.Payments.Commands;
using GalvaERP.Features.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace GalvaERP.Features.Payments;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments").WithTags("Payments").WithOpenApi();

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetPaymentsQuery(), ct);
            return Results.Ok(result);
        }).WithName("GetPayments");

        group.MapGet("/{*doku}", async (string doku, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetPaymentByIdQuery(RouteParams.Decode(doku)), ct);
            if (result is null) return Results.NotFound();
            if (result.ETag is not null)
                ctx.Response.Headers[HeaderNames.ETag] = $"\"{result.ETag}\"";
            return Results.Ok(result);
        }).WithName("GetPaymentById");

        group.MapPost("/", async ([FromBody] CreatePaymentCommand command, IMediator mediator, CancellationToken ct) =>
        {
            try
            {
                var doku = await mediator.Send(command, ct);
                return Results.Created($"/api/payments/{doku}", new { Doku = doku });
            }
            catch (Common.Exceptions.DomainException ex)
            {
                return Results.Problem(statusCode: 422, title: "Business Rule Violation", detail: ex.Message);
            }
            catch (Common.Exceptions.NotFoundException ex)
            {
                return Results.Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
            }
        }).WithName("CreatePayment");

        // ponytail: matches the canonical If-Match/ETag pattern used by Invoices/PO/PR/PCF.
        // Previous shape took the ETag in the body; that's been discarded to align with the rest of the API.
        group.MapPut("/{*doku}", async (string doku, HttpContext ctx, IMediator mediator, CancellationToken ct) =>
        {
            if (!TryReadIfMatch(ctx, out var ifMatch, out var bodyError))
            {
                return bodyError!;
            }

            UpdatePaymentCommand command;
            try
            {
                command = await ctx.Request.ReadFromJsonAsync<UpdatePaymentCommand>(cancellationToken: ct)
                          ?? throw new InvalidOperationException("Empty request body.");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }

            try
            {
                var result = await mediator.Send(
                    command with { Doku = RouteParams.Decode(doku), IfMatchRowVersion = ifMatch }, ct);
                if (result.ETag is not null)
                    ctx.Response.Headers[HeaderNames.ETag] = $"\"{result.ETag}\"";
                return Results.Ok(result);
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
        }).WithName("UpdatePayment");

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