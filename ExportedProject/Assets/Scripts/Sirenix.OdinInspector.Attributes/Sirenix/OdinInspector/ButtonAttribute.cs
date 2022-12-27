using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ButtonAttribute : ShowInInspectorAttribute
	{
		public int ButtonHeight { get; private set; }

		public string Name { get; private set; }

		public ButtonAttribute(ButtonSizes buttonSize = ButtonSizes.Small)
		{
			Name = null;
			ButtonHeight = (int)buttonSize;
		}

		public ButtonAttribute(int buttonSize)
		{
			ButtonHeight = buttonSize;
			Name = null;
		}

		public ButtonAttribute(string name, ButtonSizes buttonSize = ButtonSizes.Small)
		{
			Name = name;
			ButtonHeight = (int)buttonSize;
		}

		public ButtonAttribute(string name, int buttonSize)
		{
			Name = name;
			ButtonHeight = buttonSize;
		}
	}
}
