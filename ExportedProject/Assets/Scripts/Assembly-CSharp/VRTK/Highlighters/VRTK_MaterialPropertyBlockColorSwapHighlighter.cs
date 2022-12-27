using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Highlighters
{
	public class VRTK_MaterialPropertyBlockColorSwapHighlighter : VRTK_MaterialColorSwapHighlighter
	{
		protected Dictionary<string, MaterialPropertyBlock> originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();

		protected Dictionary<string, MaterialPropertyBlock> highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();

		public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
		{
			originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
			highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
			base.Initialise(color, options);
		}

		public override void Unhighlight(Color? color = null, float duration = 0f)
		{
			if (originalMaterialPropertyBlocks == null)
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
				if (originalMaterialPropertyBlocks.ContainsKey(key))
				{
					renderer.SetPropertyBlock(originalMaterialPropertyBlocks[key]);
				}
			}
		}

		protected override void StoreOriginalMaterials()
		{
			originalMaterialPropertyBlocks.Clear();
			highlightMaterialPropertyBlocks.Clear();
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				string key = renderer.gameObject.GetInstanceID().ToString();
				originalMaterialPropertyBlocks[key] = new MaterialPropertyBlock();
				renderer.GetPropertyBlock(originalMaterialPropertyBlocks[key]);
				highlightMaterialPropertyBlocks[key] = new MaterialPropertyBlock();
			}
		}

		protected override void ChangeToHighlightColor(Color color, float duration = 0f)
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				string key = renderer.gameObject.GetInstanceID().ToString();
				if (originalMaterialPropertyBlocks.ContainsKey(key))
				{
					if (faderRoutines.ContainsKey(key) && faderRoutines[key] != null)
					{
						StopCoroutine(faderRoutines[key]);
						faderRoutines.Remove(key);
					}
					MaterialPropertyBlock materialPropertyBlock = highlightMaterialPropertyBlocks[key];
					renderer.GetPropertyBlock(materialPropertyBlock);
					if (resetMainTexture)
					{
						materialPropertyBlock.SetTexture("_MainTex", Texture2D.whiteTexture);
					}
					if (duration > 0f)
					{
						faderRoutines[key] = StartCoroutine(CycleColor(renderer, materialPropertyBlock, color, duration));
						continue;
					}
					materialPropertyBlock.SetColor("_Color", color);
					materialPropertyBlock.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, emissionDarken));
					renderer.SetPropertyBlock(materialPropertyBlock);
				}
			}
		}

		protected virtual IEnumerator CycleColor(Renderer renderer, MaterialPropertyBlock highlightMaterialPropertyBlock, Color endColor, float duration)
		{
			float elapsedTime = 0f;
			while (elapsedTime <= duration)
			{
				elapsedTime += Time.deltaTime;
				Color startColor = highlightMaterialPropertyBlock.GetVector("_Color");
				highlightMaterialPropertyBlock.SetColor("_Color", Color.Lerp(startColor, endColor, elapsedTime / duration));
				highlightMaterialPropertyBlock.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, elapsedTime / duration));
				if (!renderer)
				{
					break;
				}
				renderer.SetPropertyBlock(highlightMaterialPropertyBlock);
				yield return null;
			}
		}
	}
}
