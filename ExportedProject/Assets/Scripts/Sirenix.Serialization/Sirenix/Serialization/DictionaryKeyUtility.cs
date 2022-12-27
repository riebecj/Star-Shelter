using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public static class DictionaryKeyUtility
	{
		private class UnityObjectKeyComparer<T> : IComparer<T>
		{
			public int Compare(T x, T y)
			{
				return ((UnityEngine.Object)(object)x).name.CompareTo(((UnityEngine.Object)(object)y).name);
			}
		}

		private class FallbackKeyComparer<T> : IComparer<T>
		{
			public int Compare(T x, T y)
			{
				return GetDictionaryKeyString(x).CompareTo(GetDictionaryKeyString(y));
			}
		}

		public class KeyComparer<T> : IComparer<T>
		{
			public static readonly KeyComparer<T> Default = new KeyComparer<T>();

			private readonly IComparer<T> actualComparer;

			public KeyComparer()
			{
				IDictionaryKeyPathProvider value;
				if (TypeToKeyPathProviders.TryGetValue(typeof(T), out value))
				{
					actualComparer = (IComparer<T>)value;
				}
				else if (typeof(IComparable).IsAssignableFrom(typeof(T)) || typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
				{
					actualComparer = Comparer<T>.Default;
				}
				else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
				{
					actualComparer = new UnityObjectKeyComparer<T>();
				}
				else
				{
					actualComparer = new FallbackKeyComparer<T>();
				}
			}

			public int Compare(T x, T y)
			{
				return actualComparer.Compare(x, y);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass12_0
		{
			public Assembly ass;

			internal _003C_003Ef__AnonymousType1<Assembly, RegisterDictionaryKeyPathProviderAttribute> _003C_002Ecctor_003Eb__1(object attr)
			{
				return new _003C_003Ef__AnonymousType1<Assembly, RegisterDictionaryKeyPathProviderAttribute>(ass, (RegisterDictionaryKeyPathProviderAttribute)attr);
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			internal IEnumerable<_003C_003Ef__AnonymousType1<Assembly, RegisterDictionaryKeyPathProviderAttribute>> _003C_002Ecctor_003Eb__12_0(Assembly ass)
			{
				_003C_003Ec__DisplayClass12_0 _003C_003Ec__DisplayClass12_ = new _003C_003Ec__DisplayClass12_0
				{
					ass = ass
				};
				return _003C_003Ec__DisplayClass12_.ass.GetCustomAttributes(typeof(RegisterDictionaryKeyPathProviderAttribute), false).Select(_003C_003Ec__DisplayClass12_._003C_002Ecctor_003Eb__1);
			}

			internal bool _003C_002Ecctor_003Eb__12_2(_003C_003Ef__AnonymousType1<Assembly, RegisterDictionaryKeyPathProviderAttribute> n)
			{
				return n.Attribute.ProviderType != null;
			}
		}

		private static readonly Dictionary<Type, bool> GetSupportedDictionaryKeyTypesResults;

		private static readonly HashSet<Type> BaseSupportedDictionaryKeyTypes;

		private static readonly HashSet<char> AllowedSpecialKeyStrChars;

		private static readonly Dictionary<Type, IDictionaryKeyPathProvider> TypeToKeyPathProviders;

		private static readonly Dictionary<string, IDictionaryKeyPathProvider> IDToKeyPathProviders;

		private static readonly Dictionary<IDictionaryKeyPathProvider, string> ProviderToID;

		private static readonly Dictionary<object, string> ObjectsToTempKeys;

		private static readonly Dictionary<string, object> TempKeysToObjects;

		private static long tempKeyCounter;

		static DictionaryKeyUtility()
		{
			GetSupportedDictionaryKeyTypesResults = new Dictionary<Type, bool>();
			BaseSupportedDictionaryKeyTypes = new HashSet<Type>
			{
				typeof(string),
				typeof(char),
				typeof(byte),
				typeof(sbyte),
				typeof(ushort),
				typeof(short),
				typeof(uint),
				typeof(int),
				typeof(ulong),
				typeof(long),
				typeof(float),
				typeof(double),
				typeof(decimal),
				typeof(Guid)
			};
			AllowedSpecialKeyStrChars = new HashSet<char> { ',', '(', ')', '\\', '|', '-' };
			TypeToKeyPathProviders = new Dictionary<Type, IDictionaryKeyPathProvider>();
			IDToKeyPathProviders = new Dictionary<string, IDictionaryKeyPathProvider>();
			ProviderToID = new Dictionary<IDictionaryKeyPathProvider, string>();
			ObjectsToTempKeys = new Dictionary<object, string>();
			TempKeysToObjects = new Dictionary<string, object>();
			tempKeyCounter = 0L;
			IEnumerable<_003C_003Ef__AnonymousType1<Assembly, RegisterDictionaryKeyPathProviderAttribute>> enumerable = AppDomain.CurrentDomain.GetAssemblies().SelectMany(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__12_0).Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__12_2);
			foreach (_003C_003Ef__AnonymousType1<Assembly, RegisterDictionaryKeyPathProviderAttribute> item in enumerable)
			{
				Assembly assembly = item.Assembly;
				Type providerType = item.Attribute.ProviderType;
				if (providerType.IsAbstract)
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Type cannot be abstract");
					continue;
				}
				if (providerType.IsInterface)
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Type cannot be an interface");
					continue;
				}
				if (!providerType.ImplementsOpenGenericInterface(typeof(IDictionaryKeyPathProvider<>)))
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Type must implement the " + typeof(IDictionaryKeyPathProvider<>).GetNiceName() + " interface");
					continue;
				}
				if (providerType.IsGenericType)
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Type cannot be generic");
					continue;
				}
				if (providerType.GetConstructor(Type.EmptyTypes) == null)
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Type must have a public parameterless constructor");
					continue;
				}
				Type type = providerType.GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionaryKeyPathProvider<>))[0];
				if (!type.IsValueType)
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Key type to support '" + type.GetNiceFullName() + "' must be a value type - support for extending dictionaries with reference type keys may come at a later time");
					continue;
				}
				if (TypeToKeyPathProviders.ContainsKey(type))
				{
					Debug.LogWarning("Ignoring dictionary key path provider '" + providerType.GetNiceFullName() + "' registered on assembly '" + assembly.GetName().Name + "': A previous provider '" + TypeToKeyPathProviders[type].GetType().GetNiceFullName() + "' was already registered for the key type '" + type.GetNiceFullName() + "'.");
					continue;
				}
				IDictionaryKeyPathProvider dictionaryKeyPathProvider;
				try
				{
					dictionaryKeyPathProvider = (IDictionaryKeyPathProvider)Activator.CreateInstance(providerType);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					Debug.LogWarning(string.Concat("Ignoring dictionary key path provider '", providerType.GetNiceFullName(), "' registered on assembly '", assembly.GetName().Name, "': An exception of type '", ex.GetType(), "' was thrown when trying to instantiate a provider instance."));
					continue;
				}
				string providerID;
				try
				{
					providerID = dictionaryKeyPathProvider.ProviderID;
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					Debug.LogWarning(string.Concat("Ignoring dictionary key path provider '", providerType.GetNiceFullName(), "' registered on assembly '", assembly.GetName().Name, "': An exception of type '", ex2.GetType(), "' was thrown when trying to get the provider ID string."));
					continue;
				}
				if (providerID == null)
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Provider ID is null");
					continue;
				}
				if (providerID.Length == 0)
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Provider ID is an empty string");
					continue;
				}
				for (int i = 0; i < providerID.Length; i++)
				{
					if (!char.IsLetterOrDigit(providerID[i]))
					{
						LogInvalidKeyPathProvider(providerType, assembly, "Provider ID '" + providerID + "' cannot contain characters which are not letters or digits");
					}
				}
				if (IDToKeyPathProviders.ContainsKey(providerID))
				{
					LogInvalidKeyPathProvider(providerType, assembly, "Provider ID '" + providerID + "' is already in use for the provider '" + IDToKeyPathProviders[providerID].GetType().GetNiceFullName() + "'");
				}
				else
				{
					TypeToKeyPathProviders[type] = dictionaryKeyPathProvider;
					IDToKeyPathProviders[providerID] = dictionaryKeyPathProvider;
					ProviderToID[dictionaryKeyPathProvider] = providerID;
				}
			}
		}

		private static void LogInvalidKeyPathProvider(Type type, Assembly assembly, string reason)
		{
			Debug.LogError("Invalid dictionary key path provider '" + type.GetNiceFullName() + "' registered on assembly '" + assembly.GetName().Name + "': " + reason);
		}

		public static IEnumerable<Type> GetPersistentPathKeyTypes()
		{
			foreach (Type baseSupportedDictionaryKeyType in BaseSupportedDictionaryKeyTypes)
			{
				yield return baseSupportedDictionaryKeyType;
			}
			foreach (Type key in TypeToKeyPathProviders.Keys)
			{
				yield return key;
			}
		}

		public static bool KeyTypeSupportsPersistentPaths(Type type)
		{
			bool value;
			if (!GetSupportedDictionaryKeyTypesResults.TryGetValue(type, out value))
			{
				value = PrivateIsSupportedDictionaryKeyType(type);
				GetSupportedDictionaryKeyTypesResults.Add(type, value);
			}
			return value;
		}

		private static bool PrivateIsSupportedDictionaryKeyType(Type type)
		{
			if (!type.IsEnum && !BaseSupportedDictionaryKeyTypes.Contains(type))
			{
				return TypeToKeyPathProviders.ContainsKey(type);
			}
			return true;
		}

		public static string GetDictionaryKeyString(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Type type = key.GetType();
			if (!KeyTypeSupportsPersistentPaths(type))
			{
				string value;
				if (!ObjectsToTempKeys.TryGetValue(key, out value))
				{
					value = tempKeyCounter++.ToString();
					ObjectsToTempKeys[key] = "{temp:" + value + "}";
				}
				return value;
			}
			IDictionaryKeyPathProvider value2;
			if (TypeToKeyPathProviders.TryGetValue(type, out value2))
			{
				string pathStringFromKey = value2.GetPathStringFromKey(key);
				string text = null;
				bool flag = true;
				if (pathStringFromKey == null || pathStringFromKey.Length == 0)
				{
					flag = false;
					text = "String is null or empty";
				}
				if (flag)
				{
					for (int i = 0; i < pathStringFromKey.Length; i++)
					{
						char c = pathStringFromKey[i];
						if (!char.IsLetterOrDigit(c) && !AllowedSpecialKeyStrChars.Contains(c))
						{
							flag = false;
							text = "Invalid character '" + c.ToString() + "' at index " + i;
							break;
						}
					}
				}
				if (!flag)
				{
					throw new ArgumentException("Invalid key path '" + pathStringFromKey + "' given by provider '" + value2.GetType().GetNiceFullName() + "': " + text);
				}
				return "{id:" + ProviderToID[value2] + ":" + pathStringFromKey + "}";
			}
			if (type.IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(type);
				if (underlyingType == typeof(ulong))
				{
					return "{" + Convert.ToUInt64(key).ToString("D", CultureInfo.InvariantCulture) + "eu}";
				}
				return "{" + Convert.ToInt64(key).ToString("D", CultureInfo.InvariantCulture) + "es}";
			}
			if (type == typeof(string))
			{
				return string.Concat("{\"", key, "\"}");
			}
			if (type == typeof(char))
			{
				return "{'" + ((char)key).ToString(CultureInfo.InvariantCulture) + "'}";
			}
			if (type == typeof(byte))
			{
				return "{" + ((byte)key).ToString("D", CultureInfo.InvariantCulture) + "ub}";
			}
			if (type == typeof(sbyte))
			{
				return "{" + ((sbyte)key).ToString("D", CultureInfo.InvariantCulture) + "sb}";
			}
			if (type == typeof(ushort))
			{
				return "{" + ((ushort)key).ToString("D", CultureInfo.InvariantCulture) + "us}";
			}
			if (type == typeof(short))
			{
				return "{" + ((short)key).ToString("D", CultureInfo.InvariantCulture) + "ss}";
			}
			if (type == typeof(uint))
			{
				return "{" + ((uint)key).ToString("D", CultureInfo.InvariantCulture) + "ui}";
			}
			if (type == typeof(int))
			{
				return "{" + ((int)key).ToString("D", CultureInfo.InvariantCulture) + "si}";
			}
			if (type == typeof(ulong))
			{
				return "{" + ((ulong)key).ToString("D", CultureInfo.InvariantCulture) + "ul}";
			}
			if (type == typeof(long))
			{
				return "{" + ((long)key).ToString("D", CultureInfo.InvariantCulture) + "sl}";
			}
			if (type == typeof(float))
			{
				return "{" + ((float)key).ToString("R", CultureInfo.InvariantCulture) + "fl}";
			}
			if (type == typeof(double))
			{
				return "{" + ((double)key).ToString("R", CultureInfo.InvariantCulture) + "dl}";
			}
			if (type == typeof(decimal))
			{
				return "{" + ((decimal)key).ToString("G", CultureInfo.InvariantCulture) + "dc}";
			}
			if (type == typeof(Guid))
			{
				return "{" + ((Guid)key).ToString("N", CultureInfo.InvariantCulture) + "gu}";
			}
			throw new NotImplementedException("Support has not been implemented for the supported dictionary key type '" + type.GetNiceName() + "'.");
		}

		public static object GetDictionaryKeyValue(string keyStr, Type expectedType)
		{
			if (keyStr == null)
			{
				throw new ArgumentNullException("keyStr");
			}
			if (keyStr.Length < 4 || keyStr[0] != '{' || keyStr[keyStr.Length - 1] != '}')
			{
				throw new ArgumentException("Invalid key string: " + keyStr);
			}
			if (keyStr[1] == '"')
			{
				if (keyStr[keyStr.Length - 2] != '"')
				{
					throw new ArgumentException("Invalid key string: " + keyStr);
				}
				return keyStr.Substring(2, keyStr.Length - 4);
			}
			if (keyStr[1] == '\'')
			{
				if (keyStr.Length != 5 || keyStr[keyStr.Length - 2] != '\'')
				{
					throw new ArgumentException("Invalid key string: " + keyStr);
				}
				return keyStr[2];
			}
			if (keyStr.StartsWith("{temp:"))
			{
				object value;
				if (!TempKeysToObjects.TryGetValue(keyStr, out value))
				{
					throw new ArgumentException("The temp dictionary key '" + keyStr + "' has not been allocated yet.");
				}
				return value;
			}
			if (keyStr.StartsWith("{id:"))
			{
				int num = keyStr.IndexOf(':', 4);
				if (num == -1 || num > keyStr.Length - 3)
				{
					throw new ArgumentException("Invalid key string: " + keyStr);
				}
				string text = keyStr.FromTo(4, num);
				string pathStr = keyStr.FromTo(num + 1, keyStr.Length - 1);
				IDictionaryKeyPathProvider value2;
				if (!IDToKeyPathProviders.TryGetValue(text, out value2))
				{
					throw new ArgumentException("No provider found for provider ID '" + text + "' in key string '" + keyStr + "'.");
				}
				return value2.GetKeyFromPathString(pathStr);
			}
			if (keyStr.EndsWith("ub}"))
			{
				return byte.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("sb}"))
			{
				return sbyte.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("us}"))
			{
				return ushort.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("ss}"))
			{
				return short.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("ui}"))
			{
				return uint.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("si}"))
			{
				return int.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("ul}"))
			{
				return ulong.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("sl}"))
			{
				return long.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("fl}"))
			{
				return float.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("dl}"))
			{
				return double.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("dc}"))
			{
				return decimal.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
			}
			if (keyStr.EndsWith("gu}"))
			{
				return new Guid(keyStr.Substring(1, keyStr.Length - 4));
			}
			if (keyStr.EndsWith("es}"))
			{
				return Enum.ToObject(expectedType, long.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any));
			}
			if (keyStr.EndsWith("eu}"))
			{
				return Enum.ToObject(expectedType, ulong.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any));
			}
			throw new ArgumentException("Invalid key string: " + keyStr);
		}

		private static string FromTo(this string str, int from, int to)
		{
			return str.Substring(from, to - from);
		}
	}
}
