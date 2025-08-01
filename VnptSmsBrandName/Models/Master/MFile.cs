using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_file")]
	public class MFile: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long FileId { get; set; }

		/// <summary>
		/// tên file
		/// </summary>
		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Name { get; set; }

		/// <summary>
		/// file url
		/// </summary>
		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string FileUrl { get; set; }

		/// <summary>
		/// loại file, vd: image, video, audio, document
		/// </summary>
		[Required]
		[MaxLength(255)]
		[Column(TypeName = "nvarchar(255)")]
		public string Type { get; set; }
	}
}
