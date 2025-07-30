using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Master_v2
{
	public class BaseModel
	{
		/// <summary>
		/// nguoi tao
		/// </summary>
		[MaxLength(250)]
		[Column(TypeName = "nvarchar(250)")]
		public string CreateBy { get; set; }
		/// <summary>
		/// ngay tao
		/// </summary>
		public DateTime CreateAt { get; set; }
		/// <summary>
		/// nguoi update
		/// </summary>
		[MaxLength(250)]
		[Column(TypeName = "nvarchar(250)")]
		public string UpdatedBy { get; set; }
		/// <summary>
		/// ngay update
		/// </summary>
		public DateTime UpdatedAt { get; set; }
		/// <summary>
		/// trạng thái xóa, default value = 0
		/// </summary>
		public int IsDeleted { get; set; } = 0;
	}
}
