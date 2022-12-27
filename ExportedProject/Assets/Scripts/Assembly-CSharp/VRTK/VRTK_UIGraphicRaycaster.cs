using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRTK
{
	public class VRTK_UIGraphicRaycaster : GraphicRaycaster
	{
		protected Canvas currentCanvas;

		protected Vector2 lastKnownPosition;

		protected const float UI_CONTROL_OFFSET = 1E-05f;

		[NonSerialized]
		private static List<RaycastResult> s_RaycastResults = new List<RaycastResult>();

		[CompilerGenerated]
		private static Comparison<RaycastResult> _003C_003Ef__am_0024cache0;

		protected virtual Canvas canvas
		{
			get
			{
				if (currentCanvas != null)
				{
					return currentCanvas;
				}
				currentCanvas = base.gameObject.GetComponent<Canvas>();
				return currentCanvas;
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (!(canvas == null))
			{
				Ray ray = new Ray(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal);
				Raycast(canvas, eventCamera, ray, ref s_RaycastResults);
				SetNearestRaycast(ref eventData, ref resultAppendList, ref s_RaycastResults);
				s_RaycastResults.Clear();
			}
		}

		protected virtual void SetNearestRaycast(ref PointerEventData eventData, ref List<RaycastResult> resultAppendList, ref List<RaycastResult> raycastResults)
		{
			RaycastResult? raycastResult = null;
			for (int i = 0; i < raycastResults.Count; i++)
			{
				RaycastResult raycastResult2 = raycastResults[i];
				raycastResult2.index = resultAppendList.Count;
				if (!raycastResult.HasValue || raycastResult2.distance < raycastResult.Value.distance)
				{
					raycastResult = raycastResult2;
				}
				resultAppendList.Add(raycastResult2);
			}
			if (raycastResult.HasValue)
			{
				eventData.position = raycastResult.Value.screenPosition;
				eventData.delta = eventData.position - lastKnownPosition;
				lastKnownPosition = eventData.position;
				eventData.pointerCurrentRaycast = raycastResult.Value;
			}
		}

		protected virtual float GetHitDistance(Ray ray)
		{
			float result = float.MaxValue;
			if (canvas.renderMode != 0 && base.blockingObjects != 0)
			{
				float num = Vector3.Distance(ray.origin, canvas.transform.position);
				if (base.blockingObjects == BlockingObjects.ThreeD || base.blockingObjects == BlockingObjects.All)
				{
					RaycastHit hitInfo;
					Physics.Raycast(ray, out hitInfo, num);
					if ((bool)hitInfo.collider && !VRTK_PlayerObject.IsPlayerObject(hitInfo.collider.gameObject))
					{
						result = hitInfo.distance;
					}
				}
				if (base.blockingObjects == BlockingObjects.TwoD || base.blockingObjects == BlockingObjects.All)
				{
					RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, num);
					if (raycastHit2D.collider != null && !VRTK_PlayerObject.IsPlayerObject(raycastHit2D.collider.gameObject))
					{
						result = raycastHit2D.fraction * num;
					}
				}
			}
			return result;
		}

		protected virtual void Raycast(Canvas canvas, Camera eventCamera, Ray ray, ref List<RaycastResult> results)
		{
			float hitDistance = GetHitDistance(ray);
			IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
			for (int i = 0; i < graphicsForCanvas.Count; i++)
			{
				Graphic graphic = graphicsForCanvas[i];
				if (graphic.depth == -1 || !graphic.raycastTarget)
				{
					continue;
				}
				Transform transform = graphic.transform;
				Vector3 forward = transform.forward;
				float num = Vector3.Dot(forward, transform.position - ray.origin) / Vector3.Dot(forward, ray.direction);
				if (!(num < 0f) && !(num - 1E-05f > hitDistance))
				{
					Vector3 point = ray.GetPoint(num);
					Vector2 vector = eventCamera.WorldToScreenPoint(point);
					if (RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, vector, eventCamera) && graphic.Raycast(vector, eventCamera))
					{
						RaycastResult raycastResult = default(RaycastResult);
						raycastResult.gameObject = graphic.gameObject;
						raycastResult.module = this;
						raycastResult.distance = num;
						raycastResult.screenPosition = vector;
						raycastResult.worldPosition = point;
						raycastResult.depth = graphic.depth;
						raycastResult.sortingLayer = canvas.sortingLayerID;
						raycastResult.sortingOrder = canvas.sortingOrder;
						RaycastResult item = raycastResult;
						results.Add(item);
					}
				}
			}
			List<RaycastResult> obj = results;
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CRaycast_003Em__0;
			}
			obj.Sort(_003C_003Ef__am_0024cache0);
		}

		[CompilerGenerated]
		private static int _003CRaycast_003Em__0(RaycastResult g1, RaycastResult g2)
		{
			return g2.depth.CompareTo(g1.depth);
		}
	}
}
