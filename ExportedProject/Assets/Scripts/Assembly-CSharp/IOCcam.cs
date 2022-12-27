using UnityEngine;

public class IOCcam : MonoBehaviour
{
	public string tags;

	public LayerMask layerMsk;

	public int occludeeLayer;

	public int samples;

	public float raysFov;

	public bool preCullCheck;

	public float viewDistance;

	public int hideDelay;

	public bool realtimeShadows;

	public float lod1Distance;

	public float lod2Distance;

	public int lightProbes;

	public float probeRadius;

	private RaycastHit hit;

	private Ray r;

	private int layerMask;

	private IOCcomp iocComp;

	private int haltonIndex;

	private float[] hx;

	private float[] hy;

	private int pixels;

	private Camera cam;

	private Camera rayCaster;

	private CullingGroup cGroup;

	private BoundingSphere[] spheres;

	private int boundingSphereCounter;

	private int currentSphereIndex;

	private int[] sphereIndices = new int[2048];

	private void Awake()
	{
		cam = GetComponent<Camera>();
		hit = default(RaycastHit);
		if (viewDistance == 0f)
		{
			viewDistance = 100f;
		}
		cam.farClipPlane = viewDistance;
		haltonIndex = 0;
		if (GetComponent<SphereCollider>() == null)
		{
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = 1f;
			sphereCollider.isTrigger = true;
		}
		cGroup = new CullingGroup();
		cGroup.targetCamera = cam;
		spheres = new BoundingSphere[2048];
		cGroup.SetBoundingSpheres(spheres);
		cGroup.SetBoundingSphereCount(0);
	}

	private void OnApplicationQuit()
	{
		cGroup.Dispose();
		cGroup = null;
	}

	public void AddBoundingSphere(BoundingSphere sphere)
	{
		spheres[boundingSphereCounter] = sphere;
		boundingSphereCounter++;
		cGroup.SetBoundingSphereCount(boundingSphereCounter);
	}

	private void Start()
	{
		pixels = Mathf.FloorToInt((float)(Screen.width * Screen.height) / 4f);
		hx = new float[pixels];
		hy = new float[pixels];
		for (int i = 0; i < pixels; i++)
		{
			hx[i] = HaltonSequence(i, 2);
			hy[i] = HaltonSequence(i, 3);
		}
		Object[] array = Object.FindObjectsOfType(typeof(GameObject));
		for (int j = 0; j < array.Length; j++)
		{
			GameObject gameObject = (GameObject)array[j];
			if (!tags.Contains(gameObject.tag))
			{
				continue;
			}
			if (gameObject.GetComponent<Light>() != null)
			{
				if (gameObject.GetComponent<IOClight>() == null)
				{
					gameObject.AddComponent<IOClight>();
				}
			}
			else if (gameObject.GetComponent<Terrain>() != null)
			{
				gameObject.AddComponent<IOCterrain>();
			}
			else if (gameObject.GetComponent<IOClod>() == null)
			{
				gameObject.AddComponent<IOClod>();
			}
		}
		GameObject gameObject2 = new GameObject("RayCaster");
		gameObject2.transform.Translate(base.transform.position);
		gameObject2.transform.rotation = base.transform.rotation;
		rayCaster = gameObject2.AddComponent<Camera>();
		rayCaster.enabled = false;
		rayCaster.clearFlags = CameraClearFlags.Nothing;
		rayCaster.cullingMask = 0;
		rayCaster.aspect = cam.aspect;
		rayCaster.nearClipPlane = cam.nearClipPlane;
		rayCaster.farClipPlane = cam.farClipPlane;
		rayCaster.fieldOfView = raysFov;
		gameObject2.transform.parent = base.transform;
	}

	private void Update()
	{
		int num = cGroup.QueryIndices(true, sphereIndices, 0);
		for (int i = 0; i < num; i++)
		{
			BoundingSphere boundingSphere = spheres[sphereIndices[currentSphereIndex]];
			Vector3 vector = Random.onUnitSphere * boundingSphere.radius + boundingSphere.position;
			Vector3 position = base.transform.position;
			r = new Ray(position, vector - position);
			currentSphereIndex++;
			if (currentSphereIndex >= num)
			{
				currentSphereIndex = 0;
			}
			if (Physics.Raycast(r, out hit, viewDistance, layerMsk.value))
			{
				Unhide(hit.transform, hit);
			}
		}
		for (int j = 0; j <= samples; j++)
		{
			r = rayCaster.ViewportPointToRay(new Vector3(hx[haltonIndex], hy[haltonIndex], 0f));
			haltonIndex++;
			if (haltonIndex >= pixels)
			{
				haltonIndex = 0;
			}
			if (Physics.Raycast(r, out hit, viewDistance, layerMsk.value))
			{
				Unhide(hit.transform, hit);
			}
		}
	}

	private void Unhide(Transform t, RaycastHit hit)
	{
		if ((bool)(iocComp = t.GetComponent<IOCcomp>()))
		{
			iocComp.UnHide(hit);
		}
		else if (t.parent != null)
		{
			Unhide(t.parent, hit);
		}
	}

	private float HaltonSequence(int index, int b)
	{
		float num = 0f;
		float num2 = 1f / (float)b;
		int num3 = index;
		while (num3 > 0)
		{
			num += num2 * (float)(num3 % b);
			num3 = Mathf.FloorToInt(num3 / b);
			num2 /= (float)b;
		}
		return num;
	}
}
