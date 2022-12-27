using System;

namespace Reaktion
{
	[Serializable]
	public class InjectorLink : GenericLink<InjectorBase>
	{
		public float DbLevel
		{
			get
			{
				return (!base.linkedObject) ? (-1E+12f) : base.linkedObject.DbLevel;
			}
		}
	}
}
