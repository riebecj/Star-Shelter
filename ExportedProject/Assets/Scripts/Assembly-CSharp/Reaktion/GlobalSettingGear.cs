using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Global Setting Gear")]
	public class GlobalSettingGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public AnimationCurve timeScaleCurve = AnimationCurve.Linear(0f, 0.2f, 1f, 1f);

		private void OnDisable()
		{
			Time.timeScale = 1f;
		}

		private void Awake()
		{
			reaktor.Initialize(this);
		}

		private void Update()
		{
			Time.timeScale = timeScaleCurve.Evaluate(reaktor.Output);
		}
	}
}
