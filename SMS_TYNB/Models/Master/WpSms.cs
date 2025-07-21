using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models.Master
{
    public partial class WpSms
    {
        public WpSms()
        {
            WpSmsCanbo = new HashSet<WpSmsCanbo>();
        }

        public long IdSms { get; set; }
        public string? Noidung { get; set; }
        public string? FileDinhKem { get; set; }
        public string? IdNguoigui { get; set; }
        public DateTime? Ngaygui { get; set; }
        public int? SoTn { get; set; }

        public virtual ICollection<WpSmsCanbo> WpSmsCanbo { get; set; }
    }
}
