using UnityEngine;

public class VRLever : VRInteractable
{
	public LeverChangeEvent LeverListeners;

	[Tooltip("Minimum angle in degrees, get's updated by and updates the joint if there is one")]
	public float Min;

	[Tooltip("Maximum angle in degrees, get's updated by and updates the joint if there is one")]
	public float Max;

	private HingeJoint CurrentHinge;

	protected float mValue;

	protected float valueCache;

	private float mMinCache;

	private float mMaxCache;

	public float VibrationStrength = 0.2f;

	public bool triggerEnabled;

	private VRGripper controller;

	private bool isActing;

	public float Value
	{
		get
		{
			return mValue;
		}
		set
		{
			if (mValue == value)
			{
				return;
			}
			mValue = value;
			CheckHingeValue();
			if (LeverListeners != null)
			{
				try
				{
					LeverListeners.Invoke(this, mValue, valueCache);
				}
				catch
				{
					Debug.LogError("A delegate failed to execute for ValueChangedhandler in VRLever");
				}
			}
			valueCache = mValue;
		}
	}

	private void CheckHingeValue()
	{
		if (!isActing && Mathf.Round(ValueToAngle()) != Mathf.Round(base.transform.rotation.eulerAngles.x))
		{
			float num = Mathf.Round(ValueToAngle());
			float num2 = Mathf.Round(base.transform.rotation.eulerAngles.x);
			Debug.Log("Setting value to angle value = " + num + " cur angle " + num2);
			SetAngleToValue();
		}
	}

	private void Reset()
	{
		mMinCache = 0f;
		mMaxCache = 0f;
		Validate();
	}

	private void OnValidate()
	{
		Validate();
	}

	private void Validate()
	{
		CurrentHinge = GetComponent<HingeJoint>();
		if (CurrentHinge != null)
		{
			EditorUpdateHinges();
		}
	}

	private void EditorUpdateHinges()
	{
		if (CurrentHinge.limits.min != mMinCache)
		{
			mMinCache = CurrentHinge.limits.min;
			Min = CurrentHinge.limits.min;
		}
		else if (mMinCache != Min)
		{
			mMinCache = Min;
			JointLimits limits = CurrentHinge.limits;
			limits.min = Min;
			CurrentHinge.limits = limits;
		}
		if (CurrentHinge.limits.max != mMaxCache)
		{
			mMaxCache = CurrentHinge.limits.max;
			Max = mMaxCache;
		}
		else if (mMaxCache != Max)
		{
			mMaxCache = Max;
			JointLimits limits2 = CurrentHinge.limits;
			limits2.max = Max;
			CurrentHinge.limits = limits2;
		}
	}

	private void OnEnable()
	{
		SetAngleToValue();
	}

	private void SetAngleToValue()
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		eulerAngles.x = ValueToAngle();
		Quaternion rotation = base.transform.rotation;
		rotation.eulerAngles = eulerAngles;
		base.transform.rotation = rotation;
	}

	private float AngleToValue()
	{
		float value = ((!(base.transform.rotation.eulerAngles.x > 180f)) ? base.transform.rotation.eulerAngles.x : (base.transform.rotation.eulerAngles.x - 360f));
		value = Mathf.Clamp(value, Min, Max);
		value += Min * Mathf.Sign(Min);
		return value / (Max + Min * Mathf.Sign(Min));
	}

	private float ValueToAngle()
	{
		float num = (Max + Min * Mathf.Sign(Min)) * Value - Min * Mathf.Sign(Min) + 360f;
		return (!(num >= 360f)) ? num : (num - 360f);
	}

	private new void Update()
	{
		base.Update();
		if (Interactable)
		{
			if (AngleToValue() != valueCache)
			{
				Value = AngleToValue();
			}
			if (isActing)
			{
				controller.HapticVibration(VibrationStrength, Time.deltaTime);
			}
		}
	}

	private void OnCollisionEnter(Collision _collision)
	{
		if (!(_collision.rigidbody == null))
		{
			controller = _collision.rigidbody.GetComponent<VRGripper>();
			if (!(controller == null))
			{
				BeginAction();
			}
		}
	}

	private void OnCollisionExit(Collision _collision)
	{
		if (!(_collision.rigidbody == null))
		{
			VRGripper component = _collision.rigidbody.GetComponent<VRGripper>();
			if (!(controller != component))
			{
				EndAction();
			}
		}
	}

	private void BeginAction()
	{
		isActing = true;
	}

	private void EndAction()
	{
		isActing = false;
	}

	public void OnGripBegin()
	{
		BeginAction();
	}

	public void OnGripEnd()
	{
		EndAction();
	}
}
