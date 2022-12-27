using System;
using System.Linq;

namespace VRTK
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class SDK_DescriptionAttribute : Attribute
	{
		public static readonly SDK_DescriptionAttribute Fallback = new SDK_DescriptionAttribute("Fallback", null);

		public readonly string prettyName;

		public readonly string symbol;

		public SDK_DescriptionAttribute(string prettyName, string symbol)
		{
			if (prettyName == null)
			{
				throw new ArgumentNullException("prettyName");
			}
			if (prettyName == string.Empty)
			{
				throw new ArgumentOutOfRangeException("prettyName", prettyName, "An empty string isn't allowed.");
			}
			this.prettyName = prettyName;
			this.symbol = symbol;
		}

		public SDK_DescriptionAttribute(Type typeToCopyExistingDescriptionFrom)
		{
			if (typeToCopyExistingDescriptionFrom == null)
			{
				throw new ArgumentNullException("typeToCopyExistingDescriptionFrom");
			}
			Type typeFromHandle = typeof(SDK_DescriptionAttribute);
			SDK_DescriptionAttribute sDK_DescriptionAttribute = (SDK_DescriptionAttribute)typeToCopyExistingDescriptionFrom.GetCustomAttributes(typeFromHandle, false).FirstOrDefault();
			if (sDK_DescriptionAttribute == null)
			{
				throw new ArgumentOutOfRangeException("typeToCopyExistingDescriptionFrom", typeToCopyExistingDescriptionFrom, string.Format("'{0}' doesn't specify an SDK description via '{1}' to copy.", typeToCopyExistingDescriptionFrom.Name, typeFromHandle.Name));
			}
			prettyName = sDK_DescriptionAttribute.prettyName;
			symbol = sDK_DescriptionAttribute.symbol;
		}
	}
}
