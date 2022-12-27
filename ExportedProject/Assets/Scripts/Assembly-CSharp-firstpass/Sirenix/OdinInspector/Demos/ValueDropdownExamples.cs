using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ValueDropdownExamples : MonoBehaviour
	{
		public GameObject[] AvailablePrefabs;

		[ValueDropdown("AvailablePrefabs")]
		public GameObject ActivePrefab;

		[ValueDropdown("GetLayers")]
		public string Layer;

		[ValueDropdown("textureSizes")]
		public int TextureSize;

		[ValueDropdown("friendlyTextureSizes")]
		public int FriendlyTextureSize;

		private static int[] textureSizes = new int[3] { 256, 512, 1024 };

		private static ValueDropdownList<int> friendlyTextureSizes = new ValueDropdownList<int>
		{
			{ "Small", 256 },
			{ "Medium", 512 },
			{ "Large", 1024 }
		};

		[CompilerGenerated]
		private static Func<int, string> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<string, bool> _003C_003Ef__am_0024cache0;

		private static List<string> GetLayers()
		{
			IEnumerable<int> source = Enumerable.Range(0, 32);
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = LayerMask.LayerToName;
			}
			IEnumerable<string> source2 = source.Select(_003C_003Ef__mg_0024cache0);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CGetLayers_003Em__0;
			}
			return source2.Where(_003C_003Ef__am_0024cache0).ToList();
		}

		[CompilerGenerated]
		private static bool _003CGetLayers_003Em__0(string s)
		{
			return s.Length > 0;
		}
	}
}
