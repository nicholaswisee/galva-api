using GalvaERP.Features.GoodsReceipts.Commands;
using GalvaERP.Features.GoodsReceipts.DTOs;
using GalvaERP.Features.GoodsReceipts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalvaERP.Features.GoodsReceipts;

public static class GoodsReceiptEndpoints
{
    public static IEndpointRouteBuilder MapGoodsReceiptEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/goods-receipts").WithTags("GoodsReceipts").WithOpenApi();

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetGoodsReceiptsQuery(), ct);
            return Results.Ok(result);
        }).WithName("GetGoodsReceipts");

        group.MapGet("/{doku}", async (string doku, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetGoodsReceiptByIdQuery(doku), ct);
            if (result is null) return Results.NotFound();
            if (result.ETag is not null)
                ctx.Response.Headers["ETag"] = $"\"{result.ETag}\"";
            return Results.Ok(result);
        }).WithName("GetGoodsReceiptById");

        group.MapPost("/", async ([FromBody] CreateGoodsReceiptCommand command, IMediator mediator, CancellationToken ct) =>
        {
            try
            {
                var doku = await mediator.Send(command, ct);
                return Results.Created($"/api/goods-receipts/{doku}", new { Doku = doku });
            }
            catch (GalvaERP.Common.Exceptions.DomainException ex)
            {
                return Results.Problem(statusCode: 422, title: "Business Rule Violation", detail: ex.Message);
            }
            catch (GalvaERP.Common.Exceptions.NotFoundException ex)
            {
                return Results.Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
            }
        }).WithName("CreateGoodsReceipt");

        group.MapPut("/{doku}", async (string doku, [FromBody] UpdateGoodsReceiptCommand command, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
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
        }).WithName("UpdateGoodsReceipt");

        return app;
    }
}
