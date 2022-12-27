using UnityEngine;

namespace VRTK
{
	[SDK_Description(typeof(SDK_OculusVRSystem))]
	public class SDK_OculusVRBoundaries : SDK_BaseBoundaries
	{
		public override void InitBoundaries()
		{
		}

		public override Transform GetPlayArea()
		{
			cachedPlayArea = GetSDKManagerPlayArea();
			if (cachedPlayArea == null)
			{
				OVRManager oVRManager = VRTK_SharedMethods.FindEvenInactiveComponent<OVRManager>();
				if ((bool)oVRManager)
				{
					cachedPlayArea = oVRManager.transform;
				}
			}
			return cachedPlayArea;
		}

		public override Vector3[] GetPlayAreaVertices(GameObject playArea)
		{
			OVRBoundary oVRBoundary = new OVRBoundary();
			if (oVRBoundary.GetConfigured())
			{
				Vector3 dimensions = oVRBoundary.GetDimensions(OVRBoundary.BoundaryType.OuterBoundary);
				float num = 0.1f;
				return new Vector3[8]
				{
					new Vector3(dimensions.x - num, 0f, dimensions.z - num),
					new Vector3(num, 0f, dimensions.z - num),
					new Vector3(num, 0f, num),
					new Vector3(dimensions.x - num, 0f, num),
					new Vector3(dimensions.x, 0f, dimensions.z),
					new Vector3(0f, 0f, dimensions.z),
					new Vector3(0f, 0f, 0f),
					new Vector3(dimensions.x, 0f, 0f)
				};
			}
			return null;
		}

		public override float GetPlayAreaBorderThickness(GameObject playArea)
		{
			return 0.1f;
		}

		public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
		{
			return true;
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
