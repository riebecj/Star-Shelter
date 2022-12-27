using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Highlighters
{
	public class VRTK_MaterialColorSwapHighlighter : VRTK_BaseHighlighter
	{
		[Tooltip("The emission colour of the texture will be the highlight colour but this percent darker.")]
		public float emissionDarken = 50f;

		[Tooltip("A custom material to use on the highlighted object.")]
		public Material customMaterial;

		protected Dictionary<string, Material[]> originalSharedRendererMaterials = new Dictionary<string, Material[]>();

		protected Dictionary<string, Material[]> originalRendererMaterials = new Dictionary<string, Material[]>();

		protected Dictionary<string, Coroutine> faderRoutines;

		protected bool resetMainTexture;

		public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
		{
			originalSharedRendererMaterials = new Dictionary<string, Material[]>();
			originalRendererMaterials = new Dictionary<string, Material[]>();
			faderRoutines = new Dictionary<string, Coroutine>();
			resetMainTexture = GetOption<bool>(options, "resetMainTexture");
			ResetHighlighter();
		}

		public override void ResetHighlighter()
		{
			StoreOriginalMaterials();
		}

		public override void Highlight(Color? color, float duration = 0f)
		{
			if (color.HasValue)
			{
				ChangeToHighlightColor(color.Value, duration);
			}
		}

		public override void Unhighlight(Color? color = null, float duration = 0f)
		{
			if (originalRendererMaterials == null)
			{
				return;
			}
			if (faderRoutines != null)
			{
				foreach (KeyValuePair<string, Coroutine> faderRoutine in faderRoutines)
				{
					StopCoroutine(faderRoutine.Value);
				}
				faderRoutines.Clear();
			}
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				string key = renderer.gameObject.GetInstanceID().ToString();
				if (originalRendererMaterials.ContainsKey(key))
				{
					renderer.materials = originalRendererMaterials[key];
					renderer.sharedMaterials = originalSharedRendererMaterials[key];
				}
			}
		}

		protected virtual void StoreOriginalMaterials()
		{
			originalSharedRendererMaterials.Clear();
			originalRendererMaterials.Clear();
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				string key = renderer.gameObject.GetInstanceID().ToString();
				originalSharedRendererMaterials[key] = renderer.sharedMaterials;
				originalRendererMaterials[key] = renderer.materials;
				renderer.sharedMaterials = originalSharedRendererMaterials[key];
			}
		}

		protected virtual void ChangeToHighlightColor(Color color, float duration = 0f)
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				Material[] array = new Material[renderer.materials.Length];
				for (int j = 0; j < renderer.materials.Length; j++)
				{
					Material material = renderer.materials[j];
					if ((bool)customMaterial)
					{
						material = (array[j] = customMaterial);
					}
					string key = material.GetInstanceID().ToString();
					if (faderRoutines.ContainsKey(key) && faderRoutines[key] != null)
					{
						StopCoroutine(faderRoutines[key]);
						faderRoutines.Remove(key);
					}
					material.EnableKeyword("_EMISSION");
					if (resetMainTexture && material.HasProperty("_MainTex"))
					{
						renderer.material.SetTexture("_MainTex", new Texture());
					}
					if (!material.HasProperty("_Color"))
					{
						continue;
					}
					if (duration > 0f)
					{
						faderRoutines[key] = StartCoroutine(CycleColor(material, material.color, color, duration));
						continue;
					}
					material.color = color;
					if (material.HasProperty("_EmissionColor"))
					{
						material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, emissionDarken));
					}
				}
				if ((bool)customMaterial)
				{
					renderer.materials = array;
				}
			}
		}

		protected virtual IEnumerator CycleColor(Material material, Color startColor, Color endColor, float duration)
		{
			float elapsedTime = 0f;
			while (elapsedTime <= duration)
			{
				elapsedTime += Time.deltaTime;
				if (material.HasProperty("_Color"))
				{
					material.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
				}
				if (material.HasProperty("_EmissionColor"))
				{
					material.SetColor("_EmissionColor", Color.Lerp(startColor, VRTK_SharedMethods.ColorDarken(endColor, emissionDarken), elapsedTime / duration));
				}
				yield return null;
			}
		}
	}
}
