using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DroneAI : MonoBehaviour
{
	private Rigidbody rigidbody;

	private float speed = 3f;

	private float rotationSpeed = 5f;

	private float attackDistance = 20f;

	private float shotInterval = 3f;

	private float AggroDistance = 20f;

	private float chargeWindow = 1.5f;

	private float scanDistance = 8f;

	private float scanDuration = 5f;

	public int damage = 25;

	public int health = 5;

	private float acceleration;

	private float chargeScale;

	private Vector3 direction;

	private Transform head;

	internal bool broken;

	public Transform Turret;

	public Transform target;

	public Transform chargeParticle;

	internal bool alternate;

	internal static List<GameObject> spawnedDrones = new List<GameObject>();

	internal List<Transform> shotRooms = new List<Transform>();

	internal bool warned;

	internal bool wasGrabbed;

	private int dir;

	public LayerMask wallLayers;

	private Animator anim;

	internal bool attacking;

	public Mesh brokenMesh;

	public GameObject shotImpactParticle;

	public GameObject projectile;

	public GameObject explosion;

	public GameObject damageParticle;

	public LineRenderer aimLaser;

	private AudioSource audioSource;

	public AudioClip charge;

	public AudioClip blast;

	private float tick;

	public bool targetBase;

	internal int roomsToAttack = 1;

	private void Start()
	{
		anim = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		head = target;
		spawnedDrones.Add(base.gameObject);
		audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		StartCoroutine("LookForTargets");
	}

	private void FixedUpdate()
	{
		if (broken || GameManager.instance.inMenu)
		{
			return;
		}
		if (targetBase)
		{
			if (target == null)
			{
				target = GetClosestRoom();
				return;
			}
			OnAttackBase();
		}
		else if (target == null)
		{
			FindScanTarget();
			return;
		}
		if (target == null)
		{
			return;
		}
		Move();
		UpdateAvoidance();
		if (attacking)
		{
			float num = Vector3.Distance(target.position, base.transform.position);
			if (num < attackDistance * 1.1f)
			{
				OnAttack();
			}
		}
		else
		{
			OnPatrol();
		}
	}

	private void FindScanTarget()
	{
		if (target == null && WreckManager.instance.activeWrecks.Count > 0)
		{
			target = WreckManager.instance.activeWrecks[Random.Range(0, WreckManager.instance.activeWrecks.Count)].transform;
		}
	}

	private void OnPatrol()
	{
		if (target == null)
		{
			FindScanTarget();
		}
		if (Vector3.Distance(target.position, base.transform.position) < scanDistance * 1.25f && !IsInvoking("MoveToNextTarget"))
		{
			Invoke("MoveToNextTarget", scanDuration);
		}
	}

	private void MoveToNextTarget()
	{
		target = WreckManager.instance.activeWrecks[Random.Range(0, WreckManager.instance.activeWrecks.Count)].transform;
	}

	private void Move()
	{
		rigidbody.useGravity = false;
		if (rigidbody.velocity.magnitude < speed)
		{
			DetermineDirection();
			if (acceleration < 1f)
			{
				acceleration += 1f * Time.deltaTime;
			}
			float num = Vector3.Distance(target.position, base.transform.position);
			float num2 = scanDistance;
			if (attacking)
			{
				num2 = attackDistance * 0.8f;
			}
			if (num > num2)
			{
				if (num > num2 * 1.25f)
				{
					rigidbody.MovePosition(base.transform.position + direction * acceleration * speed * Time.deltaTime);
				}
				else
				{
					rigidbody.MovePosition(base.transform.position + direction * (5f / num) * Time.deltaTime);
				}
			}
			if (IsInvoking("ShotCooldown"))
			{
				rigidbody.MovePosition(base.transform.position + base.transform.right * dir * acceleration * (speed / 2f) * Time.deltaTime);
			}
			else
			{
				GivingUpTarget();
			}
		}
		else
		{
			rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, rigidbody.velocity.magnitude * 0.95f);
		}
		Quaternion b = Quaternion.LookRotation(target.position - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, rotationSpeed * Time.deltaTime);
		Vector3 from = target.position - base.transform.position;
		float num3 = Vector3.Angle(from, base.transform.forward);
		rigidbody.angularVelocity = rigidbody.transform.forward * num3 * Time.deltaTime;
	}

	private void OnAttack()
	{
		Quaternion identity = Quaternion.identity;
		identity = Quaternion.LookRotation(target.position - Turret.position);
		Turret.rotation = Quaternion.Slerp(Turret.rotation, identity, 2f * Time.deltaTime);
		RaycastHit hitInfo;
		if (Physics.Raycast(Turret.transform.position, (GameManager.instance.Head.position - Turret.transform.position).normalized, out hitInfo, (int)wallLayers))
		{
			if (!(hitInfo.transform.root.tag == "Player"))
			{
				aimLaser.SetPosition(0, Turret.position);
				aimLaser.SetPosition(1, hitInfo.point);
				return;
			}
			aimLaser.SetPosition(0, Turret.position);
			aimLaser.SetPosition(1, target.position - Vector3.up * 0.25f);
		}
		if (chargeScale < 1f && !IsInvoking("ShotCooldown"))
		{
			chargeParticle.gameObject.SetActive(true);
			chargeScale += Time.deltaTime * 0.75f;
			chargeParticle.localScale = new Vector3(chargeScale, chargeScale, chargeScale);
		}
		if (!audioSource.isPlaying && chargeParticle.gameObject.activeSelf)
		{
			audioSource.PlayOneShot(charge);
		}
		if (!IsInvoking("ShotCooldown") && !IsInvoking("OnShoot"))
		{
			aimLaser.gameObject.SetActive(true);
			audioSource.PlayOneShot(charge);
			Invoke("OnShoot", chargeWindow);
			dir = Random.Range(-5, 5);
			dir = Mathf.Clamp(dir, -1, 1);
			acceleration = 0f;
		}
	}

	private void GivingUpTarget()
	{
		if (attacking)
		{
			tick += 1f * Time.deltaTime;
			if (tick > 10f)
			{
				attacking = false;
				target = null;
			}
		}
	}

	private void OnShoot()
	{
		audioSource.PlayOneShot(blast);
		Object.Instantiate(projectile, Turret.position, Turret.rotation);
		chargeParticle.gameObject.SetActive(false);
		chargeParticle.localScale = Vector3.zero;
		chargeScale = 0f;
		Invoke("ShotCooldown", shotInterval);
		tick = 0f;
	}

	private void UpdateAvoidance()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, 3f, wallLayers) && !hitInfo.collider.isTrigger)
		{
			direction = -base.transform.forward + base.transform.up + base.transform.right;
			rigidbody.MovePosition(base.transform.position + direction * acceleration * speed * Time.deltaTime);
		}
	}

	private void DetermineDirection()
	{
		direction = (target.position - base.transform.position).normalized;
	}

	public void OnTakeDamage(int value)
	{
		health -= value;
		if (health <= 0)
		{
			OnBreak();
			return;
		}
		GameObject gameObject = Object.Instantiate(damageParticle, base.transform.position, Quaternion.identity);
		gameObject.transform.SetParent(base.transform);
	}

	public void OnShot(Vector3 point)
	{
		Object.Instantiate(shotImpactParticle, point, Quaternion.identity);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!broken && other.relativeVelocity.magnitude > 2f)
		{
			OnBreak();
		}
	}

	private void OnBreak()
	{
		if (!broken)
		{
			StopCoroutine("LookForTargets");
			broken = true;
			GetComponent<VRTK_InteractableObject>().isGrabbable = true;
			GetComponent<MeshFilter>().mesh = brokenMesh;
			explosion.SetActive(true);
			chargeParticle.gameObject.SetActive(false);
			aimLaser.gameObject.SetActive(false);
			audioSource.enabled = false;
			CancelInvoke("OnShoot");
			if (DroneManager.instance.drones.Contains(base.gameObject))
			{
				DroneManager.instance.drones.Remove(base.gameObject);
			}
			if (spawnedDrones.Contains(base.gameObject))
			{
				spawnedDrones.Remove(base.gameObject);
			}
			EnemyTracker.instance.OnRemoveTracker(base.transform);
		}
	}

	private void ShotCooldown()
	{
	}

	private void OnReset()
	{
		Debug.Log("OnReset");
		broken = true;
		Invoke("Deactivate", 0.5f);
		base.transform.SetParent(null);
	}

	private void Deactivate()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (DroneManager.instance.drones.Contains(base.gameObject))
		{
			DroneManager.instance.drones.Remove(base.gameObject);
		}
		EnemyTracker.instance.OnRemoveTracker(base.transform);
		if (spawnedDrones.Contains(base.gameObject))
		{
			spawnedDrones.Remove(base.gameObject);
		}
	}

	private IEnumerator LookForTargets()
	{
		while (true)
		{
			RaycastHit hitInfo;
			if (Vector3.Distance(base.transform.position, GameManager.instance.Head.position) < AggroDistance && Physics.Raycast(Turret.transform.position, (GameManager.instance.Head.position - Turret.transform.position).normalized, out hitInfo, (int)wallLayers) && hitInfo.transform.root.tag == "Player")
			{
				target = GameManager.instance.Head;
				EnemyTracker.instance.OnAddTracker(base.transform);
				attacking = true;
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void OnAttackBase()
	{
		if ((bool)target && Vector3.Distance(target.position, base.transform.position) < 10f && !IsInvoking("ShotCooldown"))
		{
			Turret.rotation = Quaternion.LookRotation(target.position - Turret.position);
			OnShoot();
			shotRooms.Add(target);
			roomsToAttack--;
			if (roomsToAttack > 0)
			{
				target = null;
			}
			else
			{
				targetBase = false;
			}
		}
	}

	private Transform GetClosestRoom()
	{
		float num = float.PositiveInfinity;
		Transform result = null;
		for (int i = 0; i < BaseLoader.instance.AllRooms.Count; i++)
		{
			if (!shotRooms.Contains(BaseLoader.instance.AllRooms[i].transform))
			{
				float num2 = Vector3.Distance(base.transform.position, BaseLoader.instance.AllRooms[i].transform.position);
				if (num2 < num)
				{
					num = num2;
					result = BaseLoader.instance.AllRooms[i].transform;
				}
			}
		}
		return result;
	}
}
