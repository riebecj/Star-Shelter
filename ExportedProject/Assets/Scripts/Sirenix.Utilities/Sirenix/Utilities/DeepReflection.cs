using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Sirenix.Utilities
{
	public static class DeepReflection
	{
		private enum PathStepType
		{
			Member = 0,
			WeakListElement = 1,
			StrongListElement = 2,
			ArrayElement = 3
		}

		private struct PathStep
		{
			public readonly PathStepType StepType;

			public readonly MemberInfo Member;

			public readonly int ElementIndex;

			public readonly Type ElementType;

			public readonly MethodInfo StrongListGetItemMethod;

			public PathStep(MemberInfo member)
			{
				StepType = PathStepType.Member;
				Member = member;
				ElementIndex = -1;
				ElementType = null;
				StrongListGetItemMethod = null;
			}

			public PathStep(int elementIndex)
			{
				StepType = PathStepType.WeakListElement;
				Member = null;
				ElementIndex = elementIndex;
				ElementType = null;
				StrongListGetItemMethod = null;
			}

			public PathStep(int elementIndex, Type strongListElementType, bool isArray)
			{
				StepType = (isArray ? PathStepType.ArrayElement : PathStepType.StrongListElement);
				Member = null;
				ElementIndex = elementIndex;
				ElementType = strongListElementType;
				StrongListGetItemMethod = typeof(IList<>).MakeGenericType(strongListElementType).GetMethod("get_Item");
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass11_0<TResult>
		{
			public Func<object, object> del;

			internal TResult _003CCreateWeakInstanceValueGetter_003Eb__0(object obj)
			{
				return (TResult)del(obj);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass12_0<TResult>
		{
			public Func<object> slowDelegate;

			internal TResult _003CCreateValueGetter_003Eb__0()
			{
				return (TResult)slowDelegate();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass13_0<TTarget, TResult>
		{
			public Func<object, object> slowDelegate;

			internal TResult _003CCreateValueGetter_003Eb__0(TTarget target)
			{
				return (TResult)slowDelegate(target);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass14_0<TTarget, TResult>
		{
			public Func<TTarget, TResult> func;

			internal object _003CCreateWeakAliasForInstanceGetDelegate1_003Eb__0(object obj)
			{
				return func((TTarget)obj);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass15_0<TTarget, TResult>
		{
			public Func<TTarget, TResult> func;

			internal TResult _003CCreateWeakAliasForInstanceGetDelegate2_003Eb__0(object obj)
			{
				return func((TTarget)obj);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass16_0<TResult>
		{
			public Func<TResult> func;

			internal object _003CCreateWeakAliasForStaticGetDelegate_003Eb__0()
			{
				return func();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass17_0<TTarget, TArg1>
		{
			public Action<TTarget, TArg1> func;

			internal void _003CCreateWeakAliasForInstanceSetDelegate1_003Eb__0(object obj, object arg)
			{
				func((TTarget)obj, (TArg1)arg);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass18_0<TTarget, TArg1>
		{
			public Action<TTarget, TArg1> func;

			internal void _003CCreateWeakAliasForInstanceSetDelegate2_003Eb__0(object obj, TArg1 arg)
			{
				func((TTarget)obj, arg);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass19_0<TArg1>
		{
			public Action<TArg1> func;

			internal void _003CCreateWeakAliasForStaticSetDelegate_003Eb__0(object arg)
			{
				func((TArg1)arg);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass21_0
		{
			public List<PathStep> memberPath;

			internal object _003CCreateSlowDeepStaticValueGetterDelegate_003Eb__0()
			{
				object obj = null;
				for (int i = 0; i < memberPath.Count; i++)
				{
					obj = SlowGetMemberValue(memberPath[i], obj);
				}
				return obj;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass22_0
		{
			public List<PathStep> memberPath;

			internal object _003CCreateSlowDeepInstanceValueGetterDelegate_003Eb__0(object instance)
			{
				object obj = instance;
				for (int i = 0; i < memberPath.Count; i++)
				{
					obj = SlowGetMemberValue(memberPath[i], obj);
				}
				return obj;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass23_0
		{
			public List<PathStep> memberPath;

			internal void _003CCreateSlowDeepInstanceValueSetterDelegate_003Eb__0(object instance, object arg)
			{
				object instance2 = instance;
				int num = memberPath.Count - 1;
				for (int i = 0; i < num; i++)
				{
					instance2 = SlowGetMemberValue(memberPath[i], instance2);
				}
				SlowSetMemberValue(memberPath[memberPath.Count - 1], instance2, arg);
			}
		}

		private static MethodInfo WeakListGetItem = typeof(IList).GetMethod("get_Item");

		private static MethodInfo WeakListSetItem = typeof(IList).GetMethod("set_Item");

		private static MethodInfo CreateWeakAliasForInstanceGetDelegate1MethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForInstanceGetDelegate1", BindingFlags.Static | BindingFlags.NonPublic);

		private static MethodInfo CreateWeakAliasForInstanceGetDelegate2MethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForInstanceGetDelegate2", BindingFlags.Static | BindingFlags.NonPublic);

		private static MethodInfo CreateWeakAliasForStaticGetDelegateMethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForStaticGetDelegate", BindingFlags.Static | BindingFlags.NonPublic);

		private static MethodInfo CreateWeakAliasForInstanceSetDelegate1MethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForInstanceSetDelegate1", BindingFlags.Static | BindingFlags.NonPublic);

		public static Func<object> CreateWeakStaticValueGetter(Type rootType, Type resultType, string path, bool allowEmit = true)
		{
			if (rootType == null)
			{
				throw new ArgumentNullException("rootType");
			}
			bool rootIsStatic;
			List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out rootIsStatic, false);
			if (!rootIsStatic)
			{
				throw new ArgumentException("Given path root is not static.");
			}
			if (!allowEmit)
			{
				return CreateSlowDeepStaticValueGetterDelegate(memberPath);
			}
			Delegate @delegate = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
			MethodInfo methodInfo = CreateWeakAliasForStaticGetDelegateMethodInfo.MakeGenericMethod(resultType);
			return (Func<object>)methodInfo.Invoke(null, new object[1] { @delegate });
		}

		public static Func<object, object> CreateWeakInstanceValueGetter(Type rootType, Type resultType, string path, bool allowEmit = true)
		{
			if (rootType == null)
			{
				throw new ArgumentNullException("rootType");
			}
			bool rootIsStatic;
			List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out rootIsStatic, false);
			if (rootIsStatic)
			{
				throw new ArgumentException("Given path root is static.");
			}
			if (!allowEmit)
			{
				return CreateSlowDeepInstanceValueGetterDelegate(memberPath);
			}
			Delegate @delegate = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
			MethodInfo methodInfo = CreateWeakAliasForInstanceGetDelegate1MethodInfo.MakeGenericMethod(rootType, resultType);
			return (Func<object, object>)methodInfo.Invoke(null, new object[1] { @delegate });
		}

		public static Action<object, object> CreateWeakInstanceValueSetter(Type rootType, Type argType, string path, bool allowEmit = true)
		{
			if (rootType == null)
			{
				throw new ArgumentNullException("rootType");
			}
			bool rootIsStatic;
			List<PathStep> memberPath = GetMemberPath(rootType, ref argType, path, out rootIsStatic, true);
			if (rootIsStatic)
			{
				throw new ArgumentException("Given path root is static.");
			}
			allowEmit = false;
			if (!allowEmit)
			{
				return CreateSlowDeepInstanceValueSetterDelegate(memberPath);
			}
			Delegate @delegate = null;
			MethodInfo methodInfo = CreateWeakAliasForInstanceSetDelegate1MethodInfo.MakeGenericMethod(rootType, argType);
			return (Action<object, object>)methodInfo.Invoke(null, new object[1] { @delegate });
		}

		public static Func<object, TResult> CreateWeakInstanceValueGetter<TResult>(Type rootType, string path, bool allowEmit = true)
		{
			if (rootType == null)
			{
				throw new ArgumentNullException("rootType");
			}
			Type resultType = typeof(TResult);
			bool rootIsStatic;
			List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out rootIsStatic, false);
			if (rootIsStatic)
			{
				throw new ArgumentException("Given path root is static.");
			}
			if (!allowEmit)
			{
				_003C_003Ec__DisplayClass11_0<TResult> _003C_003Ec__DisplayClass11_ = new _003C_003Ec__DisplayClass11_0<TResult>();
				_003C_003Ec__DisplayClass11_.del = CreateSlowDeepInstanceValueGetterDelegate(memberPath);
				return _003C_003Ec__DisplayClass11_._003CCreateWeakInstanceValueGetter_003Eb__0;
			}
			Delegate @delegate = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
			MethodInfo methodInfo = CreateWeakAliasForInstanceGetDelegate2MethodInfo.MakeGenericMethod(rootType, resultType);
			return (Func<object, TResult>)methodInfo.Invoke(null, new object[1] { @delegate });
		}

		public static Func<TResult> CreateValueGetter<TResult>(Type rootType, string path, bool allowEmit = true)
		{
			if (rootType == null)
			{
				throw new ArgumentNullException("rootType");
			}
			Type resultType = typeof(TResult);
			bool rootIsStatic;
			List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out rootIsStatic, false);
			if (!rootIsStatic)
			{
				throw new ArgumentException("Given path root is not static; use the generic overload with a target type.");
			}
			if (!allowEmit)
			{
				_003C_003Ec__DisplayClass12_0<TResult> _003C_003Ec__DisplayClass12_ = new _003C_003Ec__DisplayClass12_0<TResult>();
				_003C_003Ec__DisplayClass12_.slowDelegate = CreateSlowDeepStaticValueGetterDelegate(memberPath);
				return _003C_003Ec__DisplayClass12_._003CCreateValueGetter_003Eb__0;
			}
			Delegate @delegate = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
			return (Func<TResult>)@delegate;
		}

		public static Func<TTarget, TResult> CreateValueGetter<TTarget, TResult>(string path, bool allowEmit = true)
		{
			Type resultType = typeof(TResult);
			bool rootIsStatic;
			List<PathStep> memberPath = GetMemberPath(typeof(TTarget), ref resultType, path, out rootIsStatic, false);
			if (rootIsStatic)
			{
				throw new ArgumentException("Given path root is static; use the generic overload without a target type.");
			}
			if (!allowEmit)
			{
				_003C_003Ec__DisplayClass13_0<TTarget, TResult> _003C_003Ec__DisplayClass13_ = new _003C_003Ec__DisplayClass13_0<TTarget, TResult>();
				_003C_003Ec__DisplayClass13_.slowDelegate = CreateSlowDeepInstanceValueGetterDelegate(memberPath);
				return _003C_003Ec__DisplayClass13_._003CCreateValueGetter_003Eb__0;
			}
			Delegate @delegate = CreateEmittedDeepValueGetterDelegate(path, typeof(TTarget), resultType, memberPath, rootIsStatic);
			return (Func<TTarget, TResult>)@delegate;
		}

		private static Func<object, object> CreateWeakAliasForInstanceGetDelegate1<TTarget, TResult>(Func<TTarget, TResult> func)
		{
			_003C_003Ec__DisplayClass14_0<TTarget, TResult> _003C_003Ec__DisplayClass14_ = new _003C_003Ec__DisplayClass14_0<TTarget, TResult>();
			_003C_003Ec__DisplayClass14_.func = func;
			return _003C_003Ec__DisplayClass14_._003CCreateWeakAliasForInstanceGetDelegate1_003Eb__0;
		}

		private static Func<object, TResult> CreateWeakAliasForInstanceGetDelegate2<TTarget, TResult>(Func<TTarget, TResult> func)
		{
			_003C_003Ec__DisplayClass15_0<TTarget, TResult> _003C_003Ec__DisplayClass15_ = new _003C_003Ec__DisplayClass15_0<TTarget, TResult>();
			_003C_003Ec__DisplayClass15_.func = func;
			return _003C_003Ec__DisplayClass15_._003CCreateWeakAliasForInstanceGetDelegate2_003Eb__0;
		}

		private static Func<object> CreateWeakAliasForStaticGetDelegate<TResult>(Func<TResult> func)
		{
			_003C_003Ec__DisplayClass16_0<TResult> _003C_003Ec__DisplayClass16_ = new _003C_003Ec__DisplayClass16_0<TResult>();
			_003C_003Ec__DisplayClass16_.func = func;
			return _003C_003Ec__DisplayClass16_._003CCreateWeakAliasForStaticGetDelegate_003Eb__0;
		}

		private static Action<object, object> CreateWeakAliasForInstanceSetDelegate1<TTarget, TArg1>(Action<TTarget, TArg1> func)
		{
			_003C_003Ec__DisplayClass17_0<TTarget, TArg1> _003C_003Ec__DisplayClass17_ = new _003C_003Ec__DisplayClass17_0<TTarget, TArg1>();
			_003C_003Ec__DisplayClass17_.func = func;
			return _003C_003Ec__DisplayClass17_._003CCreateWeakAliasForInstanceSetDelegate1_003Eb__0;
		}

		private static Action<object, TArg1> CreateWeakAliasForInstanceSetDelegate2<TTarget, TArg1>(Action<TTarget, TArg1> func)
		{
			_003C_003Ec__DisplayClass18_0<TTarget, TArg1> _003C_003Ec__DisplayClass18_ = new _003C_003Ec__DisplayClass18_0<TTarget, TArg1>();
			_003C_003Ec__DisplayClass18_.func = func;
			return _003C_003Ec__DisplayClass18_._003CCreateWeakAliasForInstanceSetDelegate2_003Eb__0;
		}

		private static Action<object> CreateWeakAliasForStaticSetDelegate<TArg1>(Action<TArg1> func)
		{
			_003C_003Ec__DisplayClass19_0<TArg1> _003C_003Ec__DisplayClass19_ = new _003C_003Ec__DisplayClass19_0<TArg1>();
			_003C_003Ec__DisplayClass19_.func = func;
			return _003C_003Ec__DisplayClass19_._003CCreateWeakAliasForStaticSetDelegate_003Eb__0;
		}

		private static Delegate CreateEmittedDeepValueGetterDelegate(string path, Type rootType, Type resultType, List<PathStep> memberPath, bool rootIsStatic)
		{
			DynamicMethod dynamicMethod = ((!rootIsStatic) ? new DynamicMethod(rootType.FullName + "_getter<" + path + ">", resultType, new Type[1] { rootType }, true) : new DynamicMethod(rootType.FullName + "_getter<" + path + ">", resultType, new Type[0], true));
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (!rootIsStatic)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
			}
			for (int i = 0; i < memberPath.Count; i++)
			{
				PathStep pathStep = memberPath[i];
				switch (pathStep.StepType)
				{
				case PathStepType.Member:
				{
					MemberInfo member = pathStep.Member;
					FieldInfo fieldInfo = member as FieldInfo;
					if (fieldInfo != null)
					{
						if (fieldInfo.IsStatic)
						{
							iLGenerator.Emit(OpCodes.Ldsfld, fieldInfo);
						}
						else
						{
							iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
						}
					}
					PropertyInfo propertyInfo = member as PropertyInfo;
					if (propertyInfo != null)
					{
						MethodInfo getMethod = propertyInfo.GetGetMethod(true);
						if (getMethod.IsStatic)
						{
							iLGenerator.Emit(OpCodes.Call, getMethod);
						}
						else if (getMethod.DeclaringType.IsValueType)
						{
							LocalBuilder local = iLGenerator.DeclareLocal(getMethod.DeclaringType);
							iLGenerator.Emit(OpCodes.Stloc, local);
							iLGenerator.Emit(OpCodes.Ldloca, local);
							iLGenerator.Emit(OpCodes.Call, getMethod);
						}
						else
						{
							iLGenerator.Emit(OpCodes.Callvirt, getMethod);
						}
					}
					MethodInfo methodInfo = member as MethodInfo;
					if (methodInfo != null)
					{
						if (methodInfo.IsStatic)
						{
							iLGenerator.Emit(OpCodes.Call, methodInfo);
						}
						else if (methodInfo.DeclaringType.IsValueType)
						{
							LocalBuilder local2 = iLGenerator.DeclareLocal(methodInfo.DeclaringType);
							iLGenerator.Emit(OpCodes.Stloc, local2);
							iLGenerator.Emit(OpCodes.Ldloca, local2);
							iLGenerator.Emit(OpCodes.Call, methodInfo);
						}
						else
						{
							iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
						}
					}
					Type returnType = member.GetReturnType();
					if ((resultType == typeof(object) || returnType.IsInterface) && returnType.IsValueType)
					{
						iLGenerator.Emit(OpCodes.Box, returnType);
					}
					break;
				}
				case PathStepType.ArrayElement:
					iLGenerator.Emit(OpCodes.Ldc_I4, pathStep.ElementIndex);
					iLGenerator.Emit(OpCodes.Ldelem, pathStep.ElementType);
					break;
				case PathStepType.WeakListElement:
					iLGenerator.Emit(OpCodes.Ldc_I4, pathStep.ElementIndex);
					iLGenerator.Emit(OpCodes.Callvirt, WeakListGetItem);
					break;
				case PathStepType.StrongListElement:
				{
					Type type = typeof(IList<>).MakeGenericType(pathStep.ElementType);
					MethodInfo method = type.GetMethod("get_Item");
					iLGenerator.Emit(OpCodes.Ldc_I4, pathStep.ElementIndex);
					iLGenerator.Emit(OpCodes.Callvirt, method);
					break;
				}
				}
			}
			iLGenerator.Emit(OpCodes.Ret);
			if (rootIsStatic)
			{
				return dynamicMethod.CreateDelegate(typeof(Func<>).MakeGenericType(resultType));
			}
			return dynamicMethod.CreateDelegate(typeof(Func<, >).MakeGenericType(rootType, resultType));
		}

		private static Func<object> CreateSlowDeepStaticValueGetterDelegate(List<PathStep> memberPath)
		{
			_003C_003Ec__DisplayClass21_0 _003C_003Ec__DisplayClass21_ = new _003C_003Ec__DisplayClass21_0();
			_003C_003Ec__DisplayClass21_.memberPath = memberPath;
			return _003C_003Ec__DisplayClass21_._003CCreateSlowDeepStaticValueGetterDelegate_003Eb__0;
		}

		private static Func<object, object> CreateSlowDeepInstanceValueGetterDelegate(List<PathStep> memberPath)
		{
			_003C_003Ec__DisplayClass22_0 _003C_003Ec__DisplayClass22_ = new _003C_003Ec__DisplayClass22_0();
			_003C_003Ec__DisplayClass22_.memberPath = memberPath;
			return _003C_003Ec__DisplayClass22_._003CCreateSlowDeepInstanceValueGetterDelegate_003Eb__0;
		}

		private static Action<object, object> CreateSlowDeepInstanceValueSetterDelegate(List<PathStep> memberPath)
		{
			_003C_003Ec__DisplayClass23_0 _003C_003Ec__DisplayClass23_ = new _003C_003Ec__DisplayClass23_0();
			_003C_003Ec__DisplayClass23_.memberPath = memberPath;
			return _003C_003Ec__DisplayClass23_._003CCreateSlowDeepInstanceValueSetterDelegate_003Eb__0;
		}

		private static object SlowGetMemberValue(PathStep step, object instance)
		{
			switch (step.StepType)
			{
			case PathStepType.Member:
			{
				FieldInfo fieldInfo = step.Member as FieldInfo;
				if (fieldInfo != null)
				{
					return fieldInfo.GetValue(instance);
				}
				PropertyInfo propertyInfo = step.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					return propertyInfo.GetValue(instance, null);
				}
				MethodInfo methodInfo = step.Member as MethodInfo;
				if (methodInfo != null)
				{
					return methodInfo.Invoke(instance, null);
				}
				throw new NotSupportedException(step.Member.GetType().GetNiceName());
			}
			case PathStepType.WeakListElement:
				return WeakListGetItem.Invoke(instance, new object[1] { step.ElementIndex });
			case PathStepType.ArrayElement:
				return (instance as Array).GetValue(step.ElementIndex);
			case PathStepType.StrongListElement:
				return step.StrongListGetItemMethod.Invoke(instance, new object[1] { step.ElementIndex });
			default:
			{
				PathStepType stepType = step.StepType;
				throw new NotImplementedException(stepType.ToString());
			}
			}
		}

		private static void SlowSetMemberValue(PathStep step, object instance, object value)
		{
			switch (step.StepType)
			{
			case PathStepType.Member:
			{
				FieldInfo fieldInfo = step.Member as FieldInfo;
				if (fieldInfo != null)
				{
					fieldInfo.SetValue(instance, value);
					break;
				}
				PropertyInfo propertyInfo = step.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					propertyInfo.SetValue(instance, value, null);
					break;
				}
				throw new NotSupportedException(step.Member.GetType().GetNiceName());
			}
			case PathStepType.WeakListElement:
				WeakListSetItem.Invoke(instance, new object[2] { step.ElementIndex, value });
				break;
			case PathStepType.ArrayElement:
				(instance as Array).SetValue(value, step.ElementIndex);
				break;
			case PathStepType.StrongListElement:
			{
				MethodInfo method = typeof(IList<>).MakeGenericType(step.ElementType).GetMethod("set_Item");
				method.Invoke(instance, new object[2] { step.ElementIndex, value });
				break;
			}
			default:
			{
				PathStepType stepType = step.StepType;
				throw new NotImplementedException(stepType.ToString());
			}
			}
		}

		private static List<PathStep> GetMemberPath(Type rootType, ref Type resultType, string path, out bool rootIsStatic, bool isSet)
		{
			if (path.IsNullOrWhitespace())
			{
				throw new ArgumentException("Invalid path; is null or whitespace.");
			}
			rootIsStatic = false;
			List<PathStep> list = new List<PathStep>();
			string[] array = path.Split('.');
			Type type = rootType;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				bool flag = false;
				if (text.StartsWith("[", StringComparison.InvariantCulture) && text.EndsWith("]", StringComparison.InvariantCulture))
				{
					string s = text.Substring(1, text.Length - 2);
					int result;
					if (!int.TryParse(s, out result))
					{
						throw new ArgumentException("Couldn't parse an index from the path step '" + text + "'.");
					}
					if (type.IsArray)
					{
						Type elementType = type.GetElementType();
						list.Add(new PathStep(result, elementType, true));
						type = elementType;
						continue;
					}
					if (type.ImplementsOpenGenericInterface(typeof(IList<>)))
					{
						Type type2 = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
						list.Add(new PathStep(result, type2, false));
						type = type2;
						continue;
					}
					if (typeof(IList).IsAssignableFrom(type))
					{
						list.Add(new PathStep(result));
						type = typeof(object);
						continue;
					}
					throw new ArgumentException("Cannot get elements by index from the type '" + type.GetNiceName() + "'.");
				}
				if (text.EndsWith("()", StringComparison.InvariantCulture))
				{
					flag = true;
					text = text.Substring(0, text.Length - 2);
				}
				MemberInfo stepMember = GetStepMember(type, text, flag);
				if (stepMember.IsStatic())
				{
					if (type != rootType)
					{
						throw new ArgumentException("The non-root member '" + text + "' is static; use that member as the path root instead.");
					}
					rootIsStatic = true;
				}
				type = stepMember.GetReturnType();
				if (flag && (type == null || type == typeof(void)))
				{
					throw new ArgumentException("The method '" + stepMember.GetNiceName() + "' has no return type and cannot be part of a deep reflection path.");
				}
				list.Add(new PathStep(stepMember));
			}
			if (resultType == null)
			{
				resultType = type;
			}
			else if (type != typeof(object) && !resultType.IsAssignableFrom(type))
			{
				throw new ArgumentException("Last member of path '" + list[list.Count - 1].Member.GetNiceName() + "' contains type '" + type.GetNiceName() + "', which is not assignable to expected type '" + resultType.GetNiceName() + "'.");
			}
			return list;
		}

		private static MemberInfo GetStepMember(Type owningType, string name, bool expectMethod)
		{
			MemberInfo memberInfo = null;
			MemberInfo[] array = owningType.GetAllMembers(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).ToArray();
			int num = int.MaxValue;
			foreach (MemberInfo memberInfo2 in array)
			{
				if (expectMethod)
				{
					MethodInfo methodInfo = memberInfo2 as MethodInfo;
					if (methodInfo != null)
					{
						int num2 = methodInfo.GetParameters().Length;
						if (memberInfo == null || num2 < num)
						{
							memberInfo = methodInfo;
							num = num2;
						}
					}
					continue;
				}
				if (memberInfo2 is MethodInfo)
				{
					throw new ArgumentException("Found method member for name '" + name + "', but expected a field or property.");
				}
				memberInfo = memberInfo2;
				break;
			}
			if (memberInfo == null)
			{
				throw new ArgumentException("Could not find expected " + (expectMethod ? "method" : "field or property") + " '" + name + "' on type '" + owningType.GetNiceName() + "' while parsing reflection path.");
			}
			if (expectMethod && num > 0)
			{
				throw new NotSupportedException("Method '" + memberInfo.GetNiceName() + "' has " + num + " parameters, but method parameters are currently not supported.");
			}
			if (!(memberInfo is FieldInfo) && !(memberInfo is PropertyInfo) && !(memberInfo is MethodInfo))
			{
				throw new NotSupportedException("Members of type " + memberInfo.GetType().GetNiceName() + " are not support; only fields, properties and methods are supported.");
			}
			return memberInfo;
		}
	}
}
