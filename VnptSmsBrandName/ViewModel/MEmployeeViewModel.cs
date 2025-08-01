namespace VnptSmsBrandName.ViewModel
{
	public class MEmployeeViewModel
	{
		public long? EmployeeId { get; set; }

		public string? Name { get; set; }

		public string? PhoneNumber { get; set; }
		public string? PhoneNumberSend { get; set; }

		public string? Description { get; set; }

		public string? Gender { get; set; }

		public string? IsDeleted { get; set; }

		public long? IdGroup { get; set; }
		public string? GroupName { get; set; }
	}

	public class MEmployeeMessageStatisticalViewModel
	{
		public long? EmployeeId { get; set; }

		public string? Name { get; set; }

		public string? PhoneNumber { get; set; }

		public long? GroupId { get; set; }
		public string? GroupName { get; set; }
		/// <summary>
		/// trạng thái tin nhắn gửi đến cán bộ
		/// </summary>
		public string? ERROR { get; set; }
		/// <summary>
		/// mô tả lỗi của tin nhắn gửi đến cán bộ
		/// </summary>
		public string? ERROR_DESC { get; set; }
	}

	/// <summary>
	/// ViewModel cho import danh sách cán bộ từ file excel
	/// </summary>
	public class MEmployeeCreateRangeViewModel
	{
		/// <summary>
		/// danh sách cán bộ đã được import thành công
		/// </summary>
		public List<MEmployeeViewModel>? MEmployeeNew { get; set; }
		/// <summary>
		/// danh sách cán bộ bị trùng số điện thoại
		/// </summary>
		public List<MEmployeeViewModel>? MEmployeeDupplicate { get; set; }
	}
	public class MEmployeeSearchViewModel
	{
		public string? searchInput { get; set; }
		public int? IsDeleted { get; set; }
	}
}
