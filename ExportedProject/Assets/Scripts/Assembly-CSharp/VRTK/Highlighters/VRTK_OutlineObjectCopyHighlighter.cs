using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Highlighters
{
	public class VRTK_OutlineObjectCopyHighlighter : VRTK_BaseHighlighter
	{
		[Tooltip("The thickness of the outline effect")]
		public float thickness = 1f;

		[Tooltip("The GameObjects to use as the model to outline. If one isn't provided then the first GameObject with a valid Renderer in the current GameObject hierarchy will be used.")]
		public GameObject[] customOutlineModels;

		[Tooltip("A path to a GameObject to find at runtime, if the GameObject doesn't exist at edit time.")]
		public string[] customOutlineModelPaths;

		[Tooltip("If the mesh has multiple sub-meshes to highlight then this should be checked, otherwise only the first mesh will be highlighted.")]
		public bool enableSubmeshHighlight;

		protected Material stencilOutline;

		protected GameObject[] highlightModels;

		protected string[] copyComponents = new string[2] { "UnityEngine.MeshFilter", "UnityEngine.MeshRenderer" };

		public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
		{
			usesClonedObject = true;
			if (stencilOutline == null)
			{
				stencilOutline = UnityEngine.Object.Instantiate((Material)Resources.Load("OutlineBasic"));
			}
			SetOptions(options);
			ResetHighlighter();
		}

		public override void ResetHighlighter()
		{
			DeleteExistingHighlightModels();
			ResetHighlighterWithCustomModelPaths();
			ResetHighlighterWithCustomModels();
			ResetHighlightersWithCurrentGameObject();
		}

		public override void Highlight(Color? color, float duration = 0f)
		{
			if (highlightModels == null || highlightModels.Length <= 0)
			{
				return;
			}
			stencilOutline.SetFloat("_Thickness", thickness);
			stencilOutline.SetColor("_OutlineColor", color.Value);
			for (int i = 0; i < highlightModels.Length; i++)
			{
				if ((bool)highlightModels[i])
				{
					highlightModels[i].SetActive(true);
				}
			}
		}

		public override void Unhighlight(Color? color = null, float duration = 0f)
		{
			if (highlightModels == null)
			{
				return;
			}
			for (int i = 0; i < highlightModels.Length; i++)
			{
				if ((bool)highlightModels[i])
				{
					highlightModels[i].SetActive(false);
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (customOutlineModels == null)
			{
				customOutlineModels = new GameObject[0];
			}
			if (customOutlineModelPaths == null)
			{
				customOutlineModelPaths = new string[0];
			}
		}

		protected virtual void OnDestroy()
		{
			if (highlightModels != null)
			{
				for (int i = 0; i < highlightModels.Length; i++)
				{
					if ((bool)highlightModels[i])
					{
						UnityEngine.Object.Destroy(highlightModels[i]);
					}
				}
			}
			UnityEngine.Object.Destroy(stencilOutline);
		}

		protected virtual void ResetHighlighterWithCustomModels()
		{
			if (customOutlineModels != null && customOutlineModels.Length > 0)
			{
				highlightModels = new GameObject[customOutlineModels.Length];
				for (int i = 0; i < customOutlineModels.Length; i++)
				{
					highlightModels[i] = CreateHighlightModel(customOutlineModels[i], string.Empty);
				}
			}
		}

		protected virtual void ResetHighlighterWithCustomModelPaths()
		{
			if (customOutlineModelPaths != null && customOutlineModelPaths.Length > 0)
			{
				highlightModels = new GameObject[customOutlineModels.Length];
				for (int i = 0; i < customOutlineModelPaths.Length; i++)
				{
					highlightModels[i] = CreateHighlightModel(null, customOutlineModelPaths[i]);
				}
			}
		}

		protected virtual void ResetHighlightersWithCurrentGameObject()
		{
			if (highlightModels == null || highlightModels.Length == 0)
			{
				highlightModels = new GameObject[1];
				highlightModels[0] = CreateHighlightModel(null, string.Empty);
			}
		}

		protected virtual void SetOptions(Dictionary<string, object> options = null)
		{
			float option = GetOption<float>(options, "thickness");
			if (option > 0f)
			{
				thickness = option;
			}
			GameObject[] option2 = GetOption<GameObject[]>(options, "customOutlineModels");
			if (option2 != null)
			{
				customOutlineModels = option2;
			}
			string[] option3 = GetOption<string[]>(options, "customOutlineModelPaths");
			if (option3 != null)
			{
				customOutlineModelPaths = option3;
			}
		}

		protected virtual void DeleteExistingHighlightModels()
		{
			VRTK_PlayerObject[] componentsInChildren = GetComponentsInChildren<VRTK_PlayerObject>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].objectType == VRTK_PlayerObject.ObjectTypes.Highlighter)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
				}
			}
		}

		protected virtual GameObject CreateHighlightModel(GameObject givenOutlineModel, string givenOutlineModelPath)
		{
			if (givenOutlineModel != null)
			{
				givenOutlineModel = ((!givenOutlineModel.GetComponent<Renderer>()) ? givenOutlineModel.GetComponentInChildren<Renderer>().gameObject : givenOutlineModel);
			}
			else if (givenOutlineModelPath != string.Empty)
			{
				Transform transform = base.transform.Find(givenOutlineModelPath);
				givenOutlineModel = ((!transform) ? null : transform.gameObject);
			}
			GameObject gameObject = givenOutlineModel;
			if (gameObject == null)
			{
				gameObject = ((!GetComponent<Renderer>()) ? GetComponentInChildren<Renderer>().gameObject : base.gameObject);
			}
			if (gameObject == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_OutlineObjectCopyHighlighter", "Renderer", "the same or child", " to add the highlighter to"));
				return null;
			}
			GameObject gameObject2 = new GameObject(base.name + "_HighlightModel");
			gameObject2.transform.SetParent(gameObject.transform.parent, false);
			gameObject2.transform.localPosition = gameObject.transform.localPosition;
			gameObject2.transform.localRotation = gameObject.transform.localRotation;
			gameObject2.transform.localScale = gameObject.transform.localScale;
			gameObject2.transform.SetParent(base.transform);
			Component[] components = gameObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (Array.IndexOf(copyComponents, component.GetType().ToString()) >= 0)
				{
					VRTK_SharedMethods.CloneComponent(component, gameObject2);
				}
			}
			MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
			MeshFilter component3 = gameObject2.GetComponent<MeshFilter>();
			if ((bool)component3)
			{
				if (enableSubmeshHighlight)
				{
					List<CombineInstance> list = new List<CombineInstance>();
					for (int j = 0; j < component2.mesh.subMeshCount; j++)
					{
						CombineInstance item = default(CombineInstance);
						item.mesh = component2.mesh;
						item.subMeshIndex = j;
						item.transform = component2.transform.localToWorldMatrix;
						list.Add(item);
					}
					component3.mesh = new Mesh();
					component3.mesh.CombineMeshes(list.ToArray(), true, false);
				}
				else
				{
					component3.mesh = component2.mesh;
				}
				gameObject2.GetComponent<Renderer>().material = stencilOutline;
			}
			gameObject2.SetActive(false);
			VRTK_PlayerObject.SetPlayerObject(gameObject2, VRTK_PlayerObject.ObjectTypes.Highlighter);
			return gameObject2;
		}
	}
}
