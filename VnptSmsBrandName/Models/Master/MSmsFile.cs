using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_sms_file")]
	public class MSmsFile: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long SmsFileId { get; set; }
		public long SmsId { get; set; }
		public long FileId { get; set; }
	}
}
