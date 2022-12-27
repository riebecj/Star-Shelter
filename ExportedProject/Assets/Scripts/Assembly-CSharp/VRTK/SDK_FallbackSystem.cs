namespace VRTK
{
	public class SDK_FallbackSystem : SDK_BaseSystem
	{
		public override bool IsDisplayOnDesktop()
		{
			return false;
		}

		public override bool ShouldAppRenderWithLowResources()
		{
			return false;
		}

		public override void ForceInterleavedReprojectionOn(bool force)
		{
		}
	}
}
