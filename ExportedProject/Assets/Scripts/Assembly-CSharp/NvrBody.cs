using System;
using UnityEngine;

public class NvrBody : MonoBehaviour
{
	public enum HeadTrackingMode
	{
		None = 0,
		Position2D = 1,
		Position3D = 2
	}

	public HeadTrackingMode headTrackingMode;

	public Transform vrCameraRootTransform;

	public float cameraOffsetXZ = 0.18f;

	public float cameraOffsetY = 0.18f;

	public float cameraHeightOffset = 2f;

	public float bodyTurnAngleIdle = 75f;

	public float bodyTurnAngleMoving = 10f;

	public bool rotateWhenLookingDown = true;

	public bool rotateWhenMoving = true;

	public bool turnAnimWhenMoving;

	public bool showEditorControls = true;

	public Vector3 newCamPos;

	public float walkAnimationSpeed = 1f;

	public bool smoothCameraMovement;

	public float cameraSmooth = 2f;

	private Animator myAnim;

	private Vector3 vrRot;

	private Vector3 myRot;

	private float speed;

	private Vector3 previousPosition;

	private CharacterController myCC;

	private Rigidbody myRigidbody;

	private Transform vrCamera;

	private float x;

	private float y;

	private float z;

	private Vector3 cameraUp;

	private Vector3 cameraFw;

	private bool cameraUpsidedown;

	private bool bodyIsTurning;

	private bool shouldBodyTurn;

	private bool shouldTurnAnimate;

	private float myMagnitude;

	private bool usingRB;

	private bool usingCC;

	private bool usingAnim;

	private void Start()
	{
		if (vrCameraRootTransform == null)
		{
			Debug.LogErrorFormat("<color=#00ffffff><size=16><b>Please assign a Gameobject to field: 'VR Main Object' on the VRBody script.</b></size></color>");
		}
		if (Camera.main == null)
		{
			Debug.LogErrorFormat("<color=#00ffffff><size=16><b>Make sure there is a Main Camera with Tag: 'MainCamera'.</b></size></color>");
		}
		else
		{
			vrCamera = Camera.main.transform;
		}
		myAnim = GetComponent<Animator>();
		if (myAnim == null)
		{
			Debug.LogWarning("Did not find an Animator on " + base.gameObject.name + ". Cannot animate walking/running.");
			usingAnim = false;
		}
		else
		{
			usingAnim = true;
			if (myAnim.GetLayerName(0) != "VRBodyBaseLayer")
			{
				Debug.LogWarningFormat("<color=#00ffffff><size=16><b>Animator's controller is not 'VRBodyAnimController'.</b></size> Assign the controller located at /VRBody/Animation/Controllers/ to the Animator.</color>");
				usingAnim = false;
			}
		}
		myCC = GetComponent<CharacterController>();
		if (myCC == null)
		{
			myRigidbody = GetComponent<Rigidbody>();
			if (myRigidbody == null)
			{
				Debug.LogWarning("Did not find a CharacterController or Rigidbody on " + base.gameObject.name + ". Cannot animate walking/running.");
				usingCC = false;
				usingRB = false;
			}
			else
			{
				usingCC = false;
				usingRB = true;
			}
		}
		else
		{
			usingCC = true;
			usingRB = false;
		}
		if (!showEditorControls)
		{
			showEditorControls = false;
		}
	}

