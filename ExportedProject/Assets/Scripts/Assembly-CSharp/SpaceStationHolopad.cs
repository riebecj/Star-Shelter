using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class SpaceStationHolopad : MonoBehaviour
{
	public GameObject player;

	public Animator target;

	private GameObject stationWarpAnimation;

	public GameObject world;

	public GameObject planetSurface;

	public Material skyOne;

	public Material skyTwo;

	public GameObject targetHolodeck;

	private Rigidbody rigidBody;

	private bool shouldLerp1;

	private bool lerping1;

	private bool shouldLerp2;

	private bool lerping2;

	public float timeStartedLerping1;

	public float lerpTime1;

	public Vector3 startPosition1;

	public Vector3 endPosition1;

	public float timeStartedLerping2;

	public float lerpTime2;

	public Vector3 startPosition2;

	public Vector3 endPosition2;

	public void StartWarping()
	{
		rigidBody.useGravity = true;
		world.SetActive(false);
		RenderSettings.skybox = skyTwo;
		rigidBody.mass = 0.3f;
		player.GetComponent<VRTK_TouchpadWalking>().enabled = true;
		player.GetComponent<RagdollRef>().enabled = false;
		player.transform.position = targetHolodeck.transform.position + new Vector3(0f, 3f, 0f);
		lerping2 = false;
		shouldLerp2 = false;
		FadeIn();
	}

	private void FadeIn()
	{
		VRTK_ScreenFade.Start(Color.clear, 3f);
	}

	private void FadeOut()
	{
		VRTK_ScreenFade.Start(Color.black, 3f);
	}

	private void StartLerping1()
	{
		timeStartedLerping1 = Time.time;
		shouldLerp1 = true;
	}

	private void StartLerping2()
	{
		timeStartedLerping2 = Time.time;
		shouldLerp2 = true;
	}

	private void Update()
	{
		if (shouldLerp1)
		{
			player.transform.position = Lerp1(startPosition1, endPosition1, timeStartedLerping1, lerpTime1);
			lerping1 = true;
		}
		if (Time.time >= timeStartedLerping1 + lerpTime1 && lerping1)
		{
			lerping1 = false;
			shouldLerp1 = false;
			StartLerping2();
		}
		if (shouldLerp2)
		{
			player.transform.position = Lerp2(startPosition2, endPosition2, timeStartedLerping2, lerpTime2);
			lerping2 = true;
		}
		if (Time.time >= timeStartedLerping2 + lerpTime2 && lerping2)
		{
			FadeOut();
			StartWarping();
		}
	}

	public Vector3 Lerp1(Vector3 start, Vector3 end, float timeStartedLerping1, float lerpTime1 = 1f)
	{
		float num = Time.time - timeStartedLerping1;
		float t = num / lerpTime1;
		return Vector3.Lerp(start, end, t);
	}

	public Vector3 Lerp2(Vector3 start, Vector3 end, float timeStartedLerping2, float lerpTime2 = 1f)
	{
		float num = Time.time - timeStartedLerping2;
		float t = num / lerpTime2;
		return Vector3.Lerp(start, end, t);
	}

	private void Start()
	{
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		if (world == null)
		{
			world = GameObject.FindWithTag("World");
		}
		stationWarpAnimation = GameObject.FindWithTag("StationWarpAnim");
		RenderSettings.skybox = skyOne;
		rigidBody = player.GetComponent<Rigidbody>();
		target = stationWarpAnimation.GetComponent<Animator>();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (target.enabled && !shouldLerp1)
		{
			target.SetBool("Warping", true);
		}
		else if (!target.enabled && !shouldLerp1)
		{
			StartLerping1();
			target.enabled = true;
		}
	}
}
