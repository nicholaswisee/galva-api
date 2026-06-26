namespace GalvaERP.Common.Push;

public class VapidOptions
{
    public const string ConfigSection = "VAPID";

    public string Subject { get; set; } = string.Empty;

    public string PublicKey { get; set; } = string.Empty;

    public string PrivateKey { get; set; } = string.Empty;
}
