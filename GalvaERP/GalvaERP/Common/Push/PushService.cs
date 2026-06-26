using System.Net;
using GalvaERP.Common.Exceptions;
using Microsoft.Extensions.Options;
using WebPush;

namespace GalvaERP.Common.Push;

public class PushService : IPushService
{
    private readonly VapidOptions _vapidOptions;

    public PushService(IOptions<VapidOptions> vapidOptions)
    {
        _vapidOptions = vapidOptions.Value;
    }

    public async Task SendNotificationAsync(string endpoint, string p256dh, string auth, string payload, CancellationToken cancellationToken)
    {
        var subscription = new PushSubscription(endpoint, p256dh, auth);
        var vapidDetails = new VapidDetails(_vapidOptions.Subject, _vapidOptions.PublicKey, _vapidOptions.PrivateKey);

        var webPushClient = new WebPushClient();
        webPushClient.SetVapidDetails(vapidDetails);

        try
        {
            await webPushClient.SendNotificationAsync(subscription, payload, vapidDetails, cancellationToken);
        }
        catch (WebPushException ex) when (ex.StatusCode == HttpStatusCode.Gone)
        {
            // Subscription expired — caller should remove it
            throw new NotFoundException("Push subscription expired");
        }
    }
}
