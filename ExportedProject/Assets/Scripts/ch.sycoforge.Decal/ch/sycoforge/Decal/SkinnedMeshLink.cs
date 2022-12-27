using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	public class SkinnedMeshLink : MeshLink
	{
		public List<SkinnedMeshRenderer> SkinnedMeshes = new List<SkinnedMeshRenderer>();

		protected override void Start()
		{
		}
	}
}
