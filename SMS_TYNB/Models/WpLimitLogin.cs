using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models;

public partial class WpLimitLogin
{
    public int LoginId { get; set; }

    public string LoginIp { get; set; } = null!;

    public int LoginAttempts { get; set; }

    public DateTime? AttemptTime { get; set; }

    public string LockedTime { get; set; } = null!;
}
