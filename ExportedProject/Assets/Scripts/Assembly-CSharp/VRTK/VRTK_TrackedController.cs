using System.Collections;
using UnityEngine;

namespace VRTK
{
	public class VRTK_TrackedController : MonoBehaviour
	{
		public uint index = uint.MaxValue;

		private Coroutine enableControllerCoroutine;

		private GameObject aliasController;

		public event VRTKTrackedControllerEventHandler ControllerEnabled;

		public event VRTKTrackedControllerEventHandler ControllerDisabled;

		public event VRTKTrackedControllerEventHandler ControllerIndexChanged;

		public virtual void OnControllerEnabled(VRTKTrackedControllerEventArgs e)
		{
			if (index < uint.MaxValue && this.ControllerEnabled != null)
			{
				this.ControllerEnabled(this, e);
			}
		}

		public virtual void OnControllerDisabled(VRTKTrackedControllerEventArgs e)
		{
			if (index < uint.MaxValue && this.ControllerDisabled != null)
			{
				this.ControllerDisabled(this, e);
			}
		}

		public virtual void OnControllerIndexChanged(VRTKTrackedControllerEventArgs e)
		{
			if (index < uint.MaxValue && this.ControllerIndexChanged != null)
			{
				this.ControllerIndexChanged(this, e);
			}
		}

		protected virtual VRTKTrackedControllerEventArgs SetEventPayload(uint previousIndex = uint.MaxValue)
		{
			VRTKTrackedControllerEventArgs result = default(VRTKTrackedControllerEventArgs);
			result.currentIndex = index;
			result.previousIndex = previousIndex;
			return result;
		}

		protected virtual void OnEnable()
		{
			aliasController = VRTK_DeviceFinder.GetScriptAliasController(base.gameObject);
			if (aliasController == null)
			{
				aliasController = base.gameObject;
			}
			if (enableControllerCoroutine != null)
			{
				StopCoroutine(enableControllerCoroutine);
			}
			enableControllerCoroutine = StartCoroutine(Enable());
		}

		protected virtual void OnDisable()
		{
			Invoke("Disable", 0f);
		}

		protected virtual void Disable()
		{
			if (enableControllerCoroutine != null)
			{
				StopCoroutine(enableControllerCoroutine);
			}
			OnControllerDisabled(SetEventPayload());
		}

		protected virtual void FixedUpdate()
		{
			VRTK_SDK_Bridge.ControllerProcessFixedUpdate(index);
		}

		protected virtual void Update()
		{
			uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(base.gameObject);
			if (index < uint.MaxValue && controllerIndex != index)
			{
				uint eventPayload = index;
				index = controllerIndex;
				OnControllerIndexChanged(SetEventPayload(eventPayload));
			}
			VRTK_SDK_Bridge.ControllerProcessUpdate(index);
			if (aliasController != null && base.gameObject.activeInHierarchy && !aliasController.activeSelf)
			{
				aliasController.SetActive(true);
			}
		}

		protected virtual IEnumerator Enable()
		{
			yield return new WaitForEndOfFrame();
			while (!base.gameObject.activeInHierarchy)
			{
				yield return null;
			}
			index = VRTK_DeviceFinder.GetControllerIndex(base.gameObject);
			OnControllerEnabled(SetEventPayload());
		}
	}
}
