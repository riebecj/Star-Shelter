using System;

namespace Sirenix.OdinInspector
{
	public class ValueDropdownAttribute : Attribute
	{
		public string MemberName { get; private set; }

		public ValueDropdownAttribute(string memberName)
		{
			MemberName = memberName;
		}
	}
}
