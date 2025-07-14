using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models;

public partial class WpTerms
{
    public ulong TermId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public long TermGroup { get; set; }
}
