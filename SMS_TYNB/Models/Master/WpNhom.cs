using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models.Master
{
    public partial class WpNhom
    {
        public int IdNhom { get; set; }
        public int? IdNhomCha { get; set; }
        public string? TenNhom { get; set; }
        public int? TrangThai { get; set; }

        public virtual ICollection<WpNhomCanbo> WpNhomCanbo { get; set; }
        public virtual ICollection<WpSmsCanbo> WpSmsCanbo { get; set; }
    }
}
