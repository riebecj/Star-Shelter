using UnityEngine;

namespace VRTK
{
	public abstract class SDK_BaseBoundaries : SDK_Base
	{
		protected Transform cachedPlayArea;

		public abstract void InitBoundaries();

		public abstract Transform GetPlayArea();

		public abstract Vector3[] GetPlayAreaVertices(GameObject playArea);

		public abstract float GetPlayAreaBorderThickness(GameObject playArea);

		public abstract bool IsPlayAreaSizeCalibrated(GameObject playArea);

		public abstract bool GetDrawAtRuntime();

		public abstract void SetDrawAtRuntime(bool value);

		protected Transform GetSDKManagerPlayArea()
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null && instance.actualBoundaries != null)
			{
				cachedPlayArea = ((!instance.actualBoundaries) ? null : instance.actualBoundaries.transform);
				return cachedPlayArea;
			}
			return null;
		}
	}
}
