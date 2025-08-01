using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_sms")]
	public class MSms: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long SmsId { get; set; }

		/// <summary>
		/// Nội dung tin nhắn
		/// </summary>
		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Content { get; set; }
		/// <summary>
		/// số tin nhắn gửi đi
		/// </summary>
		public int? NumberMessages { get; set; }
		/// <summary>
		/// số tin nhắn gửi lỗi
		/// </summary>
		public int? NumberMessageError { get; set; }
	}

}
