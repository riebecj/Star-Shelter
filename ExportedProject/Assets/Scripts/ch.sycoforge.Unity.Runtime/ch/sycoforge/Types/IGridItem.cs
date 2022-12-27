using UnityEngine;

namespace ch.sycoforge.Types
{
	public interface IGridItem
	{
		Texture2D Thumbnail { get; set; }

		string DsiplayName { get; set; }
	}
}
