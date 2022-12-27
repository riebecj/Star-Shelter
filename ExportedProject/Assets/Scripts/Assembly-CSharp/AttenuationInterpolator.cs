using UnityEngine;

public class AttenuationInterpolator
{
	private float frameIndex;

	private float numInterpFrames;

	private float currentValue;

	private float nextValue;

	private float startValue;

	private float endValue;

	private bool isDone;

	private bool isInit;

	public void Init(float interpolationFrames)
	{
		numInterpFrames = interpolationFrames;
		startValue = 0f;
		endValue = 0f;
		currentValue = 0f;
		nextValue = 0f;
		frameIndex = 0f;
		isInit = true;
		isDone = false;
	}

	public void Reset()
	{
		isInit = true;
	}

	public float Update(out float perSampleIncrement, int samplesToInterpolate)
	{
		if (isDone)
		{
			perSampleIncrement = 0f;
			return currentValue;
		}
		float num = 1f / numInterpFrames;
		float num2 = frameIndex * num;
		if (num2 >= 1f)
		{
			isDone = true;
			currentValue = endValue;
			nextValue = endValue;
		}
		else if (num2 + num >= 1f)
		{
			currentValue = Mathf.Lerp(startValue, endValue, num2);
			nextValue = endValue;
		}
		else
		{
			currentValue = Mathf.Lerp(startValue, endValue, num2);
			nextValue = Mathf.Lerp(startValue, endValue, num2 + num);
		}
		perSampleIncrement = (nextValue - currentValue) / (float)samplesToInterpolate;
		frameIndex += 1f;
		return currentValue;
	}

	public void Set(float value)
	{
		if (isInit || numInterpFrames == 0f)
		{
			isInit = false;
			currentValue = value;
			startValue = value;
			endValue = value;
			frameIndex = numInterpFrames;
			if (numInterpFrames == 0f)
			{
				isDone = true;
			}
			else
			{
				isDone = false;
			}
		}
		else
		{
			startValue = nextValue;
			endValue = value;
			frameIndex = 0f;
			isDone = false;
		}
	}

	public float Get()
	{
		return currentValue;
	}
}
