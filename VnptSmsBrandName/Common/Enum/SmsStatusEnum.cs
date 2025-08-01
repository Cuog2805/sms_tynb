using System.ComponentModel.DataAnnotations;

namespace VnptSmsBrandName.Common.Enum
{
	/// <summary>
	/// enum tr?ng thái tin nh?n
	/// </summary>
	public enum SmsStatusEnum
	{
		[Display(Name = "Lỗi gửi")]
		Error = 0,
		[Display(Name = "Thành công")]
		Sent = 1,
	}
}
