using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master_v2
{
	[Table(name: "m_group_employee")]
	public class MGroupEmployee
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int IdGroupEmployee { get; set; }
		public int IdGroup { get; set; }
		public int IdEmployee { get; set; }
	}
}
