using System.Collections;
using UnityEngine;

namespace VRTK
{
	public class VRTK_StraightPointerRenderer : VRTK_BasePointerRenderer
	{
		[Header("Straight Pointer Appearance Settings")]
		[Tooltip("The maximum length the pointer tracer can reach.")]
		public float maximumLength = 100f;

		[Tooltip("The scale factor to scale the pointer tracer object by.")]
		public float scaleFactor = 0.002f;

		[Tooltip("The scale multiplier to scale the pointer cursor object by in relation to the `Scale Factor`.")]
		public float cursorScaleMultiplier = 25f;

		[Tooltip("The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
		public bool cursorMatchTargetRotation;

		[Tooltip("Rescale the cursor proportionally to the distance from the tracer origin.")]
		public bool cursorDistanceRescale;

		[Tooltip("The maximum scale the cursor is allowed to reach. This is only used when rescaling the cursor proportionally to the distance from the tracer origin.")]
		public Vector3 maximumCursorScale = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

		[Header("Straight Pointer Custom Appearance Settings")]
		[Tooltip("A custom game object to use as the appearance for the pointer tracer. If this is empty then a Box primitive will be created and used.")]
		public GameObject customTracer;

		[Tooltip("A custom game object to use as the appearance for the pointer cursor. If this is empty then a Sphere primitive will be created and used.")]
		public GameObject customCursor;

		internal bool fading;

		internal bool red;

		protected GameObject actualContainer;

		protected GameObject actualTracer;

		protected GameObject actualCursor;

		protected Vector3 cursorOriginalScale = Vector3.one;

		public override void UpdateRenderer()
		{
			if (((bool)controllingPointer && controllingPointer.IsPointerActive()) || IsVisible())
			{
				float pointerAppearance = CastRayForward();
				SetPointerAppearance(pointerAppearance);
				MakeRenderersVisible();
			}
			base.UpdateRenderer();
		}

		protected override void ToggleRenderer(bool pointerState, bool actualState)
		{
			ToggleElement(actualTracer, pointerState, actualState, tracerVisibility, ref tracerVisible);
			ToggleElement(actualCursor, pointerState, actualState, cursorVisibility, ref cursorVisible);
		}

		protected override void CreatePointerObjects()
		{
			actualContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "StraightPointerRenderer_Container"));
			actualContainer.transform.localPosition = Vector3.zero;
			VRTK_PlayerObject.SetPlayerObject(actualContainer, VRTK_PlayerObject.ObjectTypes.Pointer);
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
			if (actualContainer != null)
			{
				Object.Destroy(actualContainer);
			}
		}

		protected override void ChangeMaterial(Color givenColor)
		{
			base.ChangeMaterial(givenColor);
			ChangeMaterialColor(actualTracer, givenColor);
			ChangeMaterialColor(actualCursor, givenColor);
		}

		protected override void UpdateObjectInteractor()
		{
			base.UpdateObjectInteractor();
			if ((bool)objectInteractor && (bool)actualCursor && Vector3.Distance(objectInteractor.transform.position, actualCursor.transform.position) > 0f)
			{
				objectInteractor.transform.position = actualCursor.transform.position;
			}
		}

		protected virtual void CreateTracer()
		{
			if ((bool)customTracer)
			{
				actualTracer = Object.Instantiate(customTracer);
			}
			else
			{
				actualTracer = GameObject.CreatePrimitive(PrimitiveType.Cube);
				actualTracer.GetComponent<BoxCollider>().isTrigger = true;
				actualTracer.AddComponent<Rigidbody>().isKinematic = true;
				actualTracer.layer = LayerMask.NameToLayer("Ignore Raycast");
				SetupMaterialRenderer(actualTracer);
			}
			actualTracer.transform.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "StraightPointerRenderer_Tracer");
			actualTracer.transform.SetParent(actualContainer.transform);
			VRTK_PlayerObject.SetPlayerObject(actualTracer, VRTK_PlayerObject.ObjectTypes.Pointer);
		}

