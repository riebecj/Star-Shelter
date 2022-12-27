using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Highlighters
{
	public abstract class VRTK_BaseHighlighter : MonoBehaviour
	{
		[Tooltip("Determines if this highlighter is the active highlighter for the object the component is attached to. Only 1 active highlighter can be applied to a game object.")]
		public bool active = true;

		[Tooltip("Determines if the highlighted object should be unhighlighted when it is disabled.")]
		public bool unhighlightOnDisable = true;

		protected bool usesClonedObject;

		public abstract void Initialise(Color? color = null, Dictionary<string, object> options = null);

		public abstract void ResetHighlighter();

		public abstract void Highlight(Color? color = null, float duration = 0f);

		public abstract void Unhighlight(Color? color = null, float duration = 0f);

		public virtual T GetOption<T>(Dictionary<string, object> options, string key)
		{
			if (options != null && options.ContainsKey(key) && options[key] != null)
			{
				return (T)options[key];
			}
			return default(T);
		}

		public virtual bool UsesClonedObject()
		{
			return usesClonedObject;
		}

		public static VRTK_BaseHighlighter GetActiveHighlighter(GameObject obj)
		{
			VRTK_BaseHighlighter result = null;
			VRTK_BaseHighlighter[] components = obj.GetComponents<VRTK_BaseHighlighter>();
			foreach (VRTK_BaseHighlighter vRTK_BaseHighlighter in components)
			{
				if (vRTK_BaseHighlighter.active)
				{
					result = vRTK_BaseHighlighter;
					break;
				}
			}
			return result;
		}

		protected virtual void OnDisable()
		{
			if (unhighlightOnDisable)
			{
				Unhighlight();
			}
		}
	}
}
