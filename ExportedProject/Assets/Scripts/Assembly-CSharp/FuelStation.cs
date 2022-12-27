using System.Collections;
using UnityEngine;
using VRTK;

public class FuelStation : MonoBehaviour
{
	public Vector3 offset;

	internal Transform target;

	public static FuelStation instance;

	internal bool drain;

	public float drainSpeed = 10f;

	private float targetMaxValue;

	public bool power;

	public Transform snapPoint;

	private void Awake()
	{
		instance = this;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(target != null))
		{
			if ((bool)other.GetComponent<CraftObject>() && other.GetComponent<CraftObject>().craftObjectType == CraftObject.CraftObjectType.smallCanister)
			{
				SetupEmptyCan(other.gameObject);
			}
			if ((power && (bool)other.GetComponent<PowerContainer>() && !other.GetComponent<PowerContainer>().IsInvoking("SnapCooldown") && !other.GetComponent<PowerContainer>().broken) || (!power && (bool)other.GetComponent<OxygenContainer>() && !other.GetComponent<OxygenContainer>().IsInvoking("SnapCooldown") && !other.GetComponent<OxygenContainer>().broken))
			{
				other.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
				other.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
				other.isTrigger = true;
				target = other.transform;
				Invoke("Snap", 0.1f);
			}
		}
	}

	private void SetupEmptyCan(GameObject emptyCan)
	{
		if (!power)
		{
			OxygenContainer oxygenContainer = emptyCan.AddComponent<OxygenContainer>();
			oxygenContainer.oxygenValue = 0f;
			oxygenContainer.startValue = 25f;
			oxygenContainer.brokenMesh = MeshList.instance.meshes[2];
			GameObject gameObject = emptyCan.transform.GetChild(0).gameObject;
			gameObject.GetComponent<MeshFilter>().mesh = MeshList.instance.meshes[0];
			gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.atlasMaterial;
			gameObject.transform.localScale = new Vector3(1f, 1f, 0f);
			oxygenContainer.oxygenVisual = gameObject.transform;
			oxygenContainer.name = "OxygenCan Small";
		}
		else
		{
			PowerContainer powerContainer = emptyCan.AddComponent<PowerContainer>();
			powerContainer.powerValue = 0f;
			powerContainer.startValue = 13f;
			powerContainer.brokenMesh = MeshList.instance.meshes[2];
			GameObject gameObject2 = emptyCan.transform.GetChild(0).gameObject;
			gameObject2.GetComponent<MeshFilter>().mesh = MeshList.instance.meshes[1];
			gameObject2.GetComponent<MeshRenderer>().material = GameManager.instance.atlasMaterial;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 0f);
			powerContainer.powerVisual = gameObject2.transform;
			powerContainer.name = "PowerCan Small";
		}
		drain = true;
		Object.Destroy(emptyCan.GetComponent<CraftObject>());
	}

	private void Snap()
	{
		target.SetParent(snapPoint);
		if (target.parent != null)
		{
			target.localPosition = new Vector3(0f, 0f, 0f);
			target.localEulerAngles = new Vector3(0f, 0f, 0f);
			target.GetComponent<VRTK_InteractableObject>().previousParent = base.transform;
			target.GetComponent<VRTK_InteractableObject>().previousKinematicState = true;
			target.GetComponent<Rigidbody>().isKinematic = true;
			if (power)
			{
				target.GetComponent<PowerContainer>().locked = true;
				target.GetComponent<PowerContainer>().fuelStation = this;
			}
			else
			{
				target.GetComponent<OxygenContainer>().locked = true;
				target.GetComponent<OxygenContainer>().fuelStation = this;
			}
			if ((bool)target)
			{
				UpdateState();
			}
		}
		target.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
	}

	public void ToggleDrain()
	{
		BaseComputer.instance.Beep();
		if ((bool)target)
		{
			UpdateState();
		}
	}

	public void UpdateState()
	{
		if (!drain)
		{
			if ((bool)target.GetComponent<PowerContainer>())
			{
				StopCoroutine("FillPower");
				target.GetComponent<PowerContainer>().StartCoroutine("DrainToBase");
			}
			else if ((bool)target.GetComponent<OxygenContainer>())
			{
				StopCoroutine("FillOxygen");
				target.GetComponent<OxygenContainer>().StartCoroutine("DrainToBase");
			}
		}
		else if ((bool)target.GetComponent<PowerContainer>())
		{
			target.GetComponent<PowerContainer>().StopCoroutine("DrainToBase");
			StartCoroutine("FillPower");
		}
		else if ((bool)target.GetComponent<OxygenContainer>())
		{
			target.GetComponent<OxygenContainer>().StopCoroutine("DrainToBase");
			StartCoroutine("FillOxygen");
		}
	}

	public IEnumerator FillPower()
	{
		float refreshRate = 0.05f;
		PowerContainer powerContainer = target.GetComponent<PowerContainer>();
		targetMaxValue = powerContainer.startValue;
		while (target != null && BaseManager.instance.power > 0f)
		{
			if (powerContainer.powerValue < targetMaxValue)
			{
				BaseManager.instance.power -= refreshRate * drainSpeed;
				powerContainer.powerValue += refreshRate * drainSpeed;
				float z = powerContainer.powerValue / powerContainer.startValue;
				powerContainer.powerVisual.localScale = new Vector3(1f, 1f, z);
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public IEnumerator FillOxygen()
	{
		float refreshRate = 0.05f;
		OxygenContainer oxygenContainer = target.GetComponent<OxygenContainer>();
		targetMaxValue = oxygenContainer.startValue;
		while (target != null)
		{
			if (oxygenContainer.oxygenValue < targetMaxValue)
			{
				oxygenContainer.oxygenValue += refreshRate * drainSpeed;
				float z = oxygenContainer.oxygenValue / oxygenContainer.startValue;
				oxygenContainer.oxygenVisual.localScale = new Vector3(1f, 1f, z);
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
