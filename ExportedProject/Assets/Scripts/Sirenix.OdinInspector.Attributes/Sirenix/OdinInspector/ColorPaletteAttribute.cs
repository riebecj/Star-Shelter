using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class ColorPaletteAttribute : Attribute
	{
		public string PaletteName { get; private set; }

		public bool ShowAlpha { get; set; }

		public ColorPaletteAttribute()
		{
			PaletteName = null;
			ShowAlpha = true;
		}

		public ColorPaletteAttribute(string paletteName)
		{
			PaletteName = paletteName;
			ShowAlpha = true;
		}
	}
}
