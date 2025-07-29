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
		/// <summary>
		/// dùng cho truyền danh sách cán bộ được tin nhắn gửi đến
		/// </summary>
		public virtual ICollection<WpCanboViewModel> WpCanbos { get; set; } = new List<WpCanboViewModel>();
		/// <summary>
		/// dùng cho hiển thị chi tiết các cán bộ nhận tin nhắn
		/// </summary>
		public virtual ICollection<WpCanboMessageStatisticalViewModel> WpCanbosView { get; set; } = new List<WpCanboMessageStatisticalViewModel>();
	}

	public class WpSmsSearchViewModel
	{
		public long? IdSms { get; set; }
		public string? searchInput { get; set; }
		public DateTime? dateFrom { get; set; }
		public DateTime? dateTo { get; set; }
		public long? IdNhom { get; set; }
		public long? IdCanBo { get; set; }
		public long? IdFile { get; set; }
		public long? Trangthai { get; set; }
	}
}
