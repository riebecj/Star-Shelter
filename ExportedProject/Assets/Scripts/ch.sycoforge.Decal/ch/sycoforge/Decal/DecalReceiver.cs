using UnityEngine;

namespace ch.sycoforge.Decal
{
	public class DecalReceiver : MonoBehaviour
	{
		public int MaxDecals = 25;

		public float DepthStep = 0.0001f;

		internal int receivedAmount = 0;

		internal float sum;

		internal float fractionsum = 0.01f;

		internal float step = 1f;

		internal EasyDecal LastDecal;

		public void Receive(EasyDecal decal)
		{
			if (receivedAmount <= MaxDecals)
			{
				LastDecal = decal;
				if (receivedAmount == 0)
				{
					sum = decal.Distance;
					fractionsum = decal.Distance;
				}
				receivedAmount++;
				sum += step;
				fractionsum += DepthStep;
			}
		}
	}
}
