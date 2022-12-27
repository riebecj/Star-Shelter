using System.Collections;
using UnityEngine;
using VRTK.Highlighters;

namespace VRTK
{
	[ExecuteInEditMode]
	public class VRTK_SnapDropZone : MonoBehaviour
	{
		public enum SnapTypes
		{
			UseKinematic = 0,
			UseJoint = 1,
			UseParenting = 2
		}

		[Tooltip("A game object that is used to draw the highlighted destination for within the drop zone. This object will also be created in the Editor for easy placement.")]
		public GameObject highlightObjectPrefab;

		[Tooltip("The Snap Type to apply when a valid interactable object is dropped within the snap zone.")]
		public SnapTypes snapType;

		[Tooltip("The amount of time it takes for the object being snapped to move into the new snapped position, rotation and scale.")]
		public float snapDuration;

		[Tooltip("If this is checked then the scaled size of the snap drop zone will be applied to the object that is snapped to it.")]
		public bool applyScalingOnSnap;

		[Tooltip("The colour to use when showing the snap zone is active.")]
		public Color highlightColor;

		[Tooltip("The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.")]
		public bool highlightAlwaysActive;

		[Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.")]
		public VRTK_PolicyList validObjectListPolicy;

		[Tooltip("If this is checked then the drop zone highlight section will be displayed in the scene editor window.")]
		public bool displayDropZoneInEditor = true;

		[Tooltip("The game object to snap into the dropzone when the drop zone is enabled. The game object must be valid in any given policy list to snap.")]
		public GameObject defaultSnappedObject;

		protected GameObject previousPrefab;

		protected GameObject highlightContainer;

		protected GameObject highlightObject;

		protected GameObject highlightEditorObject;

		protected GameObject currentValidSnapObject;

		protected GameObject currentSnappedObject;

		protected VRTK_BaseHighlighter objectHighlighter;

		protected bool willSnap;

		protected bool isSnapped;

		protected bool isHighlighted;

		protected Coroutine transitionInPlace;

		protected bool originalJointCollisionState;

		protected const string HIGHLIGHT_CONTAINER_NAME = "HighlightContainer";

		protected const string HIGHLIGHT_OBJECT_NAME = "HighlightObject";

		protected const string HIGHLIGHT_EDITOR_OBJECT_NAME = "EditorHighlightObject";

		public event SnapDropZoneEventHandler ObjectEnteredSnapDropZone;

		public event SnapDropZoneEventHandler ObjectExitedSnapDropZone;

		public event SnapDropZoneEventHandler ObjectSnappedToDropZone;

		public event SnapDropZoneEventHandler ObjectUnsnappedFromDropZone;

		public virtual void OnObjectEnteredSnapDropZone(SnapDropZoneEventArgs e)
		{
			if (this.ObjectEnteredSnapDropZone != null)
			{
				this.ObjectEnteredSnapDropZone(this, e);
			}
		}

		public virtual void OnObjectExitedSnapDropZone(SnapDropZoneEventArgs e)
		{
			if (this.ObjectExitedSnapDropZone != null)
			{
				this.ObjectExitedSnapDropZone(this, e);
			}
		}

		public virtual void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e)
		{
			if (this.ObjectSnappedToDropZone != null)
			{
				this.ObjectSnappedToDropZone(this, e);
			}
		}

		public virtual void OnObjectUnsnappedFromDropZone(SnapDropZoneEventArgs e)
		{
			UnsnapObject();
			if (this.ObjectUnsnappedFromDropZone != null)
			{
				this.ObjectUnsnappedFromDropZone(this, e);
			}
		}

		public virtual SnapDropZoneEventArgs SetSnapDropZoneEvent(GameObject interactableObject)
		{
			SnapDropZoneEventArgs result = default(SnapDropZoneEventArgs);
			result.snappedObject = interactableObject;
			return result;
		}

