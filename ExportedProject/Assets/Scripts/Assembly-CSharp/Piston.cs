using System.Collections;
using UnityEngine;

public class Piston : ExposableMonobehaviour
{
	public Vector3 TargetPosition;

	public float TransitionTime = 3f;

	[SerializeField]
	private float mCurrentValue;

	[SerializeField]
	private float mValueTarget = 0.6f;

	private Vector3 StartPosition;

	private IEnumerator Function;

	[ExposeProperty]
	public float Value
	{
		get
		{
			return mValueTarget;
		}
		set
		{
			mValueTarget = Mathf.Clamp(value, 0f, 1f);
			if (mValueTarget < mCurrentValue)
			{
				if (Function != null)
				{
					StopCoroutine(Function);
				}
				Function = MoveToTarget(false);
				StartCoroutine(Function);
			}
			else if (mValueTarget > mCurrentValue)
			{
				if (Function != null)
				{
					StopCoroutine(Function);
				}
				Function = MoveToTarget(true);
				StartCoroutine(Function);
			}
		}
	}

	private void Awake()
	{
		StartPosition = base.transform.position;
	}

	private void Reset()
	{
		TargetPosition = base.transform.position;
	}

	private IEnumerator MoveToTarget(bool up)
	{
		if (up)
		{
			while (mValueTarget != mCurrentValue)
			{
				mCurrentValue = Mathf.Clamp(mCurrentValue + Time.fixedDeltaTime / TransitionTime, 0f, mValueTarget);
				Vector3 newPosition2 = Vector3.Lerp(StartPosition, StartPosition + TargetPosition, mCurrentValue);
				base.transform.position = newPosition2;
				yield return new WaitForFixedUpdate();
			}
		}
		else
		{
			while (mValueTarget != mCurrentValue)
			{
				mCurrentValue = Mathf.Clamp(mCurrentValue - Time.fixedDeltaTime / TransitionTime, mValueTarget, 1f);
				Vector3 newPosition2 = Vector3.Lerp(StartPosition, StartPosition + TargetPosition, mCurrentValue);
				base.transform.position = newPosition2;
				yield return new WaitForFixedUpdate();
			}
		}
		Function = null;
	}
}
