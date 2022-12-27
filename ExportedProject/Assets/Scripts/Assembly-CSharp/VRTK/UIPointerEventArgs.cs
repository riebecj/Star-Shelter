using UnityEngine;
using UnityEngine.EventSystems;

namespace VRTK
{
	public struct UIPointerEventArgs
	{
		public uint controllerIndex;

		public bool isActive;

		public GameObject currentTarget;

		public GameObject previousTarget;

		public RaycastResult raycastResult;
	}
}
