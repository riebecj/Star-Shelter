using System.Collections.Generic;
using System.Threading;

namespace ch.sycoforge.Util.Collections
{
	public class SafeQueue<T>
	{
		private readonly Queue<T> queue;

		private readonly int maxSize;

		private int length = 0;

		private bool closing;

		public bool IsEmpty
		{
			get
			{
				return length == 0;
			}
		}

		public int MaxSize
		{
			get
			{
				return maxSize;
			}
		}

		public int Size
		{
			get
			{
				return queue.Count;
			}
		}

		public SafeQueue(int maxSize)
		{
			this.maxSize = maxSize;
			queue = new Queue<T>();
		}

		public void Enqueue(T item)
		{
			lock (queue)
			{
				while (queue.Count >= maxSize)
				{
					Monitor.Wait(queue);
				}
				queue.Enqueue(item);
				length++;
				if (queue.Count == 1)
				{
					Monitor.PulseAll(queue);
				}
			}
		}

		public T Dequeue()
		{
			lock (queue)
			{
				while (queue.Count == 0)
				{
					Monitor.Wait(queue);
				}
				T result = queue.Dequeue();
				length = ((length > 0) ? (length - 1) : 0);
				if (queue.Count == maxSize - 1)
				{
					Monitor.PulseAll(queue);
				}
				return result;
			}
		}

		public void Close()
		{
			lock (queue)
			{
				closing = true;
				Monitor.PulseAll(queue);
			}
		}

		public bool TryDequeue(out T value)
		{
			lock (queue)
			{
				while (queue.Count == 0)
				{
					if (closing)
					{
						value = default(T);
						return false;
					}
					Monitor.Wait(queue);
				}
				value = queue.Dequeue();
				length = ((length > 0) ? (length - 1) : 0);
				if (queue.Count == maxSize - 1)
				{
					Monitor.PulseAll(queue);
				}
				return true;
			}
		}
	}
}
