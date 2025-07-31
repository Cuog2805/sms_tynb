using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace VnptSmsBrandName.Helper
{
	public static class EnumHelper
	{
		public static IEnumerable<SelectListItem> ToSelectListItem<TEnum>() where TEnum : Enum
		{
			return Enum.GetValues(typeof(TEnum))
						.Cast<TEnum>()
						.Select(e => new SelectListItem
						{
							Value = Convert.ToInt32(e).ToString(),
							Text = e.GetDisplayName()
						});
		}
		public static Dictionary<int, string> ToDictionary<TEnum>() where TEnum : Enum
		{
			return Enum.GetValues(typeof(TEnum))
						.Cast<TEnum>()
						.ToDictionary(e => Convert.ToInt32(e), e => e.GetDisplayName());
		}
		public static string GetDisplayName(this Enum enumValue)
		{
			return enumValue.GetType()
							.GetMember(enumValue.ToString())
							.First()
							.GetCustomAttribute<DisplayAttribute>()
							?.GetName() ?? enumValue.ToString();
		}
	}
}
