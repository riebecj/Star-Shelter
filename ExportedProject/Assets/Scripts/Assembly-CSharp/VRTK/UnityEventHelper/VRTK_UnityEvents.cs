using UnityEngine;

namespace VRTK.UnityEventHelper
{
	public abstract class VRTK_UnityEvents<T> : MonoBehaviour where T : Component
	{
		private T component;

		protected abstract void AddListeners(T component);

		protected abstract void RemoveListeners(T component);

		protected virtual void OnEnable()
		{
			component = GetComponent<T>();
			if ((Object)component != (Object)null)
			{
				AddListeners(component);
				return;
			}
			string arg = GetType().Name;
			string arg2 = component.GetType().Name;
			VRTK_Logger.Error(string.Format("The {0} script requires to be attached to a GameObject that contains a {1} script.", arg, arg2));
		}

		protected virtual void OnDisable()
		{
			if ((Object)component != (Object)null)
			{
				RemoveListeners(component);
			}
		}
	}
}
