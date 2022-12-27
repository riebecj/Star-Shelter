using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class AssetListAttribute : Attribute
	{
		public bool AutoPopulate { get; set; }

		public string Tags { get; set; }

		public string LayerNames { get; set; }

		public string AssetNamePrefix { get; set; }

		public string Path { get; set; }

		public string CustomFilterMethod { get; set; }

		public AssetListAttribute()
		{
			AutoPopulate = false;
			Tags = null;
			LayerNames = null;
			AssetNamePrefix = null;
			CustomFilterMethod = null;
		}
	}
}
