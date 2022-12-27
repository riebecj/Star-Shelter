using System;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class GUIColorAttribute : Attribute
	{
		public Color Color { get; private set; }

		public GUIColorAttribute(float r, float g, float b, float a = 1f)
		{
			Color = new Color(r, g, b, a);
		}
	}
}
