using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace ch.sycoforge.Util.Collections
{
	[Serializable]
	public class ConcurrentQueue<T> : SynchronizedCollection, ICollection, IEnumerable<T>, IEnumerable, ISerializable, IDeserializationCallback
	{
		private SerializationInfo _info;

		private T[] items;

		private volatile int head;

		private volatile int start;

		private volatile int _size;

		private volatile int count = 0;

		public int Count
		{
			get
			{
				return count;
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

		public ConcurrentQueue()
			: this(0)
		{
		}

		public ConcurrentQueue(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			items = new T[capacity + 1];
			start = (head = 0);
		}

		public ConcurrentQueue(IEnumerable<T> collection)
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

		protected ConcurrentQueue(SerializationInfo info, StreamingContext context)
		{
			_info = info;
		}

		public void Clear()
		{
			lock (_synclock)
			{
				Array.Clear(items, head, _size);
			}
		}

		public bool Contains(T item)
		{
			if (_size > 0)
			{
				lock (_synclock)
				{
					return Array.IndexOf(items, item, head, _size) >= 0;
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
					Array.Copy(items, head, array, arrayIndex, Math.Min(array.Length - arrayIndex, _size));
				}
			}
		}

		public T Dequeue()
		{
			lock (_synclock)
			{
				if (IsEmpty)
				{
					return default(T);
				}
				head = (head + 1) % items.Length;
				T result = items[head];
				items[head] = default(T);
				count--;
				return result;
			}
		}

		public void Enqueue(T item)
		{
			lock (_synclock)
			{
				int num = (start + 1) % items.Length;
				if (num != head)
				{
					items[start = num] = item;
					count++;
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			object synclock;
			object obj = (synclock = _synclock);
			Monitor.Enter(synclock);
			try
			{
				for (int i = head; i < _size; i++)
				{
					yield return items[i];
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
					items = new T[(array.Length << 1) | 1];
					head = 0;
					_size = array.Length;
					Array.Copy(array, 0, items, 0, array.Length);
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
				return items[head];
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
				items = ToArray();
				head = 0;
				_size = items.Length;
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
					Array.ConstrainedCopy(items, head, array, index, Math.Min(array.Length - index, _size));
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
