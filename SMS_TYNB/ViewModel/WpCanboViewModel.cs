namespace SMS_TYNB.ViewModel
{
	public class WpCanboViewModel
	{
		public int? IdCanbo { get; set; }

		public string? MaCanbo { get; set; }

		public string? TenCanbo { get; set; }

		public string? SoDt { get; set; }
		public string? SoDTGui { get; set; }

		public string? Mota { get; set; }

		public string? Gioitinh { get; set; }

		public string? Trangthai { get; set; }

		public int? IdNhom { get; set; }
		public string? TenNhom { get; set; }
	}

	public class WpCanboMessageStatisticalViewModel
	{
		public int? IdCanbo { get; set; }

		public string? TenCanbo { get; set; }

		public string? SoDt { get; set; }

		public int? IdNhom { get; set; }
		public string? TenNhom { get; set; }
		/// <summary>
		/// trạng thái tin nhắn gửi đến cán bộ
		/// </summary>
		public string? ERROR { get; set; }
		/// <summary>
		/// mô tả lỗi của tin nhắn gửi đến cán bộ
		/// </summary>
		public string? ERROR_DESC { get; set; }
	}

	public class WpCanboSearchViewModel
	{
		public string? searchInput { get; set; }
		public int? Trangthai { get; set; }
	}
}
