using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models.Master
{
    public partial class WpSmsCanbo
    {
        public long IdSmsCanbo { get; set; }
        public long IdSms { get; set; }
        public int IdCanbo { get; set; }
        public int IdNhom { get; set; }
		public string REQID { get; set; }
		public string name { get; set; }
		public string ERROR { get; set; }
		public string ERROR_DESC { get; set; }

		public virtual WpCanbo IdCanboNavigation { get; set; } = null!;
        public virtual WpNhom IdNhomNavigation { get; set; } = null!;
        public virtual WpSms IdSmsNavigation { get; set; } = null!;
    }
}
