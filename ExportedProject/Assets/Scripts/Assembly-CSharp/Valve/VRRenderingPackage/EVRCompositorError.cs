namespace Valve.VRRenderingPackage
{
	public enum EVRCompositorError
	{
		None = 0,
		IncompatibleVersion = 100,
		DoNotHaveFocus = 101,
		InvalidTexture = 102,
		IsNotSceneApplication = 103,
		TextureIsOnWrongDevice = 104,
		TextureUsesUnsupportedFormat = 105,
		SharedTexturesNotSupported = 106,
		IndexOutOfRange = 107
	}
}