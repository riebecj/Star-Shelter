using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Plugins.Isolationist
{
	public class IsolateInfo : MonoBehaviour
	{
		private static IsolateInfo _instance;

		private static bool _searched;

		public List<GameObject> FocusObjects;

		public List<GameObject> HiddenObjects;

		[CompilerGenerated]
		private static Func<GameObject, bool> _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static Func<GameObject, bool> _003C_003Ef__am_0024cache2;

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__am_0024cache3;

		public static IsolateInfo Instance
		{
			get
			{
				return _instance ? _instance : (_instance = UnityEngine.Object.FindObjectOfType<IsolateInfo>());
			}
			set
			{
				_instance = value;
			}
		}

		public static bool IsIsolated
		{
			get
			{
				if (!_searched)
				{
					_instance = UnityEngine.Object.FindObjectOfType<IsolateInfo>();
					_searched = true;
				}
				return _instance;
			}
		}

		public static void Hide()
		{
			if ((bool)Instance && Instance.HiddenObjects != null)
			{
				List<GameObject> hiddenObjects = Instance.HiddenObjects;
				if (_003C_003Ef__am_0024cache0 == null)
				{
					_003C_003Ef__am_0024cache0 = _003CHide_003Em__0;
				}
				List<GameObject> list = hiddenObjects.Where(_003C_003Ef__am_0024cache0).ToList();
				if (_003C_003Ef__am_0024cache1 == null)
				{
					_003C_003Ef__am_0024cache1 = _003CHide_003Em__1;
				}
				list.ForEach(_003C_003Ef__am_0024cache1);
			}
		}

		public static void Show()
		{
			if ((bool)Instance && Instance.HiddenObjects != null)
			{
				List<GameObject> hiddenObjects = Instance.HiddenObjects;
				if (_003C_003Ef__am_0024cache2 == null)
				{
					_003C_003Ef__am_0024cache2 = _003CShow_003Em__2;
				}
				List<GameObject> list = hiddenObjects.Where(_003C_003Ef__am_0024cache2).ToList();
				if (_003C_003Ef__am_0024cache3 == null)
				{
					_003C_003Ef__am_0024cache3 = _003CShow_003Em__3;
				}
				list.ForEach(_003C_003Ef__am_0024cache3);
			}
		}

		private void Awake()
		{
			Show();
		}

		[CompilerGenerated]
		private static bool _003CHide_003Em__0(GameObject go)
		{
			return go;
		}

		[CompilerGenerated]
		private static void _003CHide_003Em__1(GameObject go)
		{
			go.SetActive(false);
		}

		[CompilerGenerated]
		private static bool _003CShow_003Em__2(GameObject go)
		{
			return go;
		}

		[CompilerGenerated]
		private static void _003CShow_003Em__3(GameObject go)
		{
			go.SetActive(true);
		}
	}
}
