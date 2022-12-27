using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class SDK_ControllerSim : MonoBehaviour
	{
		private Vector3 lastPos;

		private Vector3 lastRot;

		private List<Vector3> posList;

		private List<Vector3> rotList;

		private bool selected;

		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
			}
		}

		public Vector3 GetVelocity()
		{
			Vector3 zero = Vector3.zero;
			foreach (Vector3 pos in posList)
			{
				zero += pos;
			}
			return zero / posList.Count;
		}

		public Vector3 GetAngularVelocity()
		{
			Vector3 zero = Vector3.zero;
			foreach (Vector3 rot in rotList)
			{
				zero += rot;
			}
			return zero / rotList.Count;
		}

		private void Awake()
		{
			posList = new List<Vector3>();
			rotList = new List<Vector3>();
			lastPos = base.transform.position;
			lastRot = base.transform.rotation.eulerAngles;
		}

		private void Update()
		{
			posList.Add((base.transform.position - lastPos) / Time.deltaTime);
			if (posList.Count > 10)
			{
				posList.RemoveAt(0);
			}
			rotList.Add(Quaternion.FromToRotation(lastRot, base.transform.rotation.eulerAngles).eulerAngles / Time.deltaTime);
			if (rotList.Count > 10)
			{
				rotList.RemoveAt(0);
			}
			lastPos = base.transform.position;
			lastRot = base.transform.rotation.eulerAngles;
		}
	}
}
