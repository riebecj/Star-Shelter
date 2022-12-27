using System;
using UnityEngine;

namespace ch.sycoforge.Types
{
	[Serializable]
	public struct IntThickness
	{
		public int bottom;

		public int left;

		public int right;

		public int top;

		public int horizontal
		{
			get
			{
				return left + right;
			}
		}

		public int vertical
		{
			get
			{
				return top + bottom;
			}
		}

		public IntThickness(int left, int right, int top, int bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		public override string ToString()
		{
			return string.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", left, right, top, bottom);
		}

		public static implicit operator RectOffset(IntThickness margin)
		{
			return new RectOffset(margin.left, margin.right, margin.top, margin.bottom);
		}

		public static implicit operator IntThickness(RectOffset margin)
		{
			return new IntThickness(margin.left, margin.right, margin.top, margin.bottom);
		}
	}
}
