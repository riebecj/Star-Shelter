using UnityEngine;
using UnityEngine.Rendering;

namespace VRTK
{
	public class VRTK_BezierPointerRenderer : VRTK_BasePointerRenderer
	{
		[Header("Bezier Pointer Appearance Settings")]
		[Tooltip("The maximum length of the projected beam. The x value is the length of the forward beam, the y value is the length of the downward beam.")]
		public Vector2 maximumLength = new Vector2(10f, float.PositiveInfinity);

		[Tooltip("The number of items to render in the bezier curve tracer beam. A high number here will most likely have a negative impact of game performance due to large number of rendered objects.")]
		public int tracerDensity = 10;

		[Tooltip("The size of the ground cursor. This number also affects the size of the objects in the bezier curve tracer beam. The larger the radius, the larger the objects will be.")]
		public float cursorRadius = 0.5f;

		[Header("Bezier Pointer Render Settings")]
		[Tooltip("The maximum angle in degrees of the origin before the beam curve height is restricted. A lower angle setting will prevent the beam being projected high into the sky and curving back down.")]
		[Range(1f, 100f)]
		public float heightLimitAngle = 100f;

		[Tooltip("The amount of height offset to apply to the projected beam to generate a smoother curve even when the beam is pointing straight.")]
		public float curveOffset = 1f;

		[Tooltip("Rescale each tracer element according to the length of the Bezier curve.")]
		public bool rescaleTracer;

		[Tooltip("The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
		public bool cursorMatchTargetRotation;

		[Tooltip("The number of points along the bezier curve to check for an early beam collision. Useful if the bezier curve is appearing to clip through teleport locations. 0 won't make any checks and it will be capped at `Pointer Density`. The higher the number, the more CPU intensive the checks become.")]
		public int collisionCheckFrequency;

		[Header("Bezier Pointer Custom Appearance Settings")]
		[Tooltip("A custom game object to use as the appearance for the pointer tracer. If this is empty then a collection of Sphere primitives will be created and used.")]
		public GameObject customTracer;

		[Tooltip("A custom game object to use as the appearance for the pointer cursor. If this is empty then a Cylinder primitive will be created and used.")]
		public GameObject customCursor;

		[Tooltip("A custom game object can be applied here to appear only if the location is valid.")]
		public GameObject validLocationObject;

		[Tooltip("A custom game object can be applied here to appear only if the location is invalid.")]
		public GameObject invalidLocationObject;

		protected VRTK_CurveGenerator actualTracer;

		protected GameObject actualContainer;

		protected GameObject actualCursor;

		protected GameObject actualValidLocationObject;

		protected GameObject actualInvalidLocationObject;

		protected Vector3 fixedForwardBeamForward;

		public override void UpdateRenderer()
		{
			if (((bool)controllingPointer && controllingPointer.IsPointerActive()) || IsVisible())
			{
				Vector3 jointPosition = ProjectForwardBeam();
				Vector3 downPosition = ProjectDownBeam(jointPosition);
				AdjustForEarlyCollisions(jointPosition, downPosition);
				MakeRenderersVisible();
			}
			base.UpdateRenderer();
		}

		protected override void ToggleRenderer(bool pointerState, bool actualState)
		{
			TogglePointerCursor(pointerState, actualState);
			TogglePointerTracer(pointerState, actualState);
			if (actualTracer != null && actualState && tracerVisibility != VisibilityStates.AlwaysOn)
			{
				ToggleRendererVisibility(actualTracer.gameObject, false);
				AddVisibleRenderer(actualTracer.gameObject);
			}
		}

		protected override void CreatePointerObjects()
		{
			actualContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "BezierPointerRenderer_Container"));
			VRTK_PlayerObject.SetPlayerObject(actualContainer, VRTK_PlayerObject.ObjectTypes.Pointer);
			actualContainer.SetActive(false);
			CreateTracer();
			CreateCursor();
			Toggle(false, false);
			if ((bool)controllingPointer)
			{
				controllingPointer.ResetActivationTimer(true);
				controllingPointer.ResetSelectionTimer(true);
			}
		}

		protected override void DestroyPointerObjects()
		{
			if (actualCursor != null)
			{
				Object.Destroy(actualCursor);
			}
			if (actualTracer != null)
			{
				Object.Destroy(actualTracer);
			}
			if (actualContainer != null)
			{
				Object.Destroy(actualContainer);
			}
		}

