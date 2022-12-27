using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public abstract class PropertyGroupAttribute : ShowInInspectorAttribute
	{
		public string GroupID { get; protected set; }

		public string GroupName { get; protected set; }

		public int Order { get; protected set; }

		public PropertyGroupAttribute(string groupId, int order)
		{
			GroupID = groupId;
			Order = order;
			int num = groupId.LastIndexOf('/');
			GroupName = ((num >= 0 && num < groupId.Length) ? groupId.Substring(num + 1) : groupId);
		}

		public PropertyGroupAttribute Combine(PropertyGroupAttribute other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other.GetType() != GetType())
			{
				throw new ArgumentException("Attributes to combine are not of the same type.");
			}
			if (other.GroupID != GroupID)
			{
				throw new ArgumentException("PropertyGroupAttributes to combine must have the same group id.");
			}
			if (Order == 0)
			{
				Order = other.Order;
			}
			else if (other.Order != 0)
			{
				Order = Math.Min(Order, other.Order);
			}
			CombineValuesWith(other);
			return this;
		}

		protected virtual void CombineValuesWith(PropertyGroupAttribute other)
		{
		}
	}
}
