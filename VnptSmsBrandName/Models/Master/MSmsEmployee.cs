using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "m_sms_employee")]
	public class MSmsEmployee
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdSmsEmployee { get; set; }
		public long IdSms { get; set; }
		public long IdEmployee { get; set; }
		public long IdGroup { get; set; }
		/// <summary>
		/// id t? ch?c
		/// </summary>
		public long IdOrganization { get; set; }

		/// <summary>
		/// id c?a req tr? v? khi g?i tin nh?n
		/// </summary>
		public string REQID { get; set; }

		/// <summary>
		/// tên SmsResponse tr? v? khi g?i tin nh?n
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// tr?ng thái tr? v? khi g?i tin nh?n
		/// </summary>
		public string ERROR { get; set; }
		/// <summary>
		/// mô t? tr?ng thái tr? v? khi g?i tin nh?n
		/// </summary>
		public string ERROR_DESC { get; set; }
	}
}
