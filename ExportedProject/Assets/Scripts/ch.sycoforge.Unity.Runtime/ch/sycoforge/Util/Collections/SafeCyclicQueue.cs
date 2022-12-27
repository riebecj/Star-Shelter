namespace ch.sycoforge.Util.Collections
{
	public class SafeCyclicQueue<T> : ConcurrentQueue<T>
	{
		private int MaxSize = 10;

		public SafeCyclicQueue(int maxSize)
			: base(maxSize)
		{
			MaxSize = maxSize;
		}

		public new void Enqueue(T obj)
		{
			lock (this)
			{
				while (base.Count > MaxSize - 1)
				{
					T val = Dequeue();
				}
			}
			base.Enqueue(obj);
		}
	}
}