		public virtual void InitaliseHighlightObject(bool removeOldObject = false)
		{
			if (removeOldObject)
			{
				DeleteHighlightObject();
			}
			ChooseDestroyType(base.transform.Find(ObjectPath("EditorHighlightObject")));
			highlightEditorObject = null;
			GenerateObjects();
		}

		public virtual void ForceSnap(GameObject objectToSnap)
		{
			VRTK_InteractableObject componentInParent = objectToSnap.GetComponentInParent<VRTK_InteractableObject>();
			if ((bool)componentInParent)
			{
				componentInParent.SaveCurrentState();
				StopCoroutine("AttemptForceSnapAtEndOfFrame");
				if (componentInParent.IsGrabbed())
				{
					componentInParent.ForceStopInteracting();
					StartCoroutine(AttemptForceSnapAtEndOfFrame(objectToSnap));
				}
				else
				{
					AttemptForceSnap(objectToSnap);
				}
			}
		}

		public virtual void ForceUnsnap()
		{
			if (isSnapped && (bool)currentSnappedObject)
			{
				VRTK_InteractableObject vRTK_InteractableObject = ValidSnapObject(currentSnappedObject, false);
				vRTK_InteractableObject.ToggleSnapDropZone(this, false);
			}
		}

		protected virtual void Awake()
		{
			if (Application.isPlaying)
			{
				InitaliseHighlightObject();
			}
		}

		protected virtual void OnApplicationQuit()
		{
			if ((bool)objectHighlighter)
			{
				objectHighlighter.Unhighlight();
			}
		}

		protected virtual void OnEnable()
		{
			if (defaultSnappedObject != null)
			{
				ForceSnap(defaultSnappedObject);
			}
		}

		protected virtual void Update()
		{
			if (previousPrefab != null && previousPrefab != highlightObjectPrefab)
			{
				DeleteHighlightObject();
			}
			CreateHighlightersInEditor();
			CheckCurrentValidSnapObjectStillValid();
			previousPrefab = highlightObjectPrefab;
			if (highlightAlwaysActive && !isSnapped && !isHighlighted)
			{
				highlightObject.SetActive(true);
			}
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (!isSnapped && currentValidSnapObject == null)
			{
				ToggleHighlight(collider, true);
			}
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			if (currentValidSnapObject == collider.gameObject)
			{
				ToggleHighlight(collider, false);
			}
		}

		protected virtual void OnTriggerStay(Collider collider)
		{
			if (!isSnapped && currentValidSnapObject == null && (bool)ValidSnapObject(collider.gameObject, true))
			{
				currentValidSnapObject = collider.gameObject;
			}
			if (currentValidSnapObject == collider.gameObject)
			{
				if (!isSnapped)
				{
					ToggleHighlight(collider, true);
				}
				SnapObject(collider);
			}
		}

		protected virtual VRTK_InteractableObject ValidSnapObject(GameObject checkObject, bool grabState)
		{
			VRTK_InteractableObject componentInParent = checkObject.GetComponentInParent<VRTK_InteractableObject>();
			return (!(componentInParent != null) || componentInParent.IsGrabbed() != grabState || VRTK_PolicyList.Check(componentInParent.gameObject, validObjectListPolicy)) ? null : componentInParent;
		}

		protected virtual string ObjectPath(string name)
		{
			return "HighlightContainer/" + name;
		}

		protected virtual void CreateHighlightersInEditor()
		{
			if (VRTK_SharedMethods.IsEditTime())
			{
				GenerateHighlightObject();
				if (snapType == SnapTypes.UseJoint && GetComponent<Joint>() == null)
				{
					VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + base.name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
				}
				GenerateEditorHighlightObject();
				ForceSetObjects();
				if ((bool)highlightEditorObject)
				{
					highlightEditorObject.SetActive(displayDropZoneInEditor);
				}
			}
		}

