using System.Collections;
using PreviewLabs;
using UnityEngine;

public class Turret : MonoBehaviour
{
	public float powerDraw = 0.15f;

	public float dotResult;

	public int health;

	public int shotPowerCost = 15;

	public int shotRange = 30;

	public Vector2 minMaxX;

	public Vector2 minMaxY;

	public Vector2 minMaxZ;

	public GameObject shootParticle;

	public GameObject _on;

	public GameObject _off;

	public GameObject flash;

	public GameObject laser;

	public GameObject visualCone;

	private LineRenderer line;

	public Transform turret;

	internal bool isRepairing;

	internal bool broken;

	public Transform target;

	internal bool active;

	private float chargeScale;

	public Mesh brokenMesh;

	private void Start()
	{
		line = laser.GetComponent<LineRenderer>();
		if (GameManager.instance.debugMode || !GetComponent<CraftComponent>().constructing)
		{
			CraftingComplete();
		}
		if (PreviewLabs.PlayerPrefs.GetBool(string.Concat(base.name, base.transform.position, "Broken")))
		{
			OnBreak();
		}
	}

	public void OnTakeDamage(int value)
	{
		TakeDamage(value);
	}

	public void TakeDamage(int value)
	{
		if (value <= health)
		{
			health -= value;
		}
		else
		{
			health = 0;
		}
		if (health == 0 && !broken)
		{
			OnBreak();
		}
	}

	private void OnEnable()
	{
		if (BaseManager.instance == null)
		{
			Invoke("OnEnable", 0.1f);
			return;
		}
		if (!BaseManager.instance.Turrets.Contains(this))
		{
			BaseManager.instance.Turrets.Add(this);
		}
		visualCone.SetActive(false);
	}

	private void OnDisable()
	{
		if (BaseManager.instance.Turrets.Contains(this))
		{
			BaseManager.instance.Turrets.Remove(this);
		}
	}

