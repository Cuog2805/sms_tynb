using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_group_employee")]
	public class MGroupEmployee
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdGroupEmployee { get; set; }
		public long IdGroup { get; set; }
		public long IdEmployee { get; set; }
		/// <summary>
		/// id tổ chức
		/// </summary>
		public long IdOrganization { get; set; }
	}
}
