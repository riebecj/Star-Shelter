using System;
using UnityEngine;

namespace VRTK
{
	[Obsolete("`VRTK_ControllerActions` has been replaced with a combination of `VRTK_ControllerHighlighter` and calls to `VRTK_SharedMethods`. This script will be removed in a future version of VRTK.")]
	public class VRTK_ControllerActions : MonoBehaviour
	{
		[Tooltip("A collection of strings that determine the path to the controller model sub elements for identifying the model parts at runtime. If the paths are left empty they will default to the model element paths of the selected SDK Bridge.\n\n* The available model sub elements are:\n\n * `Body Model Path`: The overall shape of the controller.\n * `Trigger Model Path`: The model that represents the trigger button.\n * `Grip Left Model Path`: The model that represents the left grip button.\n * `Grip Right Model Path`: The model that represents the right grip button.\n * `Touchpad Model Path`: The model that represents the touchpad.\n * `Button One Model Path`: The model that represents button one.\n * `Button Two Model Path`: The model that represents button two.\n * `System Menu Model Path`: The model that represents the system menu button. * `Start Menu Model Path`: The model that represents the start menu button.")]
		[Obsolete("`VRTK_ControllerActions.modelElementPaths` has been replaced with `VRTK_ControllerHighlighter.modelElementPaths`, it will be removed in a future version of VRTK.")]
		public VRTK_ControllerModelElementPaths modelElementPaths;

		[Tooltip("A collection of highlighter overrides for each controller model sub element. If no highlighter override is given then highlighter on the Controller game object is used.\n\n* The available model sub elements are:\n\n * `Body`: The highlighter to use on the overall shape of the controller.\n * `Trigger`: The highlighter to use on the trigger button.\n * `Grip Left`: The highlighter to use on the left grip button.\n * `Grip Right`: The highlighter to use on the  right grip button.\n * `Touchpad`: The highlighter to use on the touchpad.\n * `Button One`: The highlighter to use on button one.\n * `Button Two`: The highlighter to use on button two.\n * `System Menu`: The highlighter to use on the system menu button. * `Start Menu`: The highlighter to use on the start menu button.")]
		[Obsolete("`VRTK_ControllerActions.elementHighlighterOverrides` has been replaced with `VRTK_ControllerHighlighter.elementHighlighterOverrides`, it will be removed in a future version of VRTK.")]
		public VRTK_ControllerElementHighlighters elementHighlighterOverrides;

		protected GameObject modelContainer;

		protected bool controllerVisible = true;

		protected VRTK_ControllerHighlighter controllerHighlighter;

		protected bool generateControllerHighlighter;

		[Obsolete("`VRTK_ControllerActions.ControllerModelVisible` has been replaced with `VRTK_ControllerEvents.ControllerVisible`. This method will be removed in a future version of VRTK.")]
		public event ControllerActionsEventHandler ControllerModelVisible;

		[Obsolete("`VRTK_ControllerActions.ControllerModelInvisible` has been replaced with `VRTK_ControllerEvents.ControllerHidden`. This method will be removed in a future version of VRTK.")]
		public event ControllerActionsEventHandler ControllerModelInvisible;

