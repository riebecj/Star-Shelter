using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;

namespace Sirenix.Serialization
{
	public class DefaultSerializationBinder : TwoWaySerializationBinder
	{
		private static readonly Dictionary<string, Assembly> assemblyNameLookUp;

		private static readonly object TYPEMAP_LOCK;

		private static readonly object NAMEMAP_LOCK;

		private static readonly Dictionary<string, Type> typeMap;

		private static readonly Dictionary<Type, string> nameMap;

		static DefaultSerializationBinder()
		{
			assemblyNameLookUp = new Dictionary<string, Assembly>();
			TYPEMAP_LOCK = new object();
			NAMEMAP_LOCK = new object();
			typeMap = new Dictionary<string, Type>();
			nameMap = new Dictionary<Type, string>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				string name = assembly.GetName().Name;
				if (!assemblyNameLookUp.ContainsKey(name))
				{
					assemblyNameLookUp.Add(name, assembly);
				}
			}
		}

		public override string BindToName(Type type, DebugContext debugContext = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			lock (NAMEMAP_LOCK)
			{
				string value;
				if (!nameMap.TryGetValue(type, out value))
				{
					if (!type.IsGenericType)
					{
						value = ((!type.IsDefined(typeof(CompilerGeneratedAttribute), false)) ? (type.FullName + ", " + type.Assembly.GetName().Name) : (type.FullName + ", " + type.Assembly.GetName().Name));
					}
					else
					{
						List<Type> list = type.GetGenericArguments().ToList();
						HashSet<Assembly> hashSet = new HashSet<Assembly>();
						while (list.Count > 0)
						{
							Type type2 = list[0];
							if (type2.IsGenericType)
							{
								list.AddRange(type2.GetGenericArguments());
							}
							hashSet.Add(type2.Assembly);
							list.RemoveAt(0);
						}
						value = type.FullName + ", " + type.Assembly.GetName().Name;
						foreach (Assembly item in hashSet)
						{
							value = value.Replace(item.FullName, item.GetName().Name);
						}
					}
					nameMap.Add(type, value);
					return value;
				}
				return value;
			}
		}

		public override bool ContainsType(string typeName)
		{
			lock (TYPEMAP_LOCK)
			{
				return typeMap.ContainsKey(typeName);
			}
		}

		public override Type BindToType(string typeName, DebugContext debugContext = null)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			lock (TYPEMAP_LOCK)
			{
				Type value;
				if (!typeMap.TryGetValue(typeName, out value))
				{
					if (value == null)
					{
						value = Type.GetType(typeName);
					}
					if (value == null)
					{
						value = AssemblyUtilities.GetType(typeName);
					}
					string typeName2;
					string assemblyName;
					ParseName(typeName, out typeName2, out assemblyName);
					if (value == null && assemblyName != null && assemblyNameLookUp.ContainsKey(assemblyName))
					{
						Assembly assembly = assemblyNameLookUp[assemblyName];
						value = assembly.GetType(typeName2);
					}
					if (value == null)
					{
						value = AssemblyUtilities.GetType(typeName2);
					}
					if (value == null && debugContext != null)
					{
						debugContext.LogWarning("Failed deserialization type lookup for type name '" + typeName + "'.");
					}
					typeMap.Add(typeName, value);
					return value;
				}
				return value;
			}
		}

		private static void ParseName(string fullName, out string typeName, out string assemblyName)
		{
			typeName = null;
			assemblyName = null;
			int num = fullName.IndexOf(',');
			if (num < 0 || num + 1 == fullName.Length)
			{
				typeName = fullName.Trim(',', ' ');
				return;
			}
			typeName = fullName.Substring(0, num);
			int num2 = fullName.IndexOf(',', num + 1);
			if (num2 < 0)
			{
				assemblyName = fullName.Substring(num).Trim(',', ' ');
			}
			else
			{
				assemblyName = fullName.Substring(num, num2 - num).Trim(',', ' ');
			}
		}
	}
}
