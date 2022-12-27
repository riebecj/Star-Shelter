using System;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	[Serializable]
	public class AtlasRegion
	{
		[SerializeField]
		private string name;

		[SerializeField]
		private Rect region;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public Rect Region
		{
			get
			{
				return region;
			}
			set
			{
				region = value;
			}
		}

		public float AspectRatio
		{
			get
			{
				float num = Mathf.Max(region.height, 1E-05f);
				return region.width / num;
			}
		}

		public override bool Equals(object other)
		{
			return Equals(other as AtlasRegion);
		}

		public virtual bool Equals(AtlasRegion other)
		{
			if (other == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			return region == other.region;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
