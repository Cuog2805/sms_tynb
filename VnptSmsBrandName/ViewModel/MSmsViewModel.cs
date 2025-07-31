using VnptSmsBrandName.Models;
using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.ViewModel
{
	public class MSmsViewModel
	{
		public long IdSms { get; set; }

		public string? Content { get; set; }
		public string? ContentSend { get; set; }

		public List<MFile>? FileAttach { get; set; }
		public int? NumberMessages { get; set; }
		public int? NumberMessageError { get; set; }

		public string? CreatorId { get; set; }
		public string? CreateBy { get; set; }
		public DateTime? CreateAt { get; set; }

		/// <summary>
		/// dùng cho truy?n danh sách cán b? du?c tin nh?n g?i d?n
		/// </summary>
		public virtual ICollection<MEmployeeViewModel> Employees { get; set; } = new List<MEmployeeViewModel>();
		/// <summary>
		/// dùng cho hi?n th? chi ti?t các cán b? nh?n tin nh?n
		/// </summary>
		public virtual ICollection<MEmployeeMessageStatisticalViewModel> EmployeesView { get; set; } = new List<MEmployeeMessageStatisticalViewModel>();
	}

	public class MSmsSearchViewModel
	{
		public long? IdSms { get; set; }
		public string? searchInput { get; set; }
		public DateTime? dateFrom { get; set; }
		public DateTime? dateTo { get; set; }
		public long? IdGroup { get; set; }
		public long? IdEmployee { get; set; }
		public long? IdFile { get; set; }
		public long? Status { get; set; }
	}
}
