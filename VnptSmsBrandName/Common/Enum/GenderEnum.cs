using System.ComponentModel.DataAnnotations;

namespace VnptSmsBrandName.Common.Enum
{
	/// <summary>
	/// enum gi?i tính
	/// </summary>
	public enum GenderEnum
	{
		[Display(Name = "Nữ")]
		Female = 0,
		[Display(Name = "Nam")]
		Male = 1,
	}
}
