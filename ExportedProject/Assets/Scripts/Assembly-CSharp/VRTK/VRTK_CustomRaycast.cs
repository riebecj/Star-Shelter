using UnityEngine;

namespace VRTK
{
	public class VRTK_CustomRaycast : MonoBehaviour
	{
		[Tooltip("The layers to ignore when raycasting.")]
		public LayerMask layersToIgnore = 4;

		[Tooltip("Determines whether the ray will interact with trigger colliders.")]
		public QueryTriggerInteraction triggerInteraction;

		public static bool Raycast(VRTK_CustomRaycast customCast, Ray ray, out RaycastHit hitData, LayerMask ignoreLayers, float length = float.PositiveInfinity)
		{
			if (customCast != null)
			{
				return customCast.CustomRaycast(ray, out hitData, length);
			}
			return Physics.Raycast(ray, out hitData, length, ~(int)ignoreLayers);
		}

		public static bool Linecast(VRTK_CustomRaycast customCast, Vector3 startPosition, Vector3 endPosition, out RaycastHit hitData, LayerMask ignoreLayers)
		{
			if (customCast != null)
			{
				return customCast.CustomLinecast(startPosition, endPosition, out hitData);
			}
			return Physics.Linecast(startPosition, endPosition, out hitData);
		}

		public virtual bool CustomRaycast(Ray ray, out RaycastHit hitData, float length = float.PositiveInfinity)
		{
			return Physics.Raycast(ray, out hitData, length, ~(int)layersToIgnore, triggerInteraction);
		}

		public virtual bool CustomLinecast(Vector3 startPosition, Vector3 endPosition, out RaycastHit hitData)
		{
			return Physics.Linecast(startPosition, endPosition, out hitData, ~(int)layersToIgnore);
		}
	}
}
