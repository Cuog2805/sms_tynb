using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master
{
	[Table(name: "m_sms_file")]
	public class MSmsFile
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdSmsFile { get; set; }
		public long IdSms { get; set; }
		public long IdFile { get; set; }
	}
}
