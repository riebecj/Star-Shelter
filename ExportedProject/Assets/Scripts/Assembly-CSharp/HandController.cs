using UnityEngine;
using VRTK;

public class HandController : MonoBehaviour
{
	internal VRTK_ControllerEvents events;

	internal VRTK_InteractGrab grabController;

	private CraftingManager craftingManager;

	public Animator anim;

	public Rigidbody handAttachPoint;

	public Rigidbody fingerAttachPoint;

	private float smooth;

	public LayerMask layerMask;

	public Transform holdPos;

	public GameObject deteriorationParticle;

	public GameObject repairParticle;

	public GameObject componentUI;

	internal GameObject particle;

	internal GameObject deteriorationCone;

	public AudioSource toolAudio;

	public AudioClip salvageTone;

	public AudioClip salvageComplete;

	public AudioClip[] grabSounds;

	internal static HandController currentHand;

	public Material salvageMat;

	public Material repairMat;

	internal Transform triggerTarget;

	internal bool gripSwap;

	private void Start()
	{
		events = GetComponentInChildren<VRTK_ControllerEvents>();
		events.StartMenuPressed += OnStartMenuPressed;
		events.ButtonTwoPressed += OnStartMenuPressed;
		events.TriggerPressed += OnTriggerPressed;
		events.TriggerReleased += OnTriggerReleased;
		events.ButtonTwoReleased += OnButtonTwoReleased;
		events.TouchpadPressed += OnTouchpadPressed;
		events.GripPressed += OnGripPressed;
		grabController = GetComponentInChildren<VRTK_InteractGrab>();
		craftingManager = CraftingManager.instance;
		deteriorationCone = deteriorationParticle.transform.parent.gameObject;
		ToggleSalvageCone();
		if (componentUI != null)
		{
			componentUI.SetActive(false);
		}
	}

	private void OnStartMenuPressed(object sender, ControllerInteractionEventArgs e)
	{
		if (!anim.GetBool("Gun") && !DroneHelper.instance.VRControlled)
		{
			Inventory.instance.ToggleMenu(events.transform);
		}
		else if (DroneHelper.instance.VRControlled)
		{
			DroneArmUIManager.instance.ToggleTool();
		}
	}

	private void OnButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
	{
		if (!IntroOxygenEvent.instance)
		{
			Gun.instance.reloadDuration = 0f;
			Gun.instance.audioSource.Stop();
			Gun.instance.UpdateUI();
		}
	}

	private void OnTouchpadPressed(object sender, ControllerInteractionEventArgs e)
	{
		Vector2 touchpadAxis = e.touchpadAxis;
		if (anim.GetBool("Gun") || Mathf.Abs(touchpadAxis.magnitude) > 0.5f || (DroneHelper.instance.VRControlled && DroneArmUIManager.instance.toolIndex != 0))
		{
			return;
		}
		if (SuitManager.instance.power > 0f)
		{
			if (GetComponentInChildren<VRTK_Pointer>().IsPointerActive())
			{
				ArmUIManager.instance.ShowInventory();
				DisblePointer();
			}
			else if (!deteriorationCone.activeSelf)
			{
				ArmUIManager.instance.ShowInventory();
				anim.SetBool("Point", true);
				deteriorationCone.SetActive(true);
				if (GetComponentInChildren<VRTK_Pointer>().IsPointerActive())
				{
					GetComponentInChildren<VRTK_Pointer>().currentActivationState = 0;
					GetComponentInChildren<VRTK_Pointer>().Toggle(false);
				}
			}
			else
			{
				ExitSalvageMode();
			}
		}
		else
		{
			SuitManager.instance.LowPowerPrompt();
		}
		if (currentHand != null && currentHand != this)
		{
			currentHand.componentUI.SetActive(false);
			if (currentHand.GetComponentInChildren<VRTK_Pointer>().IsPointerActive())
			{
				currentHand.GetComponentInChildren<VRTK_Pointer>().currentActivationState = 0;
				currentHand.GetComponentInChildren<VRTK_Pointer>().Toggle(false);
			}
			currentHand.anim.SetBool("Point", false);
			currentHand.deteriorationCone.SetActive(false);
			currentHand = this;
			craftingManager.StopSalvage();
			craftingManager.StopRepair();
			craftingManager.repairObject = null;
			craftingManager.scrapObject = null;
		}
		else
		{
			currentHand = this;
		}
	}