	private void Update()
	{
		cameraUp = vrCamera.transform.up;
		cameraFw = vrCamera.transform.forward;
		vrRot = Camera.main.transform.rotation.eulerAngles;
		myRot = base.transform.rotation.eulerAngles;
		Quaternion b = Quaternion.identity;
		float num = bodyTurnAngleIdle;
		shouldBodyTurn = true;
		shouldTurnAnimate = true;
		if (cameraUp.y <= 0f && cameraFw.y < 0.5f)
		{
			cameraUpsidedown = true;
			if (!rotateWhenLookingDown)
			{
				shouldBodyTurn = false;
			}
		}
		else
		{
			cameraUpsidedown = false;
		}
		if (usingCC)
		{
			myMagnitude = myCC.velocity.magnitude;
		}
		else if (usingRB)
		{
			myMagnitude = myRigidbody.velocity.magnitude;
		}
		else
		{
			myMagnitude = 0f;
		}
		if (myMagnitude > 0.1f)
		{
			num = bodyTurnAngleMoving;
			if (!rotateWhenMoving)
			{
				shouldBodyTurn = false;
			}
			if (!turnAnimWhenMoving)
			{
				shouldTurnAnimate = false;
			}
		}
		if (!usingAnim)
		{
			shouldTurnAnimate = false;
		}
		if (shouldBodyTurn)
		{
			if (cameraUpsidedown)
			{
				if (Mathf.DeltaAngle(vrRot.y - 180f, myRot.y) > num)
				{
					if (shouldTurnAnimate && !myAnim.GetBool("bTurnLeft"))
					{
						myAnim.SetBool("bTurnLeft", true);
					}
					b = Quaternion.Euler(new Vector3(myRot.x, vrRot.y - 180f, myRot.z));
					bodyIsTurning = true;
				}
				else if (Mathf.DeltaAngle(vrRot.y - 180f, myRot.y) < 0f - num)
				{
					if (shouldTurnAnimate && !myAnim.GetBool("bTurnRight"))
					{
						myAnim.SetBool("bTurnRight", true);
					}
					b = Quaternion.Euler(new Vector3(myRot.x, vrRot.y - 180f, myRot.z));
					bodyIsTurning = true;
				}
				else if (bodyIsTurning)
				{
					b = Quaternion.Euler(new Vector3(myRot.x, vrRot.y - 180f, myRot.z));
				}
			}
			else if (Mathf.DeltaAngle(vrRot.y, myRot.y) > num)
			{
				if (shouldTurnAnimate && !myAnim.GetBool("bTurnLeft"))
				{
					myAnim.SetBool("bTurnLeft", true);
				}
				b = Quaternion.Euler(new Vector3(myRot.x, vrRot.y, myRot.z));
				bodyIsTurning = true;
			}
			else if (Mathf.DeltaAngle(vrRot.y, myRot.y) < 0f - num)
			{
				if (shouldTurnAnimate && !myAnim.GetBool("bTurnRight"))
				{
					myAnim.SetBool("bTurnRight", true);
				}
				b = Quaternion.Euler(new Vector3(myRot.x, vrRot.y, myRot.z));
				bodyIsTurning = true;
			}
			else if (bodyIsTurning)
			{
				b = Quaternion.Euler(new Vector3(myRot.x, vrRot.y, myRot.z));
			}
		}
		if (bodyIsTurning)
		{
			float num2 = Mathf.DeltaAngle(vrRot.y, myRot.y);
			if (cameraUpsidedown)
			{
				num2 = Mathf.DeltaAngle(vrRot.y - 180f, myRot.y);
			}
			if (num2 < 0f)
			{
				num2 += 180f;
				num2 /= 180f;
			}
			else
			{
				num2 -= 180f;
				num2 *= -1f;
				num2 /= 180f;
			}
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, num2 * 0.1f);
			if (num2 > 0.95f)
			{
				bodyIsTurning = false;
			}
		}
		else if (usingAnim)
		{
			if (myAnim.GetBool("bTurnLeft"))
			{
				myAnim.SetBool("bTurnLeft", false);
			}
			if (myAnim.GetBool("bTurnRight"))
			{
				myAnim.SetBool("bTurnRight", false);
			}
		}
		if (usingAnim)
		{
			myAnim.SetFloat("Speed", myMagnitude * walkAnimationSpeed);
		}
	}

	private void LateUpdate()
	{
		if (headTrackingMode == HeadTrackingMode.Position3D)
		{
			cameraUp = vrCamera.transform.up;
			cameraFw = vrCamera.transform.forward;
			if (cameraUp.y < 0.1f && cameraFw.y < 0.5f)
			{
				x = cameraOffsetXZ * Mathf.Sin(base.transform.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				y = cameraHeightOffset - cameraOffsetY * 2f;
				y += cameraOffsetY * Mathf.Sin(vrCamera.rotation.eulerAngles.x * ((float)Math.PI / 180f));
				z = cameraOffsetXZ * Mathf.Cos(base.transform.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				newCamPos = base.transform.position + new Vector3(x, y, z);
				if (smoothCameraMovement)
				{
					vrCameraRootTransform.position = Vector3.Lerp(vrCameraRootTransform.position, newCamPos, Vector3.Distance(vrCameraRootTransform.position, newCamPos) * cameraSmooth / 2f);
				}
				else
				{
					vrCameraRootTransform.position = newCamPos;
				}
			}
			else
			{
				x = cameraOffsetXZ * Mathf.Sin(vrCamera.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				y = cameraOffsetY * Mathf.Sin(vrCamera.rotation.eulerAngles.x * ((float)Math.PI / 180f));
				y = cameraHeightOffset - y;
				z = cameraOffsetXZ * Mathf.Cos(vrCamera.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				newCamPos = base.transform.position + new Vector3(x, y, z);
				if (smoothCameraMovement)
				{
					vrCameraRootTransform.position = Vector3.Lerp(vrCameraRootTransform.position, newCamPos, Vector3.Distance(vrCameraRootTransform.position, newCamPos) * cameraSmooth);
				}
				else
				{
					vrCameraRootTransform.position = newCamPos;
				}
			}
		}
		else if (headTrackingMode == HeadTrackingMode.Position2D)
		{
			cameraUp = vrCamera.transform.up;
			cameraFw = vrCamera.transform.forward;
			if (cameraUp.y < 0.1f && cameraFw.y < 0.5f)
			{
				x = cameraOffsetXZ * Mathf.Sin(base.transform.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				y = cameraHeightOffset;
				z = cameraOffsetXZ * Mathf.Cos(base.transform.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				newCamPos = base.transform.position + new Vector3(x, y, z);
				if (smoothCameraMovement)
				{
					vrCameraRootTransform.position = Vector3.Lerp(vrCameraRootTransform.position, newCamPos, Vector3.Distance(vrCameraRootTransform.position, newCamPos) * cameraSmooth / 2f);
				}
				else
				{
					vrCameraRootTransform.position = newCamPos;
				}
			}
			else
			{
				x = cameraOffsetXZ * Mathf.Sin(vrCamera.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				y = cameraHeightOffset;
				z = cameraOffsetXZ * Mathf.Cos(vrCamera.rotation.eulerAngles.y * ((float)Math.PI / 180f));
				newCamPos = base.transform.position + new Vector3(x, y, z);
				if (smoothCameraMovement)
				{
					vrCameraRootTransform.position = Vector3.Lerp(vrCameraRootTransform.position, newCamPos, Vector3.Distance(vrCameraRootTransform.position, newCamPos) * cameraSmooth);
				}
				else
				{
					vrCameraRootTransform.position = newCamPos;
				}
			}
		}
		else if (headTrackingMode == HeadTrackingMode.None)
		{
			x = 0f;
			z = 0f;
			y = cameraHeightOffset;
			newCamPos = base.transform.position + new Vector3(x, y, z);
			if (smoothCameraMovement)
			{
				vrCameraRootTransform.position = Vector3.Lerp(vrCameraRootTransform.position, newCamPos, Vector3.Distance(vrCameraRootTransform.position, newCamPos) * cameraSmooth);
			}
			else
			{
				vrCameraRootTransform.position = newCamPos;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (showEditorControls && Application.isPlaying)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(new Vector3(base.transform.position.x, base.transform.position.y + cameraHeightOffset, base.transform.position.z), newCamPos);
		}
	}
}