		protected virtual void CheckCurrentValidSnapObjectStillValid()
		{
			if (!currentValidSnapObject)
			{
				return;
			}
			VRTK_InteractableObject componentInParent = currentValidSnapObject.GetComponentInParent<VRTK_InteractableObject>();
			if ((bool)componentInParent && componentInParent.GetStoredSnapDropZone() != null && componentInParent.GetStoredSnapDropZone() != base.gameObject)
			{
				currentValidSnapObject = null;
				if (isHighlighted && (bool)highlightObject && !highlightAlwaysActive)
				{
					highlightObject.SetActive(false);
				}
			}
		}

		protected virtual void ForceSetObjects()
		{
			if (!highlightEditorObject)
			{
				Transform transform = base.transform.Find(ObjectPath("EditorHighlightObject"));
				highlightEditorObject = ((!transform) ? null : transform.gameObject);
			}
			if (!highlightObject)
			{
				Transform transform2 = base.transform.Find(ObjectPath("HighlightObject"));
				highlightObject = ((!transform2) ? null : transform2.gameObject);
			}
			if (!highlightContainer)
			{
				Transform transform3 = base.transform.Find("HighlightContainer");
				highlightContainer = ((!transform3) ? null : transform3.gameObject);
			}
		}

		protected virtual void GenerateContainer()
		{
			if (!highlightContainer || !base.transform.Find("HighlightContainer"))
			{
				highlightContainer = new GameObject("HighlightContainer");
				highlightContainer.transform.SetParent(base.transform);
				highlightContainer.transform.localPosition = Vector3.zero;
				highlightContainer.transform.localRotation = Quaternion.identity;
				highlightContainer.transform.localScale = Vector3.one;
			}
		}

		protected virtual void SetContainer()
		{
			Transform transform = base.transform.Find("HighlightContainer");
			if ((bool)transform)
			{
				highlightContainer = transform.gameObject;
			}
		}

		protected virtual void GenerateObjects()
		{
			GenerateHighlightObject();
			if ((bool)highlightObject && objectHighlighter == null)
			{
				InitialiseHighlighter();
			}
		}

		protected virtual void SnapObject(Collider collider)
		{
			VRTK_InteractableObject vRTK_InteractableObject = ValidSnapObject(collider.gameObject, false);
			if (willSnap && !isSnapped && (bool)vRTK_InteractableObject && !vRTK_InteractableObject.IsInSnapDropZone())
			{
				if (highlightObject != null)
				{
					highlightObject.SetActive(false);
				}
				Vector3 newLocalScale = GetNewLocalScale(vRTK_InteractableObject);
				if (transitionInPlace != null)
				{
					StopCoroutine(transitionInPlace);
				}
				isSnapped = true;
				currentSnappedObject = vRTK_InteractableObject.gameObject;
				transitionInPlace = StartCoroutine(UpdateTransformDimensions(vRTK_InteractableObject, highlightContainer, newLocalScale, snapDuration));
				vRTK_InteractableObject.ToggleSnapDropZone(this, true);
			}
			isSnapped = (!isSnapped || !vRTK_InteractableObject || !vRTK_InteractableObject.IsGrabbed()) && isSnapped;
		}

		protected virtual void UnsnapObject()
		{
			isSnapped = false;
			currentSnappedObject = null;
			ResetSnapDropZoneJoint();
			if (transitionInPlace != null)
			{
				StopCoroutine(transitionInPlace);
			}
		}

		protected virtual Vector3 GetNewLocalScale(VRTK_InteractableObject ioCheck)
		{
			Vector3 result = ioCheck.transform.localScale;
			if (applyScalingOnSnap)
			{
				ioCheck.StoreLocalScale();
				result = Vector3.Scale(ioCheck.transform.localScale, base.transform.localScale);
			}
			return result;
		}

