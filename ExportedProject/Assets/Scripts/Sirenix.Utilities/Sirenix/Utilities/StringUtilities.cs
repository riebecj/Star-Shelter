using System;
using System.Text;

namespace Sirenix.Utilities
{
	public static class StringUtilities
	{
		public static string NicifyByteSize(int bytes, int decimals = 1)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (bytes < 0)
			{
				stringBuilder.Append('-');
				bytes = Math.Abs(bytes);
			}
			int num = 0;
			string text = null;
			if (bytes > 1000000000)
			{
				stringBuilder.Append(bytes / 1000000000);
				bytes -= bytes / 1000000000 * 1000000000;
				num = 9;
				text = " GB";
			}
			else if (bytes > 1000000)
			{
				stringBuilder.Append(bytes / 1000000);
				bytes -= bytes / 1000000 * 1000000;
				num = 6;
				text = " MB";
			}
			else if (bytes > 1000)
			{
				stringBuilder.Append(bytes / 1000);
				bytes -= bytes / 1000 * 1000;
				num = 3;
				text = " KB";
			}
			else
			{
				stringBuilder.Append(bytes);
				decimals = 0;
				num = 0;
				text = " bytes";
			}
			if (decimals > 0 && num > 0 && bytes > 0)
			{
				string text2 = bytes.ToString().PadLeft(num, '0');
				text2 = text2.Substring(0, (decimals < text2.Length) ? decimals : text2.Length).TrimEnd('0');
				if (text2.Length > 0)
				{
					stringBuilder.Append('.');
					stringBuilder.Append(text2);
				}
			}
			stringBuilder.Append(text);
			return stringBuilder.ToString();
		}
	}
}
