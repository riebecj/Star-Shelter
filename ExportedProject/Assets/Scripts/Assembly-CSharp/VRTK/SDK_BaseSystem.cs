namespace VRTK
{
	public abstract class SDK_BaseSystem : SDK_Base
	{
		public abstract bool IsDisplayOnDesktop();

		public abstract bool ShouldAppRenderWithLowResources();

		public abstract void ForceInterleavedReprojectionOn(bool force);
	}
}
