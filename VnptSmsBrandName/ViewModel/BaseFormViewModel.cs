using Microsoft.AspNetCore.Mvc.Rendering;
using VnptSmsBrandName.Models;

namespace VnptSmsBrandName.ViewModel
{
	public class BaseFormViewModel<T>
	{
		public T Data { get; set; }
		public Dictionary<string, SelectList> SelectLists { get; set; }
	}
}
