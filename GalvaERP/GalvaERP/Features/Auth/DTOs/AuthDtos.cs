namespace GalvaERP.Features.Auth.DTOs;

public record LoginRequest(string Username, string Password);

public record AuthResponse(string AccessToken, int ExpiresIn);

public record LoginResult(string AccessToken, int ExpiresIn, string RefreshToken);
