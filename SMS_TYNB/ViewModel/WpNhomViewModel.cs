using SMS_TYNB.Models;

namespace SMS_TYNB.ViewModel
{
	public class WpNhomViewModel
	{
		public int IdNhom { get; set; }

		public int? IdNhomCha { get; set; }

		public string? TenNhom { get; set; }
		public string? TenNhomCha { get; set; }

		public string? TrangThai { get; set; }

		public List<WpCanboViewModel> WpCanbos { get; set;}
	}
	public class WpNhomSearchViewModel
	{
		public string? searchInput { get; set; }
		public int? TrangThai { get; set; }
	}
}
