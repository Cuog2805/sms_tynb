using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_history")]
	public class MHistory
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdHistory { get; set; }
		public long IdOrganization { get; set; }
		/// <summary>
		/// tên b?ng luu l?ch s?
		/// </summary>
		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string TableName { get; set; }

		/// <summary>
		/// id c?a b?n ghi ? b?ng luu l?ch s?
		/// </summary>
		public long IdRecord { get; set; }

		[Required]
		[MaxLength(255)]
		[Column(TypeName = "nvarchar(255)")]
		public string Action { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
