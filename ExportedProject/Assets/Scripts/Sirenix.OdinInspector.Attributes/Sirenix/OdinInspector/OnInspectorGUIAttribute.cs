using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	public sealed class OnInspectorGUIAttribute : ShowInInspectorAttribute
	{
		public string PrependMethodName { get; private set; }

		public string AppendMethodName { get; private set; }

		public OnInspectorGUIAttribute()
		{
		}

		public OnInspectorGUIAttribute(string methodName, bool append = true)
		{
			if (append)
			{
				AppendMethodName = methodName;
			}
			else
			{
				PrependMethodName = methodName;
			}
		}

		public OnInspectorGUIAttribute(string prependMethodName, string appendMethodName)
		{
			PrependMethodName = prependMethodName;
			AppendMethodName = appendMethodName;
		}
	}
}
