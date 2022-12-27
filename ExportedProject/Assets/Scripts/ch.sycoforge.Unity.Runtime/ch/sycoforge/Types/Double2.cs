using System;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	[Serializable]
	[ComVisible(true)]
	public struct Double2
	{
		private double x;

		private double y;

		public double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public Double2(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
	}
}
