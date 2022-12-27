using System;
using System.Globalization;
using System.Reflection;

namespace Sirenix.Utilities
{
	public sealed class MemberAliasMethodInfo : MethodInfo
	{
		private const string FAKE_NAME_SEPARATOR_STRING = "+";

		private MethodInfo aliasedMethod;

		private string mangledName;

		public MethodInfo AliasedMethod
		{
			get
			{
				return aliasedMethod;
			}
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				return aliasedMethod.ReturnTypeCustomAttributes;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return aliasedMethod.MethodHandle;
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return aliasedMethod.Attributes;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return aliasedMethod.DeclaringType;
			}
		}

		public override string Name
		{
			get
			{
				return mangledName;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return aliasedMethod.ReflectedType;
			}
		}

		public MemberAliasMethodInfo(MethodInfo method, string namePrefix)
		{
			aliasedMethod = method;
			mangledName = namePrefix + "+" + aliasedMethod.Name;
		}

		public MemberAliasMethodInfo(MethodInfo method, string namePrefix, string separatorString)
		{
			aliasedMethod = method;
			mangledName = namePrefix + separatorString + aliasedMethod.Name;
		}

		public override MethodInfo GetBaseDefinition()
		{
			return aliasedMethod.GetBaseDefinition();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return aliasedMethod.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return aliasedMethod.GetCustomAttributes(attributeType, inherit);
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return aliasedMethod.GetMethodImplementationFlags();
		}

		public override ParameterInfo[] GetParameters()
		{
			return aliasedMethod.GetParameters();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return aliasedMethod.Invoke(obj, invokeAttr, binder, parameters, culture);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return aliasedMethod.IsDefined(attributeType, inherit);
		}
	}
}
