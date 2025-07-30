using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master_v2
{
	[Table(name: "m_sms")]
	public class MSms
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdSms { get; set; }
		public long IdOrganization { get; set; }

		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Content { get; set; }
		public int? NumberMessages { get; set; }
		public int? NumberMessageError { get; set; }
	}

}
