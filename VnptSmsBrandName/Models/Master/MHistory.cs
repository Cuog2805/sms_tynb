using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_history")]
	public class MHistory
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long HistoryId { get; set; }
		public long OrganizationId { get; set; }
		/// <summary>
		/// tên bảng lưu lịch sử
		/// </summary>
		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string TableName { get; set; }

		/// <summary>
		/// id của bản ghi ở bảng lưu lịch sử
		/// </summary>
		public long RecordId { get; set; }

		[Required]
		[MaxLength(255)]
		[Column(TypeName = "nvarchar(255)")]
		public string Action { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
