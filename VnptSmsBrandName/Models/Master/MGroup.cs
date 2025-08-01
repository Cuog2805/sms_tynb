using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_group")]
	public class MGroup: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long GroupId { get; set; }
		/// <summary>
		/// key của nhóm cha
		/// </summary>
		public long? GroupParentId { get; set; }

		/// <summary>
		/// tên nhóm
		/// </summary>
		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Name { get; set; }
	}
}
