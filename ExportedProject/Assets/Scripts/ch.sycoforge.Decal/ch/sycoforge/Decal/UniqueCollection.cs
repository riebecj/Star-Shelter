using System.Collections;
using System.Collections.Generic;

namespace ch.sycoforge.Decal
{
	internal class UniqueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private Dictionary<T, bool> _innerDictionary;

		public int Count
		{
			get
			{
				return _innerDictionary.Keys.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public UniqueCollection()
		{
			_innerDictionary = new Dictionary<T, bool>();
		}

		void ICollection<T>.Add(T item)
		{
			AddInternal(item);
		}

		private void AddInternal(T item)
		{
			_innerDictionary.Add(item, false);
		}

		public bool Add(T item)
		{
			if (_innerDictionary.ContainsKey(item))
			{
				return false;
			}
			AddInternal(item);
			return true;
		}

		public void Clear()
		{
			_innerDictionary.Clear();
			_innerDictionary = new Dictionary<T, bool>();
		}

		public bool Contains(T item)
		{
			return _innerDictionary.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_innerDictionary.Keys.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return _innerDictionary.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _innerDictionary.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
