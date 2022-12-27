using System;
using Sirenix.Utilities;
using UnityEngine.Events;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class UnityEventFormatter : ReflectionFormatter<UnityEvent>
	{
		protected override UnityEvent GetUninitializedObject()
		{
			return new UnityEvent();
		}
	}
	[CustomFormatter]
	public class UnityEventFormatter<T> : ReflectionFormatter<T> where T : class, new()
	{
		static UnityEventFormatter()
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(UnityEvent) || !typeFromHandle.ImplementsOrInherits(typeof(UnityEventBase)) || (!typeFromHandle.ImplementsOrInherits(typeof(UnityEvent)) && !typeFromHandle.ImplementsOpenGenericClass(typeof(UnityEvent<>)) && !typeFromHandle.ImplementsOpenGenericClass(typeof(UnityEvent<, >)) && !typeFromHandle.ImplementsOpenGenericClass(typeof(UnityEvent<, , >)) && !typeFromHandle.ImplementsOpenGenericClass(typeof(UnityEvent<, , , >))))
			{
				throw new ArgumentException("Cannot create a UnityEventFormatter for type " + typeof(T).Name);
			}
		}

		protected override T GetUninitializedObject()
		{
			return new T();
		}
	}
}