		protected virtual IEnumerator UpdateTransformDimensions(VRTK_InteractableObject ioCheck, GameObject endSettings, Vector3 endScale, float duration)
		{
			float elapsedTime = 0f;
			Transform ioTransform = ioCheck.transform;
			Vector3 startPosition = ioTransform.position;
			Quaternion startRotation = ioTransform.rotation;
			Vector3 startScale = ioTransform.localScale;
			bool storedKinematicState = ioCheck.isKinematic;
			ioCheck.isKinematic = true;
			while (elapsedTime <= duration)
			{
				elapsedTime += Time.deltaTime;
				if (ioTransform != null && endSettings != null)
				{
					ioTransform.position = Vector3.Lerp(startPosition, endSettings.transform.position, elapsedTime / duration);
					ioTransform.rotation = Quaternion.Lerp(startRotation, endSettings.transform.rotation, elapsedTime / duration);
					ioTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
				}
				yield return null;
			}
			ioTransform.position = endSettings.transform.position;
			ioTransform.rotation = endSettings.transform.rotation;
			ioTransform.localScale = endScale;
			ioCheck.isKinematic = storedKinematicState;
			SetDropSnapType(ioCheck);
		}

		protected virtual void SetDropSnapType(VRTK_InteractableObject ioCheck)
		{
			switch (snapType)
			{
			case SnapTypes.UseKinematic:
				ioCheck.SaveCurrentState();
				ioCheck.isKinematic = true;
				break;
			case SnapTypes.UseParenting:
				ioCheck.SaveCurrentState();
				ioCheck.isKinematic = true;
				ioCheck.transform.SetParent(base.transform);
				break;
			case SnapTypes.UseJoint:
				SetSnapDropZoneJoint(ioCheck.GetComponent<Rigidbody>());
				break;
			}
			OnObjectSnappedToDropZone(SetSnapDropZoneEvent(ioCheck.gameObject));
		}

		protected virtual void SetSnapDropZoneJoint(Rigidbody snapTo)
		{
			Joint component = GetComponent<Joint>();
			if (component == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + base.name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
			}
			else if (snapTo == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_SnapDropZone", "Rigidbody", "the `VRTK_InteractableObject`"));
			}
			else
			{
				component.connectedBody = snapTo;
				originalJointCollisionState = component.enableCollision;
				component.enableCollision = true;
			}
		}

		protected virtual void ResetSnapDropZoneJoint()
		{
			Joint component = GetComponent<Joint>();
			if ((bool)component)
			{
				component.enableCollision = originalJointCollisionState;
			}
		}

		protected virtual void AttemptForceSnap(GameObject objectToSnap)
		{
			willSnap = true;
			currentValidSnapObject = objectToSnap;
			OnTriggerStay(objectToSnap.GetComponentInChildren<Collider>());
		}

		protected virtual IEnumerator AttemptForceSnapAtEndOfFrame(GameObject objectToSnap)
		{
			yield return new WaitForEndOfFrame();
			AttemptForceSnap(objectToSnap);
		}

		protected virtual void ToggleHighlight(Collider collider, bool state)
		{
			VRTK_InteractableObject vRTK_InteractableObject = ValidSnapObject(collider.gameObject, true);
			if ((bool)highlightObject && (bool)vRTK_InteractableObject)
			{
				highlightObject.SetActive(state);
				vRTK_InteractableObject.SetSnapDropZoneHover(this, state);
				willSnap = state;
				isHighlighted = state;
				if (state)
				{
					OnObjectEnteredSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
					currentValidSnapObject = collider.gameObject;
				}
				else
				{
					OnObjectExitedSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
					currentValidSnapObject = null;
				}
			}
		}

		protected virtual void CopyObject(GameObject objectBlueprint, ref GameObject clonedObject, string givenName)
		{
			GenerateContainer();
			Vector3 localScale = base.transform.localScale;
			base.transform.localScale = Vector3.one;
			clonedObject = Object.Instantiate(objectBlueprint, highlightContainer.transform);
			clonedObject.name = givenName;
			clonedObject.transform.localPosition = Vector3.zero;
			clonedObject.transform.localRotation = Quaternion.identity;
			base.transform.localScale = localScale;
			CleanHighlightObject(clonedObject);
		}

