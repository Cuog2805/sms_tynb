using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_employee")]
	public class MEmployee: BaseModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long EmployeeId { get; set; }

		/// <summary>
		/// tên cán bộ
		/// </summary>
		[Required]
		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		public string Name { get; set; }

		/// <summary>
		/// số điện thoại cán bộ
		/// </summary>
		[Required]
		[MaxLength(15)]
		[Column(TypeName = "nvarchar(15)")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// sdt gửi tin nhắn
		/// </summary>
		[Required]
		[MaxLength(15)]
		[Column(TypeName = "nvarchar(15)")]
		public string PhoneNumberSend { get; set; }

		/// <summary>
		/// mô tả cán bộ
		/// </summary>
		[MaxLength(2000)]
		[Column(TypeName = "nvarchar(2000)")]
		public string? Description { get; set; }

		/// <summary>
		/// giới tính
		/// </summary>
		public int Gender { get; set; }
	}
}
