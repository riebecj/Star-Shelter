using System;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;

public class WallTurret : MonoBehaviour
{
	[Serializable]
	public class Part
	{
		public Transform transform;

		private RotationLimit rotationLimit;

		public void AimAt(Transform target)
		{
			transform.LookAt(target.position, transform.up);
			if (rotationLimit == null)
			{
				rotationLimit = transform.GetComponent<RotationLimit>();
				rotationLimit.Disable();
			}
			rotationLimit.Apply();
		}
	}

	public int health;

	public int damage = 15;

	public int aggroDistance = 8;

	public int shotInterval = 2;

	private float chargeScale;

	public Vector2 minMaxX;

	public Vector2 minMaxY;

	public Vector2 minMaxZ;

	public GameObject trail;

	public GameObject ragdoll;

	public GameObject projectile;

	public GameObject damageParticle;

	public Transform baseTransform;

	public Transform gunTransform;

	public Transform target;

	public Transform chargeParticle;

	internal bool isRepairing;

	internal Transform player;

	internal bool open;

	public LayerMask layerMask;

	internal AudioSource audioSource;

	public AudioClip shoot;

	public AudioClip charge;

	private Animation animation;

	public LineRenderer aimLaser;

	internal bool charged;

	public Part[] parts;

	private Transform proxy;

	private void Start()
	{
		animation = GetComponent<Animation>();
		player = GameManager.instance.Head;
		audioSource = GetComponent<AudioSource>();
		GameObject gameObject = new GameObject();
		gameObject.name = "proxy";
		proxy = gameObject.transform;
		StartCoroutine("LookForTarget");
	}

	private void OnEnable()
	{
		if (player != null)
		{
			StartCoroutine("LookForTarget");
		}
	}

	private void AssignGenerator()
	{
		foreach (PowerGenerator powerGenerator in PowerGenerator.powerGenerators)
		{
			if (Vector3.Distance(base.transform.position, powerGenerator.transform.position) < (float)PowerGenerator.generatorDistance)
			{
				powerGenerator.togglePower = (PowerGenerator.TogglePower)Delegate.Combine(powerGenerator.togglePower, new PowerGenerator.TogglePower(TogglePower));
			}
		}
	}

	public void TogglePower(bool on)
	{
		if (on)
		{
			StartCoroutine("LookForTarget");
		}
		else
		{
			StopCoroutine("LookForTarget");
		}
	}

	public void OnTakeDamage(int value)
	{
		if (value <= health)
		{
			health -= value;
		}
		else
		{
			health = 0;
		}
		if (health == 0)
		{
			ragdoll.SetActive(true);
			ragdoll.transform.SetParent(base.transform.parent);
			EnemyTracker.instance.OnRemoveTracker(base.transform);
			base.gameObject.SetActive(false);
		}
		else
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(damageParticle, base.transform.position, Quaternion.identity);
			gameObject.transform.SetParent(base.transform);
		}
	}

	private IEnumerator LookForTarget()
	{
		float waitTime = 0.25f;
		while (true)
		{
			if (target == null && Vector3.Distance(base.transform.position, player.position) < (float)aggroDistance && CheckLineOfSight())
			{
				target = player;
				EnemyTracker.instance.OnAddTracker(base.transform);
				proxy.position = player.position;
				if (!open)
				{
					animation.Play();
					open = true;
					Invoke("OnLock", animation.clip.length);
				}
				else
				{
					Invoke("OnLock", 0.1f);
				}
			}
			yield return new WaitForSeconds(waitTime);
		}
	}

	private void OnLock()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("LockOnTarget");
		}
	}

	private IEnumerator LockOnTarget()
	{
		float waitTime = 0.015f;
		while (target != null)
		{
			proxy.position = Vector3.Lerp(proxy.position, target.position - Vector3.up * 0.5f, 5f * waitTime);
			Part[] array = parts;
			foreach (Part part in array)
			{
				part.AimAt(proxy);
			}
			aimLaser.SetPosition(0, gunTransform.position);
			float distance = Vector3.Distance(gunTransform.position, target.position);
			if (!CheckLineOfSight())
			{
				distance = 0.25f;
			}
			aimLaser.SetPosition(1, gunTransform.position + gunTransform.forward * distance);
			if (chargeScale < 2f && !IsInvoking("Cooldown"))
			{
				chargeParticle.gameObject.SetActive(true);
				chargeScale += waitTime * 2f;
				chargeParticle.localScale = new Vector3(chargeScale, chargeScale, chargeScale);
				aimLaser.widthMultiplier = 0.15f * chargeScale;
			}
			if (!audioSource.isPlaying && chargeParticle.gameObject.activeSelf)
			{
				audioSource.PlayOneShot(charge);
			}
			if (chargeScale > 2f)
			{
				OnShoot();
			}
			if (CheckLineOfSight())
			{
				CancelInvoke("LooseTarget");
			}
			else if (!IsInvoking("LooseTarget"))
			{
				Invoke("LooseTarget", 5f);
			}
			if (Vector3.Distance(base.transform.position, player.position) > (float)aggroDistance)
			{
				target = null;
				chargeParticle.gameObject.SetActive(false);
				EnemyTracker.instance.OnRemoveTracker(base.transform);
				CancelInvoke("Cooldown");
			}
			yield return new WaitForSeconds(waitTime);
		}
	}

	private bool CheckLineOfSight()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(gunTransform.position, player.position - gunTransform.position, out hitInfo, (int)layerMask) && (hitInfo.transform.root.tag == "Player" || (bool)hitInfo.transform.root.GetComponent<HandShield>()))
		{
			return true;
		}
		return false;
	}

	private void OnShoot()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(projectile, gunTransform.position, gunTransform.rotation);
		charged = false;
		chargeParticle.localScale = Vector3.zero;
		chargeScale = 0f;
		Invoke("Cooldown", shotInterval);
		chargeParticle.gameObject.SetActive(false);
		audioSource.PlayOneShot(shoot);
		aimLaser.widthMultiplier = 0.05f;
	}

	private void LooseTarget()
	{
		target = null;
	}

	private void OnDestroy()
	{
		EnemyTracker.instance.OnRemoveTracker(base.transform);
	}

	private void Cooldown()
	{
	}
}
