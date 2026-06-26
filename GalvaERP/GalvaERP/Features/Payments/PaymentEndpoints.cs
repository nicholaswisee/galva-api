using GalvaERP.Features.Payments.Commands;
using GalvaERP.Features.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        group.MapGet("/{doku}", async (string doku, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetPaymentByIdQuery(doku), ct);
            if (result is null) return Results.NotFound();
            if (result.ETag is not null)
                ctx.Response.Headers["ETag"] = $"\"{result.ETag}\"";
            return Results.Ok(result);
        }).WithName("GetPaymentById");

        group.MapPost("/", async ([FromBody] CreatePaymentCommand command, IMediator mediator, CancellationToken ct) =>
        {
            try
            {
                var doku = await mediator.Send(command, ct);
                return Results.Created($"/api/payments/{doku}", new { Doku = doku });
            }
            catch (GalvaERP.Common.Exceptions.DomainException ex)
            {
                return Results.Problem(statusCode: 422, title: "Business Rule Violation", detail: ex.Message);
            }
            catch (GalvaERP.Common.Exceptions.NotFoundException ex)
            {
                return Results.Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
            }
        }).WithName("CreatePayment");

        group.MapPut("/{doku}", async (string doku, [FromBody] UpdatePaymentCommand command, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            try
            {
                var commandWithDoku = command with { Doku = doku };
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
        }).WithName("UpdatePayment");

        return app;
    }
}
