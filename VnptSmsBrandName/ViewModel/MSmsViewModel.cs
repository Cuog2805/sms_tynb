using SMS_TYNB.Models;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.ViewModel
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
		/// dùng cho truyền danh sách cán bộ được tin nhắn gửi đến
		/// </summary>
		public virtual ICollection<MEmployeeViewModel> Employees { get; set; } = new List<MEmployeeViewModel>();
		/// <summary>
		/// dùng cho hiển thị chi tiết các cán bộ nhận tin nhắn
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