		[Obsolete("`VRTK_ControllerActions.OnControllerModelVisible(e)` has been replaced with `VRTK_ControllerEvents.OnControllerVisible(e)`. This method will be removed in a future version of VRTK.")]
		public virtual void OnControllerModelVisible(ControllerActionsEventArgs e)
		{
			if (this.ControllerModelVisible != null)
			{
				this.ControllerModelVisible(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerActions.OnControllerModelInvisible(e)` has been replaced with `VRTK_ControllerEvents.OnControllerHidden(e)`. This method will be removed in a future version of VRTK.")]
		public virtual void OnControllerModelInvisible(ControllerActionsEventArgs e)
		{
			if (this.ControllerModelInvisible != null)
			{
				this.ControllerModelInvisible(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerActions.IsControllerVisible()` has been replaced with `VRTK_ControllerEvents.controllerVisible`. This method will be removed in a future version of VRTK.")]
		public virtual bool IsControllerVisible()
		{
			return controllerVisible;
		}

		[Obsolete("`VRTK_ControllerActions.ToggleControllerModel(state, grabbedChildObject)` has been replaced with `VRTK_SharedMethods.ToggleRenderer(model, state, ignoredModel)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleControllerModel(bool state, GameObject grabbedChildObject)
		{
			if (base.enabled)
			{
				VRTK_SharedMethods.ToggleRenderer(state, modelContainer, grabbedChildObject);
				controllerVisible = state;
				uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(base.gameObject);
				if (state)
				{
					OnControllerModelVisible(SetActionEvent(controllerIndex));
				}
				else
				{
					OnControllerModelInvisible(SetActionEvent(controllerIndex));
				}
			}
		}

		[Obsolete("`VRTK_ControllerActions.SetControllerOpacity(alpha)` has been replaced with `VRTK_SharedMethods.SetOpacity(model, alpha)`. This method will be removed in a future version of VRTK.")]
		public virtual void SetControllerOpacity(float alpha)
		{
			if (base.enabled)
			{
				VRTK_SharedMethods.SetOpacity(modelContainer, alpha);
			}
		}

		[Obsolete("`VRTK_ControllerActions.HighlightControllerElement(element, highlight, fadeDuration)` has been replaced with `VRTK_SharedMethods.HighlightObject(model, highlight, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void HighlightControllerElement(GameObject element, Color? highlight, float fadeDuration = 0f)
		{
			if (base.enabled)
			{
				VRTK_SharedMethods.HighlightObject(element, highlight, fadeDuration);
			}
		}

		[Obsolete("`VRTK_ControllerActions.UnhighlightControllerElement(element)` has been replaced with `VRTK_SharedMethods.UnhighlightObject(model)`. This method will be removed in a future version of VRTK.")]
		public virtual void UnhighlightControllerElement(GameObject element)
		{
			if (base.enabled)
			{
				VRTK_SharedMethods.UnhighlightObject(element);
			}
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightControllerElement(state, element, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)/UnhighlightElement(elementType)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightControllerElement(bool state, GameObject element, Color? highlight = null, float duration = 0f)
		{
			if ((bool)element)
			{
				if (state)
				{
					VRTK_SharedMethods.HighlightObject(element, highlight ?? new Color?(Color.white), duration);
				}
				else
				{
					VRTK_SharedMethods.UnhighlightObject(element);
				}
			}
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightTrigger(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightTrigger(bool state, Color? highlight = null, float duration = 0f)
		{
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.Trigger, highlight, duration);
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightGrip(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightGrip(bool state, Color? highlight = null, float duration = 0f)
		{
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.GripLeft, highlight, duration);
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.GripRight, highlight, duration);
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightTouchpad(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightTouchpad(bool state, Color? highlight = null, float duration = 0f)
		{
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.Touchpad, highlight, duration);
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightButtonOne(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightButtonOne(bool state, Color? highlight = null, float duration = 0f)
		{
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.ButtonOne, highlight, duration);
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightButtonTwo(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightButtonTwo(bool state, Color? highlight = null, float duration = 0f)
		{
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.ButtonTwo, highlight, duration);
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightStartMenu(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightStartMenu(bool state, Color? highlight = null, float duration = 0f)
		{
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.StartMenu, highlight, duration);
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlighBody(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlighBody(bool state, Color? highlight = null, float duration = 0f)
		{
			ToggleElementHighlight(state, SDK_BaseController.ControllerElements.Body, highlight, duration);
		}

		[Obsolete("`VRTK_ControllerActions.ToggleHighlightController(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightController(color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
		public virtual void ToggleHighlightController(bool state, Color? highlight = null, float duration = 0f)
		{
			if (controllerHighlighter != null)
			{
				if (state)
				{
					controllerHighlighter.HighlightController((highlight ?? new Color?(Color.white)).Value, duration);
				}
				else
				{
					controllerHighlighter.UnhighlightController();
				}
			}
		}

		[Obsolete("`VRTK_ControllerActions.TriggerHapticPulse(strength)` has been replaced with `VRTK_SharedMethods.TriggerHapticPulse(index, strength)`. This method will be removed in a future version of VRTK.")]
		public virtual void TriggerHapticPulse(float strength)
		{
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(base.gameObject), strength);
		}

		[Obsolete("`VRTK_ControllerActions.TriggerHapticPulse(strength, duration, pulseInterval)` has been replaced with `VRTK_SharedMethods.TriggerHapticPulse(index, strength, duration, pulseInterval)`. This method will be removed in a future version of VRTK.")]
		public virtual void TriggerHapticPulse(float strength, float duration, float pulseInterval)
		{
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(base.gameObject), strength, duration, pulseInterval);
		}

		[Obsolete("`VRTK_ControllerActions.InitaliseHighlighters()` has been replaced with `VRTK_ControllerHighlighter.PopulateHighlighters()`. This method will be removed in a future version of VRTK.")]
		public virtual void InitaliseHighlighters()
		{
			controllerHighlighter.PopulateHighlighters();
		}

		protected virtual void OnEnable()
		{
			modelContainer = (modelContainer ? modelContainer : VRTK_DeviceFinder.GetModelAliasController(base.gameObject));
			generateControllerHighlighter = false;
			VRTK_ControllerHighlighter component = GetComponent<VRTK_ControllerHighlighter>();
			if (component == null)
			{
				generateControllerHighlighter = true;
				controllerHighlighter = base.gameObject.AddComponent<VRTK_ControllerHighlighter>();
				controllerHighlighter.modelElementPaths = modelElementPaths;
				controllerHighlighter.elementHighlighterOverrides = elementHighlighterOverrides;
				controllerHighlighter.ConfigureControllerPaths();
			}
			else
			{
				controllerHighlighter = component;
			}
		}

		protected virtual void OnDisable()
		{
			if (generateControllerHighlighter)
			{
				UnityEngine.Object.Destroy(controllerHighlighter);
			}
		}

		protected virtual void ToggleElementHighlight(bool state, SDK_BaseController.ControllerElements elementType, Color? color, float fadeDuration = 0f)
		{
			if (controllerHighlighter != null)
			{
				if (state)
				{
					controllerHighlighter.HighlightElement(elementType, (color ?? new Color?(Color.white)).Value, fadeDuration);
				}
				else
				{
					controllerHighlighter.UnhighlightElement(elementType);
				}
			}
		}

		protected virtual ControllerActionsEventArgs SetActionEvent(uint index)
		{
			ControllerActionsEventArgs result = default(ControllerActionsEventArgs);
			result.controllerIndex = index;
			return result;
		}
	}
}
