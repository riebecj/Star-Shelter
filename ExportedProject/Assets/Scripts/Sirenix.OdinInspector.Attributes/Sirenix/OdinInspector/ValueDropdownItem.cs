using System;
using System.Collections.Generic;

namespace Sirenix.OdinInspector
{
	public class ValueDropdownItem<T> : IEquatable<ValueDropdownItem<T>>
	{
		public readonly string Text;

		public readonly T Value;

		public ValueDropdownItem(string text, T value)
		{
			Text = text;
			Value = value;
		}

		public override string ToString()
		{
			return Text;
		}

		public bool Equals(ValueDropdownItem<T> other)
		{
			if (other == null)
			{
				return false;
			}
			return EqualityComparer<T>.Default.Equals(Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != typeof(ValueDropdownItem<T>))
			{
				return false;
			}
			return Equals((ValueDropdownItem<T>)obj);
		}

		public override int GetHashCode()
		{
			if (Value == null)
			{
				return -1;
			}
			T value = Value;
			return value.GetHashCode();
		}
	}
}
