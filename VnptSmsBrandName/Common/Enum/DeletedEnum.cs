using System.ComponentModel.DataAnnotations;

namespace VnptSmsBrandName.Common.Enum
{
	/// <summary>
	/// enum tr?ng thái xóa
	/// </summary>
	public enum DeletedEnum
	{
		[Display(Name = "Sử dụng")]
		NotDeleted = 0,
		[Display(Name = "Xóa")]
		Deleted = 1,
	}
}
