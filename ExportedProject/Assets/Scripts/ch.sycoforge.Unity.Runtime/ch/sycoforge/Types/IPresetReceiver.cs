namespace ch.sycoforge.Types
{
	public interface IPresetReceiver<P> where P : BasePreset
	{
		P Preset { get; set; }
	}
}
