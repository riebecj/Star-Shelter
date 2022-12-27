using System;
using System.Collections;
using System.Collections.Generic;

namespace ch.sycoforge.Types
{
	[Serializable]
	public class BufferStack<T> : IEnumerable<T>, ICollection, IEnumerable
	{
		private T[] items;

		private int top;

		private int count;

		public int Count
		{
			get
			{
				return count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return items.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return items.SyncRoot;
			}
		}

		public BufferStack(int capacity)
		{
			items = new T[capacity];
		}

		public void Push(T item)
		{
			count++;
			count = ((count > items.Length) ? items.Length : count);
			items[top] = item;
			top = (top + 1) % items.Length;
		}

		public T Pop()
		{
			count--;
			count = ((count >= 0) ? count : 0);
			top = (items.Length + top - 1) % items.Length;
			return items[top];
		}

		public T Peek()
		{
			return items[(items.Length + top - 1) % items.Length];
		}

		public T GetItem(int index)
		{
			if (index > count)
			{
				throw new InvalidOperationException("Index out of bounds");
			}
			return items[(items.Length + top - (index + 1)) % items.Length];
		}

		public void Clear()
		{
			count = 0;
		}

		public IEnumerator<T> GetEnumerator()
		{
			try
			{
				T[] array = items;
				for (int i = 0; i < array.Length; i++)
				{
					yield return array[i];
				}
			}
			finally
			{
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			Array.Copy(items, 0, array, index, items.Length);
		}
	}
}
