using System;
using System.Collections.Generic;

namespace ch.sycoforge.Util.Collections
{
	public static class ListExtensions
	{
		public static void Shuffle<T>(IList<T> list)
		{
			int num = list.Count;
			Random random = new Random();
			while (num > 1)
			{
				int index = random.Next(0, num);
				num--;
				T value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}
	}
}
