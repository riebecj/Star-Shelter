using Valve.VR;

namespace VRTK
{
	[SDK_Description("SteamVR", "VRTK_DEFINE_SDK_STEAMVR")]
	public class SDK_SteamVRSystem : SDK_BaseSystem
	{
		public override bool IsDisplayOnDesktop()
		{
			return OpenVR.System == null || OpenVR.System.IsDisplayOnDesktop();
		}

		public override bool ShouldAppRenderWithLowResources()
		{
			return OpenVR.Compositor != null && OpenVR.Compositor.ShouldAppRenderWithLowResources();
		}

		public override void ForceInterleavedReprojectionOn(bool force)
		{
			if (OpenVR.Compositor != null)
			{
				OpenVR.Compositor.ForceInterleavedReprojectionOn(force);
			}
		}
	}
}
