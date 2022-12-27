using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace ch.sycoforge.Util.Collections
{
	[Serializable]
	public class ConcurrentQueueFast<T> : SynchronizedCollection, ICollection, IEnumerable<T>, IEnumerable, ISerializable, IDeserializationCallback
	{
		private SerializationInfo _info;

		private T[] _items;

		private volatile int _head;

		private volatile int _size;

		private volatile int count = 0;

		public int Count
		{
			get
			{
				return _size;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return count == 0;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		public ConcurrentQueueFast()
			: this(0)
		{
		}

		public ConcurrentQueueFast(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			_items = new T[capacity];
		}

		public ConcurrentQueueFast(IEnumerable<T> collection)
			: this()
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach (T item in collection)
			{
				Enqueue(item);
			}
		}

		protected ConcurrentQueueFast(SerializationInfo info, StreamingContext context)
		{
			_info = info;
		}

		public void Clear()
		{
			lock (_synclock)
			{
				Array.Clear(_items, _head, _size);
			}
		}

		public bool Contains(T item)
		{
			if (_size > 0)
			{
				lock (_synclock)
				{
					return Array.IndexOf(_items, item, _head, _size) >= 0;
				}
			}
			return false;
		}

		public void CopyTo(T[] array)
		{
			CopyTo(array, 0);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex >= array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (_size > 0)
			{
				lock (_synclock)
				{
					Array.Copy(_items, _head, array, arrayIndex, Math.Min(array.Length - arrayIndex, _size));
				}
			}
		}

		public T Dequeue()
		{
			if (_size < 1)
			{
				throw new InvalidOperationException();
			}
			lock (_synclock)
			{
				T result = _items[_head];
				_items[_head] = default(T);
				if (++_head >= _items.Length)
				{
					_head = 0;
				}
				count--;
				return result;
			}
		}

		public void Enqueue(T item)
		{
			lock (_synclock)
			{
				if (_size++ > _items.Length)
				{
					if (_head > 0)
					{
						Array.Copy(_items, _head, _items, 0, _size);
					}
					else
					{
						Array.Resize(ref _items, (_items.Length << 1) | 1);
					}
				}
				_items[_head + (_size - 1)] = item;
				count++;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			object synclock;
			object obj = (synclock = _synclock);
			Monitor.Enter(synclock);
			try
			{
				for (int i = _head; i < _size; i++)
				{
					yield return _items[i];
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			lock (_synclock)
			{
				info.AddValue("items", ToArray());
			}
		}

		public virtual void OnDeserialization(object sender)
		{
			lock (_synclock)
			{
				if (_info != null)
				{
					T[] array = (T[])_info.GetValue("items", typeof(T[]));
					_items = new T[(array.Length << 1) | 1];
					_head = 0;
					_size = array.Length;
					Array.Copy(array, 0, _items, 0, array.Length);
					_info = null;
				}
			}
		}

		public T Peek()
		{
			if (_size < 1)
			{
				throw new InvalidOperationException();
			}
			lock (_synclock)
			{
				return _items[_head];
			}
		}

		public T[] ToArray()
		{
			lock (_synclock)
			{
				T[] array = new T[_size];
				CopyTo(array);
				return array;
			}
		}

		public void TrimExcess()
		{
			lock (_synclock)
			{
				_items = ToArray();
				_head = 0;
				_size = _items.Length;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (_size > 0)
			{
				lock (_synclock)
				{
					Array.ConstrainedCopy(_items, _head, array, index, Math.Min(array.Length - index, _size));
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
