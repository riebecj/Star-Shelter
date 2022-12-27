using System;
using System.Collections;
using System.Collections.Generic;

namespace Sirenix.Utilities
{
	[Serializable]
	public class IndexedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection
	{
		private Dictionary<TKey, TValue> dictionary;

		private List<TKey> indexer;

		public ICollection<TKey> Keys
		{
			get
			{
				return dictionary.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return dictionary.Values;
			}
		}

		public int Count
		{
			get
			{
				return dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((IDictionary)dictionary).IsReadOnly;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return ((IDictionary)dictionary).IsFixedSize;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return ((IDictionary)dictionary).IsReadOnly;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return ((IDictionary)dictionary).Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return ((IDictionary)dictionary).Values;
			}
		}

		int ICollection.Count
		{
			get
			{
				return ((ICollection)dictionary).Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)dictionary).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)dictionary).SyncRoot;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (!(key is TKey))
				{
					throw new InvalidOperationException("Wrong key type.");
				}
				return this[(TKey)key];
			}
			set
			{
				if (!(key is TKey) || !(value is TValue))
				{
					throw new InvalidOperationException("Wrong type.");
				}
				this[(TKey)key] = (TValue)value;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return dictionary[key];
			}
			set
			{
				if (dictionary.ContainsKey(key))
				{
					dictionary[key] = value;
				}
				else
				{
					Add(key, value);
				}
			}
		}

		public IndexedDictionary()
		{
			dictionary = new Dictionary<TKey, TValue>(0);
			indexer = new List<TKey>(0);
		}

		public IndexedDictionary(int capacity)
		{
			dictionary = new Dictionary<TKey, TValue>(capacity);
			indexer = new List<TKey>(capacity);
		}

		public KeyValuePair<TKey, TValue> Get(int index)
		{
			TKey key = indexer[index];
			return new KeyValuePair<TKey, TValue>(key, this[key]);
		}

		public TKey GetKey(int index)
		{
			return indexer[index];
		}

		public TValue GetValue(int index)
		{
			return this[indexer[index]];
		}

		public int IndexOf(TKey key)
		{
			return indexer.IndexOf(key);
		}

		public void Add(TKey key, TValue value)
		{
			dictionary.Add(key, value);
			indexer.Add(key);
		}

		public void Clear()
		{
			indexer.Clear();
			dictionary.Clear();
		}

		public bool Remove(TKey key)
		{
			indexer.Remove(key);
			return dictionary.Remove(key);
		}

		public void RemoveAt(int index)
		{
			if (index >= 0 && index < Count)
			{
				dictionary.Remove(indexer[index]);
				indexer.RemoveAt(index);
			}
		}

		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			dictionary.Add(item.Key, item.Value);
			indexer.Add(item.Key);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (dictionary.ContainsKey(item.Key))
			{
				return dictionary.ContainsValue(item.Value);
			}
			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			indexer.Remove(item.Key);
			return dictionary.Remove(item.Key);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void IDictionary.Add(object key, object value)
		{
			if (!(key is TKey) || !(value is TValue))
			{
				throw new InvalidOperationException("Wrong type.");
			}
			Add((TKey)key, (TValue)value);
		}

		void IDictionary.Clear()
		{
			Clear();
		}

		bool IDictionary.Contains(object key)
		{
			if (!(key is TKey))
			{
				throw new InvalidOperationException("Wrong key type.");
			}
			return ContainsKey((TKey)key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return (IDictionaryEnumerator)GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			if (!(key is TKey))
			{
				throw new InvalidOperationException("Wrong key type.");
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}
	}
}
