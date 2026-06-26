using System.Security.Claims;

namespace GalvaERP.Common.Security;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string username, string role);

    string GenerateRefreshToken();

    (ClaimsPrincipal? principal, bool isValid) ValidateAccessToken(string token);
}
