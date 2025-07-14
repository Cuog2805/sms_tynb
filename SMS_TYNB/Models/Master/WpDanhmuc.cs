using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models.Master;

public partial class WpDanhmuc
{
    public int IdDanhmuc { get; set; }

    public int? MaDanhmuc { get; set; }

    public string? TenDanhmuc { get; set; }

    public string? Type { get; set; }
}