	private IEnumerator LookForTarget()
	{
		float waitTime = 0.05f;
		while (target == null && !broken)
		{
			if (BaseManager.instance.power < 5f)
			{
				yield return new WaitForSeconds(waitTime);
			}
			if ((bool)TitanMissile.currentMissile)
			{
				Transform currentMissile = TitanMissile.currentMissile;
				if (Vector3.Distance(base.transform.position, currentMissile.position) < (float)shotRange)
				{
					dotResult = Vector3.Dot(base.transform.TransformDirection(Vector3.forward), (currentMissile.position - base.transform.position).normalized);
					if (target == null && dotResult > 0.6f && BaseManager.instance.power > (float)shotPowerCost)
					{
						target = currentMissile;
						break;
					}
				}
			}
			if ((bool)TitanProjectile.currentProjectile)
			{
				Transform currentProjectile = TitanProjectile.currentProjectile;
				if (Vector3.Distance(base.transform.position, currentProjectile.position) < (float)shotRange)
				{
					dotResult = Vector3.Dot(base.transform.TransformDirection(Vector3.forward), (currentProjectile.position - base.transform.position).normalized);
					if (target == null && dotResult > 0.6f && BaseManager.instance.power > (float)shotPowerCost)
					{
						target = currentProjectile;
						break;
					}
				}
			}
			foreach (Transform activeComet in CometManager.instance.activeComets)
			{
				if (Vector3.Distance(base.transform.position, activeComet.position) < (float)shotRange)
				{
					dotResult = Vector3.Dot(base.transform.TransformDirection(Vector3.forward), (activeComet.position - base.transform.position).normalized);
					if (target == null && dotResult > 0.6f && BaseManager.instance.power > (float)shotPowerCost)
					{
						target = activeComet;
						break;
					}
				}
			}
			if (TitanAI.instance != null)
			{
				Transform transform = TitanAI.instance.transform;
				if (Vector3.Distance(base.transform.position, transform.position) < (float)shotRange)
				{
					dotResult = Vector3.Dot(base.transform.TransformDirection(Vector3.forward), (transform.position - base.transform.position).normalized);
					if (target == null && dotResult > 0.6f && BaseManager.instance.power > (float)shotPowerCost)
					{
						target = TitanAI.instance.Core.transform;
						break;
					}
				}
			}
			foreach (GameObject spawnedDrone in DroneAI.spawnedDrones)
			{
				Transform transform2 = spawnedDrone.transform;
				if (Vector3.Distance(base.transform.position, transform2.position) < (float)shotRange)
				{
					dotResult = Vector3.Dot(base.transform.TransformDirection(Vector3.forward), (transform2.position - base.transform.position).normalized);
					if (target == null && dotResult > 0.6f && BaseManager.instance.power > (float)shotPowerCost)
					{
						target = transform2;
						break;
					}
				}
			}
			Quaternion targetRot = Quaternion.LookRotation(base.transform.position - base.transform.up - base.transform.position);
			turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRot, 100f * Time.deltaTime);
			yield return new WaitForSeconds(waitTime);
		}
		OnLock();
	}

	private void OnLock()
	{
		StartCoroutine("LockOnTarget");
	}

	private IEnumerator LockOnTarget()
	{
		float waitTime = 0.05f;
		line.enabled = true;
		while (target != null)
		{
			Quaternion targetRot = Quaternion.LookRotation(target.position - turret.position);
			turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRot, 1500f * Time.deltaTime);
			Vector3 targetDir = target.position - turret.position;
			line.SetPosition(0, turret.position);
			float distance = Vector3.Distance(turret.position, target.position);
			line.SetPosition(1, turret.position + turret.forward * distance);
			if (chargeScale < 2f && !IsInvoking("ShotCooldown"))
			{
				laser.gameObject.SetActive(true);
				chargeScale += waitTime * 4f;
				line.widthMultiplier = 0.15f * chargeScale;
			}
			if (chargeScale > 2f && BaseManager.instance.power > (float)shotPowerCost)
			{
				OnShoot();
				break;
			}
			yield return new WaitForSeconds(waitTime);
		}
	}

	public void CraftingComplete()
	{
		GetComponent<MeshRenderer>().enabled = true;
		turret.GetComponent<MeshRenderer>().enabled = true;
		StopAllCoroutines();
		StartCoroutine("LookForTarget");
		active = true;
	}

	private void OnShoot()
	{
		if ((bool)target.GetComponent<Comet>())
		{
			target.GetComponent<Comet>().OnGetShot();
		}
		else if ((bool)target.GetComponent<TitanWeakPoint>())
		{
			target.GetComponent<TitanWeakPoint>().TakeDamage(10);
		}
		else if ((bool)target.GetComponent<DroneAI>())
		{
			target.GetComponent<DroneAI>().OnTakeDamage(10);
		}
		else if ((bool)target.GetComponent<TitanMissile>())
		{
			target.GetComponent<TitanMissile>().OnTakeDamage(10);
		}
		else if ((bool)target.GetComponent<TitanProjectile>())
		{
			target.GetComponent<TitanProjectile>().OnTakeDamage(10);
		}
		target = null;
		chargeScale = 0f;
		Object.Instantiate(shootParticle, turret.position, Quaternion.identity);
		GameObject gameObject = Object.Instantiate(shootParticle, turret.position, turret.rotation);
		BaseManager.instance.power -= shotPowerCost;
		StartCoroutine("LookForTarget");
		flash.SetActive(true);
		laser.gameObject.SetActive(false);
		Invoke("ShotCooldown", 0.8f);
		line.enabled = false;
	}

	private void OnBreak()
	{
		GetComponent<MeshFilter>().mesh = brokenMesh;
		turret.gameObject.SetActive(false);
		broken = true;
		active = false;
		if (BaseManager.instance.Turrets.Contains(this))
		{
			BaseManager.instance.Turrets.Remove(this);
		}
		if (!GameManager.instance.loading)
		{
			Object.Instantiate(GameManager.instance.breakPraticle, base.transform.position, base.transform.rotation);
			GameAudioManager.instance.AddToSuitQueue(BaseManager.instance.turretBrokenWarning);
			PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.name, base.transform.position, "Broken"), true);
		}
	}

	public void OnActivate()
	{
		if (!broken)
		{
			StartCoroutine("LookForTarget");
			active = true;
			_on.SetActive(true);
			_off.SetActive(false);
		}
	}

	public void OnDeactivate()
	{
		StopCoroutine("LookForTarget");
		active = false;
		_off.SetActive(true);
		_on.SetActive(false);
	}

	private void ShotCooldown()
	{
		flash.SetActive(false);
	}

	public void OnSalvage()
	{
		OnRemove();
	}

	public void OnRemove()
	{
		PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.name, base.transform.position, "Broken"), false);
	}
}
