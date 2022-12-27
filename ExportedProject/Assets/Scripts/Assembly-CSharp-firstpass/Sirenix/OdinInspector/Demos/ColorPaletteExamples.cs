using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ColorPaletteExamples : MonoBehaviour
	{
		[Serializable]
		public class ColorPalette
		{
			[HideInInspector]
			public string Name;

			[LabelText("$Name")]
			[ListDrawerSettings(IsReadOnly = true, Expanded = false)]
			public Color[] Colors;
		}

		[ColorPalette]
		public Color ColorOptions;

		[ColorPalette("Underwater")]
		public Color UnderwaterColor;

		[ColorPalette("My Palette")]
		public Color MyColor;

		public string DynamicPaletteName = "Clovers";

		[ColorPalette("$DynamicPaletteName")]
		public Color DynamicPaletteColor;

		[ColorPalette("Fall")]
		[HideLabel]
		public Color WideColorPalette;

		[ColorPalette("Clovers")]
		public Color[] ColorArray;

		[FoldoutGroup("Color Palettes", false, 0)]
		[ListDrawerSettings(IsReadOnly = true)]
		[PropertyOrder(9)]
		public List<ColorPalette> ColorPalettes;
	}
}
