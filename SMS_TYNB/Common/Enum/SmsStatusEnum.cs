using System.ComponentModel.DataAnnotations;

namespace SMS_TYNB.Common.Enum
{
	/// <summary>
	/// enum trạng thái tin nhắn
	/// </summary>
	public enum SmsStatusEnum
	{
		[Display(Name = "Lỗi gửi")]
		Error = 0,
		[Display(Name = "Thành công")]
		Sent = 1,
	}
}
