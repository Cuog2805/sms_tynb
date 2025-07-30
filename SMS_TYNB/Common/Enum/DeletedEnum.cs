using System.ComponentModel.DataAnnotations;

namespace SMS_TYNB.Common.Enum
{
	/// <summary>
	/// enum trạng thái xóa
	/// </summary>
	public enum DeletedEnum
	{
		[Display(Name = "Sử dụng")]
		NotDeleted = 0,
		[Display(Name = "Xóa")]
		Deleted = 1,
	}
}
