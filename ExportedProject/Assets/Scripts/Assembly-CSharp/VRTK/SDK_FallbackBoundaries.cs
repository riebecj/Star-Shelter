using UnityEngine;

namespace VRTK
{
	public class SDK_FallbackBoundaries : SDK_BaseBoundaries
	{
		public override void InitBoundaries()
		{
		}

		public override Transform GetPlayArea()
		{
			return null;
		}

		public override Vector3[] GetPlayAreaVertices(GameObject playArea)
		{
			return null;
		}

		public override float GetPlayAreaBorderThickness(GameObject playArea)
		{
			return 0f;
		}

		public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
		{
			return false;
		}

		public override bool GetDrawAtRuntime()
		{
			return false;
		}

		public override void SetDrawAtRuntime(bool value)
		{
		}
	}
}
