namespace GalvaERP.Features.Push.DTOs;

public record PushSubscriptionRequest(string Endpoint, string P256dh, string Auth);

public record PushTestRequest(string? Message);
