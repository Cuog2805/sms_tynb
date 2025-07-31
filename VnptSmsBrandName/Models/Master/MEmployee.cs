using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SMS_TYNB.Models.Master
{
	[Table(name: "m_employee")]
	public class MEmployee: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdEmployee { get; set; }

		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Name { get; set; }

		[Required]
		[MaxLength(15)]
		[Column(TypeName = "nvarchar(15)")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// sđt gửi tin nhắn
		/// </summary>
		[Required]
		[MaxLength(15)]
		[Column(TypeName = "nvarchar(15)")]
		public string PhoneNumberSend { get; set; }

		[MaxLength(2000)]
		[Column(TypeName = "nvarchar(2000)")]
		public string? Description { get; set; }

		public int Gender { get; set; }
	}
}
