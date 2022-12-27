using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public static class FormatterEmitter
	{
		[EmittedFormatter]
		public abstract class AOTEmittedFormatter<T> : EasyBaseFormatter<T>
		{
		}

		public abstract class EmptyAOTEmittedFormatter<T> : AOTEmittedFormatter<T>
		{
			protected override void ReadDataEntry(ref T value, string entryName, EntryType entryType, IDataReader reader)
			{
			}

			protected override void WriteDataEntries(ref T value, IDataWriter writer)
			{
			}
		}

		internal delegate void ReadDataEntryMethodDelegate<T>(ref T value, string entryName, EntryType entryType, IDataReader reader);

		internal delegate void WriteDataEntriesMethodDelegate<T>(ref T value, IDataWriter writer);

		[EmittedFormatter]
		internal sealed class RuntimeEmittedFormatter<T> : EasyBaseFormatter<T>
		{
			private readonly ReadDataEntryMethodDelegate<T> read;

			private readonly WriteDataEntriesMethodDelegate<T> write;

			public RuntimeEmittedFormatter(ReadDataEntryMethodDelegate<T> read, WriteDataEntriesMethodDelegate<T> write)
			{
				this.read = read;
				this.write = write;
			}

			protected override void ReadDataEntry(ref T value, string entryName, EntryType entryType, IDataReader reader)
			{
				read(ref value, entryName, entryType, reader);
			}

			protected override void WriteDataEntries(ref T value, IDataWriter writer)
			{
				write(ref value, writer);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass13_0
		{
			public MethodBuilder readMethod;

			internal void _003CEmitAOTFormatter_003Eb__1(ParameterInfo n)
			{
				readMethod.DefineParameter(n.Position, n.Attributes, n.Name);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass13_1
		{
			public MethodBuilder dynamicWriteMethod;

			internal void _003CEmitAOTFormatter_003Eb__3(ParameterInfo n)
			{
				dynamicWriteMethod.DefineParameter(n.Position + 1, n.Attributes, n.Name);
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<ParameterInfo, Type> _003C_003E9__13_0;

			public static Func<ParameterInfo, Type> _003C_003E9__13_2;

			public static Func<ParameterInfo, Type> _003C_003E9__14_0;

			public static Func<ParameterInfo, Type> _003C_003E9__14_2;

			public static Func<MemberInfo, Type> _003C_003E9__15_0;

			internal Type _003CEmitAOTFormatter_003Eb__13_0(ParameterInfo n)
			{
				return n.ParameterType;
			}

			internal Type _003CEmitAOTFormatter_003Eb__13_2(ParameterInfo n)
			{
				return n.ParameterType;
			}

			internal Type _003CCreateGenericFormatter_003Eb__14_0(ParameterInfo n)
			{
				return n.ParameterType;
			}

			internal Type _003CCreateGenericFormatter_003Eb__14_2(ParameterInfo n)
			{
				return n.ParameterType;
			}

			internal Type _003CBuildHelperType_003Eb__15_0(MemberInfo n)
			{
				return FormatterUtilities.GetContainedType(n);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass14_0
		{
			public DynamicMethod dynamicReadMethod;

			internal void _003CCreateGenericFormatter_003Eb__1(ParameterInfo n)
			{
				dynamicReadMethod.DefineParameter(n.Position, n.Attributes, n.Name);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass14_1
		{
			public DynamicMethod dynamicWriteMethod;

			internal void _003CCreateGenericFormatter_003Eb__3(ParameterInfo n)
			{
				dynamicWriteMethod.DefineParameter(n.Position + 1, n.Attributes, n.Name);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass15_0
		{
			public string name;

			internal bool _003CBuildHelperType_003Eb__1(FieldBuilder n)
			{
				return n.Name == name;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass16_0
		{
			public ILGenerator gen;

			internal Label _003CEmitReadMethodContents_003Eb__0(KeyValuePair<MemberInfo, List<string>> n)
			{
				return gen.DefineLabel();
			}
		}

		public const string PRE_EMITTED_ASSEMBLY_NAME = "Sirenix.Serialization.AOTGenerated";

		public const string RUNTIME_EMITTED_ASSEMBLY_NAME = "Sirenix.Serialization.RuntimeEmitted";

		private static readonly object LOCK = new object();

		private static readonly DoubleLookupDictionary<ISerializationPolicy, Type, IFormatter> Formatters = new DoubleLookupDictionary<ISerializationPolicy, Type, IFormatter>();

		private static AssemblyBuilder runtimeEmittedAssembly;

		private static ModuleBuilder runtimeEmittedModule;

		public static IFormatter GetEmittedFormatter(Type type, ISerializationPolicy policy)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (policy == null)
			{
				policy = SerializationPolicies.Strict;
			}
			IFormatter value = null;
			if (!Formatters.TryGetInnerValue(policy, type, out value))
			{
				lock (LOCK)
				{
					if (Formatters.TryGetInnerValue(policy, type, out value))
					{
						return value;
					}
					EnsureRuntimeAssembly();
					try
					{
						value = CreateGenericFormatter(type, runtimeEmittedModule, policy);
					}
					catch (Exception exception)
					{
						Debug.LogError("The following error occurred while emitting a formatter for the type " + type.Name);
						Debug.LogException(exception);
					}
					Formatters.AddInner(policy, type, value);
					return value;
				}
			}
			return value;
		}

		private static void EnsureRuntimeAssembly()
		{
			if (runtimeEmittedAssembly == null)
			{
				AssemblyName assemblyName = new AssemblyName("Sirenix.Serialization.RuntimeEmitted");
				assemblyName.CultureInfo = CultureInfo.InvariantCulture;
				assemblyName.Flags = AssemblyNameFlags.None;
				assemblyName.ProcessorArchitecture = ProcessorArchitecture.MSIL;
				assemblyName.VersionCompatibility = AssemblyVersionCompatibility.SameDomain;
				runtimeEmittedAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			}
			if (runtimeEmittedModule == null)
			{
				bool emitSymbolInfo = false;
				runtimeEmittedModule = runtimeEmittedAssembly.DefineDynamicModule("Sirenix.Serialization.RuntimeEmitted", emitSymbolInfo);
			}
		}

		public static Type EmitAOTFormatter(Type formattedType, ModuleBuilder moduleBuilder, ISerializationPolicy policy)
		{
			Dictionary<string, MemberInfo> serializableMembersMap = FormatterUtilities.GetSerializableMembersMap(formattedType, policy);
			string name = moduleBuilder.Name + "." + formattedType.GetCompilableNiceFullName() + "__AOTFormatter";
			string helperTypeName = moduleBuilder.Name + "." + formattedType.GetCompilableNiceFullName() + "__FormatterHelper";
			if (serializableMembersMap.Count == 0)
			{
				return moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed, typeof(EmptyAOTEmittedFormatter<>).MakeGenericType(formattedType)).CreateType();
			}
			Dictionary<Type, MethodInfo> serializerReadMethods;
			Dictionary<Type, MethodInfo> serializerWriteMethods;
			Dictionary<Type, FieldBuilder> serializerFields;
			FieldBuilder dictField;
			Dictionary<MemberInfo, List<string>> memberNames;
			BuildHelperType(moduleBuilder, helperTypeName, formattedType, serializableMembersMap, out serializerReadMethods, out serializerWriteMethods, out serializerFields, out dictField, out memberNames);
			TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed, typeof(AOTEmittedFormatter<>).MakeGenericType(formattedType));
			typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(CustomFormatterAttribute).GetConstructor(Type.EmptyTypes), new object[0]));
			_003C_003Ec__DisplayClass13_0 _003C_003Ec__DisplayClass13_ = new _003C_003Ec__DisplayClass13_0();
			MethodInfo method = typeBuilder.BaseType.GetMethod("ReadDataEntry", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			_003C_003Ec__DisplayClass13_.readMethod = typeBuilder.DefineMethod(method.Name, MethodAttributes.Family | MethodAttributes.Virtual, method.ReturnType, method.GetParameters().Select(_003C_003Ec._003C_003E9__13_0 ?? (_003C_003Ec._003C_003E9__13_0 = _003C_003Ec._003C_003E9._003CEmitAOTFormatter_003Eb__13_0)).ToArray());
			method.GetParameters().ForEach(_003C_003Ec__DisplayClass13_._003CEmitAOTFormatter_003Eb__1);
			EmitReadMethodContents(_003C_003Ec__DisplayClass13_.readMethod.GetILGenerator(), formattedType, dictField, serializerFields, memberNames, serializerReadMethods);
			_003C_003Ec__DisplayClass13_1 _003C_003Ec__DisplayClass13_2 = new _003C_003Ec__DisplayClass13_1();
			MethodInfo method2 = typeBuilder.BaseType.GetMethod("WriteDataEntries", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			_003C_003Ec__DisplayClass13_2.dynamicWriteMethod = typeBuilder.DefineMethod(method2.Name, MethodAttributes.Family | MethodAttributes.Virtual, method2.ReturnType, method2.GetParameters().Select(_003C_003Ec._003C_003E9__13_2 ?? (_003C_003Ec._003C_003E9__13_2 = _003C_003Ec._003C_003E9._003CEmitAOTFormatter_003Eb__13_2)).ToArray());
			method2.GetParameters().ForEach(_003C_003Ec__DisplayClass13_2._003CEmitAOTFormatter_003Eb__3);
			EmitWriteMethodContents(_003C_003Ec__DisplayClass13_2.dynamicWriteMethod.GetILGenerator(), formattedType, serializerFields, memberNames, serializerWriteMethods);
			return typeBuilder.CreateType();
		}

		private static IFormatter CreateGenericFormatter(Type formattedType, ModuleBuilder moduleBuilder, ISerializationPolicy policy)
		{
			Dictionary<string, MemberInfo> serializableMembersMap = FormatterUtilities.GetSerializableMembersMap(formattedType, policy);
			if (serializableMembersMap.Count == 0)
			{
				return (IFormatter)Activator.CreateInstance(typeof(EmptyTypeFormatter<>).MakeGenericType(formattedType));
			}
			string helperTypeName = moduleBuilder.Name + "." + formattedType.GetCompilableNiceFullName() + "__FormatterHelper";
			Dictionary<Type, MethodInfo> serializerReadMethods;
			Dictionary<Type, MethodInfo> serializerWriteMethods;
			Dictionary<Type, FieldBuilder> serializerFields;
			FieldBuilder dictField;
			Dictionary<MemberInfo, List<string>> memberNames;
			BuildHelperType(moduleBuilder, helperTypeName, formattedType, serializableMembersMap, out serializerReadMethods, out serializerWriteMethods, out serializerFields, out dictField, out memberNames);
			Type type = typeof(RuntimeEmittedFormatter<>).MakeGenericType(formattedType);
			_003C_003Ec__DisplayClass14_0 _003C_003Ec__DisplayClass14_ = new _003C_003Ec__DisplayClass14_0();
			Type delegateType = typeof(ReadDataEntryMethodDelegate<>).MakeGenericType(formattedType);
			MethodInfo method = type.GetMethod("ReadDataEntry", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			_003C_003Ec__DisplayClass14_.dynamicReadMethod = new DynamicMethod("Dynamic_" + formattedType.GetCompilableNiceFullName(), null, method.GetParameters().Select(_003C_003Ec._003C_003E9__14_0 ?? (_003C_003Ec._003C_003E9__14_0 = _003C_003Ec._003C_003E9._003CCreateGenericFormatter_003Eb__14_0)).ToArray(), true);
			method.GetParameters().ForEach(_003C_003Ec__DisplayClass14_._003CCreateGenericFormatter_003Eb__1);
			EmitReadMethodContents(_003C_003Ec__DisplayClass14_.dynamicReadMethod.GetILGenerator(), formattedType, dictField, serializerFields, memberNames, serializerReadMethods);
			Delegate @delegate = _003C_003Ec__DisplayClass14_.dynamicReadMethod.CreateDelegate(delegateType);
			_003C_003Ec__DisplayClass14_1 _003C_003Ec__DisplayClass14_2 = new _003C_003Ec__DisplayClass14_1();
			Type delegateType2 = typeof(WriteDataEntriesMethodDelegate<>).MakeGenericType(formattedType);
			MethodInfo method2 = type.GetMethod("WriteDataEntries", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			_003C_003Ec__DisplayClass14_2.dynamicWriteMethod = new DynamicMethod("Dynamic_Write_" + formattedType.GetCompilableNiceFullName(), null, method2.GetParameters().Select(_003C_003Ec._003C_003E9__14_2 ?? (_003C_003Ec._003C_003E9__14_2 = _003C_003Ec._003C_003E9._003CCreateGenericFormatter_003Eb__14_2)).ToArray(), true);
			method2.GetParameters().ForEach(_003C_003Ec__DisplayClass14_2._003CCreateGenericFormatter_003Eb__3);
			EmitWriteMethodContents(_003C_003Ec__DisplayClass14_2.dynamicWriteMethod.GetILGenerator(), formattedType, serializerFields, memberNames, serializerWriteMethods);
			Delegate delegate2 = _003C_003Ec__DisplayClass14_2.dynamicWriteMethod.CreateDelegate(delegateType2);
			return (IFormatter)Activator.CreateInstance(type, @delegate, delegate2);
		}

		private static Type BuildHelperType(ModuleBuilder moduleBuilder, string helperTypeName, Type formattedType, Dictionary<string, MemberInfo> serializableMembers, out Dictionary<Type, MethodInfo> serializerReadMethods, out Dictionary<Type, MethodInfo> serializerWriteMethods, out Dictionary<Type, FieldBuilder> serializerFields, out FieldBuilder dictField, out Dictionary<MemberInfo, List<string>> memberNames)
		{
			TypeBuilder typeBuilder = moduleBuilder.DefineType(helperTypeName, TypeAttributes.Public | TypeAttributes.Sealed);
			memberNames = new Dictionary<MemberInfo, List<string>>();
			foreach (KeyValuePair<string, MemberInfo> serializableMember in serializableMembers)
			{
				List<string> value;
				if (!memberNames.TryGetValue(serializableMember.Value, out value))
				{
					value = new List<string>();
					memberNames.Add(serializableMember.Value, value);
				}
				value.Add(serializableMember.Key);
			}
			dictField = typeBuilder.DefineField("SwitchLookup", typeof(Dictionary<string, int>), FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly);
			List<Type> list = memberNames.Keys.Select(_003C_003Ec._003C_003E9__15_0 ?? (_003C_003Ec._003C_003E9__15_0 = _003C_003Ec._003C_003E9._003CBuildHelperType_003Eb__15_0)).Distinct().ToList();
			serializerReadMethods = new Dictionary<Type, MethodInfo>(list.Count);
			serializerWriteMethods = new Dictionary<Type, MethodInfo>(list.Count);
			serializerFields = new Dictionary<Type, FieldBuilder>(list.Count);
			foreach (Type item in list)
			{
				_003C_003Ec__DisplayClass15_0 _003C_003Ec__DisplayClass15_ = new _003C_003Ec__DisplayClass15_0();
				_003C_003Ec__DisplayClass15_.name = item.GetCompilableNiceFullName() + "__Serializer";
				int num = 1;
				while (serializerFields.Values.Any(_003C_003Ec__DisplayClass15_._003CBuildHelperType_003Eb__1))
				{
					num++;
					_003C_003Ec__DisplayClass15_.name = item.GetCompilableNiceFullName() + "__Serializer" + num;
				}
				Type type = typeof(Serializer<>).MakeGenericType(item);
				serializerReadMethods.Add(item, type.GetMethod("ReadValue", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public));
				serializerWriteMethods.Add(item, type.GetMethod("WriteValue", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, null, new Type[3]
				{
					typeof(string),
					item,
					typeof(IDataWriter)
				}, null));
				serializerFields.Add(item, typeBuilder.DefineField(_003C_003Ec__DisplayClass15_.name, type, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly));
			}
			MethodInfo method = typeof(Dictionary<string, int>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
			ConstructorInfo constructor = typeof(Dictionary<string, int>).GetConstructor(Type.EmptyTypes);
			MethodInfo method2 = typeof(Serializer).GetMethod("Get", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { typeof(Type) }, null);
			MethodInfo method3 = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { typeof(RuntimeTypeHandle) }, null);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineTypeInitializer();
			ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Newobj, constructor);
			int num2 = 0;
			foreach (KeyValuePair<MemberInfo, List<string>> memberName in memberNames)
			{
				foreach (string item2 in memberName.Value)
				{
					iLGenerator.Emit(OpCodes.Dup);
					iLGenerator.Emit(OpCodes.Ldstr, item2);
					iLGenerator.Emit(OpCodes.Ldc_I4, num2);
					iLGenerator.Emit(OpCodes.Call, method);
				}
				num2++;
			}
			iLGenerator.Emit(OpCodes.Stsfld, dictField);
			foreach (KeyValuePair<Type, FieldBuilder> serializerField in serializerFields)
			{
				iLGenerator.Emit(OpCodes.Ldtoken, serializerField.Key);
				iLGenerator.Emit(OpCodes.Call, method3);
				iLGenerator.Emit(OpCodes.Call, method2);
				iLGenerator.Emit(OpCodes.Stsfld, serializerField.Value);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return typeBuilder.CreateType();
		}

		private static void EmitReadMethodContents(ILGenerator gen, Type formattedType, FieldInfo dictField, Dictionary<Type, FieldBuilder> serializerFields, Dictionary<MemberInfo, List<string>> memberNames, Dictionary<Type, MethodInfo> serializerReadMethods)
		{
			_003C_003Ec__DisplayClass16_0 _003C_003Ec__DisplayClass16_ = new _003C_003Ec__DisplayClass16_0();
			_003C_003Ec__DisplayClass16_.gen = gen;
			MethodInfo method = typeof(IDataReader).GetMethod("SkipEntry", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method2 = typeof(Dictionary<string, int>).GetMethod("TryGetValue", BindingFlags.Instance | BindingFlags.Public);
			LocalBuilder localBuilder = _003C_003Ec__DisplayClass16_.gen.DeclareLocal(typeof(int));
			Label label = _003C_003Ec__DisplayClass16_.gen.DefineLabel();
			Label label2 = _003C_003Ec__DisplayClass16_.gen.DefineLabel();
			Label label3 = _003C_003Ec__DisplayClass16_.gen.DefineLabel();
			Label[] array = memberNames.Select(_003C_003Ec__DisplayClass16_._003CEmitReadMethodContents_003Eb__0).ToArray();
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldarg_1);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldnull);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ceq);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Brtrue, label);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldsfld, dictField);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldarg_1);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldloca, (short)localBuilder.LocalIndex);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Callvirt, method2);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Brtrue, label2);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Br, label);
			_003C_003Ec__DisplayClass16_.gen.MarkLabel(label2);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldloc, localBuilder);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Switch, array);
			int num = 0;
			foreach (MemberInfo key in memberNames.Keys)
			{
				Type containedType = FormatterUtilities.GetContainedType(key);
				PropertyInfo propertyInfo = key as PropertyInfo;
				FieldInfo fieldInfo = key as FieldInfo;
				_003C_003Ec__DisplayClass16_.gen.MarkLabel(array[num]);
				_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldarg_0);
				if (!formattedType.IsValueType)
				{
					_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldind_Ref);
				}
				_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldsfld, serializerFields[containedType]);
				_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldarg, (short)3);
				_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Callvirt, serializerReadMethods[containedType]);
				if (fieldInfo != null)
				{
					_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Stfld, fieldInfo.DeAliasField());
				}
				else
				{
					if (propertyInfo == null)
					{
						throw new NotImplementedException();
					}
					_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Callvirt, propertyInfo.DeAliasProperty().GetSetMethod(true));
				}
				_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Br, label3);
				num++;
			}
			_003C_003Ec__DisplayClass16_.gen.MarkLabel(label);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ldarg, (short)3);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Callvirt, method);
			_003C_003Ec__DisplayClass16_.gen.MarkLabel(label3);
			_003C_003Ec__DisplayClass16_.gen.Emit(OpCodes.Ret);
		}

		private static void EmitWriteMethodContents(ILGenerator gen, Type formattedType, Dictionary<Type, FieldBuilder> serializerFields, Dictionary<MemberInfo, List<string>> memberNames, Dictionary<Type, MethodInfo> serializerWriteMethods)
		{
			foreach (MemberInfo key in memberNames.Keys)
			{
				Type containedType = FormatterUtilities.GetContainedType(key);
				gen.Emit(OpCodes.Ldsfld, serializerFields[containedType]);
				gen.Emit(OpCodes.Ldstr, key.Name);
				if (key is FieldInfo)
				{
					FieldInfo fieldInfo = key as FieldInfo;
					if (formattedType.IsValueType)
					{
						gen.Emit(OpCodes.Ldarg_0);
						gen.Emit(OpCodes.Ldfld, fieldInfo.DeAliasField());
					}
					else
					{
						gen.Emit(OpCodes.Ldarg_0);
						gen.Emit(OpCodes.Ldind_Ref);
						gen.Emit(OpCodes.Ldfld, fieldInfo.DeAliasField());
					}
				}
				else
				{
					if (!(key is PropertyInfo))
					{
						throw new NotImplementedException();
					}
					PropertyInfo propertyInfo = key as PropertyInfo;
					if (formattedType.IsValueType)
					{
						gen.Emit(OpCodes.Ldarg_0);
						gen.Emit(OpCodes.Call, propertyInfo.DeAliasProperty().GetGetMethod(true));
					}
					else
					{
						gen.Emit(OpCodes.Ldarg_0);
						gen.Emit(OpCodes.Ldind_Ref);
						gen.Emit(OpCodes.Callvirt, propertyInfo.DeAliasProperty().GetGetMethod(true));
					}
				}
				gen.Emit(OpCodes.Ldarg_1);
				gen.Emit(OpCodes.Callvirt, serializerWriteMethods[containedType]);
			}
			gen.Emit(OpCodes.Ret);
		}
	}
}
