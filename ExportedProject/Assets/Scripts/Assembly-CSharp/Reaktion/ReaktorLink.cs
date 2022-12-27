using System;

namespace Reaktion
{
	[Serializable]
	public class ReaktorLink : GenericLink<Reaktor>
	{
		public float Output
		{
			get
			{
				return (!base.linkedObject) ? 0f : base.linkedObject.Output;
			}
		}
	}
}
