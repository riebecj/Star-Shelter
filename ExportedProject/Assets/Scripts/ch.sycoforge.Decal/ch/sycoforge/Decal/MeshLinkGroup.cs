using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	public class MeshLinkGroup : MonoBehaviour
	{
		[HideInInspector]
		public List<MeshLink> Links = new List<MeshLink>();

		protected virtual void Start()
		{
			if (Links.Count == 0)
			{
				Links.AddRange(GetComponentsInChildren<MeshLink>());
			}
		}

		public void Clear()
		{
			foreach (MeshLink link in Links)
			{
				Object.DestroyImmediate(link);
			}
			Links.Clear();
		}

		public void Initialize()
		{
			Links.Clear();
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				EasyDecal component = skinnedMeshRenderer.transform.parent.GetComponent<EasyDecal>();
				if (!(component == null))
				{
					continue;
				}
				Transform[] bones = skinnedMeshRenderer.bones;
				foreach (Transform transform in bones)
				{
					GameObject gameObject = transform.gameObject;
					SkinnedMeshLink skinnedMeshLink = gameObject.GetComponent<SkinnedMeshLink>();
					if (skinnedMeshLink == null)
					{
						skinnedMeshLink = gameObject.AddComponent<SkinnedMeshLink>();
					}
					Links.Add(skinnedMeshLink);
					if (!skinnedMeshLink.SkinnedMeshes.Contains(skinnedMeshRenderer))
					{
						skinnedMeshLink.SkinnedMeshes.Add(skinnedMeshRenderer);
					}
				}
			}
		}
	}
}
