using System;
using System.Collections.Generic;

namespace Sirenix.Utilities
{
	[Serializable]
	public class DoubleLookupDictionary<TFirstKey, TSecondKey, TValue> : Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>>
	{
		public new Dictionary<TSecondKey, TValue> this[TFirstKey firstKey]
		{
			get
			{
				Dictionary<TSecondKey, TValue> value;
				if (!TryGetValue(firstKey, out value))
				{
					value = new Dictionary<TSecondKey, TValue>();
					Add(firstKey, value);
				}
				return value;
			}
		}

		public int InnerCount(TFirstKey firstKey)
		{
			Dictionary<TSecondKey, TValue> value;
			if (TryGetValue(firstKey, out value))
			{
				return value.Count;
			}
			return 0;
		}

		public int TotalInnerCount()
		{
			int num = 0;
			if (base.Count > 0)
			{
				foreach (Dictionary<TSecondKey, TValue> value in base.Values)
				{
					num += value.Count;
				}
				return num;
			}
			return num;
		}

		public bool ContainsKeys(TFirstKey firstKey, TSecondKey secondKey)
		{
			Dictionary<TSecondKey, TValue> value;
			if (TryGetValue(firstKey, out value))
			{
				return value.ContainsKey(secondKey);
			}
			return false;
		}

		public bool TryGetInnerValue(TFirstKey firstKey, TSecondKey secondKey, out TValue value)
		{
			Dictionary<TSecondKey, TValue> value2;
			if (TryGetValue(firstKey, out value2) && value2.TryGetValue(secondKey, out value))
			{
				return true;
			}
			value = default(TValue);
			return false;
		}

		public TValue AddInner(TFirstKey firstKey, TSecondKey secondKey, TValue value)
		{
			if (ContainsKeys(firstKey, secondKey))
			{
				throw new ArgumentException("An element with the same keys already exists in the " + GetType().GetNiceName() + ".");
			}
			return this[firstKey][secondKey] = value;
		}

		public bool RemoveInner(TFirstKey firstKey, TSecondKey secondKey)
		{
			Dictionary<TSecondKey, TValue> value;
			if (TryGetValue(firstKey, out value))
			{
				bool result = value.Remove(secondKey);
				if (value.Count == 0)
				{
					Remove(firstKey);
				}
				return result;
			}
			return false;
		}

		public void RemoveWhere(Func<TValue, bool> predicate)
		{
			List<TFirstKey> list = new List<TFirstKey>();
			List<TSecondKey> list2 = new List<TSecondKey>();
			foreach (KeyValuePair<TFirstKey, Dictionary<TSecondKey, TValue>> item in this.GFIterator())
			{
				foreach (KeyValuePair<TSecondKey, TValue> item2 in item.Value.GFIterator())
				{
					if (predicate(item2.Value))
					{
						list.Add(item.Key);
						list2.Add(item2.Key);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				RemoveInner(list[i], list2[i]);
			}
		}
	}
}
