using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRTK
{
	[Obsolete("`VRTK_BezierPointer` has been replaced with `VRTK_BezierPointerRenderer` attached to a `VRTK_Pointer`. This script will be removed in a future version of VRTK.")]
	public class VRTK_BezierPointer : VRTK_BasePointer
	{
		[Header("Bezier Pointer Settings", order = 3)]
		[Tooltip("The length of the projected forward pointer beam, this is basically the distance able to point from the origin position.")]
		public float pointerLength = 10f;

		[Tooltip("The number of items to render in the beam bezier curve. A high number here will most likely have a negative impact of game performance due to large number of rendered objects.")]
		public int pointerDensity = 10;

		[Tooltip("The number of points along the bezier curve to check for an early beam collision. Useful if the bezier curve is appearing to clip through teleport locations. 0 won't make any checks and it will be capped at `Pointer Density`. The higher the number, the more CPU intensive the checks become.")]
		public int collisionCheckFrequency;

		[Tooltip("The amount of height offset to apply to the projected beam to generate a smoother curve even when the beam is pointing straight.")]
		public float beamCurveOffset = 1f;

		[Tooltip("The maximum angle in degrees of the origin before the beam curve height is restricted. A lower angle setting will prevent the beam being projected high into the sky and curving back down.")]
		[Range(1f, 100f)]
		public float beamHeightLimitAngle = 100f;

		[Tooltip("Rescale each pointer tracer element according to the length of the Bezier curve.")]
		public bool rescalePointerTracer;

		[Tooltip("A cursor is displayed on the ground at the location the beam ends at, it is useful to see what height the beam end location is, however it can be turned off by toggling this.")]
		public bool showPointerCursor = true;

		[Tooltip("The size of the ground pointer cursor. This number also affects the size of the objects in the bezier curve beam. The larger the radius, the larger the objects will be.")]
		public float pointerCursorRadius = 0.5f;

		[Tooltip("The pointer cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
		public bool pointerCursorMatchTargetRotation;

		[Header("Custom Appearance Settings", order = 4)]
		[Tooltip("A custom Game Object can be applied here to use instead of the default sphere for the beam tracer. The custom Game Object will match the rotation of the object attached to.")]
		public GameObject customPointerTracer;

		[Tooltip("A custom Game Object can be applied here to use instead of the default flat cylinder for the pointer cursor.")]
		public GameObject customPointerCursor;

		[Tooltip("A custom Game Object can be applied here to appear only if the teleport is allowed (its material will not be changed ).")]
		public GameObject validTeleportLocationObject;

		private GameObject pointerCursor;

		private GameObject curvedBeamContainer;

		private VRTK_CurveGenerator curvedBeam;

		private GameObject validTeleportLocationInstance;

		private bool beamActive;

		private Vector3 fixedForwardBeamForward;

		private Vector3 contactNormal;

		private const float BEAM_ADJUST_OFFSET = 1E-05f;

		protected override void OnEnable()
		{
			base.OnEnable();
			beamActive = false;
			InitPointer();
			TogglePointer(false);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			beamActive = false;
			if (pointerCursor != null)
			{
				UnityEngine.Object.Destroy(pointerCursor);
			}
			if (curvedBeam != null)
			{
				UnityEngine.Object.Destroy(curvedBeam);
			}
			if (curvedBeamContainer != null)
			{
				UnityEngine.Object.Destroy(curvedBeamContainer);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (beamActive)
			{
				Vector3 jointPosition = ProjectForwardBeam();
				Vector3 downPosition = ProjectDownBeam(jointPosition);
				AdjustForEarlyCollisions(jointPosition, downPosition);
			}
		}

		protected override void InitPointer()
		{
			pointerCursor = ((!customPointerCursor) ? CreateCursor() : UnityEngine.Object.Instantiate(customPointerCursor));
			if (validTeleportLocationObject != null)
			{
				validTeleportLocationInstance = UnityEngine.Object.Instantiate(validTeleportLocationObject);
				validTeleportLocationInstance.name = string.Format("[{0}]BasePointer_BezierPointer_TeleportBeam", base.gameObject.name);
				validTeleportLocationInstance.transform.SetParent(pointerCursor.transform);
				validTeleportLocationInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
				validTeleportLocationInstance.SetActive(false);
			}
			pointerCursor.name = string.Format("[{0}]BasePointer_BezierPointer_PointerCursor", base.gameObject.name);
			VRTK_PlayerObject.SetPlayerObject(pointerCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
			pointerCursor.layer = LayerMask.NameToLayer("Ignore Raycast");
			pointerCursor.SetActive(false);
			curvedBeamContainer = new GameObject(string.Format("[{0}]BasePointer_BezierPointer_CurvedBeamContainer", base.gameObject.name));
			VRTK_PlayerObject.SetPlayerObject(curvedBeamContainer, VRTK_PlayerObject.ObjectTypes.Pointer);
			curvedBeamContainer.SetActive(false);
			curvedBeam = curvedBeamContainer.gameObject.AddComponent<VRTK_CurveGenerator>();
			curvedBeam.transform.SetParent(null);
			curvedBeam.Create(pointerDensity, pointerCursorRadius, customPointerTracer, rescalePointerTracer);
			base.InitPointer();
		}

		protected override void SetPointerMaterial(Color color)
		{
			base.ChangeMaterialColor(pointerCursor, color);
			base.SetPointerMaterial(color);
		}

		protected override void TogglePointer(bool state)
		{
			state = pointerVisibility == pointerVisibilityStates.Always_On || state;
			beamActive = state;
			if (!beamActive)
			{
				ToggleBezierBeam(beamActive);
			}
		}

		protected override void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
		{
			base.DisablePointerBeam(sender, e);
			ToggleBezierBeam(false);
		}

		private void ToggleBezierBeam(bool state)
		{
			if (base.gameObject.activeInHierarchy)
			{
				TogglePointerCursor(state);
				curvedBeam.TogglePoints(state);
			}
		}

		private GameObject CreateCursor()
		{
			float y = 0.02f;
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			gameObject.transform.localScale = new Vector3(pointerCursorRadius, y, pointerCursorRadius);
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.material = pointerMaterial;
			UnityEngine.Object.Destroy(gameObject.GetComponent<CapsuleCollider>());
			return gameObject;
		}

		private void TogglePointerCursor(bool state)
		{
			bool active = showPointerCursor && state && showPointerCursor;
			pointerCursor.gameObject.SetActive(active);
			base.TogglePointer(state);
		}

		private Vector3 ProjectForwardBeam()
		{
			Transform origin = GetOrigin();
			float num = Vector3.Dot(Vector3.up, origin.forward.normalized);
			float num2 = pointerLength;
			Vector3 direction = origin.forward;
			if (num * 100f > beamHeightLimitAngle)
			{
				direction = new Vector3(direction.x, fixedForwardBeamForward.y, direction.z);
				float num3 = 1f - (num - beamHeightLimitAngle / 100f);
				num2 = pointerLength * num3 * num3;
			}
			else
			{
				fixedForwardBeamForward = origin.forward;
			}
			float num4 = num2;
			Ray ray = new Ray(origin.position, direction);
			RaycastHit hitInfo;
			bool flag = Physics.Raycast(ray, out hitInfo, num2, ~(int)layersToIgnore);
			if (!flag || ((bool)pointerContactRaycastHit.collider && pointerContactRaycastHit.collider != hitInfo.collider))
			{
				pointerContactDistance = 0f;
			}
			if (flag)
			{
				pointerContactDistance = hitInfo.distance;
			}
			if (flag && pointerContactDistance < num2)
			{
				num4 = pointerContactDistance;
			}
			return ray.GetPoint(num4 - 1E-05f) + Vector3.up * 1E-05f;
		}

		private Vector3 ProjectDownBeam(Vector3 jointPosition)
		{
			Vector3 result = Vector3.zero;
			Ray ray = new Ray(jointPosition, Vector3.down);
			RaycastHit hitInfo;
			bool flag = Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, ~(int)layersToIgnore);
			if (!flag || ((bool)pointerContactRaycastHit.collider && pointerContactRaycastHit.collider != hitInfo.collider))
			{
				if (pointerContactRaycastHit.collider != null)
				{
					base.PointerOut();
				}
				pointerContactTarget = null;
				pointerContactRaycastHit = default(RaycastHit);
				contactNormal = Vector3.zero;
				result = ray.GetPoint(0f);
			}
			if (flag)
			{
				pointerContactTarget = hitInfo.transform;
				pointerContactRaycastHit = hitInfo;
				contactNormal = hitInfo.normal;
				result = ray.GetPoint(hitInfo.distance);
				base.PointerIn();
			}
			return result;
		}

		private void SetPointerCursor(Vector3 cursorPosition)
		{
			destinationPosition = cursorPosition;
			if (pointerContactTarget != null)
			{
				TogglePointerCursor(true);
				pointerCursor.transform.position = cursorPosition;
				if (pointerCursorMatchTargetRotation)
				{
					pointerCursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, contactNormal);
				}
				base.UpdateDependencies(pointerCursor.transform.position);
				UpdatePointerMaterial(pointerHitColor);
				if (validTeleportLocationInstance != null)
				{
					validTeleportLocationInstance.SetActive(ValidDestination(pointerContactTarget, destinationPosition));
				}
			}
			else
			{
				TogglePointerCursor(false);
				UpdatePointerMaterial(pointerMissColor);
			}
		}

		private void AdjustForEarlyCollisions(Vector3 jointPosition, Vector3 downPosition)
		{
			Vector3 downPosition2 = downPosition;
			Vector3 jointPosition2 = jointPosition;
			if (collisionCheckFrequency > 0)
			{
				collisionCheckFrequency = Mathf.Clamp(collisionCheckFrequency, 0, pointerDensity);
				Vector3[] controlPoints = new Vector3[4]
				{
					GetOrigin().position,
					jointPosition + new Vector3(0f, beamCurveOffset, 0f),
					downPosition,
					downPosition
				};
				Vector3[] points = curvedBeam.GetPoints(controlPoints);
				int num = pointerDensity / collisionCheckFrequency;
				for (int i = 0; i < pointerDensity - num; i += num)
				{
					Vector3 vector = points[i];
					Vector3 vector2 = ((i + num >= points.Length) ? points[points.Length - 1] : points[i + num]);
					Vector3 normalized = (vector2 - vector).normalized;
					float maxDistance = Vector3.Distance(vector, vector2);
					Ray ray = new Ray(vector, normalized);
					RaycastHit hitInfo;
					if (Physics.Raycast(ray, out hitInfo, maxDistance, ~(int)layersToIgnore))
					{
						Vector3 point = ray.GetPoint(hitInfo.distance);
						Ray ray2 = new Ray(point + Vector3.up * 0.01f, Vector3.down);
						RaycastHit hitInfo2;
						if (Physics.Raycast(ray2, out hitInfo2, float.PositiveInfinity, ~(int)layersToIgnore))
						{
							downPosition2 = ray2.GetPoint(hitInfo2.distance);
							jointPosition2 = ((!(downPosition2.y < jointPosition.y)) ? jointPosition : new Vector3(downPosition2.x, jointPosition.y, downPosition2.z));
							break;
						}
					}
				}
			}
			DisplayCurvedBeam(jointPosition2, downPosition2);
			SetPointerCursor(downPosition2);
		}

		private void DisplayCurvedBeam(Vector3 jointPosition, Vector3 downPosition)
		{
			Vector3[] controlPoints = new Vector3[4]
			{
				GetOrigin(false).position,
				jointPosition + new Vector3(0f, beamCurveOffset, 0f),
				downPosition,
				downPosition
			};
			Material material = ((!customPointerTracer) ? pointerMaterial : null);
			curvedBeam.SetPoints(controlPoints, material, currentPointerColor);
			if (pointerVisibility != pointerVisibilityStates.Always_Off)
			{
				curvedBeam.TogglePoints(true);
			}
		}
	}
}
