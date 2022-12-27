using System;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Sirenix.Serialization
{
	public static class UnitySerializationInitializer
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Type, IFormatter> _003C_003E9__2_0;

			internal IFormatter _003CInitialize_003Eb__2_0(Type type)
			{
				if (type != typeof(UnityEvent) && type.ImplementsOrInherits(typeof(UnityEventBase)) && (type.ImplementsOrInherits(typeof(UnityEvent)) || type.ImplementsOpenGenericClass(typeof(UnityEvent<>)) || type.ImplementsOpenGenericClass(typeof(UnityEvent<, >)) || type.ImplementsOpenGenericClass(typeof(UnityEvent<, , >)) || type.ImplementsOpenGenericClass(typeof(UnityEvent<, , , >))))
				{
					return (IFormatter)Activator.CreateInstance(typeof(UnityEventFormatter<>).MakeGenericType(type));
				}
				return null;
			}
		}

		private static readonly object LOCK = new object();

		private static bool initialized = false;

		private static void Initialize()
		{
			if (initialized)
			{
				return;
			}
			lock (LOCK)
			{
				if (!initialized)
				{
					GlobalConfig<GlobalSerializationConfig>.LoadInstanceIfAssetExists();
					FormatterLocator.FormatterResolve += _003C_003Ec._003C_003E9__2_0 ?? (_003C_003Ec._003C_003E9__2_0 = _003C_003Ec._003C_003E9._003CInitialize_003Eb__2_0);
					initialized = true;
				}
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeRuntime()
		{
			Initialize();
		}

		private static void InitializeEditor()
		{
			Initialize();
		}
	}
}
