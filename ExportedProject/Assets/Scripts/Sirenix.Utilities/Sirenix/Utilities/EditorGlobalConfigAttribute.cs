using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sirenix.Utilities
{
	public abstract class EditorGlobalConfigAttribute : GlobalConfigAttribute
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<MethodInfo, bool> _003C_003E9__4_0;

			internal bool _003COpenWindow_003Eb__4_0(MethodInfo x)
			{
				if (x.Name == "OpenGlobalConfigWindow")
				{
					return x.GetParameters().Length == 2;
				}
				return false;
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec__5<T> where T : EditorGlobalConfigAttribute
		{
			public static readonly _003C_003Ec__5<T> _003C_003E9 = new _003C_003Ec__5<T>();

			public static Func<MethodInfo, bool> _003C_003E9__5_0;

			internal bool _003COpenWindow_003Eb__5_0(MethodInfo x)
			{
				if (x.Name == "OpenGlobalConfigWindow")
				{
					return x.GetParameters().Length == 2;
				}
				return false;
			}
		}

		public readonly string MenuItemPath;

		protected abstract string EditorWindowTitle { get; }

		protected EditorGlobalConfigAttribute(string path, string menuItem)
			: base(path.Replace("\\", "/").TrimEnd('/') + "/")
		{
			MenuItemPath = menuItem;
		}

		public void OpenWindow(UnityEngine.Object config)
		{
			if (!config.GetType().InheritsFrom(typeof(GlobalConfig<>)))
			{
				throw new ArgumentException("Config parameter must inherit from GlobalConfig<>");
			}
			Type type = AssemblyUtilities.GetType("Sirenix.OdinInspector.Editor.SirenixPreferencesWindow");
			if (type != null)
			{
				type.GetMethods().Where(_003C_003Ec._003C_003E9__4_0 ?? (_003C_003Ec._003C_003E9__4_0 = _003C_003Ec._003C_003E9._003COpenWindow_003Eb__4_0)).First()
					.MakeGenericMethod(GetType())
					.Invoke(null, new object[2] { EditorWindowTitle, config });
			}
			else
			{
				Debug.LogError("Failed to open window, could not find Sirenix.OdinInspector.Editor.GlobalConfigWindow");
			}
		}

		public static void OpenWindow<T>(string title, UnityEngine.Object config = null) where T : EditorGlobalConfigAttribute
		{
			Type type = AssemblyUtilities.GetType("Sirenix.OdinInspector.Editor.SirenixPreferencesWindow");
			if (type != null)
			{
				type.GetMethods().Where(_003C_003Ec__5<T>._003C_003E9__5_0 ?? (_003C_003Ec__5<T>._003C_003E9__5_0 = _003C_003Ec__5<T>._003C_003E9._003COpenWindow_003Eb__5_0)).First()
					.MakeGenericMethod(typeof(T))
					.Invoke(null, new object[2] { title, null });
			}
			else
			{
				Debug.LogError("Failed to open window, could not find Sirenix.OdinInspector.Editor.GlobalConfigWindow");
			}
		}
	}
}
