using UnityEngine;

namespace VRTK
{
	public class VRTK_InstanceMethods : MonoBehaviour
	{
		public static VRTK_InstanceMethods instance;

		public VRTK_Haptics haptics;

		public VRTK_ObjectAppearance objectAppearance;

		protected virtual void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			VRTK_SDKManager vRTK_SDKManager = VRTK_SDKManager.instance;
			if (vRTK_SDKManager != null && vRTK_SDKManager.persistOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			haptics = base.gameObject.AddComponent<VRTK_Haptics>();
			objectAppearance = base.gameObject.AddComponent<VRTK_ObjectAppearance>();
		}
	}
}
