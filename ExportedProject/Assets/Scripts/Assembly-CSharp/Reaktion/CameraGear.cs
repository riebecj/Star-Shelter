using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Gear/Camera Gear")]
	public class CameraGear : MonoBehaviour
	{
		public ReaktorLink reaktor;

		public Modifier fieldOfView = Modifier.Linear(60f, 45f);

		public Modifier viewportWidth = Modifier.Linear(1f, 0.2f);

		public Modifier viewportHeight = Modifier.Linear(1f, 0.2f);

		private void Awake()
		{
			reaktor.Initialize(this);
		}

		private void Update()
		{
			if (fieldOfView.enabled)
			{
				GetComponent<Camera>().fieldOfView = fieldOfView.Evaluate(reaktor.Output);
			}
			if (viewportWidth.enabled || viewportHeight.enabled)
			{
				Rect rect = GetComponent<Camera>().rect;
				if (viewportWidth.enabled)
				{
					rect.width = viewportWidth.Evaluate(reaktor.Output);
					rect.x = (1f - rect.width) * 0.5f;
				}
				if (viewportHeight.enabled)
				{
					rect.height = viewportHeight.Evaluate(reaktor.Output);
					rect.y = (1f - rect.height) * 0.5f;
				}
				GetComponent<Camera>().rect = rect;
			}
		}
	}
}
