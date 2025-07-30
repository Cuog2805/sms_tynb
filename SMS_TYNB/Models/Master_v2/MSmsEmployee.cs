using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master_v2
{
	[Table(name: "m_sms_employee")]
	public class MSmsEmployee
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdSmsEmployee { get; set; }
		public long IdSms { get; set; }
		public int IdEmployee { get; set; }
		public int IdGroup { get; set; }

		/// <summary>
		/// id của req trả về khi gửi tin nhắn
		/// </summary>
		public string REQID { get; set; }

		/// <summary>
		/// tên SmsResponse trả về khi gửi tin nhắn
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// trạng thái trả về khi gửi tin nhắn
		/// </summary>
		public string ERROR { get; set; }
		/// <summary>
		/// mô tả trạng thái trả về khi gửi tin nhắn
		/// </summary>
		public string ERROR_DESC { get; set; }
	}
}