		protected virtual void CreateCursor()
		{
			if ((bool)customCursor)
			{
				actualCursor = Object.Instantiate(customCursor);
			}
			else
			{
				actualCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				actualCursor.transform.localScale = Vector3.one * (scaleFactor * cursorScaleMultiplier);
				actualCursor.GetComponent<Collider>().isTrigger = true;
				actualCursor.AddComponent<Rigidbody>().isKinematic = true;
				actualCursor.layer = LayerMask.NameToLayer("Ignore Raycast");
				SetupMaterialRenderer(actualCursor);
			}
			cursorOriginalScale = actualCursor.transform.localScale;
			actualCursor.transform.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, base.gameObject.name, "StraightPointerRenderer_Cursor");
			actualCursor.transform.SetParent(actualContainer.transform);
			VRTK_PlayerObject.SetPlayerObject(actualCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
		}

		protected virtual void CheckRayMiss(bool rayHit, RaycastHit pointerCollidedWith)
		{
			if (!rayHit || ((bool)destinationHit.collider && destinationHit.collider != pointerCollidedWith.collider))
			{
				if (destinationHit.collider != null)
				{
					PointerExit(destinationHit);
				}
				destinationHit = default(RaycastHit);
				ChangeColor(invalidCollisionColor);
				if (!red && !fading)
				{
					red = true;
					StartCoroutine("FadeToInvalidColor");
				}
			}
		}

		protected virtual void CheckRayHit(bool rayHit, RaycastHit pointerCollidedWith)
		{
			if (rayHit)
			{
				PointerEnter(pointerCollidedWith);
				destinationHit = pointerCollidedWith;
				ChangeColor(validCollisionColor);
				if (red && !fading)
				{
					red = false;
					StartCoroutine("FadeToValidColor");
				}
			}
		}

		private IEnumerator FadeToValidColor()
		{
			fading = true;
			SkinnedMeshRenderer renduru = actualCursor.GetComponent<SkinnedMeshRenderer>();
			float value = 0f;
			while (value < 100f)
			{
				value += Time.deltaTime * 600f;
				renduru.SetBlendShapeWeight(0, value);
				yield return new WaitForSeconds(0.05f);
			}
			fading = false;
		}

		private IEnumerator FadeToInvalidColor()
		{
			fading = true;
			SkinnedMeshRenderer renduru = actualCursor.GetComponent<SkinnedMeshRenderer>();
			float value = 100f;
			while (value > 0f)
			{
				value -= Time.deltaTime * 600f;
				renduru.SetBlendShapeWeight(0, value);
				yield return new WaitForSeconds(0.05f);
			}
			fading = false;
		}

		protected virtual float CastRayForward()
		{
			Transform origin = GetOrigin();
			RaycastHit hitData;
			bool flag = VRTK_CustomRaycast.Raycast(ray: new Ray(origin.position, origin.forward), customCast: customRaycast, hitData: out hitData, ignoreLayers: layersToIgnore, length: maximumLength);
			CheckRayMiss(flag, hitData);
			CheckRayHit(flag, hitData);
			float distance = maximumLength;
			if (flag && hitData.distance < maximumLength)
			{
				distance = hitData.distance;
			}
			return OverrideBeamLength(distance);
		}

		protected virtual void SetPointerAppearance(float tracerLength)
		{
			if (!actualContainer)
			{
				return;
			}
			float num = tracerLength / 2.0001f;
			actualTracer.transform.localScale = new Vector3(scaleFactor, scaleFactor, tracerLength);
			actualTracer.transform.localPosition = Vector3.forward * num;
			actualCursor.transform.localScale = Vector3.one * (scaleFactor * cursorScaleMultiplier);
			actualCursor.transform.localPosition = new Vector3(0f, 0f, tracerLength);
			Transform origin = GetOrigin();
			actualContainer.transform.position = origin.position;
			actualContainer.transform.rotation = origin.rotation;
			ScaleObjectInteractor(actualCursor.transform.localScale * 1.05f);
			if ((bool)destinationHit.transform)
			{
				if (cursorMatchTargetRotation)
				{
					actualCursor.transform.forward = -destinationHit.normal;
				}
				if (cursorDistanceRescale)
				{
					float num2 = Vector3.Distance(destinationHit.point, origin.position);
					actualCursor.transform.localScale = Vector3.Min(cursorOriginalScale * num2, maximumCursorScale);
				}
			}
			else
			{
				if (cursorMatchTargetRotation)
				{
					actualCursor.transform.forward = origin.forward;
				}
				if (cursorDistanceRescale)
				{
					actualCursor.transform.localScale = Vector3.Min(cursorOriginalScale * tracerLength, maximumCursorScale);
				}
			}
			ToggleRenderer(controllingPointer.IsPointerActive(), false);
			UpdateDependencies(actualCursor.transform.position);
		}
	}
}
