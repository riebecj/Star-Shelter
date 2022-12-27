using System;
using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public sealed class CoroutineFormatter : IFormatter<Coroutine>, IFormatter
	{
		public Type SerializedType
		{
			get
			{
				return typeof(Coroutine);
			}
		}

		object IFormatter.Deserialize(IDataReader reader)
		{
			return null;
		}

		public Coroutine Deserialize(IDataReader reader)
		{
			return null;
		}

		public void Serialize(object value, IDataWriter writer)
		{
		}

		public void Serialize(Coroutine value, IDataWriter writer)
		{
		}
	}
}
