using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GalvaERP.Common.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public IdempotencyMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only intercept POST requests
        if (context.Request.Method != "POST")
        {
            await _next(context);
            return;
        }

        // Check for Idempotency-Key header
        if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var keyValues) ||
            string.IsNullOrEmpty(keyValues.ToString()))
        {
            await _next(context);
            return;
        }

        var key = keyValues.ToString();

        // Enable buffering so we can read the body and still let downstream read it
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync(context.RequestAborted);
        context.Request.Body.Position = 0;

        var requestHash = ComputeHash(context.Request.Method + context.Request.Path + body);

        // Check DB for existing idempotency record
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existing = await db.Tx_IdempotencyRecords
            .FirstOrDefaultAsync(r => r.IdempotencyKey == key, context.RequestAborted);

        if (existing != null)
        {
            if (existing.ExpiresAt < DateTime.UtcNow)
            {
                // Expired — delete and fall through to process the request
                db.Tx_IdempotencyRecords.Remove(existing);
                await db.SaveChangesAsync(context.RequestAborted);
            }
            else if (existing.RequestHash != requestHash)
            {
                // Same key, different payload — conflict
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json; charset=utf-8";
                var conflict = new
                {
                    error = "Idempotency-Key reuse with a different request payload is not allowed."
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(conflict), context.RequestAborted);
                return;
            }
            else
            {
                // Return cached response
                context.Response.StatusCode = existing.ResponseStatusCode;
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(existing.ResponseBody, context.RequestAborted);
                return;
            }
        }

        // Proceed with the request, but capture the response
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync(context.RequestAborted);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        await responseBodyStream.CopyToAsync(originalBodyStream, context.RequestAborted);
        context.Response.Body = originalBodyStream;

        // Only cache successful responses (2xx)
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            var record = new Tx_IdempotencyRecord
            {
                IdempotencyKey = key,
                RequestHash = requestHash,
                ResponseStatusCode = context.Response.StatusCode,
                ResponseBody = responseBody,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            db.Tx_IdempotencyRecords.Add(record);
            try
            {
                await db.SaveChangesAsync(context.RequestAborted);
            }
            catch (DbUpdateException)
            {
                // Race condition: another concurrent request stored the record first.
                // The first one wins — silently ignore.
            }
        }
    }

    private static string ComputeHash(string input) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
}
