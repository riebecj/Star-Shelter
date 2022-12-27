using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sirenix.Utilities
{
	public static class AssemblyUtilities
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			internal void _003C_002Ecctor_003Eb__26_0(object sender, AssemblyLoadEventArgs args)
			{
				if (!args.LoadedAssembly.IsDynamic())
				{
					RegisterAssembly(args.LoadedAssembly);
				}
			}
		}

		private static string[] userAssemblyPrefixes;

		private static string[] pluginAssemblyPrefixes;

		private static Assembly unityEngineAssembly;

		private static DirectoryInfo projectFolderDirectory;

		private static DirectoryInfo scriptAssembliesDirectory;

		private static Dictionary<string, Type> stringTypeLookup;

		private static Dictionary<Assembly, AssemblyTypeFlags> assemblyTypeFlagLookup;

		private static ImmutableList<Assembly> allAssemblies;

		private static List<Assembly> userAssemblies;

		private static List<Assembly> userEditorAssemblies;

		private static List<Assembly> pluginAssemblies;

		private static List<Assembly> pluginEditorAssemblies;

		private static List<Assembly> unityAssemblies;

		private static List<Assembly> unityEditorAssemblies;

		private static List<Assembly> otherAssemblies;

		private static List<Type> userTypes;

		private static List<Type> userEditorTypes;

		private static List<Type> pluginTypes;

		private static List<Type> pluginEditorTypes;

		private static List<Type> unityTypes;

		private static List<Type> unityEditorTypes;

		private static List<Type> otherTypes;

		private static string dataPath;

		private static string scriptAssembliesPath;

		public static void Reload()
		{
			dataPath = Environment.CurrentDirectory.Replace("\\", "//").Replace("//", "/").TrimEnd('/') + "/Assets";
			scriptAssembliesPath = Environment.CurrentDirectory.Replace("\\", "//").Replace("//", "/").TrimEnd('/') + "/Library/ScriptAssemblies";
			userAssemblies = new List<Assembly>();
			userEditorAssemblies = new List<Assembly>();
			pluginAssemblies = new List<Assembly>();
			pluginEditorAssemblies = new List<Assembly>();
			unityAssemblies = new List<Assembly>();
			unityEditorAssemblies = new List<Assembly>();
			otherAssemblies = new List<Assembly>();
			userTypes = new List<Type>();
			userEditorTypes = new List<Type>();
			pluginTypes = new List<Type>();
			pluginEditorTypes = new List<Type>();
			unityTypes = new List<Type>();
			unityEditorTypes = new List<Type>();
			otherTypes = new List<Type>();
			stringTypeLookup = new Dictionary<string, Type>();
			assemblyTypeFlagLookup = new Dictionary<Assembly, AssemblyTypeFlags>();
			unityEngineAssembly = typeof(Vector3).Assembly;
			projectFolderDirectory = new DirectoryInfo(dataPath);
			scriptAssembliesDirectory = new DirectoryInfo(scriptAssembliesPath);
			allAssemblies = new ImmutableList<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			for (int i = 0; i < allAssemblies.Count; i++)
			{
				RegisterAssembly(allAssemblies[i]);
			}
		}

		private static void RegisterAssembly(Assembly assembly)
		{
			try
			{
				AssemblyTypeFlags assemblyTypeFlag = assembly.GetAssemblyTypeFlag();
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type.Namespace != null)
					{
						stringTypeLookup[type.Namespace + "." + type.Name] = type;
					}
					else
					{
						stringTypeLookup[type.Name] = type;
					}
				}
				switch (assemblyTypeFlag)
				{
				case AssemblyTypeFlags.UserTypes:
					userAssemblies.Add(assembly);
					userTypes.AddRange(assembly.GetTypes());
					break;
				case AssemblyTypeFlags.UserEditorTypes:
					userEditorAssemblies.Add(assembly);
					userEditorTypes.AddRange(assembly.GetTypes());
					break;
				case AssemblyTypeFlags.PluginTypes:
					pluginAssemblies.Add(assembly);
					pluginTypes.AddRange(assembly.GetTypes());
					break;
				case AssemblyTypeFlags.PluginEditorTypes:
					pluginEditorAssemblies.Add(assembly);
					pluginEditorTypes.AddRange(assembly.GetTypes());
					break;
				case AssemblyTypeFlags.UnityTypes:
					unityAssemblies.Add(assembly);
					unityTypes.AddRange(assembly.GetTypes());
					break;
				case AssemblyTypeFlags.UnityEditorTypes:
					unityEditorAssemblies.Add(assembly);
					unityEditorTypes.AddRange(assembly.GetTypes());
					break;
				case AssemblyTypeFlags.OtherTypes:
					otherAssemblies.Add(assembly);
					otherTypes.AddRange(assembly.GetTypes());
					break;
				default:
					throw new NotImplementedException();
				}
			}
			catch (ReflectionTypeLoadException)
			{
			}
		}

		static AssemblyUtilities()
		{
			userAssemblyPrefixes = new string[6] { "Assembly-CSharp", "Assembly-UnityScript", "Assembly-Boo", "Assembly-CSharp-Editor", "Assembly-UnityScript-Editor", "Assembly-Boo-Editor" };
			pluginAssemblyPrefixes = new string[6] { "Assembly-CSharp-firstpass", "Assembly-CSharp-Editor-firstpass", "Assembly-UnityScript-firstpass", "Assembly-UnityScript-Editor-firstpass", "Assembly-Boo-firstpass", "Assembly-Boo-Editor-firstpass" };
			Reload();
			AppDomain.CurrentDomain.AssemblyLoad += _003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__26_0;
		}

		public static ImmutableList<Assembly> GetAllAssemblies()
		{
			return allAssemblies;
		}

		public static AssemblyTypeFlags GetAssemblyTypeFlag(this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new NullReferenceException("assembly");
			}
			AssemblyTypeFlags value;
			if (!assemblyTypeFlagLookup.TryGetValue(assembly, out value))
			{
				value = GetAssemblyTypeFlagNoLookup(assembly);
				assemblyTypeFlagLookup[assembly] = value;
			}
			return value;
		}

		private static AssemblyTypeFlags GetAssemblyTypeFlagNoLookup(Assembly assembly)
		{
			string assemblyDirectory = assembly.GetAssemblyDirectory();
			string str = assembly.FullName.ToLower(CultureInfo.InvariantCulture);
			bool flag = false;
			bool flag2 = false;
			if (assemblyDirectory != null && Directory.Exists(assemblyDirectory))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(assemblyDirectory);
				flag = directoryInfo.FullName == scriptAssembliesDirectory.FullName;
				flag2 = projectFolderDirectory.HasSubDirectory(directoryInfo);
			}
			bool flag3 = str.StartsWithAnyOf(userAssemblyPrefixes, StringComparison.InvariantCultureIgnoreCase);
			bool flag4 = str.StartsWithAnyOf(pluginAssemblyPrefixes, StringComparison.InvariantCultureIgnoreCase);
			bool flag5 = assembly.IsDependentOn(unityEngineAssembly);
			bool flag6 = flag4 || flag2 || (!flag3 && flag);
			bool flag7 = !flag6 && flag3;
			bool flag8 = false;
			if (!flag5 && !flag8 && !flag6 && !flag7)
			{
				return AssemblyTypeFlags.OtherTypes;
			}
			if (flag8 && !flag6 && !flag7)
			{
				return AssemblyTypeFlags.UnityEditorTypes;
			}
			if (flag5 && !flag8 && !flag6 && !flag7)
			{
				return AssemblyTypeFlags.UnityTypes;
			}
			if (flag8 && flag6 && !flag7)
			{
				return AssemblyTypeFlags.PluginEditorTypes;
			}
			if (!flag8 && flag6 && !flag7)
			{
				return AssemblyTypeFlags.PluginTypes;
			}
			if (flag8 && flag7)
			{
				return AssemblyTypeFlags.UserEditorTypes;
			}
			if (!flag8 && flag7)
			{
				return AssemblyTypeFlags.UserTypes;
			}
			return AssemblyTypeFlags.OtherTypes;
		}

		public static Type GetType(string fullName)
		{
			Type value;
			if (stringTypeLookup.TryGetValue(fullName, out value))
			{
				return value;
			}
			return null;
		}

		public static bool IsDependentOn(this Assembly assembly, Assembly otherAssembly)
		{
			if (assembly == null)
			{
				throw new NullReferenceException("assembly");
			}
			if (otherAssembly == null)
			{
				throw new NullReferenceException("otherAssembly");
			}
			if (assembly == otherAssembly)
			{
				return true;
			}
			string text = otherAssembly.GetName().ToString();
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (text == referencedAssemblies[i].ToString())
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsDynamic(this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			return assembly.ManifestModule is ModuleBuilder;
		}

		public static string GetAssemblyDirectory(this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			string assemblyFilePath = assembly.GetAssemblyFilePath();
			if (assemblyFilePath == null)
			{
				return null;
			}
			return Path.GetDirectoryName(assemblyFilePath);
		}

		public static string GetAssemblyFilePath(this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly.IsDynamic())
			{
				return null;
			}
			if (string.IsNullOrEmpty(assembly.CodeBase))
			{
				return null;
			}
			UriBuilder uriBuilder = new UriBuilder(assembly.CodeBase);
			return Uri.UnescapeDataString(uriBuilder.Path);
		}

		public static IEnumerable<Type> GetTypes(AssemblyTypeFlags assemblyTypeFlags)
		{
			bool flag = (assemblyTypeFlags & AssemblyTypeFlags.UserTypes) == AssemblyTypeFlags.UserTypes;
			bool includeUserEditorTypes = (assemblyTypeFlags & AssemblyTypeFlags.UserEditorTypes) == AssemblyTypeFlags.UserEditorTypes;
			bool includePluginTypes = (assemblyTypeFlags & AssemblyTypeFlags.PluginTypes) == AssemblyTypeFlags.PluginTypes;
			bool includePluginEditorTypes = (assemblyTypeFlags & AssemblyTypeFlags.PluginEditorTypes) == AssemblyTypeFlags.PluginEditorTypes;
			bool includeUnityTypes = (assemblyTypeFlags & AssemblyTypeFlags.UnityTypes) == AssemblyTypeFlags.UnityTypes;
			bool includeUnityEditorTypes = (assemblyTypeFlags & AssemblyTypeFlags.UnityEditorTypes) == AssemblyTypeFlags.UnityEditorTypes;
			bool includeOtherTypes = (assemblyTypeFlags & AssemblyTypeFlags.OtherTypes) == AssemblyTypeFlags.OtherTypes;
			if (flag)
			{
				for (int i2 = 0; i2 < userTypes.Count; i2++)
				{
					yield return userTypes[i2];
				}
			}
			if (includeUserEditorTypes)
			{
				for (int n = 0; n < userEditorTypes.Count; n++)
				{
					yield return userEditorTypes[n];
				}
			}
			if (includePluginTypes)
			{
				for (int m = 0; m < pluginTypes.Count; m++)
				{
					yield return pluginTypes[m];
				}
			}
			if (includePluginEditorTypes)
			{
				for (int l = 0; l < pluginEditorTypes.Count; l++)
				{
					yield return pluginEditorTypes[l];
				}
			}
			if (includeUnityTypes)
			{
				for (int k = 0; k < unityTypes.Count; k++)
				{
					yield return unityTypes[k];
				}
			}
			if (includeUnityEditorTypes)
			{
				for (int j = 0; j < unityEditorTypes.Count; j++)
				{
					yield return unityEditorTypes[j];
				}
			}
			if (includeOtherTypes)
			{
				for (int i = 0; i < otherTypes.Count; i++)
				{
					yield return otherTypes[i];
				}
			}
		}

		private static bool StartsWithAnyOf(this string str, IEnumerable<string> values, StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			IList<string> list = values as IList<string>;
			if (list != null)
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (str.StartsWith(list[i], comparisonType))
					{
						return true;
					}
				}
			}
			else
			{
				foreach (string value in values)
				{
					if (str.StartsWith(value, comparisonType))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
