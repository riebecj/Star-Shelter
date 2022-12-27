using System.IO;
using Sirenix.Utilities;

namespace Sirenix.Serialization
{
	internal sealed class CachedMemoryStream : ICacheNotificationReceiver
	{
		public const int INITIAL_CAPACITY = 1024;

		public const int MAX_CAPACITY = 32768;

		public MemoryStream MemoryStream { get; private set; }

		public CachedMemoryStream()
		{
			MemoryStream = new MemoryStream(1024);
		}

		public void OnFreed()
		{
			MemoryStream.SetLength(0L);
			MemoryStream.Position = 0L;
			if (MemoryStream.Capacity > 32768)
			{
				MemoryStream.Capacity = 32768;
			}
		}

		public void OnClaimed()
		{
			MemoryStream.SetLength(0L);
			MemoryStream.Position = 0L;
		}
	}
}
