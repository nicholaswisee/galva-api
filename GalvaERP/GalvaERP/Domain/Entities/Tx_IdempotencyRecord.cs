using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Tx_IdempotencyRecord
{
    public string IdempotencyKey { get; set; } = null!;

    public string RequestHash { get; set; } = null!;

    public int ResponseStatusCode { get; set; }

    public string ResponseBody { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}