		protected override void UpdateObjectInteractor()
		{
			base.UpdateObjectInteractor();
			if ((bool)objectInteractor && (bool)actualCursor && Vector3.Distance(objectInteractor.transform.position, actualCursor.transform.position) > 0f)
			{
				objectInteractor.transform.position = actualCursor.transform.position;
			}
		}

		protected override void ChangeMaterial(Color givenColor)
		{
			base.ChangeMaterial(givenColor);
			ChangeMaterialColor(actualCursor, givenColor);
		}

		protected virtual void CreateTracer()
		{
			actualTracer = actualContainer.gameObject.AddComponent<VRTK_CurveGenerator>();
			actualTracer.transform.SetParent(null);
			actualTracer.Create(tracerDensity, cursorRadius, customTracer, rescaleTracer);
		}

		protected virtual GameObject CreateCursorObject()
		{
			float y = 0.02f;
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			gameObject.transform.localScale = new Vector3(cursorRadius, y, cursorRadius);
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.material = defaultMaterial;
			Object.Destroy(gameObject.GetComponent<CapsuleCollider>());
			return gameObject;
		}

		protected virtual void CreateCursorLocations()
		{
			if (validLocationObject != null)
			{
				actualValidLocationObject = Object.Instantiate(validLocationObject);
				actualValidLocationObject.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "BezierPointerRenderer_ValidLocation");
				actualValidLocationObject.transform.SetParent(actualCursor.transform);
				actualValidLocationObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				actualValidLocationObject.SetActive(false);
			}
			if (invalidLocationObject != null)
			{
				actualInvalidLocationObject = Object.Instantiate(invalidLocationObject);
				actualInvalidLocationObject.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "BezierPointerRenderer_InvalidLocation");
				actualInvalidLocationObject.transform.SetParent(actualCursor.transform);
				actualInvalidLocationObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				actualInvalidLocationObject.SetActive(false);
			}
		}

		protected virtual void CreateCursor()
		{
			actualCursor = ((!customCursor) ? CreateCursorObject() : Object.Instantiate(customCursor));
			CreateCursorLocations();
			actualCursor.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "BezierPointerRenderer_Cursor");
			VRTK_PlayerObject.SetPlayerObject(actualCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
			actualCursor.layer = LayerMask.NameToLayer("Ignore Raycast");
			actualCursor.SetActive(false);
		}

		protected virtual Vector3 ProjectForwardBeam()
		{
			Transform origin = GetOrigin();
			float num = Vector3.Dot(Vector3.up, origin.forward.normalized);
			float num2 = maximumLength.x;
			Vector3 direction = origin.forward;
			if (num * 100f > heightLimitAngle)
			{
				direction = new Vector3(direction.x, fixedForwardBeamForward.y, direction.z);
				float num3 = 1f - (num - heightLimitAngle / 100f);
				num2 = maximumLength.x * num3 * num3;
			}
			else
			{
				fixedForwardBeamForward = origin.forward;
			}
			float num4 = num2;
			Ray ray = new Ray(origin.position, direction);
			RaycastHit hitData;
			bool flag = VRTK_CustomRaycast.Raycast(customRaycast, ray, out hitData, layersToIgnore, num2);
			float num5 = 0f;
			if (!flag || ((bool)destinationHit.collider && destinationHit.collider != hitData.collider))
			{
				num5 = 0f;
			}
			if (flag)
			{
				num5 = hitData.distance;
			}
			if (flag && num5 < num2)
			{
				num4 = num5;
			}
			return ray.GetPoint(num4 - 0.0001f) + Vector3.up * 0.0001f;
		}

		protected virtual Vector3 ProjectDownBeam(Vector3 jointPosition)
		{
			Vector3 result = Vector3.zero;
			Ray ray = new Ray(jointPosition, Vector3.down);
			RaycastHit hitData;
			bool flag = VRTK_CustomRaycast.Raycast(customRaycast, ray, out hitData, layersToIgnore, maximumLength.y);
			if (!flag || ((bool)destinationHit.collider && destinationHit.collider != hitData.collider))
			{
				if (destinationHit.collider != null)
				{
					PointerExit(destinationHit);
				}
				destinationHit = default(RaycastHit);
				result = ray.GetPoint(0f);
			}
			if (flag)
			{
				result = ray.GetPoint(hitData.distance);
				PointerEnter(hitData);
				destinationHit = hitData;
			}
			return result;
		}

		protected virtual void AdjustForEarlyCollisions(Vector3 jointPosition, Vector3 downPosition)
		{
			Vector3 downPosition2 = downPosition;
			Vector3 jointPosition2 = jointPosition;
			if (collisionCheckFrequency > 0 && actualTracer != null)
			{
				collisionCheckFrequency = Mathf.Clamp(collisionCheckFrequency, 0, tracerDensity);
				Vector3[] controlPoints = new Vector3[4]
				{
					GetOrigin().position,
					jointPosition + new Vector3(0f, curveOffset, 0f),
					downPosition,
					downPosition
				};
				Vector3[] points = actualTracer.GetPoints(controlPoints);
				int num = tracerDensity / collisionCheckFrequency;
				for (int i = 0; i < tracerDensity - num; i += num)
				{
					Vector3 vector = points[i];
					Vector3 vector2 = ((i + num >= points.Length) ? points[points.Length - 1] : points[i + num]);
					Vector3 normalized = (vector2 - vector).normalized;
					float length = Vector3.Distance(vector, vector2);
					Ray ray = new Ray(vector, normalized);
					RaycastHit hitData;
					if (VRTK_CustomRaycast.Raycast(customRaycast, ray, out hitData, layersToIgnore, length))
					{
						Vector3 point = ray.GetPoint(hitData.distance);
						Ray ray2 = new Ray(point + Vector3.up * 0.01f, Vector3.down);
						RaycastHit hitData2;
						if (VRTK_CustomRaycast.Raycast(customRaycast, ray2, out hitData2, layersToIgnore))
						{
							destinationHit = hitData2;
							downPosition2 = ray2.GetPoint(hitData2.distance);
							jointPosition2 = ((!(downPosition2.y < jointPosition.y)) ? jointPosition : new Vector3(downPosition2.x, jointPosition.y, downPosition2.z));
							break;
						}
					}
				}
			}
			DisplayCurvedBeam(jointPosition2, downPosition2);
			SetPointerCursor();
		}

		protected virtual void DisplayCurvedBeam(Vector3 jointPosition, Vector3 downPosition)
		{
			if (actualTracer != null)
			{
				Vector3[] controlPoints = new Vector3[4]
				{
					GetOrigin(false).position,
					jointPosition + new Vector3(0f, curveOffset, 0f),
					downPosition,
					downPosition
				};
				Material material = ((!customTracer) ? defaultMaterial : null);
				actualTracer.SetPoints(controlPoints, material, currentColor);
				if (tracerVisibility == VisibilityStates.AlwaysOff)
				{
					TogglePointerTracer(false, false);
				}
				else if ((bool)controllingPointer)
				{
					TogglePointerTracer(controllingPointer.IsPointerActive(), controllingPointer.IsPointerActive());
				}
			}
		}

		protected virtual void TogglePointerCursor(bool pointerState, bool actualState)
		{
			ToggleElement(actualCursor, pointerState, actualState, cursorVisibility, ref cursorVisible);
		}

		protected virtual void TogglePointerTracer(bool pointerState, bool actualState)
		{
			tracerVisible = tracerVisibility == VisibilityStates.AlwaysOn || pointerState;
			if (actualTracer != null)
			{
				actualTracer.TogglePoints(tracerVisible);
			}
		}

		protected virtual void SetPointerCursor()
		{
			if ((bool)controllingPointer && (bool)destinationHit.transform)
			{
				TogglePointerCursor(controllingPointer.IsPointerActive(), controllingPointer.IsPointerActive());
				actualCursor.transform.position = destinationHit.point;
				if (cursorMatchTargetRotation)
				{
					actualCursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, destinationHit.normal);
				}
				base.UpdateDependencies(actualCursor.transform.position);
				ChangeColor(validCollisionColor);
				if ((bool)actualValidLocationObject)
				{
					actualValidLocationObject.SetActive(ValidDestination() && IsValidCollision());
				}
				if ((bool)actualInvalidLocationObject)
				{
					actualInvalidLocationObject.SetActive(!ValidDestination() || !IsValidCollision());
				}
			}
			else
			{
				TogglePointerCursor(false, false);
				ChangeColor(invalidCollisionColor);
			}
		}
	}
}
