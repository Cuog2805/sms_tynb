namespace VnptSmsBrandName.ViewModel
{
	public class MEmployeeViewModel
	{
		public long? IdEmployee { get; set; }

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
		public long? IdEmployee { get; set; }

		public string? Name { get; set; }

		public string? PhoneNumber { get; set; }

		public long? IdGroup { get; set; }
		public string? GroupName { get; set; }
		/// <summary>
		/// tr?ng thái tin nh?n g?i d?n cán b?
		/// </summary>
		public string? ERROR { get; set; }
		/// <summary>
		/// mô t? l?i c?a tin nh?n g?i d?n cán b?
		/// </summary>
		public string? ERROR_DESC { get; set; }
	}

	/// <summary>
	/// ViewModel cho import danh sách cán b? t? file excel
	/// </summary>
	public class MEmployeeCreateRangeViewModel
	{
		/// <summary>
		/// danh sách cán b? dã du?c import thành công
		/// </summary>
		public List<MEmployeeViewModel>? MEmployeeNew { get; set; }
		/// <summary>
		/// danh sách cán b? b? trùng s? di?n tho?i
		/// </summary>
		public List<MEmployeeViewModel>? MEmployeeDupplicate { get; set; }
	}
	public class MEmployeeSearchViewModel
	{
		public string? searchInput { get; set; }
		public int? IsDeleted { get; set; }
	}
}
