using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models;

public partial class WpNhomCanbo
{
    public int IdNhomCanbo { get; set; }

    public int IdNhom { get; set; }

    public int IdCanbo { get; set; }

    public virtual WpCanbo IdCanboNavigation { get; set; } = null!;

    public virtual WpNhom IdNhomNavigation { get; set; } = null!;
}
