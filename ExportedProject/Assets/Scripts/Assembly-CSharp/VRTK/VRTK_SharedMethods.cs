using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VRTK
{
	public static class VRTK_SharedMethods
	{
		[CompilerGenerated]
		private sealed class _003CFindEvenInactiveGameObject_003Ec__AnonStorey0<T> where T : Component
		{
			internal string gameObjectName;

			internal GameObject _003C_003Em__0(GameObject gameObject)
			{
				Transform transform = gameObject.transform.Find(gameObjectName);
				return (!(transform == null)) ? transform.gameObject : null;
			}
		}

		public static Bounds GetBounds(Transform transform, Transform excludeRotation = null, Transform excludeTransform = null)
		{
			Quaternion rotation = Quaternion.identity;
			if ((bool)excludeRotation)
			{
				rotation = excludeRotation.rotation;
				excludeRotation.rotation = Quaternion.identity;
			}
			bool flag = false;
			Bounds result = new Bounds(transform.position, Vector3.zero);
			Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!(excludeTransform != null) || !renderer.transform.IsChildOf(excludeTransform))
				{
					if (!flag)
					{
						result = new Bounds(renderer.transform.position, Vector3.zero);
						flag = true;
					}
					result.Encapsulate(renderer.bounds);
				}
			}
			if (result.size.magnitude == 0f)
			{
				BoxCollider[] componentsInChildren2 = transform.GetComponentsInChildren<BoxCollider>();
				BoxCollider[] array2 = componentsInChildren2;
				foreach (BoxCollider boxCollider in array2)
				{
					if (!(excludeTransform != null) || !boxCollider.transform.IsChildOf(excludeTransform))
					{
						if (!flag)
						{
							result = new Bounds(boxCollider.transform.position, Vector3.zero);
							flag = true;
						}
						result.Encapsulate(boxCollider.bounds);
					}
				}
			}
			if ((bool)excludeRotation)
			{
				excludeRotation.rotation = rotation;
			}
			return result;
		}

		public static bool IsLowest(float value, float[] others)
		{
			foreach (float num in others)
			{
				if (num <= value)
				{
					return false;
				}
			}
			return true;
		}

		public static Transform AddCameraFade()
		{
			Transform transform = VRTK_DeviceFinder.HeadsetCamera();
			VRTK_SDK_Bridge.AddHeadsetFade(transform);
			return transform;
		}

		public static void CreateColliders(GameObject obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!renderer.gameObject.GetComponent<Collider>())
				{
					renderer.gameObject.AddComponent<BoxCollider>();
				}
			}
		}

		public static Component CloneComponent(Component source, GameObject destination, bool copyProperties = false)
		{
			Component component = destination.gameObject.AddComponent(source.GetType());
			if (copyProperties)
			{
				PropertyInfo[] properties = source.GetType().GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.CanWrite)
					{
						propertyInfo.SetValue(component, propertyInfo.GetValue(source, null), null);
					}
				}
			}
			FieldInfo[] fields = source.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(component, fieldInfo.GetValue(source));
			}
			return component;
		}

		public static Color ColorDarken(Color color, float percent)
		{
			return new Color(ColorPercent(color.r, percent), ColorPercent(color.g, percent), ColorPercent(color.b, percent), color.a);
		}

		public static float RoundFloat(float givenFloat, int decimalPlaces, bool rawFidelity = false)
		{
			float num = ((!rawFidelity) ? Mathf.Pow(10f, decimalPlaces) : ((float)decimalPlaces));
			return Mathf.Round(givenFloat * num) / num;
		}

		public static bool IsEditTime()
		{
			return false;
		}

		public static void TriggerHapticPulse(uint controllerIndex, float strength)
		{
			VRTK_InstanceMethods instance = VRTK_InstanceMethods.instance;
			if (instance != null)
			{
				instance.haptics.TriggerHapticPulse(controllerIndex, strength);
			}
		}

		public static void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float pulseInterval)
		{
			VRTK_InstanceMethods instance = VRTK_InstanceMethods.instance;
			if (instance != null)
			{
				instance.haptics.TriggerHapticPulse(controllerIndex, strength, duration, pulseInterval);
			}
		}

		public static void SetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
		{
			VRTK_InstanceMethods instance = VRTK_InstanceMethods.instance;
			if (instance != null)
			{
				instance.objectAppearance.SetOpacity(model, alpha, transitionDuration);
			}
		}

		public static void SetRendererVisible(GameObject model, GameObject ignoredModel = null)
		{
			VRTK_InstanceMethods instance = VRTK_InstanceMethods.instance;
			if (instance != null)
			{
				instance.objectAppearance.SetRendererVisible(model, ignoredModel);
			}
		}

		public static void SetRendererHidden(GameObject model, GameObject ignoredModel = null)
		{
			VRTK_InstanceMethods instance = VRTK_InstanceMethods.instance;
			if (instance != null)
			{
				instance.objectAppearance.SetRendererHidden(model, ignoredModel);
			}
		}

		public static void ToggleRenderer(bool state, GameObject model, GameObject ignoredModel = null)
		{
			if (state)
			{
				SetRendererVisible(model, ignoredModel);
			}
			else
			{
				SetRendererHidden(model, ignoredModel);
			}
		}

		public static void HighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
		{
			VRTK_InstanceMethods instance = VRTK_InstanceMethods.instance;
			if (instance != null)
			{
				instance.objectAppearance.HighlightObject(model, highlightColor, fadeDuration);
			}
		}

		public static void UnhighlightObject(GameObject model)
		{
			VRTK_InstanceMethods instance = VRTK_InstanceMethods.instance;
			if (instance != null)
			{
				instance.objectAppearance.UnhighlightObject(model);
			}
		}

		public static float Mod(float a, float b)
		{
			return a - b * Mathf.Floor(a / b);
		}

		public static GameObject FindEvenInactiveGameObject<T>(string gameObjectName = null) where T : Component
		{
			_003CFindEvenInactiveGameObject_003Ec__AnonStorey0<T> _003CFindEvenInactiveGameObject_003Ec__AnonStorey = new _003CFindEvenInactiveGameObject_003Ec__AnonStorey0<T>();
			_003CFindEvenInactiveGameObject_003Ec__AnonStorey.gameObjectName = gameObjectName;
			if (string.IsNullOrEmpty(_003CFindEvenInactiveGameObject_003Ec__AnonStorey.gameObjectName))
			{
				T val = FindEvenInactiveComponent<T>();
				return (!((Object)val == (Object)null)) ? val.gameObject : null;
			}
			IEnumerable<GameObject> source = Resources.FindObjectsOfTypeAll<T>().Select(_003CFindEvenInactiveGameObject_00601_003Em__0);
			return source.Select(_003CFindEvenInactiveGameObject_003Ec__AnonStorey._003C_003Em__0).FirstOrDefault(_003CFindEvenInactiveGameObject_00601_003Em__1<T>);
		}

		public static T[] FindEvenInactiveComponents<T>() where T : Object
		{
			return Resources.FindObjectsOfTypeAll<T>();
		}

		public static T FindEvenInactiveComponent<T>() where T : Component
		{
			return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
		}

		public static string GenerateVRTKObjectName(bool autoGen, params object[] replacements)
		{
			string text = "[VRTK]";
			if (autoGen)
			{
				text += "[AUTOGEN]";
			}
			for (int i = 0; i < replacements.Length; i++)
			{
				string text2 = text;
				text = text2 + "[{" + i + "}]";
			}
			return string.Format(text, replacements);
		}

		private static float ColorPercent(float value, float percent)
		{
			percent = Mathf.Clamp(percent, 0f, 100f);
			return (percent != 0f) ? (value - percent / 100f) : value;
		}

		[CompilerGenerated]
		private static GameObject _003CFindEvenInactiveGameObject_00601_003Em__0<T>(T component) where T : Component
		{
			return component.gameObject;
		}

		[CompilerGenerated]
		private static bool _003CFindEvenInactiveGameObject_00601_003Em__1<T>(GameObject gameObject) where T : Component
		{
			return gameObject != null;
		}
	}
}
