namespace GalvaERP.Common.Web;

public static class RouteParams
{
    /// <summary>
    /// URL-decode a route parameter. ASP.NET Core leaves %2F (encoded slashes)
    /// inside catch-all values untouched for security, but our Doku values
    /// legitimately contain slashes (e.g. "VIEWS-GTC/2501/0677"), so we
    /// decode once at the handler boundary.
    /// </summary>
    public static string Decode(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : Uri.UnescapeDataString(value);
}
