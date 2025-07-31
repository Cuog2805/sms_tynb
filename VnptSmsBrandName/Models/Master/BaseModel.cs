using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	public class BaseModel
	{
		/// <summary>
		/// người tao
		/// </summary>
		[MaxLength(250)]
		[Column(TypeName = "nvarchar(250)")]
		public string CreateBy { get; set; }
		/// <summary>
		/// ngày tạo
		/// </summary>
		public DateTime CreateAt { get; set; }
		/// <summary>
		/// người update
		/// </summary>
		[MaxLength(250)]
		[Column(TypeName = "nvarchar(250)")]
		public string? UpdatedBy { get; set; }
		/// <summary>
		/// ngày update
		/// </summary>
		public DateTime? UpdatedAt { get; set; }
		/// <summary>
		/// id tổ chức
		/// </summary>
		public long IdOrganization { get; set; }
		/// <summary>
		/// trạng thái xóa, default value = 0
		/// </summary>
		public int IsDeleted { get; set; } = 0;
	}
}
