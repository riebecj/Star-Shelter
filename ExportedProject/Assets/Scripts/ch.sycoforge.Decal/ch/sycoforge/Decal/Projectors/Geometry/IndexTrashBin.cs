using System.Collections.Generic;

namespace ch.sycoforge.Decal.Projectors.Geometry
{
	internal sealed class IndexTrashBin
	{
		private const int INVALID = -1;

		private List<int> indices = new List<int>();

		private int largestValidIndex = -1;

		internal void TrashIndex(int index)
		{
			int count = indices.Count;
			if (index < count)
			{
				largestValidIndex = -1;
				indices[index] = -1;
				for (int i = index + 1; i < count; i++)
				{
					int num = indices[i] - 1;
					if (num >= 0)
					{
						largestValidIndex = num;
					}
					indices[i] = num;
				}
			}
			else
			{
				int num2 = largestValidIndex + 1;
				for (int num = count; num < index; num++)
				{
					indices.Add(largestValidIndex = num2++);
				}
				indices.Add(-1);
			}
		}

		internal int ClampIndex(int index)
		{
			if (index >= 0 && index < indices.Count)
			{
				return indices[index];
			}
			return largestValidIndex + 1 + index - indices.Count;
		}

		internal void ClearBin()
		{
			indices.Clear();
			largestValidIndex = -1;
		}

		internal bool Contains(int index)
		{
			if (index < indices.Count)
			{
				return indices[index] < 0;
			}
			return false;
		}
	}
}
