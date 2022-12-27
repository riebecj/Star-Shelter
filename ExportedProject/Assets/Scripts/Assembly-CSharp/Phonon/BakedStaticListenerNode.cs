using System.Collections.Generic;
using UnityEngine;

namespace Phonon
{
	[AddComponentMenu("Phonon/Baked Static Listener Node")]
	public class BakedStaticListenerNode : MonoBehaviour
	{
		public string uniqueIdentifier = string.Empty;

		[Range(1f, 1024f)]
		public float bakingRadius = 16f;

		public bool useAllProbeBoxes;

		public ProbeBox[] probeBoxes;

		public PhononBaker phononBaker = new PhononBaker();

		public List<string> bakedProbeNames = new List<string>();

		public List<int> bakedProbeDataSizes = new List<int>();

		public int bakedDataSize;

		private string bakedListenerPrefix = "__staticlistener__";

		private void OnDrawGizmosSelected()
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.gameObject.transform.position, bakingRadius);
			Gizmos.color = Color.magenta;
			ProbeBox[] array = probeBoxes;
			if (useAllProbeBoxes)
			{
				array = Object.FindObjectsOfType<ProbeBox>();
			}
			if (array != null)
			{
				ProbeBox[] array2 = array;
				foreach (ProbeBox probeBox in array2)
				{
					if (probeBox != null)
					{
						Gizmos.DrawWireCube(probeBox.transform.position, probeBox.transform.localScale);
					}
				}
			}
			Gizmos.color = color;
		}

		public void BeginBake()
		{
			Vector3 vector = Common.ConvertVector(base.gameObject.transform.position);
			Sphere sphere = default(Sphere);
			sphere.centerx = vector.x;
			sphere.centery = vector.y;
			sphere.centerz = vector.z;
			sphere.radius = bakingRadius;
			if (useAllProbeBoxes)
			{
				phononBaker.BeginBake(Object.FindObjectsOfType<ProbeBox>(), BakingMode.StaticListener, uniqueIdentifier, sphere);
			}
			else
			{
				phononBaker.BeginBake(probeBoxes, BakingMode.StaticListener, uniqueIdentifier, sphere);
			}
		}

		public void EndBake()
		{
			phononBaker.EndBake();
		}

		public string GetUniqueIdentifier()
		{
			return bakedListenerPrefix + uniqueIdentifier;
		}

		public void UpdateBakedDataStatistics()
		{
			ProbeBox[] array = probeBoxes;
			if (useAllProbeBoxes)
			{
				array = Object.FindObjectsOfType<ProbeBox>();
			}
			if (array == null)
			{
				return;
			}
			int num = 0;
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			ProbeBox[] array2 = array;
			foreach (ProbeBox probeBox in array2)
			{
				if (probeBox == null || uniqueIdentifier.Length == 0)
				{
					continue;
				}
				int num2 = 0;
				list.Add(probeBox.name);
				for (int j = 0; j < probeBox.probeDataName.Count; j++)
				{
					if (bakedListenerPrefix + uniqueIdentifier == probeBox.probeDataName[j])
					{
						num2 = probeBox.probeDataNameSizes[j];
						num += num2;
					}
				}
				list2.Add(num2);
			}
			bakedDataSize = num;
			bakedProbeNames = list;
			bakedProbeDataSizes = list2;
		}
	}
}
