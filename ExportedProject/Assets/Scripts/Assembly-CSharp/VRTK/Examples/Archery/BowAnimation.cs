using UnityEngine;

namespace VRTK.Examples.Archery
{
	public class BowAnimation : MonoBehaviour
	{
		public Animation animationTimeline;

		public void SetFrame(float frame)
		{
			animationTimeline["BowPullAnimation"].speed = 0f;
			animationTimeline["BowPullAnimation"].time = frame;
			animationTimeline.Play("BowPullAnimation");
		}
	}
}
