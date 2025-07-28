using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SMS_TYNB.Models.Master
{
    public partial class WpFile
    {
		public long IdFile { get; set; }
        public string TenFile { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public string DuoiFile { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? BangLuuFile { get; set; } = null!;
		public long? BangLuuFileId { get; set; }
    }
}
