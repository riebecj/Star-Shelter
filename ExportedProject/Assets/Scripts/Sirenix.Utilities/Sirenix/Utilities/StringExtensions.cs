using System;
using System.Globalization;
using System.Text;

namespace Sirenix.Utilities
{
	public static class StringExtensions
	{
		public static string ToTitleCase(this string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (c == '_' && i + 1 < input.Length)
				{
					char c2 = input[i + 1];
					if (char.IsLower(c2))
					{
						c2 = char.ToUpper(c2, CultureInfo.InvariantCulture);
					}
					stringBuilder.Append(c2);
					i++;
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static bool Contains(this string source, string toCheck, StringComparison comparisonType)
		{
			return source.IndexOf(toCheck, comparisonType) >= 0;
		}

		public static string SplitPascalCase(this string input)
		{
			StringBuilder stringBuilder = new StringBuilder(input.Length);
			stringBuilder.Append(char.ToUpper(input[0]));
			for (int i = 1; i < input.Length; i++)
			{
				char c = input[i];
				if (char.IsUpper(c) && !char.IsUpper(input[i - 1]))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public static bool IsNullOrWhitespace(this string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				for (int i = 0; i < str.Length; i++)
				{
					if (!char.IsWhiteSpace(str[i]))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
