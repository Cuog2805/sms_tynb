using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_sms_employee")]
	public class MSmsEmployee
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdSmsEmployee { get; set; }
		public long IdSms { get; set; }
		public long IdEmployee { get; set; }
		public long IdGroup { get; set; }
		/// <summary>
		/// id tổ chức
		/// </summary>
		public long IdOrganization { get; set; }

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
