using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master_v2
{
	[Table(name: "m_file")]
	public class MFile
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdFile { get; set; }
		public long IdOrganization { get; set; }

		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Name { get; set; }

		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string FileUrl { get; set; }

		[Required]
		[MaxLength(255)]
		[Column(TypeName = "nvarchar(255)")]
		public string Type { get; set; }
	}
}
