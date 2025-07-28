using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master
{
	/// <summary>
	/// bảng mapping giữa wp_sms và wp_file
	/// </summary>
	[Table(name: "wp_sms_file")]
	public class WpSmsFile
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdSmsFile { get; set; }
		public long IdSms { get; set; }
		public long IdFile { get; set; }
	}
}
