using System;
using System.Linq;
using UnityEngine;

namespace VRTK
{
	[Serializable]
	public sealed class VRTK_SDKInfo : ISerializationCallbackReceiver
	{
		[SerializeField]
		private string baseTypeName;

		[SerializeField]
		private string fallbackTypeName;

		[SerializeField]
		private string typeName;

		public Type type { get; private set; }

		public string originalTypeNameWhenFallbackIsUsed { get; private set; }

		public SDK_DescriptionAttribute description { get; private set; }

		private VRTK_SDKInfo()
		{
		}

		public VRTK_SDKInfo(VRTK_SDKInfo infoToCopy)
		{
			SetUp(Type.GetType(infoToCopy.baseTypeName), Type.GetType(infoToCopy.fallbackTypeName), infoToCopy.typeName);
		}

		public static VRTK_SDKInfo Create<BaseType, FallbackType, ActualType>() where BaseType : SDK_Base where FallbackType : BaseType where ActualType : BaseType
		{
			VRTK_SDKInfo vRTK_SDKInfo = new VRTK_SDKInfo();
			vRTK_SDKInfo.SetUp(typeof(BaseType), typeof(FallbackType), typeof(ActualType).FullName);
			return vRTK_SDKInfo;
		}

		public static VRTK_SDKInfo Create<BaseType, FallbackType>(Type actualType) where BaseType : SDK_Base where FallbackType : BaseType
		{
			VRTK_SDKInfo vRTK_SDKInfo = new VRTK_SDKInfo();
			vRTK_SDKInfo.SetUp(typeof(BaseType), typeof(FallbackType), actualType.FullName);
			return vRTK_SDKInfo;
		}

		private void SetUp(Type baseType, Type fallbackType, string actualTypeName)
		{
			if (!baseType.IsSubclassOf(typeof(SDK_Base)))
			{
				throw new ArgumentOutOfRangeException("baseType", baseType, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", baseType.Name, typeof(SDK_Base).Name));
			}
			if (!fallbackType.IsSubclassOf(baseType))
			{
				throw new ArgumentOutOfRangeException("fallbackType", fallbackType, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", fallbackType.Name, baseType.Name));
			}
			baseTypeName = baseType.FullName;
			fallbackTypeName = fallbackType.FullName;
			typeName = actualTypeName;
			if (string.IsNullOrEmpty(actualTypeName))
			{
				this.type = fallbackType;
				originalTypeNameWhenFallbackIsUsed = null;
				description = SDK_DescriptionAttribute.Fallback;
				return;
			}
			Type type = Type.GetType(actualTypeName);
			if (type == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_NOT_FOUND, actualTypeName, fallbackType.Name));
				this.type = fallbackType;
				originalTypeNameWhenFallbackIsUsed = actualTypeName;
				description = SDK_DescriptionAttribute.Fallback;
				return;
			}
			if (!type.IsSubclassOf(baseType))
			{
				throw new ArgumentOutOfRangeException("actualTypeName", actualTypeName, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", actualTypeName, baseType.Name));
			}
			string @namespace = typeof(SDK_FallbackSystem).Namespace;
			string value = typeof(SDK_FallbackSystem).Name.Replace("System", string.Empty);
			if (type.Namespace == @namespace && type.Name.StartsWith(value, StringComparison.Ordinal))
			{
				this.type = type;
				originalTypeNameWhenFallbackIsUsed = null;
				description = SDK_DescriptionAttribute.Fallback;
				return;
			}
			SDK_DescriptionAttribute sDK_DescriptionAttribute = (SDK_DescriptionAttribute)type.GetCustomAttributes(typeof(SDK_DescriptionAttribute), false).FirstOrDefault();
			if (sDK_DescriptionAttribute == null)
			{
				throw new ArgumentException(string.Format("'{0}' doesn't specify an SDK description via '{1}'.", actualTypeName, typeof(SDK_DescriptionAttribute).Name), "actualTypeName");
			}
			this.type = type;
			originalTypeNameWhenFallbackIsUsed = null;
			description = sDK_DescriptionAttribute;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			SetUp(Type.GetType(baseTypeName), Type.GetType(fallbackTypeName), typeName);
		}

		public override bool Equals(object obj)
		{
			VRTK_SDKInfo vRTK_SDKInfo = obj as VRTK_SDKInfo;
			if ((object)vRTK_SDKInfo == null)
			{
				return false;
			}
			return type == vRTK_SDKInfo.type;
		}

		public bool Equals(VRTK_SDKInfo other)
		{
			if ((object)other == null)
			{
				return false;
			}
			return type == other.type;
		}

		public override int GetHashCode()
		{
			return type.GetHashCode();
		}

		public static bool operator ==(VRTK_SDKInfo x, VRTK_SDKInfo y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			if ((object)x == null || (object)y == null)
			{
				return false;
			}
			return x.type == y.type;
		}

		public static bool operator !=(VRTK_SDKInfo x, VRTK_SDKInfo y)
		{
			return !(x == y);
		}
	}
}
