using System.Collections;
using PreviewLabs;
using Sirenix.OdinInspector;
using UnityEngine;
using VRTK;

public class TitanAI : MonoBehaviour
{
	internal Transform player;

	internal Transform _base;

	[FoldoutGroup("Parts", 0)]
	public Transform gun;

	[FoldoutGroup("Parts", 0)]
	public Transform bulletSpawnPos;

	[FoldoutGroup("Parts", 0)]
	public Transform[] missileLauchPositions;

	[FoldoutGroup("Parts", 0)]
	public GameObject Core;

	[FoldoutGroup("Variables", 0)]
	public float speed = 2f;

	[FoldoutGroup("Variables", 0)]
	public float dashSpeed = 8f;

	[FoldoutGroup("Variables", 0)]
	public float stunDuration = 4f;

	[FoldoutGroup("Variables", 0)]
	public float shotInterval = 2f;

	[FoldoutGroup("Variables", 0)]
	public float pulseRadius = 2f;

	[FoldoutGroup("Variables", 0)]
	public float missileInterval = 30f;

	[FoldoutGroup("Variables", 0)]
	public int pulseDamage = 15;

	internal float randomBaseOffset;

	public MeshRenderer[] neon;

	public bool dead;

	[FoldoutGroup("Prefabs", 0)]
	public GameObject bullet;

	[FoldoutGroup("Prefabs", 0)]
	public GameObject pulseVFX;

	[FoldoutGroup("Prefabs", 0)]
	public GameObject missile;

	[FoldoutGroup("Prefabs", 0)]
	public GameObject breakVFX;

	[FoldoutGroup("Parts", 0)]
	public Rigidbody lid;

	[FoldoutGroup("Variables", 0)]
	[MinMaxSlider(0f, 50f, false)]
	public Vector2 playerDistance;

	private int boltCount = 4;

	private Rigidbody rigidbody;

	internal bool stunned;

	internal bool patrolLeft;

	public AudioSource moveAudioSource;

	internal AudioSource audioSource;

	[FoldoutGroup("AudioClips", 0)]
	public AudioClip powerupAudio;

	[FoldoutGroup("AudioClips", 0)]
	public AudioClip powerdownAudio;

	[FoldoutGroup("AudioClips", 0)]
	public AudioClip lidOpenAudio;

	[FoldoutGroup("AudioClips", 0)]
	public AudioClip removeCoreAudio;

	[FoldoutGroup("AudioClips", 0)]
	public AudioClip pieAudio;

	internal Animator anim;

	internal bool pieInfo;

	public static TitanAI instance;

