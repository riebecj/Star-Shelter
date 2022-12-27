using UnityEngine;

namespace VRTK.Examples.Archery
{
	public class Follow : MonoBehaviour
	{
		public bool followPosition;

		public bool followRotation;

		public Transform target;

		private void Update()
		{
			if (target != null)
			{
				if (followRotation)
				{
					base.transform.rotation = target.rotation;
				}
				if (followPosition)
				{
					base.transform.position = target.position;
				}
			}
			else
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.NOT_DEFINED, "target"));
			}
		}
	}
}
