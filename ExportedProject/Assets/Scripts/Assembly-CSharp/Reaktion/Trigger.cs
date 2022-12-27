using System;
using UnityEngine;

namespace Reaktion
{
	[Serializable]
	public class Trigger
	{
		public bool enabled;

		public float threshold = 0.5f;

		public float interval = 0.1f;

		private float previous;

		private float timer;

		public bool Update(float current)
		{
			if (!enabled)
			{
				return false;
			}
			if (timer <= 0f && current >= threshold && previous < threshold)
			{
				timer = interval;
				previous = current;
				return true;
			}
			timer -= Time.deltaTime;
			previous = current;
			return false;
		}
	}
}
