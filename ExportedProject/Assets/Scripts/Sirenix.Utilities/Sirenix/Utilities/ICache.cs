using System;

namespace Sirenix.Utilities
{
	public interface ICache<T> : IDisposable where T : class, new()
	{
		T Value { get; }

		bool IsFree { get; }

		void Release();
	}
}
