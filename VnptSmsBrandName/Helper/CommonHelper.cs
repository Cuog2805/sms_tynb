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

			"áà??ãâ?????a?????",

			"ÁÀ??ÃÂ?????A?????",

			"éè???ê?????",

			"ÉÈ???Ê?????",

			"óò??õô?????o?????",

			"ÓÒ??ÕÔ?????O?????",

			"úù??uu?????",

			"ÚÙ??UU?????",

			"íì??i",

			"ÍÌ??I",

			"d",

			"Ğ",

			"ı????",

			"İ????"
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
