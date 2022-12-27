using OscJack;
using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Injector/OSC Injector")]
	public class OscInjector : InjectorBase
	{
		public enum ScaleMode
		{
			Linear01 = 0,
			Decibel = 1
		}

		public ScaleMode scaleMode;

		public string address = "/reaktion";

		public int dataIndex;

		private void Update()
		{
			object[] data = OscMaster.GetData(address);
			if (data != null)
			{
				float num = (float)data[dataIndex];
				if (scaleMode == ScaleMode.Linear01)
				{
					num = Mathf.Clamp01(num);
					dbLevel = Mathf.Log(num / 0.70710677f + 1.5849E-13f, 10f) * 20f;
				}
				else
				{
					dbLevel = Mathf.Min(num, 0f);
				}
			}
		}
	}
}
