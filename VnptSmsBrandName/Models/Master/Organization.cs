using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace VnptSmsBrandName.Models.Master
{
	[Table(name: "organization")]
	public class Organization
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long IdOrganization { get; set; }

		[MaxLength(500)]
		[Column(TypeName = "nvarchar(500)")]
		[Required]
		public string Name { get; set; }
	}
}
