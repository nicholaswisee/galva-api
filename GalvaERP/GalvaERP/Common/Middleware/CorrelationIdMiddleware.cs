using System.Diagnostics;

namespace GalvaERP.Common.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Activity.Current?.Id;

        if (context.Request.Headers.TryGetValue(HeaderName, out var incoming) &&
            !string.IsNullOrWhiteSpace(incoming))
        {
            correlationId = incoming.ToString();
        }
        else if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using var activity = new Activity("HttpRequest");
        activity.SetTag("correlationId", correlationId);
        activity.Start();

        try
        {
            await _next(context);
        }
        finally
        {
            activity.Stop();
        }
    }
}
