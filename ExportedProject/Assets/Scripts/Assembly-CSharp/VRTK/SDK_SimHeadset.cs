using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	[SDK_Description(typeof(SDK_SimSystem))]
	public class SDK_SimHeadset : SDK_BaseHeadset
	{
		private Transform camera;

		private Vector3 lastPos;

		private Vector3 lastRot;

		private List<Vector3> posList;

		private List<Vector3> rotList;

		public override void ProcessUpdate(Dictionary<string, object> options)
		{
			posList.Add((camera.position - lastPos) / Time.deltaTime);
			if (posList.Count > 10)
			{
				posList.RemoveAt(0);
			}
			rotList.Add(Quaternion.FromToRotation(lastRot, camera.rotation.eulerAngles).eulerAngles / Time.deltaTime);
			if (rotList.Count > 10)
			{
				rotList.RemoveAt(0);
			}
			lastPos = camera.position;
			lastRot = camera.rotation.eulerAngles;
		}

		public override void ProcessFixedUpdate(Dictionary<string, object> options)
		{
		}

		public override Transform GetHeadset()
		{
			if (camera == null)
			{
				GameObject gameObject = SDK_InputSimulator.FindInScene();
				if ((bool)gameObject)
				{
					camera = gameObject.transform.Find("Camera");
				}
			}
			return camera;
		}

		public override Transform GetHeadsetCamera()
		{
			return GetHeadset();
		}

		public override Vector3 GetHeadsetVelocity()
		{
			Vector3 zero = Vector3.zero;
			foreach (Vector3 pos in posList)
			{
				zero += pos;
			}
			return zero / posList.Count;
		}

		public override Vector3 GetHeadsetAngularVelocity()
		{
			Vector3 zero = Vector3.zero;
			foreach (Vector3 rot in rotList)
			{
				zero += rot;
			}
			return zero / rotList.Count;
		}

		public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
		{
			VRTK_ScreenFade.Start(color, duration);
		}

		public override bool HasHeadsetFade(Transform obj)
		{
			return obj.GetComponentInChildren<VRTK_ScreenFade>() != null;
		}

		public override void AddHeadsetFade(Transform camera)
		{
			if (camera != null && camera.GetComponent<VRTK_ScreenFade>() == null)
			{
				camera.gameObject.AddComponent<VRTK_ScreenFade>();
			}
		}

		private void Awake()
		{
			posList = new List<Vector3>();
			rotList = new List<Vector3>();
			Transform headset = GetHeadset();
			if (headset != null)
			{
				lastPos = headset.position;
				lastRot = headset.rotation.eulerAngles;
			}
		}
	}
}
