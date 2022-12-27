using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phonon
{
	[AddComponentMenu("Phonon/Probe Box")]
	public class ProbeBox : MonoBehaviour
	{
		public ProbePlacementStrategy placementStrategy;

		[Range(0.1f, 50f)]
		public float horizontalSpacing = 5f;

		[Range(0.1f, 20f)]
		public float heightAboveFloor = 1.5f;

		[Range(1f, 1024f)]
		public int maxNumTriangles = 64;

		[Range(1f, 10f)]
		public int maxOctreeDepth = 2;

		public byte[] probeBoxData;

		public float[] probeSpherePoints;

		public float[] probeSphereRadii;

		public List<string> probeDataName = new List<string>();

		public List<int> probeDataNameSizes = new List<int>();

		private void OnDrawGizmosSelected()
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(base.gameObject.transform.position, base.gameObject.transform.localScale);
			float num = 0.1f;
			Gizmos.color = Color.yellow;
			if (probeSpherePoints != null)
			{
				for (int i = 0; i < probeSpherePoints.Length / 3; i++)
				{
					UnityEngine.Vector3 center = new UnityEngine.Vector3(probeSpherePoints[3 * i], probeSpherePoints[3 * i + 1], 0f - probeSpherePoints[3 * i + 2]);
					Gizmos.DrawCube(center, new UnityEngine.Vector3(num, num, num));
				}
			}
			Gizmos.color = color;
		}

		public void GenerateProbes()
		{
			ProbePlacementParameters placementParams = default(ProbePlacementParameters);
			placementParams.placement = placementStrategy;
			placementParams.maxNumTriangles = maxNumTriangles;
			placementParams.maxOctreeDepth = maxOctreeDepth;
			placementParams.horizontalSpacing = horizontalSpacing;
			placementParams.heightAboveFloor = heightAboveFloor;
			PhononManager phononManager;
			PhononManagerContainer phononManagerContainer;
			try
			{
				phononManager = UnityEngine.Object.FindObjectOfType<PhononManager>();
				if (phononManager == null)
				{
					throw new Exception("Phonon Manager Settings object not found in the scene! Click Window > Phonon");
				}
				bool initializeRenderer = false;
				phononManager.Initialize(initializeRenderer);
				phononManagerContainer = phononManager.PhononManagerContainer();
				phononManagerContainer.Initialize(initializeRenderer, phononManager);
				if (phononManagerContainer.Scene().GetScene() == IntPtr.Zero)
				{
					Debug.LogError("Scene not found. Make sure to pre-export the scene.");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return;
			}
			IntPtr probeBox = IntPtr.Zero;
			Box box = default(Box);
			box.minCoordinates = Common.ConvertVector(base.gameObject.transform.position);
			box.maxCoordinates = Common.ConvertVector(base.gameObject.transform.position);
			box.minCoordinates.x -= base.gameObject.transform.localScale.x / 2f;
			box.minCoordinates.y -= base.gameObject.transform.localScale.y / 2f;
			box.minCoordinates.z -= base.gameObject.transform.localScale.z / 2f;
			box.maxCoordinates.x += base.gameObject.transform.localScale.x / 2f;
			box.maxCoordinates.y += base.gameObject.transform.localScale.y / 2f;
			box.maxCoordinates.z += base.gameObject.transform.localScale.z / 2f;
			PhononCore.iplCreateProbeBox(phononManagerContainer.Scene().GetScene(), box, placementParams, null, ref probeBox);
			int num = PhononCore.iplGetProbeSpheres(probeBox, null);
			probeSpherePoints = new float[3 * num];
			probeSphereRadii = new float[num];
			Sphere[] array = new Sphere[num];
			PhononCore.iplGetProbeSpheres(probeBox, array);
			for (int i = 0; i < num; i++)
			{
				probeSpherePoints[3 * i] = array[i].centerx;
				probeSpherePoints[3 * i + 1] = array[i].centery;
				probeSpherePoints[3 * i + 2] = array[i].centerz;
				probeSphereRadii[i] = array[i].radius;
			}
			int num2 = PhononCore.iplSaveProbeBox(probeBox, null);
			probeBoxData = new byte[num2];
			PhononCore.iplSaveProbeBox(probeBox, probeBoxData);
			if (phononManagerContainer.Scene().GetScene() != IntPtr.Zero)
			{
				Debug.Log("Generated " + array.Length + " probes for game object " + base.gameObject.name + ".");
			}
			PhononCore.iplDestroyProbeBox(ref probeBox);
			phononManager.Destroy();
			phononManagerContainer.Destroy();
			ClearProbeDataMapping();
		}

		public void DeleteBakedDataByName(string name)
		{
			IntPtr probeBox = IntPtr.Zero;
			try
			{
				PhononCore.iplLoadProbeBox(probeBoxData, probeBoxData.Length, ref probeBox);
				PhononCore.iplDeleteBakedDataByName(probeBox, Common.ConvertString(name));
				UpdateProbeDataMapping(name, -1);
				int num = PhononCore.iplSaveProbeBox(probeBox, null);
				probeBoxData = new byte[num];
				PhononCore.iplSaveProbeBox(probeBox, probeBoxData);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
			}
		}

		public void UpdateProbeDataMapping(string name, int size)
		{
			int num = probeDataName.IndexOf(name);
			if (size == -1 && num >= 0)
			{
				probeDataName.RemoveAt(num);
				probeDataNameSizes.RemoveAt(num);
			}
			else if (num == -1)
			{
				probeDataName.Add(name);
				probeDataNameSizes.Add(size);
			}
			else
			{
				probeDataNameSizes[num] = size;
			}
		}

		private void ClearProbeDataMapping()
		{
			probeDataName.Clear();
			probeDataNameSizes.Clear();
		}
	}
}
