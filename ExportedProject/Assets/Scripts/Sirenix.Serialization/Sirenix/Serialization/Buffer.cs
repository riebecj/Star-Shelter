using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sirenix.Serialization
{
	public sealed class Buffer<T> : IDisposable
	{
		private static readonly object LOCK = new object();

		private static readonly List<Buffer<T>> FreeBuffers = new List<Buffer<T>>();

		private int count;

		private int byteSize;

		private T[] array;

		private bool isFree;

		public int Count
		{
			get
			{
				if (isFree)
				{
					throw new InvalidOperationException("Cannot access a buffer while it is freed.");
				}
				return count;
			}
		}

		public int ByteSize
		{
			get
			{
				if (isFree)
				{
					throw new InvalidOperationException("Cannot access a buffer while it is freed.");
				}
				return byteSize;
			}
		}

		public T[] Array
		{
			get
			{
				if (isFree)
				{
					throw new InvalidOperationException("Cannot access a buffer while it is freed.");
				}
				return array;
			}
		}

		public bool IsFree
		{
			get
			{
				return isFree;
			}
		}

		private Buffer(int count)
		{
			array = new T[count];
			this.count = count;
			byteSize = count * Marshal.SizeOf(typeof(T));
			isFree = false;
		}

		public static Buffer<T> Claim(int minimumCapacity)
		{
			if (minimumCapacity < 0)
			{
				throw new ArgumentException("Requested size of buffer must be larger than or equal to 0.");
			}
			if (minimumCapacity < 256)
			{
				minimumCapacity = 256;
			}
			Buffer<T> buffer = null;
			lock (LOCK)
			{
				if (FreeBuffers.Count > 0)
				{
					for (int i = 0; i < FreeBuffers.Count; i++)
					{
						if (FreeBuffers[i].count >= minimumCapacity)
						{
							buffer = FreeBuffers[i];
							buffer.isFree = false;
							FreeBuffers.RemoveAt(i);
							break;
						}
					}
				}
			}
			if (buffer == null)
			{
				buffer = new Buffer<T>(NextPowerOfTwo(minimumCapacity));
			}
			return buffer;
		}

		public static void Free(Buffer<T> buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.isFree)
			{
				return;
			}
			lock (LOCK)
			{
				if (!buffer.isFree)
				{
					buffer.isFree = true;
					FreeBuffers.Add(buffer);
				}
			}
		}

		public void Free()
		{
			Free(this);
		}

		public void Dispose()
		{
			Free(this);
		}

		private static int NextPowerOfTwo(int v)
		{
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;
			return v;
		}
	}
}
