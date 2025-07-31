using System.ComponentModel.DataAnnotations;

namespace VnptSmsBrandName.Common.Enum
{
	/// <summary>
	/// enum tr?ng th�i x�a
	/// </summary>
	public enum DeletedEnum
	{
		[Display(Name = "S? d?ng")]
		NotDeleted = 0,
		[Display(Name = "X�a")]
		Deleted = 1,
	}
}
