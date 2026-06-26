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

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetInvoicesQuery(), ct);
            return Results.Ok(result);
        }).WithName("GetInvoices");

        group.MapGet("/{doku}", async (string doku, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetInvoiceByIdQuery(doku), ct);
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

        group.MapPut("/{doku}", async (string doku, [FromBody] UpdateAPInvoiceCommand command, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
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
        }).WithName("UpdateAPInvoice");

        return app;
    }
}
