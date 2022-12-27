using System;
using System.Collections.Generic;

namespace ch.sycoforge.Decal
{
	public class EnumHelper
	{
		public static List<T> GetAllEnums<T>() where T : struct
		{
			if (typeof(T).BaseType != typeof(Enum))
			{
				throw new ArgumentException("T must be an Enum type");
			}
			T[] array = Enum.GetValues(typeof(T)) as T[];
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (int)(object)array[i];
			}
			List<T> list = new List<T>();
			int num = 0;
			for (int i = 0; i < array2.Length; i++)
			{
				num |= array2[i];
			}
			for (int i = 0; i <= num; i++)
			{
				int num2 = i;
				for (int j = 0; j < array2.Length; j++)
				{
					num2 &= ~array2[j];
					if (num2 == 0)
					{
						list.Add((T)(object)i);
						break;
					}
				}
			}
			try
			{
				if (string.IsNullOrEmpty(Enum.GetName(typeof(T), (T)(object)0)))
				{
					list.Remove((T)(object)0);
				}
			}
			catch
			{
				list.Remove((T)(object)0);
			}
			return list;
		}
	}
}
