namespace ch.sycoforge.Types
{
	public struct IntColor
	{
		public int argb;

		public IntColor(int argb)
		{
			this.argb = argb;
		}

		public static implicit operator IntColor(ByteColor c)
		{
			IntColor result = default(IntColor);
			int num = c.a << 24;
			int num2 = c.r << 16;
			int num3 = c.g << 8;
			int b = c.b;
			result.argb = num | num2 | num3 | b;
			return result;
		}

		public static implicit operator IntColor(int c)
		{
			IntColor result = default(IntColor);
			result.argb = c;
			return result;
		}

		public static implicit operator int(IntColor c)
		{
			return c.argb;
		}

		public static implicit operator ByteColor(IntColor c)
		{
			ByteColor result = default(ByteColor);
			result.a = (byte)((uint)(c.argb >> 24) & 0xFFu);
			result.r = (byte)((uint)(c.argb >> 16) & 0xFFu);
			result.g = (byte)((uint)(c.argb >> 8) & 0xFFu);
			result.b = (byte)((uint)c.argb & 0xFFu);
			return result;
		}
	}
}
