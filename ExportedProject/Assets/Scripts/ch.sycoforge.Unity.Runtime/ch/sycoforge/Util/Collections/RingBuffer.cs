using System;
using System.Collections.Generic;

namespace ch.sycoforge.Util.Collections
{
	public class RingBuffer<T>
	{
		private int capacity;

		private int ringIndex;

		private int version = 0;

		private T[] ringBuffer;

		public int Capacity
		{
			get
			{
				return capacity;
			}
			set
			{
				capacity = value;
			}
		}

		public int Count
		{
			get
			{
				return ringIndex;
			}
		}

		public bool IsReadOnly { get; set; }

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new IndexOutOfRangeException();
				}
				return ringBuffer[index];
			}
			set
			{
				Insert(index, value);
			}
		}

		public RingBuffer(int capacity)
		{
			this.capacity = capacity;
			if (capacity <= 0)
			{
				throw new ArgumentException("Initial capacity has to be greater than zero.", "capacity");
			}
			ringBuffer = new T[capacity];
		}

		public T First()
		{
			return ringBuffer[0];
		}

		public T Last()
		{
			return ringBuffer[(ringIndex - 1) % capacity];
		}

		public void Add(T item)
		{
			ringBuffer[ringIndex++] = item;
			ringIndex %= capacity;
			version++;
		}

		public void Clear()
		{
			for (int i = 0; i < capacity; i++)
			{
				ringBuffer[i] = default(T);
			}
			version++;
		}

		public bool Contains(T item)
		{
			for (int i = 0; i < capacity; i++)
			{
				if (ringBuffer[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		public void Insert(int index, T item)
		{
			if (index >= capacity)
			{
				throw new IndexOutOfRangeException();
			}
			ringBuffer[index] = item;
			version++;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < Count; i++)
			{
				array[i + arrayIndex] = ringBuffer[(ringIndex - Count + i) % Capacity];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			long tmpVersion = version;
			for (int i = 0; i < Count; i++)
			{
				if (tmpVersion != version)
				{
					throw new InvalidOperationException("Collection changed");
				}
				yield return this[i];
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = index; i < Count - 1; i++)
			{
				int num = (ringIndex - Count + i) % Capacity;
				int num2 = (ringIndex - Count + i + 1) % Capacity;
				ringBuffer[num] = ringBuffer[num2];
			}
			int num3 = (ringIndex - 1) % Capacity;
			ringBuffer[num3] = default(T);
			ringIndex--;
			version++;
		}
	}
}
