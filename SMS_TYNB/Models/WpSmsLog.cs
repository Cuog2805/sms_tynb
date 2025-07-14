using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models;

public partial class WpSmsLog
{
    public int IdSms { get; set; }

    public int IdCanbo { get; set; }

    public string NgayGui { get; set; } = null!;

    public int Trangthai { get; set; }

    public string Chitiet { get; set; } = null!;
}
