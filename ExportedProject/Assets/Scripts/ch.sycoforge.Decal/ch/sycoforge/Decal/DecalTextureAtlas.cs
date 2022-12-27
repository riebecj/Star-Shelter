using System;
using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	public class DecalTextureAtlas : ScriptableObject
	{
		[SerializeField]
		[HideInInspector]
		private Material material;

		[HideInInspector]
		[SerializeField]
		private List<AtlasRegion> regions = new List<AtlasRegion>();

		public Material Material
		{
			get
			{
				return material;
			}
			set
			{
				material = value;
			}
		}

		public List<AtlasRegion> Regions
		{
			get
			{
				return regions;
			}
			set
			{
				regions = value;
			}
		}

		public event Action<DecalTextureAtlas> OnAtlasChanged;

		public Texture2D GetThumbnail(int index)
		{
			Texture2D result = null;
			if (index < regions.Count)
			{
				result = GetThumbnail(regions[index]);
			}
			return result;
		}

		public Texture2D GetThumbnail(AtlasRegion region)
		{
			Texture2D texture2D = null;
			if (region != null && material != null && material.mainTexture != null && material.mainTexture is Texture2D)
			{
				int num = 64;
				int num2 = Mathf.RoundToInt((float)num * (region.Region.height / region.Region.width));
				texture2D = new Texture2D(num, num2);
				Texture2D texture2D2 = material.mainTexture as Texture2D;
				Color[] array = new Color[num * num2];
				for (int i = 0; i < num; i++)
				{
					float num3 = (float)i / (float)num;
					for (int j = 0; j < num2; j++)
					{
						float num4 = (float)j / (float)num2;
						Color pixelBilinear = texture2D2.GetPixelBilinear(num3 + region.Region.xMin, num4 + region.Region.yMax);
						texture2D.SetPixel(i, j, pixelBilinear);
					}
				}
				texture2D.Apply();
			}
			return texture2D;
		}

		public void CallOnAtlasChanged()
		{
			if (this.OnAtlasChanged != null)
			{
				this.OnAtlasChanged(this);
			}
		}
	}
}
