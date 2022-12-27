using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
	public class BezierWalkerLocomotion : MonoBehaviour, IBezierWalker
	{
		private IBezierWalker walker;

		[SerializeField]
		private List<Transform> tailObjects;

		[SerializeField]
		private List<float> tailObjectDistances;

		public float rotationLerpModifier = 10f;

		public bool lookForward = true;

		public int TailLength
		{
			get
			{
				return tailObjects.Count;
			}
		}

		public Transform this[int index]
		{
			get
			{
				return tailObjects[index];
			}
		}

		public BezierSpline Spline
		{
			get
			{
				return walker.Spline;
			}
		}

		public float NormalizedT
		{
			get
			{
				return walker.NormalizedT;
			}
		}

		public bool MovingForward
		{
			get
			{
				return walker.MovingForward;
			}
		}

		private void Awake()
		{
			IBezierWalker[] components = GetComponents<IBezierWalker>();
			for (int i = 0; i < components.Length; i++)
			{
				if (!(components[i] is BezierWalkerLocomotion) && ((MonoBehaviour)components[i]).enabled)
				{
					walker = components[i];
					break;
				}
			}
			if (walker == null)
			{
				Debug.LogError("Need to attach BezierWalkerLocomotion to an IBezierWalker!");
				Object.Destroy(this);
			}
			if (tailObjects.Count != tailObjectDistances.Count)
			{
				Debug.LogError("One distance per tail object is needed!");
				Object.Destroy(this);
			}
		}

		private void LateUpdate()
		{
			float normalizedT = NormalizedT;
			BezierSpline spline = Spline;
			bool movingForward = MovingForward;
			for (int i = 0; i < tailObjects.Count; i++)
			{
				Transform transform = tailObjects[i];
				float deltaTime = Time.deltaTime;
				if (movingForward)
				{
					transform.position = spline.MoveAlongSpline(ref normalizedT, 0f - tailObjectDistances[i]);
					if (lookForward)
					{
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(spline.GetTangent(normalizedT)), rotationLerpModifier * deltaTime);
					}
				}
				else
				{
					transform.position = spline.MoveAlongSpline(ref normalizedT, tailObjectDistances[i]);
					if (lookForward)
					{
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-spline.GetTangent(normalizedT)), rotationLerpModifier * deltaTime);
					}
				}
			}
		}

		public void AddToTail(Transform transform, float distanceToPreviousObject)
		{
			if (transform == null)
			{
				Debug.LogError("Object is null!");
				return;
			}
			tailObjects.Add(transform);
			tailObjectDistances.Add(distanceToPreviousObject);
		}

		public void InsertIntoTail(int index, Transform transform, float distanceToPreviousObject)
		{
			if (transform == null)
			{
				Debug.LogError("Object is null!");
				return;
			}
			tailObjects.Insert(index, transform);
			tailObjectDistances.Insert(index, distanceToPreviousObject);
		}

		public void RemoveFromTail(Transform transform)
		{
			if (transform == null)
			{
				Debug.LogError("Object is null!");
				return;
			}
			for (int i = 0; i < tailObjects.Count; i++)
			{
				if (tailObjects[i] == transform)
				{
					tailObjects.RemoveAt(i);
					tailObjectDistances.RemoveAt(i);
					break;
				}
			}
		}
	}
}
