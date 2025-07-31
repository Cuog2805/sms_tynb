using System.Globalization;
using System.Text;

namespace VnptSmsBrandName.Helper
{
	public class CommonHelper
	{
		// string
		private static readonly string[] vietnameseSigns = new string[]
		{

			"aAeEoOuUiIdDyY",

			"��??��?????a?????",

			"��??��?????A?????",

			"��???�?????",

			"��???�?????",

			"��??��?????o?????",

			"��??��?????O?????",

			"��??uu?????",

			"��??UU?????",

			"��??i",

			"��??I",

			"d",

			"�",

			"�????",

			"�????"
		};
		public static string RemoveSign4VietnameseString(string str)
		{
			for (int i = 1; i < vietnameseSigns.Length; i++)
			{
				for (int j = 0; j < vietnameseSigns[i].Length; j++)
					str = str.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
			}
			return str;
		}

		public static string RemoveUnicodeMark(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			var normalized = input.Normalize(NormalizationForm.FormD);
			var filtered = normalized
				.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
				.ToArray();

			return new string(filtered).Normalize(NormalizationForm.FormC);
		}

	}
}
