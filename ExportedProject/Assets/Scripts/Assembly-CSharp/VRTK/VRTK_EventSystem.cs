using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRTK
{
	public class VRTK_EventSystem : EventSystem
	{
		protected EventSystem previousEventSystem;

		protected VRTK_VRInputModule vrInputModule;

		private static readonly FieldInfo[] EVENT_SYSTEM_FIELD_INFOS = typeof(EventSystem).GetFields(BindingFlags.Instance | BindingFlags.Public);

		private static readonly PropertyInfo[] EVENT_SYSTEM_PROPERTY_INFOS = typeof(EventSystem).GetProperties(BindingFlags.Instance | BindingFlags.Public).Except(new PropertyInfo[1] { typeof(EventSystem).GetProperty("enabled") }).ToArray();

		private static readonly FieldInfo BASE_INPUT_MODULE_EVENT_SYSTEM_FIELD_INFO = typeof(BaseInputModule).GetField("m_EventSystem", BindingFlags.Instance | BindingFlags.NonPublic);

		protected override void OnEnable()
		{
			previousEventSystem = EventSystem.current;
			if (previousEventSystem != null)
			{
				previousEventSystem.enabled = false;
				CopyValuesFrom(previousEventSystem, this);
			}
			vrInputModule = base.gameObject.AddComponent<VRTK_VRInputModule>();
			base.OnEnable();
			StartCoroutine(SetEventSystemOfBaseInputModulesAfterFrameDelay(this));
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Object.Destroy(vrInputModule);
			if (previousEventSystem != null)
			{
				previousEventSystem.enabled = true;
				CopyValuesFrom(this, previousEventSystem);
				SetEventSystemOfBaseInputModules(previousEventSystem);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (EventSystem.current == this)
			{
				vrInputModule.Process();
			}
		}

		protected override void OnApplicationFocus(bool hasFocus)
		{
		}

		private static void CopyValuesFrom(EventSystem fromEventSystem, EventSystem toEventSystem)
		{
			FieldInfo[] eVENT_SYSTEM_FIELD_INFOS = EVENT_SYSTEM_FIELD_INFOS;
			foreach (FieldInfo fieldInfo in eVENT_SYSTEM_FIELD_INFOS)
			{
				fieldInfo.SetValue(toEventSystem, fieldInfo.GetValue(fromEventSystem));
			}
			PropertyInfo[] eVENT_SYSTEM_PROPERTY_INFOS = EVENT_SYSTEM_PROPERTY_INFOS;
			foreach (PropertyInfo propertyInfo in eVENT_SYSTEM_PROPERTY_INFOS)
			{
				if (propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(toEventSystem, propertyInfo.GetValue(fromEventSystem, null), null);
				}
			}
		}

		private static IEnumerator SetEventSystemOfBaseInputModulesAfterFrameDelay(EventSystem eventSystem)
		{
			yield return null;
			SetEventSystemOfBaseInputModules(eventSystem);
		}

		private static void SetEventSystemOfBaseInputModules(EventSystem eventSystem)
		{
			BaseInputModule[] array = Object.FindObjectsOfType<BaseInputModule>();
			foreach (BaseInputModule obj in array)
			{
				BASE_INPUT_MODULE_EVENT_SYSTEM_FIELD_INFO.SetValue(obj, eventSystem);
			}
			eventSystem.UpdateModules();
		}
	}
}
