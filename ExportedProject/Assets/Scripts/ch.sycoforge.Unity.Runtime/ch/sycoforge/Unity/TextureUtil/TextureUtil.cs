using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace ch.sycoforge.Unity.TextureUtil
{
	public static class TextureUtil
	{
		public enum PixelMode
		{
			Repeat = 0,
			RepeatComplete = 1,
			NearestNeighbour = 2,
			Clamp = 3,
			Bilinear = 4
		}

		public static Color32 ToColor32(int c)
		{
			Color32 result = default(Color32);
			result.a = (byte)((uint)(c >> 24) & 0xFFu);
			result.r = (byte)((uint)(c >> 16) & 0xFFu);
			result.g = (byte)((uint)(c >> 8) & 0xFFu);
			result.b = (byte)((uint)c & 0xFFu);
			return result;
		}

		public static void Save(Texture2D texture, string path, Format format)
		{
			byte[] array = null;
			if (format == Format.PNG)
			{
				array = texture.EncodeToPNG();
			}
			else if (format == Format.PNG)
			{
				array = texture.EncodeToJPG();
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				for (int i = 0; i < array.Length; i++)
				{
					fileStream.WriteByte(array[i]);
				}
			}
		}

		public static void Save(Color[] texture, int width, int height, string path, Format format)
		{
			byte[] array = null;
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(texture);
			if (format == Format.PNG)
			{
				array = texture2D.EncodeToPNG();
			}
			else if (format == Format.PNG)
			{
				array = texture2D.EncodeToJPG();
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				for (int i = 0; i < array.Length; i++)
				{
					fileStream.WriteByte(array[i]);
				}
			}
		}

		public static void Save(int[] texture, int width, int height, string path, Format format)
		{
			byte[] array = null;
			Texture2D texture2D = new Texture2D(width, height);
			Color32[] array2 = new Color32[texture.Length];
			for (int i = 0; i < texture.Length; i++)
			{
				array2[i] = ToColor32(texture[i]);
			}
			texture2D.SetPixels32(array2);
			if (format == Format.PNG)
			{
				array = texture2D.EncodeToPNG();
			}
			else if (format == Format.PNG)
			{
				array = texture2D.EncodeToJPG();
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				for (int i = 0; i < array.Length; i++)
				{
					fileStream.WriteByte(array[i]);
				}
			}
		}

		public static Texture2D Load(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				byte[] array = new byte[fileStream.Length];
				int num = (int)fileStream.Length;
				int num2 = 0;
				while (num > 0)
				{
					int num3 = fileStream.Read(array, num2, num);
					if (num3 == 0)
					{
						break;
					}
					num2 += num3;
					num -= num3;
				}
				Texture2D texture2D = new Texture2D(1, 1);
				texture2D.LoadImage(array);
				return texture2D;
			}
		}

		public static Color[] LoadColors(string path, out int width, out int height)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				byte[] array = new byte[fileStream.Length];
				int num = (int)fileStream.Length;
				int num2 = 0;
				while (num > 0)
				{
					int num3 = fileStream.Read(array, num2, num);
					if (num3 == 0)
					{
						break;
					}
					num2 += num3;
					num -= num3;
				}
				Texture2D texture2D = new Texture2D(1, 1);
				texture2D.LoadImage(array);
				width = texture2D.width;
				height = texture2D.height;
				return texture2D.GetPixels();
			}
		}

		public static void SetColor(Color[] colors, int x, int y, int width, Color color)
		{
			colors[width * y + x] = color;
		}

		public static Color GetColor(Color[] colors, int x, int y, int width)
		{
			return colors[width * y + x];
		}

		public static Texture2D MakeTexture(Color color)
		{
			return MakeTexture(color.r, color.g, color.b, color.a);
		}

		public static Texture2D MakeTexture(Color color, int width, int height)
		{
			return MakeTexture(color.r, color.g, color.b, color.a, width, height);
		}

		public static Texture2D MakeTexture(float r, float g, float b, float a)
		{
			return MakeTexture(r, g, b, a, 1, 1);
		}

		public static Texture2D MakeTexture(float r, float g, float b, float a, int width, int height)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
			Color color = new Color(r, g, b, a);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D MakeTexture(int width, int height)
		{
			return new Texture2D(width, height, TextureFormat.ARGB32, false);
		}

		public static Texture2D ScaleNineSliced(Texture2D sourceTexture, Rect targetRectangle, RectOffset sizingMargins, PixelMode mode)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Texture2D texture2D = MakeTexture((int)targetRectangle.width, (int)targetRectangle.height);
			float x = 0f;
			float x2 = sourceTexture.width - sizingMargins.right;
			float x3 = sizingMargins.left;
			float y = 0f;
			float y2 = sourceTexture.height - sizingMargins.bottom;
			float y3 = sizingMargins.top;
			float height = sizingMargins.top;
			float height2 = sizingMargins.bottom;
			float height3 = sourceTexture.height - sizingMargins.vertical;
			float width = sizingMargins.left;
			float width2 = sizingMargins.right;
			float width3 = sourceTexture.width - sizingMargins.horizontal;
			Rect from = new Rect(x, y, width, height);
			Rect from2 = new Rect(x3, y, width3, height);
			Rect from3 = new Rect(x2, y, width2, height);
			Rect from4 = new Rect(x, y2, width, height2);
			Rect from5 = new Rect(x3, y2, width3, height2);
			Rect from6 = new Rect(x2, y2, width2, height2);
			Rect from7 = new Rect(x, y3, width, height3);
			Rect from8 = new Rect(x3, y3, width3, height3);
			Rect from9 = new Rect(x2, y3, width2, height3);
			x = targetRectangle.x;
			x2 = targetRectangle.xMax - (float)sizingMargins.right;
			x3 = targetRectangle.x + (float)sizingMargins.left;
			y = targetRectangle.y;
			y2 = targetRectangle.yMax - (float)sizingMargins.bottom;
			y3 = targetRectangle.y + (float)sizingMargins.top;
			height = sizingMargins.top;
			height2 = sizingMargins.bottom;
			height3 = targetRectangle.height - (float)sizingMargins.vertical;
			width = sizingMargins.left;
			width2 = sizingMargins.right;
			width3 = targetRectangle.width - (float)sizingMargins.horizontal;
			Rect to = new Rect(x, y, width, height);
			Rect to2 = new Rect(x3, y, width3, height);
			Rect to3 = new Rect(x2, y, width2, height);
			Rect to4 = new Rect(x, y2, width, height2);
			Rect to5 = new Rect(x3, y2, width3, height2);
			Rect to6 = new Rect(x2, y2, width2, height2);
			Rect to7 = new Rect(x, y3, width, height3);
			Rect to8 = new Rect(x3, y3, width3, height3);
			Rect to9 = new Rect(x2, y3, width2, height3);
			DrawToTexture(sourceTexture, texture2D, from, to, mode);
			DrawToTexture(sourceTexture, texture2D, from2, to2, mode);
			DrawToTexture(sourceTexture, texture2D, from3, to3, mode);
			DrawToTexture(sourceTexture, texture2D, from4, to4, mode);
			DrawToTexture(sourceTexture, texture2D, from5, to5, mode);
			DrawToTexture(sourceTexture, texture2D, from6, to6, mode);
			DrawToTexture(sourceTexture, texture2D, from7, to7, mode);
			DrawToTexture(sourceTexture, texture2D, from8, to8, mode);
			DrawToTexture(sourceTexture, texture2D, from9, to9, mode);
			stopwatch.Stop();
			return texture2D;
		}

		public static void DrawToTexture(Texture2D source, Texture2D target, Rect from, Rect to, PixelMode mode = PixelMode.NearestNeighbour)
		{
			bool flag = from.width < to.width;
			bool flag2 = from.height < to.height;
			if (to.height == 0f || to.width == 0f || from.height == 0f || from.width == 0f)
			{
				return;
			}
			Color[] array = new Color[(int)to.width * (int)to.height];
			for (int i = 0; i < (int)to.width; i++)
			{
				int num = (int)from.x + i;
				int num2 = (int)to.x + i;
				for (int j = 0; j < (int)to.height; j++)
				{
					int num3 = (int)from.y + j;
					int num4 = (int)to.y + j;
					int num5 = num;
					int num6 = num3;
					Color color = source.GetPixel(num5, num6);
					if (flag || flag2)
					{
						switch (mode)
						{
						case PixelMode.NearestNeighbour:
						{
							float u = (float)num2 / to.width;
							float v = (float)num4 / to.height;
							num5 = (flag ? ((int)Mathf.Clamp(u * from.width, from.x, from.width - 1f)) : num5);
							num6 = (flag2 ? ((int)Mathf.Clamp(v * from.height, from.y, from.height - 1f)) : num6);
							color = source.GetPixel(num5, num6);
							break;
						}
						case PixelMode.Repeat:
							num5 = (flag ? ((int)Mathf.Clamp(num, from.x, from.width - 1f)) : num5);
							num6 = (flag2 ? ((int)Mathf.Clamp(num3, from.y, from.height - 1f)) : num6);
							color = source.GetPixel(num5, num6);
							break;
						case PixelMode.RepeatComplete:
							color = source.GetPixelBilinear((float)num / (float)source.width, (float)num3 / (float)source.height);
							break;
						case PixelMode.Bilinear:
						{
							float u = (float)num2 / (float)target.width;
							float v = (float)num4 / (float)target.height;
							color = source.GetPixelBilinear(u, v);
							break;
						}
						}
					}
					int num7 = i + j * (int)to.width;
					array[num7] = color;
				}
			}
			target.SetPixels((int)to.x, (int)to.y, (int)to.width, (int)to.height, array);
			target.Apply();
		}

		public static Texture2D MakeTexture(float r, float g, float b, float a, int width, int height, bool linear = false)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false, linear);
			Color color = new Color(r, g, b, a);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D MakeTexture(Color color, int width, int height, bool linear = false)
		{
			return MakeTexture(color.r, color.g, color.b, color.a, width, height, linear);
		}

		public static Texture2D MakeTexture(int width, int height, bool linear = false)
		{
			return new Texture2D(width, height, TextureFormat.ARGB32, false, linear);
		}

		public static Color[] MakeColors(int width, int height)
		{
			return new Color[width * height];
		}
	}
}
