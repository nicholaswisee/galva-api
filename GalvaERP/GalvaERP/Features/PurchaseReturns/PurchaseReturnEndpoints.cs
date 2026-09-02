using GalvaERP.Common.Web;
using GalvaERP.Features.PurchaseReturns.Commands;
using GalvaERP.Features.PurchaseReturns.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GalvaERP.Features.PurchaseReturns;

public static class PurchaseReturnEndpoints
{
    public static IEndpointRouteBuilder MapPurchaseReturnEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/purchase-returns").WithTags("Purchase Returns").WithOpenApi();

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
            Results.Ok(await mediator.Send(new GetReturnsQuery(), ct)))
            .WithName("GetPurchaseReturns");

        group.MapGet("/eligible-lines", async (
            [FromQuery(Name = "doku_Faktur")] string dokuFaktur,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                return Results.Ok(await mediator.Send(new GetReturnEligibleLinesQuery(dokuFaktur), ct));
            }
            catch (Common.Exceptions.NotFoundException ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found", detail: ex.Message);
            }
        }).WithName("GetPurchaseReturnEligibleLines");

        group.MapGet("/{*doku}", async (string doku, IMediator mediator, HttpContext context, CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.Send(new GetReturnByIdQuery(RouteParams.Decode(doku)), ct);
                context.Response.Headers["ETag"] = $"\"{result.ETag}\"";
                return Results.Ok(result);
            }
            catch (Common.Exceptions.NotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("GetPurchaseReturnById");

        group.MapPost("/", async ([FromBody] CreateReturnCommand command, IMediator mediator, CancellationToken ct) =>
        {
            try
            {
                var doku = await mediator.Send(command, ct);
                return Results.Created($"/api/purchase-returns/{doku}", new { Doku = doku });
            }
            catch (Common.Exceptions.DomainException ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status422UnprocessableEntity, title: "Business Rule Violation", detail: ex.Message);
            }
            catch (Common.Exceptions.NotFoundException ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found", detail: ex.Message);
            }
        }).WithName("CreatePurchaseReturn");

        group.MapPut("/{*doku}", async (string doku, HttpContext context, IMediator mediator, CancellationToken ct) =>
        {
            if (!TryReadIfMatch(context, out var ifMatch, out var error))
                return error!;

            UpdateReturnCommand command;
            try
            {
                command = await context.Request.ReadFromJsonAsync<UpdateReturnCommand>(cancellationToken: ct)
                    ?? throw new InvalidOperationException("Empty request body.");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }

            try
            {
                var result = await mediator.Send(command with
                {
                    Doku = RouteParams.Decode(doku),
                    IfMatchRowVersion = ifMatch,
                }, ct);
                context.Response.Headers["ETag"] = $"\"{result.ETag}\"";
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
                return Results.Conflict(new { error = ex.Message });
            }
        }).WithName("UpdatePurchaseReturn");

        group.MapDelete("/{*doku}", async (string doku, HttpContext context, IMediator mediator, CancellationToken ct) =>
        {
            if (!TryReadIfMatch(context, out var ifMatch, out var error))
                return error!;

            try
            {
                await mediator.Send(new DeleteReturnCommand(RouteParams.Decode(doku), ifMatch), ct);
                return Results.NoContent();
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
                return Results.Conflict(new { error = ex.Message });
            }
        }).WithName("DeletePurchaseReturn");

        return app;
    }

    private static bool TryReadIfMatch(HttpContext context, out byte[] rowVersion, out IResult? error)
    {
        rowVersion = Array.Empty<byte>();
        error = null;

        if (!context.Request.Headers.TryGetValue("If-Match", out var header) || string.IsNullOrWhiteSpace(header))
        {
            error = Results.Json(
                new { error = "If-Match header with the ETag (base64 RowVersion) is required." },
                statusCode: StatusCodes.Status428PreconditionRequired);
            return false;
        }

        var value = header.ToString().Trim();
        if (value.StartsWith("W/\"") && value.EndsWith("\"") && value.Length >= 4)
            value = value[3..^1];
        else if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length >= 2)
            value = value[1..^1];

        try
        {
            rowVersion = Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            error = Results.BadRequest(new { error = "If-Match header is not a valid base64 ETag." });
            return false;
        }
    }
}
