using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRTK
{
	[Obsolete("`VRTK_SimplePointer` has been replaced with `VRTK_StraightPointerRenderer` attached to a `VRTK_Pointer`. This script will be removed in a future version of VRTK.")]
	public class VRTK_SimplePointer : VRTK_BasePointer
	{
		[Header("Simple Pointer Settings", order = 3)]
		[Tooltip("The thickness and length of the beam can also be set on the script as well as the ability to toggle the sphere beam tip that is displayed at the end of the beam (to represent a cursor).")]
		public float pointerThickness = 0.002f;

		[Tooltip("The distance the beam will project before stopping.")]
		public float pointerLength = 100f;

		[Tooltip("Toggle whether the cursor is shown on the end of the pointer beam.")]
		public bool showPointerTip = true;

		[Header("Custom Appearance Settings", order = 4)]
		[Tooltip("A custom Game Object can be applied here to use instead of the default sphere for the pointer cursor.")]
		public GameObject customPointerCursor;

		[Tooltip("Rotate the pointer cursor to match the normal of the target surface (or the pointer direction if no target was hit).")]
		public bool pointerCursorMatchTargetNormal;

		[Tooltip("Rescale the pointer cursor proportionally to the distance from this game object (useful when used as a gaze pointer).")]
		public bool pointerCursorRescaledAlongDistance;

		private GameObject pointerHolder;

		private GameObject pointerBeam;

		private GameObject pointerTip;

		private Vector3 pointerTipScale = new Vector3(0.05f, 0.05f, 0.05f);

		private Vector3 pointerCursorOriginalScale = Vector3.one;

		private bool activeEnabled;

		private bool storedBeamState;

		private bool storedTipState;

		protected override void OnEnable()
		{
			base.OnEnable();
			InitPointer();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (pointerHolder != null)
			{
				UnityEngine.Object.Destroy(pointerHolder);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (!pointerBeam || !pointerBeam.activeSelf)
			{
				return;
			}
			Transform origin = GetOrigin();
			Ray ray = new Ray(origin.position, origin.forward);
			RaycastHit hitInfo;
			bool flag = Physics.Raycast(ray, out hitInfo, pointerLength, ~(int)layersToIgnore);
			float pointerBeamLength = GetPointerBeamLength(flag, hitInfo);
			SetPointerTransform(pointerBeamLength, pointerThickness);
			if (flag)
			{
				if (pointerCursorMatchTargetNormal)
				{
					pointerTip.transform.forward = -hitInfo.normal;
				}
				if (pointerCursorRescaledAlongDistance)
				{
					float num = Vector3.Distance(hitInfo.point, origin.position);
					pointerTip.transform.localScale = pointerCursorOriginalScale * num;
				}
			}
			else
			{
				if (pointerCursorMatchTargetNormal)
				{
					pointerTip.transform.forward = origin.forward;
				}
				if (pointerCursorRescaledAlongDistance)
				{
					pointerTip.transform.localScale = pointerCursorOriginalScale * pointerBeamLength;
				}
			}
			if (activeEnabled)
			{
				activeEnabled = false;
				pointerBeam.GetComponentInChildren<Renderer>().enabled = storedBeamState;
				pointerTip.GetComponentInChildren<Renderer>().enabled = storedTipState;
			}
		}

		protected override void UpdateObjectInteractor()
		{
			base.UpdateObjectInteractor();
			if (Vector3.Distance(objectInteractor.transform.position, pointerTip.transform.position) > 0f)
			{
				objectInteractor.transform.position = pointerTip.transform.position;
			}
		}

		protected override void InitPointer()
		{
			pointerHolder = new GameObject(string.Format("[{0}]BasePointer_SimplePointer_Holder", base.gameObject.name));
			pointerHolder.transform.localPosition = Vector3.zero;
			VRTK_PlayerObject.SetPlayerObject(pointerHolder, VRTK_PlayerObject.ObjectTypes.Pointer);
			pointerBeam = GameObject.CreatePrimitive(PrimitiveType.Cube);
			pointerBeam.transform.name = string.Format("[{0}]BasePointer_SimplePointer_Pointer", base.gameObject.name);
			pointerBeam.transform.SetParent(pointerHolder.transform);
			pointerBeam.GetComponent<BoxCollider>().isTrigger = true;
			pointerBeam.AddComponent<Rigidbody>().isKinematic = true;
			pointerBeam.layer = LayerMask.NameToLayer("Ignore Raycast");
			MeshRenderer component = pointerBeam.GetComponent<MeshRenderer>();
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.material = pointerMaterial;
			VRTK_PlayerObject.SetPlayerObject(pointerBeam, VRTK_PlayerObject.ObjectTypes.Pointer);
			if ((bool)customPointerCursor)
			{
				pointerTip = UnityEngine.Object.Instantiate(customPointerCursor);
			}
			else
			{
				pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				pointerTip.transform.localScale = pointerTipScale;
				MeshRenderer component2 = pointerTip.GetComponent<MeshRenderer>();
				component2.shadowCastingMode = ShadowCastingMode.Off;
				component2.receiveShadows = false;
				component2.material = pointerMaterial;
			}
			pointerCursorOriginalScale = pointerTip.transform.localScale;
			pointerTip.transform.name = string.Format("[{0}]BasePointer_SimplePointer_PointerTip", base.gameObject.name);
			pointerTip.transform.SetParent(pointerHolder.transform);
			pointerTip.GetComponent<Collider>().isTrigger = true;
			pointerTip.AddComponent<Rigidbody>().isKinematic = true;
			pointerTip.layer = LayerMask.NameToLayer("Ignore Raycast");
			VRTK_PlayerObject.SetPlayerObject(pointerTip, VRTK_PlayerObject.ObjectTypes.Pointer);
			base.InitPointer();
			ResizeObjectInteractor();
			SetPointerTransform(pointerLength, pointerThickness);
			TogglePointer(false);
		}

		protected override void SetPointerMaterial(Color color)
		{
			base.SetPointerMaterial(color);
			base.ChangeMaterialColor(pointerBeam, color);
			base.ChangeMaterialColor(pointerTip, color);
		}

		protected override void TogglePointer(bool state)
		{
			state = pointerVisibility == pointerVisibilityStates.Always_On || state;
			base.TogglePointer(state);
			if ((bool)pointerBeam)
			{
				pointerBeam.SetActive(state);
			}
			bool active = showPointerTip && state;
			if ((bool)pointerTip)
			{
				pointerTip.SetActive(active);
			}
			if ((bool)pointerBeam && (bool)pointerBeam.GetComponentInChildren<Renderer>() && pointerVisibility == pointerVisibilityStates.Always_Off)
			{
				pointerBeam.GetComponentInChildren<Renderer>().enabled = false;
			}
			activeEnabled = state;
			if (activeEnabled)
			{
				storedBeamState = pointerBeam.GetComponentInChildren<Renderer>().enabled;
				storedTipState = pointerTip.GetComponentInChildren<Renderer>().enabled;
				pointerBeam.GetComponentInChildren<Renderer>().enabled = false;
				pointerTip.GetComponentInChildren<Renderer>().enabled = false;
			}
			ResizeObjectInteractor();
		}

		private void ResizeObjectInteractor()
		{
			if (showPointerTip && (bool)pointerTip && (bool)objectInteractor)
			{
				objectInteractor.transform.localScale = pointerTip.transform.localScale * 1.05f;
			}
		}

		private void SetPointerTransform(float setLength, float setThicknes)
		{
			float z = setLength / 2.00001f;
			Transform origin = GetOrigin();
			pointerBeam.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
			pointerBeam.transform.localPosition = new Vector3(0f, 0f, z);
			pointerHolder.transform.position = GetOrigin(false).position;
			pointerTip.transform.position = origin.position + origin.forward * (setLength - pointerTip.transform.localScale.z / 2f);
			pointerHolder.transform.LookAt(pointerTip.transform);
			base.UpdateDependencies(pointerTip.transform.position);
		}

		private float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
		{
			float currentLength = pointerLength;
			if (!hasRayHit || ((bool)pointerContactRaycastHit.collider && pointerContactRaycastHit.collider != collidedWith.collider))
			{
				if (pointerContactRaycastHit.collider != null)
				{
					base.PointerOut();
				}
				pointerContactDistance = 0f;
				pointerContactTarget = null;
				pointerContactRaycastHit = default(RaycastHit);
				destinationPosition = Vector3.zero;
				UpdatePointerMaterial(pointerMissColor);
			}
			if (hasRayHit)
			{
				pointerContactDistance = collidedWith.distance;
				pointerContactTarget = collidedWith.transform;
				pointerContactRaycastHit = collidedWith;
				destinationPosition = pointerTip.transform.position;
				UpdatePointerMaterial(pointerHitColor);
				base.PointerIn();
			}
			if (hasRayHit && pointerContactDistance < pointerLength)
			{
				currentLength = pointerContactDistance;
			}
			return OverrideBeamLength(currentLength);
		}
	}
}
