using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution
{
	public class BezierWalkerWithSpeed : MonoBehaviour, IBezierWalker
	{
		public enum TravelMode
		{
			Once = 0,
			Loop = 1,
			PingPong = 2
		}

		private Transform cachedTransform;

		public BezierSpline spline;

		public TravelMode travelMode;

		public float speed = 5f;

		private float progress;

		public float rotationLerpModifier = 10f;

		public bool lookForward = true;

		private bool isGoingForward = true;

		public UnityEvent onPathCompleted = new UnityEvent();

		private bool onPathCompletedCalledAt1;

		private bool onPathCompletedCalledAt0;

		public BezierSpline Spline
		{
			get
			{
				return spline;
			}
		}

		public float NormalizedT
		{
			get
			{
				return progress;
			}
			set
			{
				progress = value;
			}
		}

		public bool MovingForward
		{
			get
			{
				return speed > 0f == isGoingForward;
			}
		}

		private void Awake()
		{
			cachedTransform = base.transform;
		}

		private void Update()
		{
			float num = ((!isGoingForward) ? (0f - speed) : speed);
			Vector3 position = spline.MoveAlongSpline(ref progress, num * Time.deltaTime);
			cachedTransform.position = position;
			bool movingForward = MovingForward;
			if (lookForward)
			{
				Quaternion b = ((!movingForward) ? Quaternion.LookRotation(-spline.GetTangent(progress)) : Quaternion.LookRotation(spline.GetTangent(progress)));
				cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, b, rotationLerpModifier * Time.deltaTime);
			}
			if (movingForward)
			{
				if (progress >= 1f)
				{
					if (!onPathCompletedCalledAt1)
					{
						onPathCompleted.Invoke();
						onPathCompletedCalledAt1 = true;
					}
					if (travelMode == TravelMode.Once)
					{
						progress = 1f;
						return;
					}
					if (travelMode == TravelMode.Loop)
					{
						progress -= 1f;
						return;
					}
					progress = 2f - progress;
					isGoingForward = !isGoingForward;
				}
				else
				{
					onPathCompletedCalledAt1 = false;
				}
			}
			else if (progress <= 0f)
			{
				if (!onPathCompletedCalledAt0)
				{
					onPathCompleted.Invoke();
					onPathCompletedCalledAt0 = true;
				}
				if (travelMode == TravelMode.Once)
				{
					progress = 0f;
					return;
				}
				if (travelMode == TravelMode.Loop)
				{
					progress += 1f;
					return;
				}
				progress = 0f - progress;
				isGoingForward = !isGoingForward;
			}
			else
			{
				onPathCompletedCalledAt0 = false;
			}
		}
	}
}
