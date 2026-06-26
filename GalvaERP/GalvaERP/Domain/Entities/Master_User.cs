using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Master_User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? RefreshTokenHash { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Tx_PushSubscription> Tx_PushSubscriptions { get; set; } = new List<Tx_PushSubscription>();
}
