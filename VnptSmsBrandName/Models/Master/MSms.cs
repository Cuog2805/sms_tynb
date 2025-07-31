using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_sms")]
	public class MSms: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdSms { get; set; }

		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Content { get; set; }
		public int? NumberMessages { get; set; }
		public int? NumberMessageError { get; set; }
	}

}
