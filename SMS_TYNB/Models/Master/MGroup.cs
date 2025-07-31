using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master
{
	[Table(name: "m_group")]
	public class MGroup: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdGroup { get; set; }
		public long? IdGroupParent { get; set; }

		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Name { get; set; }
	}
}
