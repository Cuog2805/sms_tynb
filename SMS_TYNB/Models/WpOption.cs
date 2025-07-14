using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models;

public partial class WpOption
{
    public ulong OptionId { get; set; }

    public string OptionName { get; set; } = null!;

    public string OptionValue { get; set; } = null!;

    public string Autoload { get; set; } = null!;
}
