using System;
using GalvaERP.Common.Web;
using GalvaERP.Features.Invoices.Commands;
using GalvaERP.Features.Invoices.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalvaERP.Features.Invoices;

public static class APInvoiceEndpoints
{
    public static IEndpointRouteBuilder MapAPInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices").WithTags("Invoices").WithOpenApi();

        group.MapGet("/", async (IMediator mediator, [FromQuery] string? source = null, CancellationToken ct = default) =>
        {
            var result = await mediator.Send(new GetInvoicesQuery(source), ct);
            return Results.Ok(result);
        }).WithName("GetInvoices");

        group.MapGet("/{*doku}", async (string doku, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetInvoiceByIdQuery(RouteParams.Decode(doku)), ct);
            if (result is null) return Results.NotFound();
            if (result.ETag is not null)
                ctx.Response.Headers["ETag"] = $"\"{result.ETag}\"";
            return Results.Ok(result);
        }).WithName("GetInvoiceById");

        group.MapPost("/", async ([FromBody] CreateAPInvoiceCommand command, IMediator mediator, CancellationToken ct) =>
        {
            try
            {
                var doku = await mediator.Send(command, ct);
                return Results.Created($"/api/invoices/{doku}", new { Doku = doku });
            }
            catch (GalvaERP.Common.Exceptions.DomainException ex)
            {
                return Results.Problem(statusCode: 422, title: "Business Rule Violation", detail: ex.Message);
            }
            catch (GalvaERP.Common.Exceptions.NotFoundException ex)
            {
                return Results.Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
            }
        }).WithName("CreateAPInvoice");

        group.MapPost("/po-based", async ([FromBody] CreatePOBasedAPInvoiceCommand command, IMediator mediator, CancellationToken ct) =>
        {
            try
            {
                var doku = await mediator.Send(command, ct);
                return Results.Created($"/api/invoices/{doku}", new { Doku = doku });
            }
            catch (GalvaERP.Common.Exceptions.DomainException ex)
            {
                return Results.Problem(statusCode: 422, title: "Business Rule Violation", detail: ex.Message);
            }
            catch (GalvaERP.Common.Exceptions.NotFoundException ex)
            {
                return Results.Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
            }
        }).WithName("CreatePOBasedAPInvoice");

        group.MapPut("/{*doku}", async (string doku, [FromBody] UpdateAPInvoiceCommand command, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            try
            {
                var commandWithDoku = command with { Doku = RouteParams.Decode(doku) };
                var result = await mediator.Send(commandWithDoku, ct);
                if (result.ETag is not null)
                    ctx.Response.Headers["ETag"] = $"\"{result.ETag}\"";
                return Results.Ok(result);
            }
            catch (GalvaERP.Common.Exceptions.ConcurrencyException)
            {
                return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
            }
            catch (GalvaERP.Common.Exceptions.NotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("UpdateAPInvoice");

        group.MapDelete("/{*doku}", async (string doku, HttpContext ctx, IMediator mediator, CancellationToken ct) =>
        {
            if (!TryReadIfMatch(ctx, out var ifMatch, out var error))
            {
                return error!;
            }

            try
            {
                await mediator.Send(new DeleteAPInvoiceCommand(RouteParams.Decode(doku), ifMatch), ct);
                return Results.NoContent();
            }
            catch (GalvaERP.Common.Exceptions.ConcurrencyException)
            {
                return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
            }
            catch (GalvaERP.Common.Exceptions.NotFoundException)
            {
                return Results.NotFound();
            }
            catch (GalvaERP.Common.Exceptions.DomainException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        }).WithName("DeleteAPInvoice");

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
