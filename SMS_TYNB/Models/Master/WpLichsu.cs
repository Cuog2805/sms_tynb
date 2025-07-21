using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models.Master
{
    public partial class WpLichsu
    {
        public long IdLichsu { get; set; }
        public string BangLuuLichsu { get; set; } = null!;
        public long BangLuuLichsuId { get; set; }
        public string HanhDong { get; set; } = null!;
        public string NguoiTao { get; set; } = null!;
        public DateTime NgayTao { get; set; }
    }
}