	private Bounds combinedBounds;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		player = GameManager.instance.Head;
		audioSource = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
		Core.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
		DroneManager.instance.drones.Add(base.gameObject);
		Invoke("MissileCooldown", missileInterval);
		CashBounds();
	}

	private void CashBounds()
	{
		Renderer component = BaseManager.instance.GetComponent<MeshRenderer>();
		combinedBounds = component.bounds;
		Renderer[] componentsInChildren = BaseManager.instance.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer != component)
			{
				combinedBounds.Encapsulate(renderer.bounds);
			}
		}
	}

	public void PieWarning()
	{
		if (!pieInfo)
		{
			GameAudioManager.instance.AddToSuitQueue(pieAudio);
			pieInfo = true;
		}
	}

	private void LateUpdate()
	{
		if (dead)
		{
			base.transform.position = base.transform.position - base.transform.up * Time.deltaTime * 2f;
			if (Vector3.Distance(base.transform.position, player.position) > 50f)
			{
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			if (stunned)
			{
				return;
			}
			if (PlayerInSight())
			{
				if (Vector3.Distance(base.transform.position, player.position) > GetBaseBoundDistance() + 5f)
				{
					LookAtPlayer();
					ShootAtPlayer();
				}
				else if (!IsInvoking("PulseCooldown"))
				{
					OnPulse();
				}
				return;
			}
			if (_base == null)
			{
				FindClosestRoom();
			}
			TargetStation();
			if (Vector3.Distance(base.transform.position, _base.position) < 25f)
			{
				ShootAtBase();
			}
		}
	}

	private void LookAtPlayer()
	{
		if (moveAudioSource.pitch != 1f)
		{
			moveAudioSource.pitch = 1f;
		}
		anim.SetFloat("Speed", 0f, 0.5f, Time.deltaTime);
		anim.SetFloat("Direction", 0f, 0.5f, Time.deltaTime);
		Quaternion b = Quaternion.LookRotation(player.position - base.transform.position);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, 2f * Time.deltaTime);
	}

	private void FindClosestRoom()
	{
		float num = float.PositiveInfinity;
		for (int i = 0; i < BaseLoader.instance.AllRooms.Count; i++)
		{
			float num2 = Vector3.Distance(base.transform.position, BaseLoader.instance.AllRooms[i].transform.position);
			if (num2 < num)
			{
				num = num2;
				_base = BaseLoader.instance.AllRooms[i].transform;
			}
		}
	}

	private void ShootAtPlayer()
	{
		AimGun(player.position);
		if (!IsInvoking("ShotCooldown"))
		{
			Invoke("ShotCooldown", shotInterval);
			Object.Instantiate(bullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
			PieWarning();
		}
		if (!IsInvoking("MissileCooldown"))
		{
			Invoke("MissileCooldown", missileInterval);
			LaunchMissiles();
		}
	}

	private void LaunchMissiles()
	{
		int num = Random.Range(0, 1);
		GameObject gameObject = Object.Instantiate(missile, missileLauchPositions[num].position, missileLauchPositions[num].rotation);
		GameObject gameObject2 = new GameObject();
		float num2 = 0f;
		num2 = ((num != 0) ? (-5f) : 5f);
		gameObject2.transform.position = base.transform.position + (player.position - base.transform.position) * 0.5f + Vector3.up * num2;
		gameObject.GetComponent<TitanMissile>().target = gameObject2.transform;
	}

	private void ShootAtBase()
	{
		if (!IsInvoking("ShotCooldown"))
		{
			randomBaseOffset = Random.Range(0.1f, 2.5f);
			Invoke("ShotCooldown", shotInterval);
			Object.Instantiate(bullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
		}
		else
		{
			AimGun(_base.position + Vector3.up * 3f + new Vector3(randomBaseOffset, randomBaseOffset, randomBaseOffset));
		}
	}

	private void TargetStation()
	{
		if (moveAudioSource.pitch != 1.25f)
		{
			moveAudioSource.pitch = 1.25f;
		}
		if (!patrolLeft)
		{
			anim.SetFloat("Direction", 1f, 0.5f, Time.deltaTime);
		}
		else
		{
			anim.SetFloat("Direction", -1f, 0.5f, Time.deltaTime);
		}
		anim.SetFloat("Speed", 1f, 0.5f, Time.deltaTime);
		float baseBoundDistance = GetBaseBoundDistance();
		if (baseBoundDistance > 6f && !IsInvoking("MoveBack"))
		{
			base.transform.position = base.transform.position + (_base.position - base.transform.position).normalized * Time.deltaTime * speed;
		}
		else if (IsInvoking("MoveBack"))
		{
			base.transform.position = base.transform.position - (_base.position - base.transform.position).normalized * Time.deltaTime * speed * 0.5f;
		}
		if (baseBoundDistance < 4f && !IsInvoking("MoveBack"))
		{
			Invoke("MoveBack", 4f);
		}
		if (baseBoundDistance < 10f && !IsInvoking("MoveBack"))
		{
			if (!patrolLeft)
			{
				base.transform.position = base.transform.position + base.transform.right * Time.deltaTime * speed * 0.5f;
			}
			else
			{
				base.transform.position = base.transform.position + -base.transform.right * Time.deltaTime * speed * 0.5f;
			}
		}
		Quaternion b = Quaternion.LookRotation(_base.position - base.transform.position);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, 2f * Time.deltaTime);
	}

	private void MoveBack()
	{
	}

	private float GetBaseBoundDistance()
	{
		float num = 0f;
		return Vector3.Distance(base.transform.position, combinedBounds.ClosestPoint(base.transform.position));
	}

	private void AimGun(Vector3 targetPos)
	{
		Quaternion b = Quaternion.LookRotation(targetPos - gun.position);
		gun.rotation = Quaternion.Lerp(gun.rotation, b, 5f * Time.deltaTime);
	}

	private void ForcePush()
	{
		GameManager.instance.CamRig.GetComponent<Rigidbody>().AddForce((player.position - base.transform.position) * 10f);
	}

	public void OnReleaseBolt()
	{
		boltCount--;
		if (boltCount < 1)
		{
			ReleaseLid();
		}
		Core.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
		if (stunned)
		{
			CancelInvoke("EndStun");
			Invoke("EndStun", 1f);
		}
	}

	private void ReleaseLid()
	{
		audioSource.PlayOneShot(lidOpenAudio);
		lid.isKinematic = false;
		lid.transform.SetParent(null);
		lid.AddForce(base.transform.up * 15f);
	}

	private void OnPulse()
	{
		Invoke("PulseCooldown", 3.2f);
		Invoke("OnPush", 1.2f);
		pulseVFX.SetActive(true);
	}

	private void OnPush()
	{
		if (Vector3.Distance(base.transform.position, player.position) < pulseRadius)
		{
			GameManager.instance.CamRig.GetComponent<Rigidbody>().AddForce((player.position - base.transform.position).normalized * 15f, ForceMode.VelocityChange);
			SuitManager.instance.OnTakeDamage(pulseDamage, 10);
			GameManager.instance.leftController.GetComponent<VRTK_InteractGrab>().ForceRelease();
			GameManager.instance.rightController.GetComponent<VRTK_InteractGrab>().ForceRelease();
		}
	}

	private bool PlayerInSight()
	{
		if (Vector3.Distance(base.transform.position, player.position) < playerDistance.y && !BaseManager.instance.inBase)
		{
			return true;
		}
		return false;
	}

	public void OnStun()
	{
		moveAudioSource.Stop();
		stunned = true;
		Invoke("EndStun", stunDuration);
		Invoke("PowerUpdateAudio", stunDuration - 1.2f);
		audioSource.PlayOneShot(powerdownAudio);
		StartCoroutine("FadeOutNeon");
		anim.SetBool("Shutdown", true);
		anim.SetFloat("Speed", 0f);
		anim.SetFloat("Direction", 0f);
	}

	private void PowerUpdateAudio()
	{
		audioSource.PlayOneShot(powerupAudio);
	}

	private void EndStun()
	{
		moveAudioSource.Play();
		stunned = false;
		MeshRenderer[] array = neon;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.material.SetColor("_EmissionColor", Color.white);
		}
		anim.SetBool("Shutdown", false);
	}

	private void ShotCooldown()
	{
	}

	private void MissileCooldown()
	{
	}

	private void PulseCooldown()
	{
		pulseVFX.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, pulseRadius);
	}

	public void OnDeath()
	{
		breakVFX.SetActive(false);
		breakVFX.SetActive(true);
		CancelInvoke("EndStun");
		CancelInvoke("PowerUpdateAudio");
		moveAudioSource.Stop();
		audioSource.PlayOneShot(removeCoreAudio);
		anim.SetBool("Shutdown", true);
		anim.SetFloat("Speed", 0f);
		anim.SetFloat("Direction", 0f);
		dead = true;
		GameManager.instance.inTitanEvent = false;
		PreviewLabs.PlayerPrefs.SetBool("TitanFall", true);
		foreach (Transform beacon in BeaconDisc.beacons)
		{
			if ((bool)beacon)
			{
				beacon.gameObject.SetActive(false);
			}
		}
	}

	private void ChangeDirection()
	{
		patrolLeft = !patrolLeft;
	}

	private IEnumerator FadeOutNeon()
	{
		while (true)
		{
			MeshRenderer[] array = neon;
			foreach (MeshRenderer meshRenderer in array)
			{
				Color value = Color.Lerp(meshRenderer.material.GetColor("_EmissionColor"), Color.black, 0.2f);
				meshRenderer.material.SetColor("_EmissionColor", value);
			}
			yield return new WaitForSeconds(0.05f);
		}
	}
}
