using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sirenix.Utilities
{
	public static class EmitUtilities
	{
		public static bool CanEmit
		{
			get
			{
				return true;
			}
		}

		public static Func<FieldType> CreateStaticFieldGetter<FieldType>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (!fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field must be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(FieldType), new Type[0], true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldsfld, fieldInfo);
			iLGenerator.Emit(OpCodes.Ret);
			return (Func<FieldType>)dynamicMethod.CreateDelegate(typeof(Func<FieldType>));
		}

		public static Func<object> CreateWeakStaticFieldGetter(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (!fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field must be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[0], true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldsfld, fieldInfo);
			if (fieldInfo.FieldType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Box, fieldInfo.FieldType);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>));
		}

		public static Action<FieldType> CreateStaticFieldSetter<FieldType>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (!fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field must be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[1] { typeof(FieldType) }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Stsfld, fieldInfo);
			iLGenerator.Emit(OpCodes.Ret);
			return (Action<FieldType>)dynamicMethod.CreateDelegate(typeof(Action<FieldType>));
		}

		public static Action<object> CreateWeakStaticFieldSetter(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (!fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field must be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[1] { typeof(object) }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			if (fieldInfo.FieldType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Castclass, fieldInfo.FieldType);
			}
			iLGenerator.Emit(OpCodes.Stsfld, fieldInfo);
			iLGenerator.Emit(OpCodes.Ret);
			return (Action<object>)dynamicMethod.CreateDelegate(typeof(Action<object>));
		}

		public static ValueGetter<InstanceType, FieldType> CreateInstanceFieldGetter<InstanceType, FieldType>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field cannot be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(FieldType), new Type[1] { typeof(InstanceType).MakeByRefType() }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (typeof(InstanceType).IsValueType)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (ValueGetter<InstanceType, FieldType>)dynamicMethod.CreateDelegate(typeof(ValueGetter<InstanceType, FieldType>));
		}

		public static WeakValueGetter<FieldType> CreateWeakInstanceFieldGetter<FieldType>(Type instanceType, FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (instanceType == null)
			{
				throw new ArgumentNullException("instanceType");
			}
			if (fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field cannot be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(FieldType), new Type[1] { typeof(object).MakeByRefType() }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (instanceType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Unbox_Any, instanceType);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Castclass, instanceType);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (WeakValueGetter<FieldType>)dynamicMethod.CreateDelegate(typeof(WeakValueGetter<FieldType>));
		}

		public static WeakValueGetter CreateWeakInstanceFieldGetter(Type instanceType, FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (instanceType == null)
			{
				throw new ArgumentNullException("instanceType");
			}
			if (fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field cannot be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[1] { typeof(object).MakeByRefType() }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (instanceType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Unbox_Any, instanceType);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
				if (fieldInfo.FieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, fieldInfo.FieldType);
				}
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Castclass, instanceType);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
				if (fieldInfo.FieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, fieldInfo.FieldType);
				}
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (WeakValueGetter)dynamicMethod.CreateDelegate(typeof(WeakValueGetter));
		}

		public static ValueSetter<InstanceType, FieldType> CreateInstanceFieldSetter<InstanceType, FieldType>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field cannot be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[2]
			{
				typeof(InstanceType).MakeByRefType(),
				typeof(FieldType)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (typeof(InstanceType).IsValueType)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (ValueSetter<InstanceType, FieldType>)dynamicMethod.CreateDelegate(typeof(ValueSetter<InstanceType, FieldType>));
		}

		public static WeakValueSetter<FieldType> CreateWeakInstanceFieldSetter<FieldType>(Type instanceType, FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (instanceType == null)
			{
				throw new ArgumentNullException("instanceType");
			}
			if (fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field cannot be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[2]
			{
				typeof(object).MakeByRefType(),
				typeof(FieldType)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (instanceType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(instanceType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Unbox_Any, instanceType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldloc, local);
				iLGenerator.Emit(OpCodes.Box, instanceType);
				iLGenerator.Emit(OpCodes.Stind_Ref);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Castclass, instanceType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (WeakValueSetter<FieldType>)dynamicMethod.CreateDelegate(typeof(WeakValueSetter<FieldType>));
		}

		public static WeakValueSetter CreateWeakInstanceFieldSetter(Type instanceType, FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (instanceType == null)
			{
				throw new ArgumentNullException("instanceType");
			}
			if (fieldInfo.IsStatic)
			{
				throw new ArgumentException("Field cannot be static.");
			}
			fieldInfo = fieldInfo.DeAliasField();
			string name = fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[2]
			{
				typeof(object).MakeByRefType(),
				typeof(object)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (instanceType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(instanceType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Unbox_Any, instanceType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				if (fieldInfo.FieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, fieldInfo.FieldType);
				}
				iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldloc, local);
				iLGenerator.Emit(OpCodes.Box, instanceType);
				iLGenerator.Emit(OpCodes.Stind_Ref);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Castclass, instanceType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				if (fieldInfo.FieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, fieldInfo.FieldType);
				}
				iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (WeakValueSetter)dynamicMethod.CreateDelegate(typeof(WeakValueSetter));
		}

		public static WeakValueGetter CreateWeakInstancePropertyGetter(Type instanceType, PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (instanceType == null)
			{
				throw new ArgumentNullException("instanceType");
			}
			propertyInfo = propertyInfo.DeAliasProperty();
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException("Property must have a getter.");
			}
			if (getMethod.IsStatic)
			{
				throw new ArgumentException("Property cannot be static.");
			}
			string name = propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[1] { typeof(object).MakeByRefType() }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (instanceType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Unbox_Any, instanceType);
				if (getMethod.IsVirtual || getMethod.IsAbstract)
				{
					iLGenerator.Emit(OpCodes.Callvirt, getMethod);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Call, getMethod);
				}
				if (propertyInfo.PropertyType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
				}
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Castclass, instanceType);
				if (getMethod.IsVirtual || getMethod.IsAbstract)
				{
					iLGenerator.Emit(OpCodes.Callvirt, getMethod);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Call, getMethod);
				}
				if (propertyInfo.PropertyType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
				}
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (WeakValueGetter)dynamicMethod.CreateDelegate(typeof(WeakValueGetter));
		}

		public static WeakValueSetter CreateWeakInstancePropertySetter(Type instanceType, PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (instanceType == null)
			{
				throw new ArgumentNullException("instanceType");
			}
			propertyInfo = propertyInfo.DeAliasProperty();
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod.IsStatic)
			{
				throw new ArgumentException("Property cannot be static.");
			}
			string name = propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[2]
			{
				typeof(object).MakeByRefType(),
				typeof(object)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (instanceType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(instanceType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Unbox_Any, instanceType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				if (propertyInfo.PropertyType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
				}
				if (setMethod.IsVirtual || setMethod.IsAbstract)
				{
					iLGenerator.Emit(OpCodes.Callvirt, setMethod);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Call, setMethod);
				}
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldloc, local);
				iLGenerator.Emit(OpCodes.Box, instanceType);
				iLGenerator.Emit(OpCodes.Stind_Ref);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Castclass, instanceType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				if (propertyInfo.PropertyType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
				}
				if (setMethod.IsVirtual || setMethod.IsAbstract)
				{
					iLGenerator.Emit(OpCodes.Callvirt, setMethod);
				}
				else
				{
					iLGenerator.Emit(OpCodes.Call, setMethod);
				}
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (WeakValueSetter)dynamicMethod.CreateDelegate(typeof(WeakValueSetter));
		}

		public static Action<PropType> CreateStaticPropertySetter<PropType>(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			propertyInfo = propertyInfo.DeAliasProperty();
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod == null)
			{
				throw new ArgumentException("Property must have a set method.");
			}
			if (!setMethod.IsStatic)
			{
				throw new ArgumentException("Property must be static.");
			}
			string name = propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[1] { typeof(PropType) }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Callvirt, setMethod);
			iLGenerator.Emit(OpCodes.Ret);
			return (Action<PropType>)dynamicMethod.CreateDelegate(typeof(Action<PropType>));
		}

		public static Func<PropType> CreateStaticPropertyGetter<PropType>(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			propertyInfo = propertyInfo.DeAliasProperty();
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException("Property must have a get method.");
			}
			if (!getMethod.IsStatic)
			{
				throw new ArgumentException("Property must be static.");
			}
			string name = propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(PropType), new Type[0], true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Callvirt, getMethod);
			iLGenerator.Emit(OpCodes.Ret);
			return (Func<PropType>)dynamicMethod.CreateDelegate(typeof(Func<PropType>));
		}

		public static ValueSetter<InstanceType, PropType> CreateInstancePropertySetter<InstanceType, PropType>(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			propertyInfo = propertyInfo.DeAliasProperty();
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod == null)
			{
				throw new ArgumentException("Property must have a set method.");
			}
			if (setMethod.IsStatic)
			{
				throw new ArgumentException("Property cannot be static.");
			}
			string name = propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[2]
			{
				typeof(InstanceType).MakeByRefType(),
				typeof(PropType)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (typeof(InstanceType).IsValueType)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Callvirt, setMethod);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Callvirt, setMethod);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (ValueSetter<InstanceType, PropType>)dynamicMethod.CreateDelegate(typeof(ValueSetter<InstanceType, PropType>));
		}

		public static ValueGetter<InstanceType, PropType> CreateInstancePropertyGetter<InstanceType, PropType>(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			propertyInfo = propertyInfo.DeAliasProperty();
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException("Property must have a get method.");
			}
			if (getMethod.IsStatic)
			{
				throw new ArgumentException("Property cannot be static.");
			}
			string name = propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(PropType), new Type[1] { typeof(InstanceType).MakeByRefType() }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (typeof(InstanceType).IsValueType)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Callvirt, getMethod);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldind_Ref);
				iLGenerator.Emit(OpCodes.Callvirt, getMethod);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (ValueGetter<InstanceType, PropType>)dynamicMethod.CreateDelegate(typeof(ValueGetter<InstanceType, PropType>));
		}

		public static Func<InstanceType, ReturnType> CreateMethodReturner<InstanceType, ReturnType>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			methodInfo = methodInfo.DeAliasMethod();
			return (Func<InstanceType, ReturnType>)Delegate.CreateDelegate(typeof(Func<InstanceType, ReturnType>), methodInfo);
		}

		public static Action CreateStaticMethodCaller(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (!methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is an instance method when it has to be static.");
			}
			if (methodInfo.GetParameters().Length != 0)
			{
				throw new ArgumentException("Given method cannot have any parameters.");
			}
			methodInfo = methodInfo.DeAliasMethod();
			return (Action)Delegate.CreateDelegate(typeof(Action), methodInfo);
		}

		public static Action<object, TArg1> CreateWeakInstanceMethodCaller<TArg1>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 1)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' must have exactly one parameter.");
			}
			if (parameters[0].ParameterType != typeof(TArg1))
			{
				throw new ArgumentException(string.Concat("The first parameter of the method '", methodInfo.Name, "' must be of type ", typeof(TArg1), "."));
			}
			methodInfo = methodInfo.DeAliasMethod();
			Type declaringType = methodInfo.DeclaringType;
			string name = methodInfo.ReflectedType.FullName + ".call_" + methodInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[2]
			{
				typeof(object),
				typeof(TArg1)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (declaringType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Unbox_Any, declaringType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Call, methodInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Castclass, declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (Action<object, TArg1>)dynamicMethod.CreateDelegate(typeof(Action<object, TArg1>));
		}

		public static Action<object> CreateWeakInstanceMethodCaller(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			if (methodInfo.GetParameters().Length != 0)
			{
				throw new ArgumentException("Given method cannot have any parameters.");
			}
			methodInfo = methodInfo.DeAliasMethod();
			Type declaringType = methodInfo.DeclaringType;
			string name = methodInfo.ReflectedType.FullName + ".call_" + methodInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, null, new Type[1] { typeof(object) }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (declaringType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Unbox_Any, declaringType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Call, methodInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Castclass, declaringType);
				iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
			}
			if (methodInfo.ReturnType != null && methodInfo.ReturnType != typeof(void))
			{
				iLGenerator.Emit(OpCodes.Pop);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (Action<object>)dynamicMethod.CreateDelegate(typeof(Action<object>));
		}

		public static Func<object, TArg1, TResult> CreateWeakInstanceMethodCaller<TResult, TArg1>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			if (methodInfo.ReturnType != typeof(TResult))
			{
				throw new ArgumentException(string.Concat("Given method '", methodInfo.Name, "' must return type ", typeof(TResult), "."));
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 1)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' must have exactly one parameter.");
			}
			if (!typeof(TArg1).InheritsFrom(parameters[0].ParameterType))
			{
				throw new ArgumentException(string.Concat("The first parameter of the method '", methodInfo.Name, "' must be of type ", typeof(TArg1), "."));
			}
			methodInfo = methodInfo.DeAliasMethod();
			Type declaringType = methodInfo.DeclaringType;
			string name = methodInfo.ReflectedType.FullName + ".call_" + methodInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(TResult), new Type[2]
			{
				typeof(object),
				typeof(TArg1)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (declaringType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Unbox_Any, declaringType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Call, methodInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Castclass, declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (Func<object, TArg1, TResult>)dynamicMethod.CreateDelegate(typeof(Func<object, TArg1, TResult>));
		}

		public static Func<object, TResult> CreateWeakInstanceMethodCallerFunc<TResult>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			if (methodInfo.ReturnType != typeof(TResult))
			{
				throw new ArgumentException(string.Concat("Given method '", methodInfo.Name, "' must return type ", typeof(TResult), "."));
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 0)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' must have no parameter.");
			}
			methodInfo = methodInfo.DeAliasMethod();
			Type declaringType = methodInfo.DeclaringType;
			string name = methodInfo.ReflectedType.FullName + ".call_" + methodInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(TResult), new Type[1] { typeof(object) }, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (declaringType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Unbox_Any, declaringType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Call, methodInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Castclass, declaringType);
				iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (Func<object, TResult>)dynamicMethod.CreateDelegate(typeof(Func<object, TResult>));
		}

		public static Func<object, TArg, TResult> CreateWeakInstanceMethodCallerFunc<TArg, TResult>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			if (methodInfo.ReturnType != typeof(TResult))
			{
				throw new ArgumentException(string.Concat("Given method '", methodInfo.Name, "' must return type ", typeof(TResult), "."));
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 1)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' must have one parameter.");
			}
			if (!parameters[0].ParameterType.IsAssignableFrom(typeof(TArg)))
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' has an invalid parameter type.");
			}
			methodInfo = methodInfo.DeAliasMethod();
			Type declaringType = methodInfo.DeclaringType;
			string name = methodInfo.ReflectedType.FullName + ".call_" + methodInfo.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(TResult), new Type[2]
			{
				typeof(object),
				typeof(TArg)
			}, true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (declaringType.IsValueType)
			{
				LocalBuilder local = iLGenerator.DeclareLocal(declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Unbox_Any, declaringType);
				iLGenerator.Emit(OpCodes.Stloc, local);
				iLGenerator.Emit(OpCodes.Ldloca_S, local);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Call, methodInfo);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Castclass, declaringType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return (Func<object, TArg, TResult>)dynamicMethod.CreateDelegate(typeof(Func<object, TArg, TResult>));
		}

		public static Action<InstanceType> CreateInstanceMethodCaller<InstanceType>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			if (methodInfo.GetParameters().Length != 0)
			{
				throw new ArgumentException("Given method cannot have any parameters.");
			}
			methodInfo = methodInfo.DeAliasMethod();
			return (Action<InstanceType>)Delegate.CreateDelegate(typeof(Action<InstanceType>), methodInfo);
		}

		public static Action<InstanceType, Arg1> CreateInstanceMethodCaller<InstanceType, Arg1>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (methodInfo.IsStatic)
			{
				throw new ArgumentException("Given method '" + methodInfo.Name + "' is static when it has to be an instance method.");
			}
			if (methodInfo.GetParameters().Length != 1)
			{
				throw new ArgumentException("Given method must have only one parameter.");
			}
			methodInfo = methodInfo.DeAliasMethod();
			return (Action<InstanceType, Arg1>)Delegate.CreateDelegate(typeof(Action<InstanceType, Arg1>), methodInfo);
		}
	}
}
