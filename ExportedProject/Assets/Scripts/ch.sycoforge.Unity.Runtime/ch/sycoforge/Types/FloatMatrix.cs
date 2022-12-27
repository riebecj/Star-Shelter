using System;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	[Serializable]
	public struct FloatMatrix
	{
		public float[] values;

		[MarshalAs(UnmanagedType.I4)]
		public int width;

		[MarshalAs(UnmanagedType.I4)]
		public int height;

		public int Size
		{
			get
			{
				return width * height;
			}
		}

		public FloatMatrix(int width, int height)
		{
			this.width = width;
			this.height = height;
			values = new float[width * height];
		}

		public FloatMatrix(float[,] matrix)
		{
			width = matrix.GetLength(0);
			height = matrix.GetLength(1);
			values = new float[width * height];
			Initialize(matrix);
		}

		public float GetValue(int x, int y)
		{
			int num = FloatMath.Clamp(y * width + x, 0, Size - 1);
			return values[num];
		}

		public void SetValue(int x, int y, float value)
		{
			int num = FloatMath.Clamp(y * width + x, 0, Size - 1);
			values[num] = value;
		}

		private void Initialize(float[,] matrix)
		{
			int num = 0;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					values[num++] = matrix[i, j];
				}
			}
		}
	}
}
