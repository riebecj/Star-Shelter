using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

public class HoloMap : MonoBehaviour
{
	public List<Transform> debrisTransforms = new List<Transform>();

	public List<Transform> cometTransforms = new List<Transform>();

	public List<Transform> shipTransforms = new List<Transform>();

	public List<GameObject> debrisIcons = new List<GameObject>();

	public List<GameObject> cometIcons = new List<GameObject>();

	public List<GameObject> impactPointIcons = new List<GameObject>();

	public List<GameObject> shipIcons = new List<GameObject>();

	public List<GameObject> roomIcons = new List<GameObject>();

	public GameObject _cometIconSmall;

	public GameObject _cometIconMid;

	public GameObject _cometIconBig;

	public GameObject _debrisIcon;

	public GameObject impactPointIcon;

	public GameObject shieldButton;

	public GameObject _shipIcon;

	public GameObject mapTarget;

	public GameObject worldTarget;

	public GameObject roomIcon;

	public GameObject playerIcon;

	public Transform shieldButtonParent;

	internal List<ShieldButton> ShieldButtons = new List<ShieldButton>();

	internal Transform targetWreck;

	internal Transform Base;

	internal Transform player;

	public LayerMask layerMask;

	private Transform lastCometPos;

	internal GameObject _mapTarget;

	internal GameObject _worldTarget;

	public static List<HoloMap> holoMaps = new List<HoloMap>();

	internal List<Transform> rooms = new List<Transform>();

	public Material wholeRoomMaterial;

	public Material brokenRoomMaterial;

	public Transform worldObjects;

	private void Awake()
	{
		holoMaps.Add(this);
	}

	private void Start()
	{
		Base = BaseManager.instance.transform;
		InvokeRepeating("UpdatePositions", 0f, 0.05f);
		_mapTarget = Object.Instantiate(mapTarget, new Vector3(0f, 1000f, 0f), Quaternion.identity);
		_worldTarget = Object.Instantiate(worldTarget, new Vector3(0f, 1000f, 0f), Quaternion.identity);
		_worldTarget.SetActive(false);
		_mapTarget.SetActive(false);
		player = GameManager.instance.CamRig.transform;
		Setup();
		if ((bool)ThreatManager.instance)
		{
			ThreatManager.instance.OnEnableUI();
		}
	}

	private void Setup()
	{
		for (int i = 0; i < BaseLoader.instance.AllRooms.Count; i++)
		{
			OnAddRoom(BaseLoader.instance.AllRooms[i].gameObject);
		}
		for (int j = 0; j < CometManager.instance.activeComets.Count; j++)
		{
			AddComet(CometManager.instance.activeComets[j].transform, CometManager.instance.activeComets[j].GetComponent<Comet>().size);
		}
		for (int k = 0; k < WreckManager.instance.activeWrecks.Count; k++)
		{
			AddWreck(WreckManager.instance.activeWrecks[k].transform);
		}
		for (int l = 0; l < StationManager.instance.spawnedStations.Count; l++)
		{
			AddShip(StationManager.instance.spawnedStations[l]);
		}
	}

	public void OnAddRoom(GameObject room)
	{
		if (!rooms.Contains(room.transform))
		{
			Transform transform = room.transform;
			GameObject gameObject = Object.Instantiate(roomIcon, base.transform.position, transform.rotation);
			gameObject.transform.SetParent(worldObjects.transform);
			gameObject.GetComponent<MeshFilter>().sharedMesh = room.GetComponent<MeshFilter>().sharedMesh;
			gameObject.transform.rotation = room.transform.rotation;
			gameObject.transform.localScale = Vector3.one / 87f;
			Vector3 vector = (transform.position - base.transform.position) / 87f;
			vector = Vector3.ClampMagnitude(vector, 1f);
			gameObject.transform.localPosition = new Vector3(vector.x, vector.y, vector.z);
			rooms.Add(transform);
			roomIcons.Add(gameObject);
		}
	}

	public void SetTarget(Transform target)
	{
		_worldTarget.SetActive(true);
		_mapTarget.SetActive(true);
		targetWreck = target;
		_mapTarget.transform.position = targetWreck.position + Vector3.up * 0.1f;
		_mapTarget.transform.SetParent(targetWreck);
		for (int i = 0; i < debrisIcons.Count; i++)
		{
			if (debrisIcons[i].transform == targetWreck)
			{
				_worldTarget.transform.position = debrisTransforms[i].position + Vector3.up * 3f;
				_worldTarget.transform.SetParent(debrisTransforms[i]);
				_worldTarget.GetComponent<FollowTarget>().target = debrisTransforms[i];
				_worldTarget.GetComponent<FollowTarget>().offset = new Vector3(0f, 4f, 0f);
			}
		}
		for (int j = 0; j < shipIcons.Count; j++)
		{
			if (shipIcons[j].transform == targetWreck)
			{
				_worldTarget.transform.position = shipTransforms[j].position + Vector3.up * 6f;
				_worldTarget.transform.SetParent(shipTransforms[j]);
				_worldTarget.GetComponent<FollowTarget>().target = debrisTransforms[j];
				_worldTarget.GetComponent<FollowTarget>().offset = new Vector3(0f, 4f, 0f);
			}
		}
	}

