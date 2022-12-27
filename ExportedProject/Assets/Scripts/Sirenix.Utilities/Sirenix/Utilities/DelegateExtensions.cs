using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sirenix.Utilities
{
	public static class DelegateExtensions
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass0_0<TResult>
		{
			public bool hasValue;

			public TResult value;

			public Func<TResult> getValue;

			internal TResult _003CMemoize_003Eb__0()
			{
				if (!hasValue)
				{
					hasValue = true;
					value = getValue();
				}
				return value;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1_0<T, TResult>
		{
			public Dictionary<T, TResult> dic;

			public Func<T, TResult> func;

			internal TResult _003CMemoize_003Eb__0(T n)
			{
				TResult value;
				if (!dic.TryGetValue(n, out value))
				{
					value = func(n);
					dic.Add(n, value);
				}
				return value;
			}
		}

		public static Func<TResult> Memoize<TResult>(this Func<TResult> getValue)
		{
			_003C_003Ec__DisplayClass0_0<TResult> _003C_003Ec__DisplayClass0_ = new _003C_003Ec__DisplayClass0_0<TResult>();
			_003C_003Ec__DisplayClass0_.getValue = getValue;
			_003C_003Ec__DisplayClass0_.value = default(TResult);
			_003C_003Ec__DisplayClass0_.hasValue = false;
			return _003C_003Ec__DisplayClass0_._003CMemoize_003Eb__0;
		}

		public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func)
		{
			_003C_003Ec__DisplayClass1_0<T, TResult> _003C_003Ec__DisplayClass1_ = new _003C_003Ec__DisplayClass1_0<T, TResult>();
			_003C_003Ec__DisplayClass1_.func = func;
			_003C_003Ec__DisplayClass1_.dic = new Dictionary<T, TResult>();
			return _003C_003Ec__DisplayClass1_._003CMemoize_003Eb__0;
		}
	}
}
