using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Tx_PushSubscription
{
    public int SubscriptionId { get; set; }

    public int UserId { get; set; }

    public string Endpoint { get; set; } = null!;

    public string P256dh { get; set; } = null!;

    public string AuthKey { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Master_User User { get; set; } = null!;
}