	private void OnTriggerPressed(object sender, ControllerInteractionEventArgs e)
	{
		if (DroneHelper.instance.VRControlled && DroneArmUIManager.instance.toolIndex == 1)
		{
			DroneArmUIManager.instance.OnShoot(base.transform);
			return;
		}
		RaycastHit hitInfo;
		if (anim.GetBool("Point") && Physics.Raycast(fingerAttachPoint.position, GetComponentInChildren<VRTK_Pointer>().customOrigin.forward, out hitInfo, 100f, layerMask))
		{
			hitInfo.transform.SendMessage("OnSignal", SendMessageOptions.DontRequireReceiver);
		}
		if (triggerTarget != null && !ArmUIManager.instance.gripSwap)
		{
			triggerTarget.SendMessage("OnTrigger", SendMessageOptions.DontRequireReceiver);
		}
		if (base.transform == GameManager.instance.rightController.parent)
		{
			CraftingManager.instance.OnTriggerPressed(this);
		}
		if (base.transform == GameManager.instance.rightController.parent)
		{
			BaseCraftManager.instance.OnTriggerPressed(this);
		}
		if ((bool)IntroEventRepair.instance)
		{
			IntroEventRepair.instance.OnRepair();
		}
	}

	private void OnGripPressed(object sender, ControllerInteractionEventArgs e)
	{
		if (triggerTarget != null && ArmUIManager.instance.gripSwap)
		{
			triggerTarget.SendMessage("OnTrigger", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerReleased(object sender, ControllerInteractionEventArgs e)
	{
		if (currentHand == this)
		{
			craftingManager.StopSalvage();
			craftingManager.StopRepair();
		}
		if ((bool)IntroEventRepair.instance)
		{
			IntroEventRepair.instance.OnCancelRepair();
		}
		if (toolAudio.clip == salvageTone)
		{
			toolAudio.Stop();
			toolAudio.clip = null;
			Invoke("ReleaseCooldown", 0.5f);
		}
		toolAudio.pitch = 1f;
	}

	private void Update()
	{
		if (!ArmUIManager.instance)
		{
			return;
		}
		gripSwap = ArmUIManager.instance.gripSwap;
		bool flag = (!gripSwap && events.GetTriggerAxis() > 0.8f) || (gripSwap && events.gripPressed);
		if ((!gripSwap && events.GetTriggerAxis() > 0.05f) || (gripSwap && events.gripPressed) || smooth > 0f)
		{
			if (!gripSwap)
			{
				if (events.GetTriggerAxis() < 0.05f)
				{
					smooth = Mathf.MoveTowards(smooth, 0f, 5f * Time.deltaTime);
				}
				else
				{
					smooth = Mathf.MoveTowards(smooth, events.GetTriggerAxis(), 5f * Time.deltaTime);
				}
			}
			else if (events.gripPressed)
			{
				smooth = Mathf.MoveTowards(smooth, 1f, 5f * Time.deltaTime);
			}
			else
			{
				smooth = Mathf.MoveTowards(smooth, 0f, 5f * Time.deltaTime);
			}
			anim.SetFloat("Grip", smooth);
			anim.SetFloat("Shoot", smooth);
			if (GetComponentInChildren<VRTK_Pointer>().IsPointerActive() && (bool)GetComponentInChildren<VRTK_InteractGrab>().GetGrabbedObject())
			{
				GetComponentInChildren<VRTK_Pointer>().currentActivationState = 0;
				GetComponentInChildren<VRTK_Pointer>().Toggle(false);
			}
			if (deteriorationCone.activeSelf)
			{
				if (!IsInvoking("ReleaseCooldown") && flag)
				{
					craftingManager.AttemtRepair(currentHand);
				}
				else if (craftingManager.isRepairing)
				{
					craftingManager.StopRepair();
					particle.SetActive(false);
				}
				if (!IsInvoking("ReleaseCooldown") && flag)
				{
					craftingManager.AttemtSalvage(base.gameObject);
				}
				else if (craftingManager.isSalvaging)
				{
					craftingManager.StopSalvage();
					particle.SetActive(false);
				}
				particle.SetActive(true);
				if (particle.activeSelf && (craftingManager.scrapObject != null || craftingManager.repairObject != null || ((bool)IntroEventRepair.instance && IntroEventRepair.instance.active)) && flag)
				{
					if (!toolAudio.isPlaying && !IsInvoking("ReleaseCooldown"))
					{
						toolAudio.clip = salvageTone;
						toolAudio.Play();
						toolAudio.pitch = 1.5f;
					}
					if (craftingManager.scrapObject != null)
					{
						toolAudio.pitch += 0.25f * Time.deltaTime;
					}
				}
				else
				{
					if (toolAudio.clip == salvageTone)
					{
						toolAudio.Stop();
						toolAudio.clip = null;
					}
					toolAudio.pitch = 1f;
					particle.SetActive(false);
				}
				if ((bool)IntroEventRepair.instance)
				{
					IntroEventRepair.instance.OnRepair();
				}
			}
		}
		else
		{
			if (deteriorationCone.activeSelf)
			{
				particle.SetActive(false);
			}
			if (currentHand == this)
			{
				if (craftingManager.isSalvaging)
				{
					craftingManager.StopSalvage();
				}
				if (craftingManager.isRepairing)
				{
					craftingManager.StopRepair();
				}
			}
		}
		if (deteriorationCone.activeSelf && SuitManager.instance.power < 1f)
		{
			deteriorationCone.SetActive(false);
			anim.SetBool("Point", false);
			SuitManager.instance.LowPowerPrompt();
		}
		if (events.buttonTwoPressed && anim.GetBool("Gun"))
		{
			Gun.instance.Reloading();
		}
	}

	public void PoseTrigger(string poseName)
	{
		if (!(poseName == "Point"))
		{
			return;
		}
		if (anim.GetBool("Point"))
		{
			if (GetComponentInChildren<VRTK_Pointer>().IsPointerActive())
			{
				GetComponentInChildren<VRTK_Pointer>().currentActivationState = 0;
				GetComponentInChildren<VRTK_Pointer>().Toggle(false);
			}
			GetComponentInChildren<VRTK_InteractGrab>().controllerAttachPoint = fingerAttachPoint;
		}
		else
		{
			GetComponentInChildren<VRTK_InteractGrab>().controllerAttachPoint = handAttachPoint;
		}
	}

	public void EnterContructioneMode()
	{
		if (!(deteriorationCone == null))
		{
			anim.SetBool("Point", true);
			if (IntroOxygenEvent.instance == null && BaseManager.instance.atBase)
			{
				ArmUIManager.instance.ShowCraftTab();
			}
		}
	}

	public void ExitSalvageMode()
	{
		if (!(deteriorationCone == null))
		{
			anim.SetBool("Point", false);
			deteriorationCone.SetActive(false);
			componentUI.SetActive(false);
			if (!ArmUIManager.instance.tabs[1].activeSelf)
			{
				ArmUIManager.instance.ShowVitalsTab();
			}
		}
	}

	public void DisblePointer()
	{
		anim.SetBool("Point", false);
		GetComponentInChildren<VRTK_Pointer>().currentActivationState = 0;
		GetComponentInChildren<VRTK_Pointer>().Toggle(false);
		CraftingManager.instance.proxy.gameObject.SetActive(false);
		BaseCraftManager.instance.roomProxy.gameObject.SetActive(false);
		GameManager.instance.NodeProxy.SetActive(false);
		if ((bool)BaseManager.instance)
		{
			BaseManager.instance.StopCoroutine("UpdateBuildMarkers");
		}
	}

	public void CompleteSalvage()
	{
		toolAudio.Stop();
		toolAudio.clip = null;
		toolAudio.pitch = 1f;
		toolAudio.PlayOneShot(salvageComplete);
	}

	public void OnRepairAudio()
	{
		toolAudio.PlayOneShot(salvageComplete);
	}

	public void ToggleRepairCone()
	{
		deteriorationCone.GetComponent<MeshRenderer>().material = repairMat;
		deteriorationParticle.SetActive(false);
		particle = repairParticle;
	}

	public void ToggleSalvageCone()
	{
		deteriorationCone.GetComponent<MeshRenderer>().material = salvageMat;
		repairParticle.SetActive(false);
		particle = deteriorationParticle;
	}

	private void ReleaseCooldown()
	{
	}

	public void OnGrab()
	{
		toolAudio.PlayOneShot(grabSounds[Random.Range(0, grabSounds.Length)]);
	}

	public void OnPointerEnter(RaycastHit giventHit)
	{
		if ((bool)giventHit.transform.root.GetComponent<CenterRoom>() && giventHit.collider.tag == "Wall")
		{
			CraftingManager.instance.UpdateProxyState(giventHit.point, giventHit.normal, giventHit.collider);
		}
	}

	public void OnPointerExit(RaycastHit giventHit)
	{
	}
}
