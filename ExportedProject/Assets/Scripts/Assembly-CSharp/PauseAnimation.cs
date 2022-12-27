using UnityEngine;

public class PauseAnimation : MonoBehaviour
{
	public Animation anim;

	public string animationName;

	public void OnToggle(ToggleTrigger button)
	{
		if (button.On)
		{
			Resume();
		}
		else
		{
			Pause();
		}
	}

	public void Pause()
	{
		anim[animationName].speed = 0f;
	}

	public void Resume()
	{
		if (!anim.isPlaying)
		{
			anim.Play();
		}
		anim[animationName].speed = 1f;
	}
}
