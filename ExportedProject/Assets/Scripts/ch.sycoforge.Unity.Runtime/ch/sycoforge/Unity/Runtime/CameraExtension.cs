using UnityEngine;

namespace ch.sycoforge.Unity.Runtime
{
	public static class CameraExtension
	{
		public static Camera Main
		{
			get
			{
				Camera main = Camera.main;
				if (main == null)
				{
					Debug.LogError("No main camera found in scene.");
				}
				return main;
			}
		}
	}
}
