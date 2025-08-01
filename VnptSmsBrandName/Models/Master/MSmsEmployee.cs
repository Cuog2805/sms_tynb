using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_sms_employee")]
	public class MSmsEmployee: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long SmsEmployeeId { get; set; }
		/// <summary>
		/// key tin nhắn
		/// </summary>
		public long SmsId { get; set; }
		/// <summary>
		/// key cán bộ
		/// </summary>
		public long EmployeeId { get; set; }
		/// <summary>
		/// key nhóm
		/// </summary>
		public long GroupId { get; set; }

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
