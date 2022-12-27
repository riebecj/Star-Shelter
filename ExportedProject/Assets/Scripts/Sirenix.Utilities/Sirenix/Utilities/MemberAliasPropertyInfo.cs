using System;
using System.Globalization;
using System.Reflection;

namespace Sirenix.Utilities
{
	public sealed class MemberAliasPropertyInfo : PropertyInfo
	{
		private const string FakeNameSeparatorString = "+";

		private PropertyInfo aliasedProperty;

		private string mangledName;

		public PropertyInfo AliasedProperty
		{
			get
			{
				return aliasedProperty;
			}
		}

		public override Module Module
		{
			get
			{
				return aliasedProperty.Module;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return aliasedProperty.MetadataToken;
			}
		}

		public override string Name
		{
			get
			{
				return mangledName;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return aliasedProperty.DeclaringType;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return aliasedProperty.ReflectedType;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return aliasedProperty.PropertyType;
			}
		}

		public override PropertyAttributes Attributes
		{
			get
			{
				return aliasedProperty.Attributes;
			}
		}

		public override bool CanRead
		{
			get
			{
				return aliasedProperty.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return aliasedProperty.CanWrite;
			}
		}

		public MemberAliasPropertyInfo(PropertyInfo prop, string namePrefix)
		{
			aliasedProperty = prop;
			mangledName = namePrefix + "+" + aliasedProperty.Name;
		}

		public MemberAliasPropertyInfo(PropertyInfo prop, string namePrefix, string separatorString)
		{
			aliasedProperty = prop;
			mangledName = namePrefix + separatorString + aliasedProperty.Name;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return aliasedProperty.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return aliasedProperty.GetCustomAttributes(attributeType, inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return aliasedProperty.IsDefined(attributeType, inherit);
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			return aliasedProperty.GetAccessors(nonPublic);
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return aliasedProperty.GetGetMethod(nonPublic);
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			return aliasedProperty.GetIndexParameters();
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			return aliasedProperty.GetSetMethod(nonPublic);
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			return aliasedProperty.GetValue(obj, invokeAttr, binder, index, culture);
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			aliasedProperty.SetValue(obj, value, invokeAttr, binder, index, culture);
		}
	}
}
