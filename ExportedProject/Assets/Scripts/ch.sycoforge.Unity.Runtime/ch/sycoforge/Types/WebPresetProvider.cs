using UnityEngine;

namespace ch.sycoforge.Types
{
	public sealed class WebPresetProvider : ScriptableObject, IGridItem
	{
		[SerializeField]
		private string appSKU;

		[SerializeField]
		private string app;

		[SerializeField]
		private string version;

		[SerializeField]
		private string browserTypeName;

		[SerializeField]
		private string rendererTypeName;

		[SerializeField]
		private string presetTypeName;

		[SerializeField]
		private Texture2D thumbnail;

		public string DsiplayName
		{
			get
			{
				return app;
			}
			set
			{
				app = value;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public string AppSKU
		{
			get
			{
				return appSKU;
			}
			set
			{
				appSKU = value;
			}
		}

		public string App
		{
			get
			{
				return app;
			}
			set
			{
				app = value;
			}
		}

		public string BrowserTypeName
		{
			get
			{
				return browserTypeName;
			}
			set
			{
				browserTypeName = value;
			}
		}

		public string RendererTypeName
		{
			get
			{
				return rendererTypeName;
			}
			set
			{
				rendererTypeName = value;
			}
		}

		public string PresetTypeName
		{
			get
			{
				return presetTypeName;
			}
			set
			{
				presetTypeName = value;
			}
		}

		public Texture2D Thumbnail
		{
			get
			{
				return thumbnail;
			}
			set
			{
				thumbnail = value;
			}
		}
	}
}
