using VnptSmsBrandName.Models;
using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.ViewModel
{
	public class MSmsViewModel
	{
		public long SmsId { get; set; }

		public string? Content { get; set; }
		public string? ContentSend { get; set; }

		public List<MFile>? FileAttach { get; set; }
		public int? NumberMessages { get; set; }
		public int? NumberMessageError { get; set; }

		public string? CreatorId { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime? CreatedAt { get; set; }

		/// <summary>
		/// dùng cho truyền danh sách cán bộ được tin nhắn gửi đến
		/// </summary>
		public virtual ICollection<MEmployeeViewModel> Employees { get; set; } = new List<MEmployeeViewModel>();
		/// <summary>
		/// dùng cho hiện thị chi tiết các cán bộ nhận tin nhắn
		/// </summary>
		public virtual ICollection<MEmployeeMessageStatisticalViewModel> EmployeesView { get; set; } = new List<MEmployeeMessageStatisticalViewModel>();
	}

	public class MSmsSearchViewModel
	{
		public long? SmsId { get; set; }
		public string? searchInput { get; set; }
		public DateTime? dateFrom { get; set; }
		public DateTime? dateTo { get; set; }
		public long? GroupId { get; set; }
		public long? EmployeeId { get; set; }
		public long? FileId { get; set; }
		public long? Status { get; set; }
	}
}
