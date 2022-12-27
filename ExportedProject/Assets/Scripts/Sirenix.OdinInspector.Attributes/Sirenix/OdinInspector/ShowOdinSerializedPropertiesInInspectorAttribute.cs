using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ShowOdinSerializedPropertiesInInspectorAttribute : Attribute
	{
	}
}
