using System;
using System.Reflection;

namespace ch.sycoforge.Unity.Util
{
	public static class Caller
	{
		public static object GetStaticField(string typeName, string fieldName, BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic)
		{
			Type type = Type.GetType(typeName);
			FieldInfo field = type.GetField(fieldName, flags);
			return field.GetValue(null);
		}

		public static object GetStaticField(Type type, string fieldName, BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic)
		{
			FieldInfo field = type.GetField(fieldName, flags);
			return field.GetValue(null);
		}

		public static T GetStaticField<T>(Type type, string fieldName, BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic)
		{
			FieldInfo field = type.GetField(fieldName, flags);
			return (T)field.GetValue(null);
		}

		public static object GetField(object obj, string fieldName, BindingFlags flags = BindingFlags.NonPublic)
		{
			Type type = obj.GetType();
			FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | flags);
			return field.GetValue(obj);
		}

		public static T GetField<T>(object obj, string fieldName, BindingFlags flags = BindingFlags.NonPublic)
		{
			Type type = obj.GetType();
			FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | flags);
			if (field != null)
			{
				return (T)field.GetValue(obj);
			}
			return default(T);
		}

		public static void SetField(object obj, string fieldName, object value, BindingFlags flags = BindingFlags.NonPublic)
		{
			Type type = obj.GetType();
			FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | flags);
			if (field != null)
			{
				field.SetValue(obj, value);
			}
		}

		public static void SetStaticField(Type type, string fieldName, object value, BindingFlags flags = BindingFlags.NonPublic)
		{
			FieldInfo field = type.GetField(fieldName, BindingFlags.Static | flags);
			field.SetValue(null, value);
		}

		public static void SetProperty(object obj, string propertyName, object value, BindingFlags flags = BindingFlags.NonPublic)
		{
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(propertyName, flags);
			property.SetValue(obj, value, null);
		}

		public static void SetStaticProperty(string typeName, string propertyName, object value, BindingFlags flags = BindingFlags.NonPublic)
		{
			Type type = Type.GetType(typeName);
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | flags);
			property.SetValue(null, value, null);
		}

		public static void SetStaticProperty(Type type, string propertyName, object value, BindingFlags flags = BindingFlags.NonPublic)
		{
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | flags);
			property.SetValue(null, value, null);
		}

		public static object GetStaticProperty(string typeName, string propertyName, BindingFlags flags = BindingFlags.NonPublic)
		{
			Type type = Type.GetType(typeName);
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | flags);
			return property.GetValue(null, null);
		}

		public static object GetStaticProperty(Type type, string propertyName, BindingFlags flags = BindingFlags.NonPublic)
		{
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | flags);
			return property.GetValue(null, null);
		}

		public static T GetProperty<T>(object obj, string propertyName)
		{
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic);
			return (T)property.GetValue(obj, null);
		}

		public static object Call(object obj, string methodName, BindingFlags flags, Type[] types, params object[] args)
		{
			Type type = obj.GetType();
			MethodInfo method = type.GetMethod(methodName, types);
			return method.Invoke(obj, args);
		}

		public static object Call(object obj, string methodName, BindingFlags flags, params object[] args)
		{
			Type type = obj.GetType();
			Type[] array = new Type[args.Length];
			MethodInfo methodInfo = null;
			try
			{
				methodInfo = type.GetMethod(methodName, flags);
			}
			catch (Exception)
			{
				if (args != null)
				{
					array = new Type[args.Length];
					for (int i = 0; i < args.Length; i++)
					{
						if (args[i] != null)
						{
							array[i] = args[i].GetType();
						}
						else
						{
							array[i] = typeof(object);
						}
					}
					methodInfo = type.GetMethod(methodName, flags, null, array, null);
				}
				else
				{
					methodInfo = type.GetMethod(methodName, flags);
				}
			}
			return methodInfo.Invoke(obj, args);
		}

		public static T Call<T>(object obj, string methodName, BindingFlags flags, params object[] args)
		{
			Type type = obj.GetType();
			MethodInfo method = type.GetMethod(methodName, flags);
			return (T)method.Invoke(obj, args);
		}

		public static object StaticCall(Type type, string methodName, BindingFlags flags, params object[] args)
		{
			MethodInfo methodInfo = null;
			if (args != null && args.Length > 0)
			{
				Type[] array = new Type[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					array[i] = args[i].GetType();
				}
				methodInfo = type.GetMethod(methodName, BindingFlags.Static | flags, null, array, null);
			}
			else
			{
				methodInfo = type.GetMethod(methodName, BindingFlags.Static | flags);
			}
			return methodInfo.Invoke(null, args);
		}

		public static object StaticCall(string typeName, string methodName, params object[] args)
		{
			Type type = Type.GetType(typeName);
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
			return method.Invoke(null, args);
		}

		public static T StaticCall<T>(string typeName, string methodName, params object[] args)
		{
			Type type = Type.GetType(typeName);
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
			return (T)method.Invoke(null, args);
		}

		public static T StaticCall<T>(Type type, string methodName, BindingFlags flags, params object[] args)
		{
			Type[] array = null;
			MethodInfo method;
			if (args != null && args.Length > 0)
			{
				array = new Type[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					array[i] = args[i].GetType();
				}
				method = type.GetMethod(methodName, BindingFlags.Static | flags, null, array, null);
			}
			else
			{
				method = type.GetMethod(methodName, BindingFlags.Static | flags);
			}
			return (T)method.Invoke(null, args);
		}
	}
}
