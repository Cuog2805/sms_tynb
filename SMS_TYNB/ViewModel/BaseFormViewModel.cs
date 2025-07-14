using Microsoft.AspNetCore.Mvc.Rendering;
using SMS_TYNB.Models;

namespace SMS_TYNB.ViewModel
{
	public class BaseFormViewModel<T>
	{
		public T Data { get; set; }
		public Dictionary<string, SelectList> SelectLists { get; set; }
	}
}
