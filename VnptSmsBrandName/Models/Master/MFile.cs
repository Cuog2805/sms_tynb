using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_file")]
	public class MFile: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdFile { get; set; }

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