		protected virtual void GenerateHighlightObject()
		{
			if ((bool)highlightObjectPrefab && !highlightObject && !base.transform.Find(ObjectPath("HighlightObject")))
			{
				CopyObject(highlightObjectPrefab, ref highlightObject, "HighlightObject");
			}
			Transform transform = base.transform.Find(ObjectPath("HighlightObject"));
			if ((bool)transform && !highlightObject)
			{
				highlightObject = transform.gameObject;
			}
			if (!highlightObjectPrefab && (bool)highlightObject)
			{
				DeleteHighlightObject();
			}
			if ((bool)highlightObject)
			{
				highlightObject.SetActive(false);
			}
			SetContainer();
		}

		protected virtual void DeleteHighlightObject()
		{
			ChooseDestroyType(base.transform.Find("HighlightContainer"));
			highlightContainer = null;
			highlightObject = null;
			objectHighlighter = null;
		}

		protected virtual void GenerateEditorHighlightObject()
		{
			if ((bool)highlightObject && !highlightEditorObject && !base.transform.Find(ObjectPath("EditorHighlightObject")))
			{
				CopyObject(highlightObject, ref highlightEditorObject, "EditorHighlightObject");
				Renderer[] componentsInChildren = highlightEditorObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.material = Resources.Load("SnapDropZoneEditorObject") as Material;
				}
				highlightEditorObject.SetActive(true);
			}
		}

		protected virtual void CleanHighlightObject(GameObject objectToClean)
		{
			VRTK_SnapDropZone[] componentsInChildren = objectToClean.GetComponentsInChildren<VRTK_SnapDropZone>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ChooseDestroyType(componentsInChildren[i].gameObject);
			}
			string[] array = new string[5] { "Transform", "MeshFilter", "MeshRenderer", "SkinnedMeshRenderer", "VRTK_GameObjectLinker" };
			Component[] componentsInChildren2 = objectToClean.GetComponentsInChildren<Component>(true);
			foreach (Component component in componentsInChildren2)
			{
				bool flag = false;
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (component.GetType().ToString().Contains("." + text))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ChooseDestroyType(component);
				}
			}
		}

		protected virtual void InitialiseHighlighter()
		{
			VRTK_BaseHighlighter activeHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(base.gameObject);
			if (activeHighlighter == null)
			{
				highlightObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
			}
			else
			{
				VRTK_SharedMethods.CloneComponent(activeHighlighter, highlightObject);
			}
			objectHighlighter = highlightObject.GetComponent<VRTK_BaseHighlighter>();
			objectHighlighter.unhighlightOnDisable = false;
			objectHighlighter.Initialise(highlightColor);
			objectHighlighter.Highlight(highlightColor);
			if (!objectHighlighter.UsesClonedObject())
			{
				return;
			}
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (!VRTK_PlayerObject.IsPlayerObject(renderer.gameObject, VRTK_PlayerObject.ObjectTypes.Highlighter))
				{
					renderer.enabled = false;
				}
			}
		}

		protected virtual void ChooseDestroyType(Transform deleteTransform)
		{
			if ((bool)deleteTransform)
			{
				ChooseDestroyType(deleteTransform.gameObject);
			}
		}

		protected virtual void ChooseDestroyType(GameObject deleteObject)
		{
			if (VRTK_SharedMethods.IsEditTime())
			{
				if ((bool)deleteObject)
				{
					Object.DestroyImmediate(deleteObject);
				}
			}
			else if ((bool)deleteObject)
			{
				Object.Destroy(deleteObject);
			}
		}

		protected virtual void ChooseDestroyType(Component deleteComponent)
		{
			if (VRTK_SharedMethods.IsEditTime())
			{
				if ((bool)deleteComponent)
				{
					Object.DestroyImmediate(deleteComponent);
				}
			}
			else if ((bool)deleteComponent)
			{
				Object.Destroy(deleteComponent);
			}
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if ((bool)highlightObject && !displayDropZoneInEditor)
			{
				Vector3 size = VRTK_SharedMethods.GetBounds(highlightObject.transform).size * 1.05f;
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(highlightObject.transform.position, size);
			}
		}
	}
}
