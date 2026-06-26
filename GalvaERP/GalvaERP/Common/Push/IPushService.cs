namespace GalvaERP.Common.Push;

public interface IPushService
{
    Task SendNotificationAsync(string endpoint, string p256dh, string auth, string payload, CancellationToken cancellationToken);
}
