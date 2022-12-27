namespace VRTK
{
	[SDK_Description("OculusVR", "VRTK_DEFINE_SDK_OCULUSVR")]
	public class SDK_OculusVRSystem : SDK_BaseSystem
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
