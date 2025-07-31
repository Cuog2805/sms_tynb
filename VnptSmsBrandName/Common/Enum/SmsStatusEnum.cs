using System.ComponentModel.DataAnnotations;

namespace VnptSmsBrandName.Common.Enum
{
	/// <summary>
	/// enum tr?ng th�i tin nh?n
	/// </summary>
	public enum SmsStatusEnum
	{
		[Display(Name = "L?i g?i")]
		Error = 0,
		[Display(Name = "Th�nh c�ng")]
		Sent = 1,
	}
}
