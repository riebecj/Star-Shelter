using System;
using System.Collections.Generic;

namespace Sirenix.Utilities
{
	public sealed class Cache<T> : ICache<T>, IDisposable where T : class, new()
	{
		private static readonly object LOCK = new object();

		private static readonly bool IsNotificationReceiver = typeof(ICacheNotificationReceiver).IsAssignableFrom(typeof(T));

		private static readonly Stack<Cache<T>> FreeValues = new Stack<Cache<T>>();

		private T value;

		private bool isFree;

		private static int maxCacheSize = 5;

		public static int MaxCacheSize
		{
			get
			{
				return maxCacheSize;
			}
			set
			{
				maxCacheSize = Math.Max(1, value);
			}
		}

		public T Value
		{
			get
			{
				if (isFree)
				{
					throw new InvalidOperationException("Cannot access a cache while it is freed.");
				}
				return value;
			}
		}

		bool ICache<T>.IsFree
		{
			get
			{
				return isFree;
			}
		}

		private Cache()
		{
			value = new T();
			isFree = false;
		}

		public static Cache<T> Claim()
		{
			Cache<T> cache = null;
			lock (LOCK)
			{
				if (FreeValues.Count > 0)
				{
					cache = FreeValues.Pop();
					cache.isFree = false;
				}
			}
			if (cache == null)
			{
				cache = new Cache<T>();
			}
			if (IsNotificationReceiver)
			{
				(cache.value as ICacheNotificationReceiver).OnClaimed();
			}
			return cache;
		}

		public static void Release(Cache<T> cache)
		{
			if (cache == null)
			{
				throw new ArgumentNullException("cache");
			}
			if (cache.isFree)
			{
				return;
			}
			bool flag = false;
			lock (LOCK)
			{
				if (!cache.isFree)
				{
					flag = true;
					cache.isFree = true;
					if (FreeValues.Count < MaxCacheSize)
					{
						FreeValues.Push(cache);
					}
				}
			}
			if (flag && IsNotificationReceiver)
			{
				(cache.value as ICacheNotificationReceiver).OnFreed();
			}
		}

		public static implicit operator T(Cache<T> cache)
		{
			if (cache == null)
			{
				return null;
			}
			return cache.Value;
		}

		void ICache<T>.Release()
		{
			Release(this);
		}

		void IDisposable.Dispose()
		{
			Release(this);
		}
	}
}
