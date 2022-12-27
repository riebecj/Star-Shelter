using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class DroneHelper : MonoBehaviour
{
	internal bool VRControlled;

	public static DroneHelper instance;

	private float speed = 1f;

	private Vector3 moveDirection = Vector3.zero;

	private CharacterController controller;

	internal bool isRepairing;

	internal bool isSalvaging;

	internal GameObject repairObject;

	internal GameObject scrapObject;

	[BoxGroup("Icons", true, false, 0)]
	public Sprite defaultCursor;

	[BoxGroup("Icons", true, false, 0)]
	public Sprite activeCursor;

	[BoxGroup("Icons", true, false, 0)]
	public Sprite[] toolIcons;

	[BoxGroup("Icons", true, false, 0)]
	public Image cursorImage;

	[BoxGroup("Icons", true, false, 0)]
	public Image toolIcon;

	public GameObject buttonCone;

	public GameObject PCParts;

	public GameObject impactParticle;

	public GameObject trail;

	public GameObject flashLight;

	public GameObject proxyPlayer;

	[BoxGroup("UI", true, false, 0)]
	public GameObject infoPromt;

	[BoxGroup("UI", true, false, 0)]
	public GameObject powerPromt;

	[BoxGroup("UI", true, false, 0)]
	public GameObject inventoryFullPromt;

	[BoxGroup("UI", true, false, 0)]
	public GameObject controls;

	[BoxGroup("UI", true, false, 0)]
	public Text powerText;

	[BoxGroup("UI", true, false, 0)]
	public Text speedText;

	private int toolIndex;

	public float power = 100f;

	public float maxPower = 100f;

	private float powerChargeSpeed = 0.5f;

	public Image[] powerBars;

	private Collider grabSuggestion;

	public Rigidbody grabbedObject;

	private Vector3 startPos;

	private int damage = 1;

	public Transform barrelPoint;

	internal RaycastHit hit;

	public LayerMask layerMask;

	public Transform grabPos;

	internal bool inBase;

	public CapsuleCollider interactCollider;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		if (IntroManager.instance == null)
		{
			Invoke("Setup", 1f);
		}
		startPos = base.transform.position;
	}

	private void Setup()
	{
		StartCoroutine("UpdatePower");
	}

	private IEnumerator UpdatePower()
	{
		while (true)
		{
			if ((VRControlled || moveDirection.magnitude > 0f) && power > 0f)
			{
				power -= Time.deltaTime * 0.25f;
				if (flashLight.activeSelf)
				{
					power -= Time.deltaTime * 0.05f;
				}
			}
			if (power < maxPower * 0.2f)
			{
				if (power <= 0f)
				{
					powerText.text = "//No POWER//";
				}
				else
				{
					powerText.text = "//Low POWER//";
				}
				powerPromt.SetActive(true);
			}
			else
			{
				powerPromt.SetActive(false);
			}
			if (power <= 0f)
			{
				power = 0f;
				if (VRControlled)
				{
					DroneArmUIManager.instance.SwapToDrone(false);
				}
				OnPowerDown();
			}
			else
			{
				CancelInvoke("ReturnToBase");
			}
			if (inBase && power <= maxPower && BaseManager.instance.autoFillPower)
			{
				if (power + powerChargeSpeed > maxPower)
				{
					power = maxPower;
				}
				else if (BaseManager.instance.power > powerChargeSpeed)
				{
					BaseManager.instance.power -= powerChargeSpeed;
					power += powerChargeSpeed;
				}
			}
			UpdateUI();
			yield return new WaitForEndOfFrame();
		}
	}

	public void ReturnToBase()
	{
		base.transform.position = startPos;
		base.transform.rotation = Quaternion.identity;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponentInParent<RoomLeak>())
		{
			repairObject = other.GetComponentInParent<RoomLeak>().gameObject;
		}
		if ((bool)other.GetComponentInParent<CraftComponent>() && !other.transform.root.GetComponent<BaseManager>())
		{
			scrapObject = other.GetComponentInParent<CraftComponent>().gameObject;
		}
		if (IsInteractive(other))
		{
			ToggleActiveCursor(true);
		}
		Rigidbody component = other.GetComponent<Rigidbody>();
		if ((bool)component && !component.isKinematic && grabbedObject == null)
		{
			grabSuggestion = other;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsInteractive(other))
		{
			ToggleActiveCursor(false);
		}
		Rigidbody component = other.GetComponent<Rigidbody>();
		if (grabSuggestion == other)
		{
			grabSuggestion = null;
		}
	}

	private bool InteractiveCheck()
	{
		Collider[] array = Physics.OverlapCapsule(interactCollider.bounds.center + interactCollider.transform.forward * 0.2f, interactCollider.bounds.center - interactCollider.transform.forward * 0.2f, interactCollider.radius);
		for (int i = 0; i < array.Length; i++)
		{
			if (IsInteractive(array[i]))
			{
				return false;
			}
		}
		return true;
	}

	private void FixedUpdate()
	{
		if (!VRControlled && !(power <= 0f))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				speed = Mathf.Lerp(speed, 6.5f, 1f * Time.deltaTime);
			}
			else
			{
				speed = 1f;
			}
			float y = 0f;
			if (Input.GetKey(KeyCode.Q))
			{
				y = -1f;
			}
			else if (Input.GetKey(KeyCode.E))
			{
				y = 1f;
			}
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), y, Input.GetAxis("Vertical"));
			moveDirection = base.transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			controller.Move(moveDirection * Time.deltaTime);
			if ((bool)grabbedObject)
			{
				grabbedObject.velocity = (grabPos.position - grabbedObject.transform.position) * 60f;
				grabbedObject.angularVelocity = Vector3.zero;
			}
		}
	}

	private void ToggleTool(int index)
	{
		toolIndex = index;
		toolIcon.sprite = toolIcons[index];
		switch (index)
		{
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return) || (PCParts.activeSelf && Input.GetKeyDown(KeyCode.Escape)))
		{
			ToggleCoopDrone();
		}
		if (VRControlled || power <= 0f)
		{
			return;
		}
		Cursor.lockState = CursorLockMode.Confined;
		if (Input.GetMouseButton(0))
		{
			if (toolIndex == 0)
			{
				Repair();
			}
			else if (toolIndex == 1)
			{
				Salvage();
			}
			else if (toolIndex == 2)
			{
				Shoot();
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (toolIndex == 0)
			{
				OnExitRepair();
			}
			else if (toolIndex == 1)
			{
				ExitSalvage();
			}
		}
		if (Input.GetMouseButton(1))
		{
			if (grabSuggestion != null)
			{
				StartGrab();
			}
			buttonCone.SetActive(true);
		}
		else
		{
			buttonCone.SetActive(false);
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			ToggleFlashLight(!flashLight.activeSelf);
		}
		if (Input.GetMouseButtonUp(1))
		{
			EndGrab();
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ToggleTool(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ToggleTool(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ToggleTool(2);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			ToggleControls(controls.activeSelf);
		}
		ToolScroll();
	}

	private void StartGrab()
	{
		Rigidbody component = grabSuggestion.GetComponent<Rigidbody>();
		if (!(grabSuggestion == null) && !component.isKinematic && !(grabbedObject != null) && !component.transform.root.GetComponent<RagdollRef>())
		{
			grabbedObject = component;
			grabPos.position = grabbedObject.transform.position;
			if ((bool)grabbedObject.GetComponent<CraftProxy>())
			{
				grabbedObject.GetComponent<CraftProxy>().OnGrab();
			}
		}
	}

	private void EndGrab()
	{
		if (!(grabbedObject == null))
		{
			if ((bool)grabbedObject.GetComponent<CraftProxy>())
			{
				grabbedObject.GetComponent<CraftProxy>().OnDrop();
			}
			grabbedObject.isKinematic = false;
			grabbedObject = null;
		}
	}

	private void ToggleFlashLight(bool value)
	{
		flashLight.SetActive(value);
	}

	private void ToolScroll()
	{
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			toolIndex++;
			if (toolIndex > 2)
			{
				toolIndex = 0;
			}
			ToggleTool(toolIndex);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			toolIndex--;
			if (toolIndex < 0)
			{
				toolIndex = 2;
			}
			ToggleTool(toolIndex);
		}
	}

	public void ToggleCoopDrone()
	{
		if (!IntroManager.instance && !(power <= 0f))
		{
			PCParts.SetActive(!PCParts.activeSelf);
			infoPromt.SetActive(!PCParts.activeSelf);
			if (VRControlled)
			{
				ToggleControls(false);
				DroneArmUIManager.instance.SwapToDrone(false);
			}
		}
	}

	public void ToggleTransfer(bool value)
	{
		VRControlled = value;
		proxyPlayer.SetActive(value);
		if (VRControlled)
		{
			PCParts.SetActive(false);
			infoPromt.SetActive(true);
		}
		SpaceMask.instance.SnapClosed();
		GameManager.instance.CamRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
		if (value)
		{
			GameManager.instance.CamRig.GetComponent<Rigidbody>().drag = 0.5f;
			proxyPlayer.gameObject.SetActive(true);
			proxyPlayer.transform.position = GameManager.instance.Head.position - Vector3.up;
			proxyPlayer.transform.eulerAngles = new Vector3(0f, GameManager.instance.Head.eulerAngles.y, 0f);
		}
		else
		{
			GameManager.instance.CamRig.GetComponent<Rigidbody>().drag = 0f;
			proxyPlayer.gameObject.SetActive(false);
			base.transform.eulerAngles = new Vector3(0f, GameManager.instance.Head.eulerAngles.y, 0f);
			ToggleFlashLight(false);
			DroneArmUIManager.instance.flashLightToggle.TurnOff();
		}
	}

	private void Repair()
	{
		if (repairObject == null || isRepairing || IsInvoking("RepairCooldown"))
		{
			return;
		}
		Transform transform = repairObject.transform;
		if (!transform.GetComponent<RoomLeak>().isRepairing)
		{
			if ((NanoInventory.instance.GetGlobalMaterialCount(0) >= transform.GetComponent<RoomLeak>().leakRate && SuitManager.instance.power >= (float)transform.GetComponent<RoomLeak>().leakRate) || GameManager.instance.debugMode)
			{
				isRepairing = true;
				transform.GetComponent<RoomLeak>().StartCoroutine("DroneRepair");
			}
			else
			{
				repairObject = null;
			}
		}
	}

	private void Salvage()
	{
		if (!(scrapObject != null) || isRepairing || !(repairObject == null) || (bool)scrapObject.transform.root.GetComponent<BaseManager>() || (bool)scrapObject.GetComponent<Recipe>() || !(SuitManager.instance.power > 5f))
		{
			return;
		}
		if (NanoInventory.instance.GetNanoMass() < NanoInventory.instance.nanoCap || IntroManager.instance != null)
		{
			if (!isSalvaging)
			{
				isSalvaging = true;
				scrapObject.GetComponent<CraftComponent>().StartCoroutine("OnDroneSalvage");
			}
		}
		else
		{
			inventoryFullPromt.SetActive(true);
		}
	}

	private void Shoot()
	{
		if (power < 1f)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(trail, barrelPoint.position, barrelPoint.rotation);
		gameObject.transform.position = barrelPoint.transform.position;
		if (Physics.Raycast(barrelPoint.position, barrelPoint.forward, out hit, 300f, layerMask))
		{
			if (Vector3.Distance(base.transform.position, hit.point) > 5f)
			{
				Invoke("OnHit", 0.1f);
			}
			else
			{
				OnHit();
			}
		}
		OnDrawPower(1f);
	}

	public void OnHit()
	{
		Rigidbody rigidbody = null;
		if ((bool)hit.collider.GetComponent<Rigidbody>())
		{
			rigidbody = hit.collider.GetComponent<Rigidbody>();
		}
		if ((bool)hit.collider.GetComponentInParent<HoloShield>())
		{
			hit.collider.GetComponentInParent<HoloShield>().TakeDamage(damage);
			Object.Instantiate(impactParticle, hit.point, barrelPoint.rotation);
			return;
		}
		if ((bool)hit.collider.GetComponent<DroneAI>())
		{
			hit.collider.GetComponent<DroneAI>().OnTakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<Plate>())
		{
			hit.collider.GetComponent<Plate>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<SolarPanel>())
		{
			hit.collider.GetComponent<SolarPanel>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<Turret>())
		{
			hit.collider.GetComponent<Turret>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<TitanWeakPoint>())
		{
			hit.collider.GetComponent<TitanWeakPoint>().TakeDamage(damage);
		}
		else if ((bool)hit.collider.GetComponent<TitanMissile>())
		{
			hit.collider.GetComponent<TitanMissile>().OnExplode();
		}
		else if ((bool)hit.collider.GetComponentInParent<Room>())
		{
			hit.collider.GetComponentInParent<Room>().OnImpact(hit.point, damage);
		}
		else if ((bool)hit.collider.GetComponent<DroneProjectile>())
		{
			hit.collider.GetComponent<DroneProjectile>().OnDestruct();
		}
		if ((bool)rigidbody && !rigidbody.isKinematic)
		{
			rigidbody.AddForce(base.transform.forward * 1000f);
		}
		if (rigidbody != null)
		{
			rigidbody.SendMessage("OnTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			rigidbody.SendMessage("OnShot", hit.point, SendMessageOptions.DontRequireReceiver);
			if ((bool)rigidbody.GetComponent<Comet>())
			{
				rigidbody.SendMessage("OnGetShot", SendMessageOptions.DontRequireReceiver);
			}
		}
		Object.Instantiate(impactParticle, hit.point, barrelPoint.rotation);
	}

	private bool IsInteractive(Collider other)
	{
		if ((bool)other.GetComponent<DoorButton>() || (bool)other.GetComponent<CraftComponent>() || (bool)other.GetComponent<PCButton>() || (bool)other.GetComponent<AnimateButton>() || (bool)other.GetComponent<RoomLeak>())
		{
			return true;
		}
		return false;
	}

	public void ToggleActiveCursor(bool value)
	{
		if (value)
		{
			cursorImage.sprite = activeCursor;
		}
		else
		{
			cursorImage.sprite = defaultCursor;
		}
	}

	private void ExitSalvage()
	{
		if (!(scrapObject == null))
		{
			scrapObject.GetComponent<CraftComponent>().StopCoroutine("OnDroneSalvage");
			scrapObject = null;
			isSalvaging = false;
			ToggleActiveCursor(false);
		}
	}

	public void OnExitRepair()
	{
		if (!(repairObject == null))
		{
			repairObject.GetComponent<RoomLeak>().StopCoroutine("DroneRepair");
			repairObject.GetComponent<RoomLeak>().OnCancelRepair();
			repairObject = null;
			isRepairing = false;
			ToggleActiveCursor(false);
		}
	}

	public void OnDrawPower(float value)
	{
		power -= value;
	}

	private void OnPowerDown()
	{
		powerPromt.SetActive(true);
		Invoke("ReturnToBase", 60f);
	}

	private void ToggleControls(bool value)
	{
		controls.SetActive(value);
	}

	public void UpdateUI()
	{
		for (int i = 0; i < powerBars.Length; i++)
		{
			powerBars[i].fillAmount = power / maxPower;
		}
		speedText.text = (speed * 2f).ToString("F1") + "m/s";
	}
}
