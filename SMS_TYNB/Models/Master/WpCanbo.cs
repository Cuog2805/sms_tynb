using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models.Master
{
    public partial class WpCanbo
    {
        public WpCanbo()
        {
            WpNhomCanbo = new HashSet<WpNhomCanbo>();
            WpSmsCanbo = new HashSet<WpSmsCanbo>();
        }

        public int IdCanbo { get; set; }
        public string? MaCanbo { get; set; }
        public string? TenCanbo { get; set; }
        public string? SoDt { get; set; }
        public string? Mota { get; set; }
        public int? Gioitinh { get; set; }
        public int? Trangthai { get; set; }
        public string? IdUser { get; set; }

        public virtual ICollection<WpNhomCanbo> WpNhomCanbo { get; set; }
        public virtual ICollection<WpSmsCanbo> WpSmsCanbo { get; set; }
    }
}
