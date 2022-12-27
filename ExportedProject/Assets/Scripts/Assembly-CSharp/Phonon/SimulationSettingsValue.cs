using System;
using UnityEngine;

namespace Phonon
{
	[Serializable]
	public class SimulationSettingsValue
	{
		[Range(512f, 16384f)]
		public int RealtimeRays;

		[Range(128f, 4096f)]
		public int RealtimeSecondaryRays;

		[Range(1f, 32f)]
		public int RealtimeBounces;

		[Range(8192f, 65536f)]
		public int BakeRays;

		[Range(1024f, 16384f)]
		public int BakeSecondaryRays;

		[Range(16f, 256f)]
		public int BakeBounces;

		[Range(0.1f, 5f)]
		public float Duration;

		[Range(0f, 4f)]
		public int AmbisonicsOrder;

		[Range(1f, 128f)]
		public int MaxSources = 32;

		public SimulationSettingsValue()
		{
		}

		public SimulationSettingsValue(int realtimeRays, int realtimeSecondaryRays, int realtimeBounces, int bakeRays, int bakeSecondaryRays, int bakeBounces, float duration, int ambisonicsOrder, int maxSources)
		{
			RealtimeRays = realtimeRays;
			RealtimeSecondaryRays = realtimeSecondaryRays;
			RealtimeBounces = realtimeBounces;
			BakeRays = bakeRays;
			BakeSecondaryRays = bakeSecondaryRays;
			BakeBounces = bakeBounces;
			Duration = duration;
			AmbisonicsOrder = ambisonicsOrder;
			MaxSources = maxSources;
		}

		public SimulationSettingsValue(SimulationSettingsValue other)
		{
			CopyFrom(other);
		}

		public void CopyFrom(SimulationSettingsValue other)
		{
			RealtimeRays = other.RealtimeRays;
			RealtimeBounces = other.RealtimeBounces;
			RealtimeSecondaryRays = other.RealtimeSecondaryRays;
			BakeRays = other.BakeRays;
			BakeSecondaryRays = other.BakeSecondaryRays;
			BakeBounces = other.BakeBounces;
			Duration = other.Duration;
			AmbisonicsOrder = other.AmbisonicsOrder;
			MaxSources = other.MaxSources;
		}
	}
}
