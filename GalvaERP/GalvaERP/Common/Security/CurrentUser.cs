using System.Security.Claims;

namespace GalvaERP.Common.Security;

public static class CurrentUser
{
    public static int? GetUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : null;
    }
}
