using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_group_employee")]
	public class MGroupEmployee: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long GroupEmployeeId { get; set; }
		/// <summary>
		/// key nhóm
		/// </summary>
		public long GroupId { get; set; }
		/// <summary>
		/// key cán bộ
		/// </summary>
		public long EmployeeId { get; set; }
	}
}
