using UnityEngine;

namespace ch.sycoforge.Types
{
	public interface IPresetThumbRenderer
	{
		Texture2D RenderThumbnail(BasePreset preset);
	}
}
