using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
	[RequireComponent(typeof(CarController))]
	public class CarUserControl : MonoBehaviour
	{
		private CarController m_Car;

		private void Awake()
		{
			m_Car = GetComponent<CarController>();
		}

		private void FixedUpdate()
		{
		}
	}
}
