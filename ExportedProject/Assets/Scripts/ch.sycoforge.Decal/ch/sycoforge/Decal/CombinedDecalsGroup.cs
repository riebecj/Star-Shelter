using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	[AddComponentMenu("")]
	public class CombinedDecalsGroup : MonoBehaviour
	{
		[SerializeField]
		private DecalTextureAtlas atlas;

		[SerializeField]
		private List<EasyDecal> references = new List<EasyDecal>();

		public DecalTextureAtlas Atlas
		{
			get
			{
				return atlas;
			}
			set
			{
				atlas = value;
			}
		}

		public List<EasyDecal> References
		{
			get
			{
				return references;
			}
			set
			{
				references = value;
			}
		}
	}
}
