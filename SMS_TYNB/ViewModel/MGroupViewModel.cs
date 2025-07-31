using SMS_TYNB.Models;

namespace SMS_TYNB.ViewModel
{
	public class MGroupViewModel
	{
		public long IdGroup { get; set; }

		public long? IdGroupParent { get; set; }

		public string? Name { get; set; }
		public string? ParentName { get; set; }

		public string? IsDeleted { get; set; }

		public List<MEmployeeViewModel> Employees { get; set; }
	}
	public class MGroupSearchViewModel
	{
		public string? searchInput { get; set; }
		public int? IsDeleted { get; set; }
	}
}
