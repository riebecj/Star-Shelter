using System;
using UnityEngine;

namespace VRTK
{
	public class VRTK_HeightAdjustTeleport : VRTK_BasicTeleport
	{
		[Header("Height Adjust Settings")]
		[Tooltip("A custom raycaster to use when raycasting to find floors.")]
		public VRTK_CustomRaycast customRaycast;

		[Tooltip("**OBSOLETE [Use customRaycast]** The layers to ignore when raycasting to find floors.")]
		[Obsolete("`VRTK_HeightAdjustTeleport.layersToIgnore` is no longer used in the `VRTK_HeightAdjustTeleport` class, use the `customRaycast` parameter instead. This parameter will be removed in a future version of VRTK.")]
		public LayerMask layersToIgnore = 4;

		protected override void OnEnable()
		{
			base.OnEnable();
			adjustYForTerrain = true;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override Vector3 GetNewPosition(Vector3 tipPosition, Transform target, bool returnOriginalPosition)
		{
			Vector3 newPosition = base.GetNewPosition(tipPosition, target, returnOriginalPosition);
			if (!returnOriginalPosition)
			{
				newPosition.y = GetTeleportY(target, tipPosition);
			}
			return newPosition;
		}

		protected virtual float GetTeleportY(Transform target, Vector3 tipPosition)
		{
			float result = playArea.position.y;
			float num = 0.1f;
			Vector3 vector = Vector3.up * num;
			Ray ray = new Ray(tipPosition + vector, -playArea.up);
			RaycastHit hitData;
			if ((bool)target && VRTK_CustomRaycast.Raycast(customRaycast, ray, out hitData, layersToIgnore))
			{
				result = tipPosition.y - hitData.distance + num;
			}
			return result;
		}
	}
}
