using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	public struct HSBColor
	{
		public float h;

		public float s;

		public float b;

		public float a;

		[DllImport("sycoforge_imaging", CallingConvention = CallingConvention.Cdecl)]
		private static extern HSBColor RGBToHSB(FloatColor rgb);

		[DllImport("sycoforge_imaging", CallingConvention = CallingConvention.Cdecl)]
		private static extern FloatColor HSBToRGB(HSBColor hsb);

		public static implicit operator FloatColor(HSBColor hsb)
		{
			return HSBToRGB(hsb);
		}

		public static implicit operator HSBColor(FloatColor rgb)
		{
			return RGBToHSB(rgb);
		}
	}
}
