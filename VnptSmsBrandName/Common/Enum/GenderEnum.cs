using System.ComponentModel.DataAnnotations;

namespace SMS_TYNB.Common.Enum
{
	/// <summary>
	/// enum giới tính
	/// </summary>
	public enum GenderEnum
	{
		[Display(Name = "Nữ")]
		Female = 0,
		[Display(Name = "Nam")]
		Male = 1,
	}
}
