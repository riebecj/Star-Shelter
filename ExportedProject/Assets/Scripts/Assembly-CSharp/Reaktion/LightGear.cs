using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Light Gear")]
	public class LightGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public Modifier intensity;

		public bool enableColor;

		public Gradient colorGradient;

		private void Awake()
		{
			reaktor.Initialize(this);
			UpdateLight(0f);
		}

		private void Update()
		{
			UpdateLight(reaktor.Output);
		}

		private void UpdateLight(float param)
		{
			if (intensity.enabled)
			{
				GetComponent<Light>().intensity = intensity.Evaluate(param);
			}
			if (enableColor)
			{
				GetComponent<Light>().color = colorGradient.Evaluate(param);
			}
		}
	}
}
