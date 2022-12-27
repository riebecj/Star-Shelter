namespace VRTK.SecondaryControllerGrabActions
{
	public class VRTK_SwapControllerGrabAction : VRTK_BaseGrabAction
	{
		protected virtual void Awake()
		{
			isActionable = false;
			isSwappable = true;
		}
	}
}