	public void OnLoseTarget()
	{
		_mapTarget.transform.SetParent(null);
		_mapTarget.SetActive(false);
		_worldTarget.transform.SetParent(null);
		_worldTarget.SetActive(false);
	}

	public void AddWreck(Transform debrisPosition)
	{
		if (!debrisTransforms.Contains(debrisPosition))
		{
			debrisTransforms.Add(debrisPosition);
			GameObject gameObject = Object.Instantiate(_debrisIcon, base.transform.position, base.transform.rotation);
			gameObject.GetComponent<MapTarget>().holomap = this;
			gameObject.transform.SetParent(worldObjects.transform);
			debrisIcons.Add(gameObject);
		}
	}

	public void RemoveWreck(Transform cometPosition)
	{
		List<GameObject> list = debrisIcons;
		List<Transform> list2 = debrisTransforms;
		int index = debrisTransforms.IndexOf(cometPosition);
		GameObject obj = list[index];
		list.RemoveAt(index);
		list2.Remove(cometPosition);
		Object.Destroy(obj);
		debrisIcons = list;
		debrisTransforms = list2;
	}

	public void AddComet(Transform cometPosition, int size)
	{
	}

	public void AddShip(Transform shipPosition)
	{
		if (!shipTransforms.Contains(shipPosition))
		{
			GameObject gameObject = Object.Instantiate(_shipIcon, base.transform.position, shipPosition.rotation);
			gameObject.transform.SetParent(worldObjects.transform);
			gameObject.transform.eulerAngles = new Vector3(270f, 0f, 0f);
			Vector3 vector = (shipPosition.position - Base.position) / 87f;
			vector = Vector3.ClampMagnitude(vector, 1f);
			gameObject.transform.localPosition = new Vector3(vector.x, vector.y, vector.z);
			shipTransforms.Add(shipPosition);
			shipIcons.Add(gameObject);
		}
	}

	public void RemoveShip(Transform shipPosition)
	{
		for (int i = 0; i < shipIcons.Count; i++)
		{
			if (shipTransforms[i] == shipPosition)
			{
				GameObject gameObject = shipIcons[i];
				shipIcons.Remove(shipIcons[i]);
				shipTransforms.Remove(shipTransforms[i]);
				Object.Destroy(gameObject.gameObject);
			}
		}
	}

	public void RemoveComet(Transform cometPosition)
	{
	}

	public void AddImpactIcon()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(lastCometPos.position, lastCometPos.forward, out hitInfo, 10f, layerMask))
		{
			Vector3 point = hitInfo.point;
			GameObject gameObject = Object.Instantiate(impactPointIcon, base.transform.position, lastCometPos.rotation);
			gameObject.transform.SetParent(worldObjects);
			impactPointIcons.Add(impactPointIcon);
			gameObject.transform.position = point;
			gameObject.transform.LookAt(worldObjects);
			lastCometPos.GetComponent<CometIcon>().impactPoint = gameObject.transform;
			if (lastCometPos.gameObject.activeInHierarchy)
			{
				lastCometPos.GetComponent<CometIcon>().StartCoroutine("UpdateImpactPoint");
			}
		}
	}

	public void AddShield(HoloShield shield)
	{
	}

	private void UpdatePositions()
	{
		base.transform.LookAt(Base.position + Base.forward * 100f);
		Vector3 localPosition = (player.position - base.transform.position) / 87f;
		playerIcon.transform.localPosition = localPosition;
		if (debrisIcons.Count > 0 && debrisIcons[0] != null)
		{
			for (int i = 0; i < debrisIcons.Count; i++)
			{
				Vector3 vector = (debrisTransforms[i].transform.position - Base.position) / 87f;
				debrisIcons[i].transform.localPosition = new Vector3(vector.x, vector.y, vector.z);
				float num = Vector3.Distance(Base.position, debrisTransforms[i].transform.position);
				debrisIcons[i].GetComponentInChildren<Text>().text = num.ToString("F0") + "m";
				debrisIcons[i].GetComponentInChildren<Canvas>().transform.LookAt(GameManager.instance.Head.position, Vector3.up);
			}
		}
		if (cometIcons.Count <= 0 || !(cometIcons[0] != null))
		{
			return;
		}
		for (int j = 0; j < cometIcons.Count; j++)
		{
			Vector3 vector2 = (cometTransforms[j].transform.position - Base.position) / 87f;
			vector2 = Vector3.ClampMagnitude(vector2, 1f);
			cometIcons[j].transform.localPosition = new Vector3(vector2.x, vector2.y, vector2.z);
			float num2 = Vector3.Distance(Base.position, cometTransforms[j].transform.position);
			cometIcons[j].GetComponentInChildren<Text>().text = num2.ToString("F0") + "m";
			cometIcons[j].GetComponentInChildren<Canvas>().transform.LookAt(GameManager.instance.Head.position, Vector3.up);
			if ((bool)cometIcons[j].GetComponent<CometIcon>().impactPoint)
			{
				cometIcons[j].GetComponent<CometIcon>().impactPoint.rotation = Quaternion.LookRotation(cometIcons[j].transform.position + cometTransforms[j].GetComponent<Rigidbody>().velocity);
			}
		}
	}

	private float GetMapPos(float pos, float mapSize, float sceneSize)
	{
		return pos * mapSize / sceneSize;
	}

	private void OnDisable()
	{
		holoMaps.Remove(this);
	}
}
