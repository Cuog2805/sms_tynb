using VnptSmsBrandName.Models.Master;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VnptSmsBrandName.ViewModel
{
	public class MFileViewModel
	{
		public long FileId { get; set; }
		public string? Name { get; set; }
		public string? FileUrl { get; set; }
		public string? Type { get; set; }
		public IEnumerable<MHistory>? History { get; set; }
	}
}
