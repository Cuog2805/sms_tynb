using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models.Master
{
    public partial class WpNhom
    {
        public WpNhom()
        {
            WpNhomCanbo = new HashSet<WpNhomCanbo>();
            WpSmsCanbo = new HashSet<WpSmsCanbo>();
        }

        public int IdNhom { get; set; }
        public int? IdNhomCha { get; set; }
        public string? TenNhom { get; set; }
        public int? TrangThai { get; set; }

        public virtual ICollection<WpNhomCanbo> WpNhomCanbo { get; set; }
        public virtual ICollection<WpSmsCanbo> WpSmsCanbo { get; set; }
    }
}
