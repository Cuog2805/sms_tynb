using SMS_TYNB.Models;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.ViewModel
{
	public class WpSmsViewModel
	{
		public long IdSms { get; set; }

		public string? Noidung { get; set; }
		public string? NoidungGui { get; set; }

		public List<WpFile>? FileDinhKem { get; set; }

		public string? IdNguoigui { get; set; }
		public string? TenNguoigui { get; set; }

		public DateTime? Ngaygui { get; set; }

		public int? SoTn { get; set; }
		public int? SoTnLoi { get; set; }

		public virtual ICollection<WpCanboViewModel> WpCanbos { get; set; } = new List<WpCanboViewModel>();
	}

	public class WpSmsSearchViewModel
	{
		public string? searchInput { get; set; }
		public DateTime? dateFrom { get; set; }
		public DateTime? dateTo { get; set; }
	}
}
