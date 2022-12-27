using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class IOClod : IOCcomp
{
	public bool Static;

	public bool Occludee;

	public float Lod1;

	public float Lod2;

	public float LodMargin;

	public bool LodOnly;

	private int currentLayer;

	private Vector3 hitPoint;

	private float lod_1;

	private float lod_2;

	private float lodMargin;

	private bool realtimeShadows;

	private IOCcam iocCam;

	private int counter;

	private Renderer[] rs0;

	private Renderer[] rs1;

	private Renderer[] rs2;

	private Renderer[] rs;

	private bool hidden;

	private int currentLod;

	private float prevDist;

	private float distOffset;

	private int lods;

	private float dt;

	private float hitTimeOffset;

	private float prevHitTime;

	private bool sleeping;

	private ShadowCastingMode[] originalShadowCastingMode;

	private float distanceFromCam;

	private float shadowDistance;

	private int frameInterval;

	private RaycastHit h;

	private Ray r;

	private bool visible;

	private Vector3 p;

	[CompilerGenerated]
	private static Func<Renderer, bool> _003C_003Ef__am_0024cache0;

	[CompilerGenerated]
	private static Func<Renderer, bool> _003C_003Ef__am_0024cache1;

	[CompilerGenerated]
	private static Func<Renderer, bool> _003C_003Ef__am_0024cache2;

	[CompilerGenerated]
	private static Func<Renderer, bool> _003C_003Ef__am_0024cache3;

	private void Awake()
	{
	}

	public override void Init()
	{
		try
		{
			iocCam = Camera.main.GetComponent<IOCcam>();
			shadowDistance = QualitySettings.shadowDistance * 2f;
			currentLayer = base.gameObject.layer;
			prevDist = 0f;
			prevHitTime = Time.time;
			sleeping = true;
			h = default(RaycastHit);
			base.enabled = true;
		}
		catch (Exception ex)
		{
			base.enabled = false;
			Debug.Log(ex.Message);
		}
	}

	private void Start()
	{
		Init();
		UpdateValues();
		if ((bool)base.transform.Find("Lod_0"))
		{
			lods = 1;
			Renderer[] componentsInChildren = base.transform.Find("Lod_0").GetComponentsInChildren<Renderer>(false);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CStart_003Em__0;
			}
			rs0 = componentsInChildren.Where(_003C_003Ef__am_0024cache0).ToArray();
			originalShadowCastingMode = new ShadowCastingMode[rs0.Length];
			for (int i = 0; i < rs0.Length; i++)
			{
				originalShadowCastingMode[i] = rs0[i].shadowCastingMode;
			}
			if ((bool)base.transform.Find("Lod_1"))
			{
				lods++;
				Renderer[] componentsInChildren2 = base.transform.Find("Lod_1").GetComponentsInChildren<Renderer>(false);
				if (_003C_003Ef__am_0024cache1 == null)
				{
					_003C_003Ef__am_0024cache1 = _003CStart_003Em__1;
				}
				rs1 = componentsInChildren2.Where(_003C_003Ef__am_0024cache1).ToArray();
				if ((bool)base.transform.Find("Lod_2"))
				{
					lods++;
					Renderer[] componentsInChildren3 = base.transform.Find("Lod_2").GetComponentsInChildren<Renderer>(false);
					if (_003C_003Ef__am_0024cache2 == null)
					{
						_003C_003Ef__am_0024cache2 = _003CStart_003Em__2;
					}
					rs2 = componentsInChildren3.Where(_003C_003Ef__am_0024cache2).ToArray();
				}
			}
			if (Static)
			{
				BoundingSphere boundingSphere = GetBoundingSphere(rs0);
				iocCam.AddBoundingSphere(boundingSphere);
			}
			if (LodMargin != 0f)
			{
				lodMargin = LodMargin;
			}
			else
			{
				lodMargin = GetLODmargin(rs0);
			}
		}
		else
		{
			lods = 0;
			Renderer[] componentsInChildren4 = GetComponentsInChildren<Renderer>(false);
			if (_003C_003Ef__am_0024cache3 == null)
			{
				_003C_003Ef__am_0024cache3 = _003CStart_003Em__3;
			}
			rs = componentsInChildren4.Where(_003C_003Ef__am_0024cache3).ToArray();
			originalShadowCastingMode = new ShadowCastingMode[rs.Length];
			for (int j = 0; j < rs.Length; j++)
			{
				originalShadowCastingMode[j] = rs[j].shadowCastingMode;
			}
			if (Static)
			{
				BoundingSphere boundingSphere2 = GetBoundingSphere(rs);
				iocCam.AddBoundingSphere(boundingSphere2);
			}
		}
		Initialize();
	}

	public void Initialize()
	{
		if (iocCam.enabled)
		{
			HideAll();
		}
		else
		{
			base.enabled = false;
			ShowLod(1f);
		}
		base.gameObject.layer = currentLayer;
	}

	private void Update()
	{
		frameInterval = Time.frameCount % 4;
		if (frameInterval == 0)
		{
			switch (LodOnly)
			{
			case false:
				if (hidden || Time.frameCount - counter <= iocCam.hideDelay)
				{
					break;
				}
				switch (currentLod)
				{
				case 0:
					visible = rs0[0].isVisible;
					break;
				case 1:
					visible = rs1[0].isVisible;
					break;
				case 2:
					visible = rs2[0].isVisible;
					break;
				default:
					visible = rs[0].isVisible;
					break;
				}
				if (((iocCam.preCullCheck && visible) | Occludee) && visible)
				{
					p = base.transform.localToWorldMatrix.MultiplyPoint(hitPoint);
					r = new Ray(p, iocCam.transform.position - p);
					if (!Physics.Raycast(r, out h, iocCam.viewDistance))
					{
						break;
					}
					if (!h.collider.CompareTag(iocCam.tag))
					{
						Hide();
						break;
					}
					counter = Time.frameCount;
					if (Occludee & (lods > 0))
					{
						ShowLod(h.distance);
					}
				}
				else
				{
					Hide();
				}
				break;
			case true:
				if (sleeping || Time.frameCount - counter <= iocCam.hideDelay)
				{
					break;
				}
				if (Occludee)
				{
					base.gameObject.layer = currentLayer;
					Vector3 vector = base.transform.localToWorldMatrix.MultiplyPoint(hitPoint);
					r = new Ray(vector, iocCam.transform.position - vector);
					if (Physics.Raycast(r, out h, iocCam.viewDistance) && !h.collider.CompareTag(iocCam.tag))
					{
						ShowLod(3000f);
						sleeping = true;
					}
				}
				else
				{
					ShowLod(3000f);
					sleeping = true;
				}
				break;
			}
		}
		else
		{
			if (!realtimeShadows || frameInterval != 2)
			{
				return;
			}
			distanceFromCam = Vector3.Distance(base.transform.position, iocCam.transform.position);
			if (!hidden)
			{
				return;
			}
			if (lods == 0)
			{
				if (distanceFromCam > shadowDistance)
				{
					if (rs[0].enabled)
					{
						for (int i = 0; i < rs.Length; i++)
						{
							rs[i].enabled = false;
							rs[i].shadowCastingMode = originalShadowCastingMode[i];
						}
					}
				}
				else if (!rs[0].enabled)
				{
					for (int j = 0; j < rs.Length; j++)
					{
						rs[j].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
						rs[j].enabled = true;
					}
				}
			}
			else if (distanceFromCam > shadowDistance)
			{
				if (rs0[0].enabled)
				{
					for (int k = 0; k < rs0.Length; k++)
					{
						rs0[k].enabled = false;
						rs0[k].shadowCastingMode = originalShadowCastingMode[k];
					}
				}
			}
			else if (!rs0[0].enabled)
			{
				for (int l = 0; l < rs0.Length; l++)
				{
					rs0[l].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
					rs0[l].enabled = true;
				}
			}
		}
	}

	public void UpdateValues()
	{
		if (Lod1 != 0f)
		{
			lod_1 = Lod1;
		}
		else
		{
			lod_1 = iocCam.lod1Distance;
		}
		if (Lod2 != 0f)
		{
			lod_2 = Lod2;
		}
		else
		{
			lod_2 = iocCam.lod2Distance;
		}
		realtimeShadows = iocCam.realtimeShadows;
	}

	public override void UnHide(RaycastHit h)
	{
		counter = Time.frameCount;
		hitPoint = base.transform.worldToLocalMatrix.MultiplyPoint(h.point);
		if (hidden)
		{
			hidden = false;
			ShowLod(h.distance);
			if (Occludee)
			{
				base.gameObject.layer = iocCam.occludeeLayer;
			}
		}
		else
		{
			if (lods <= 0)
			{
				return;
			}
			distOffset = prevDist - h.distance;
			hitTimeOffset = Time.time - prevHitTime;
			prevHitTime = Time.time;
			if ((Mathf.Abs(distOffset) > lodMargin) | (hitTimeOffset > 1f))
			{
				ShowLod(h.distance);
				prevDist = h.distance;
				sleeping = false;
				if (Occludee)
				{
					base.gameObject.layer = iocCam.occludeeLayer;
				}
			}
		}
	}

	public void ShowLod(float d)
	{
		int num = 0;
		switch (lods)
		{
		case 0:
			currentLod = -1;
			break;
		case 2:
			if (d < lod_1)
			{
				currentLod = 0;
			}
			else
			{
				currentLod = 1;
			}
			break;
		case 3:
			if (d < lod_1)
			{
				currentLod = 0;
			}
			else if ((d > lod_1) & (d < lod_2))
			{
				currentLod = 1;
			}
			else
			{
				currentLod = 2;
			}
			break;
		}
		switch (currentLod)
		{
		case 0:
			if (!LodOnly && rs0[0].enabled)
			{
				for (num = 0; num < rs0.Length; num++)
				{
					rs0[num].shadowCastingMode = originalShadowCastingMode[num];
				}
			}
			else
			{
				for (num = 0; num < rs0.Length; num++)
				{
					rs0[num].enabled = true;
				}
			}
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			if (lods == 3)
			{
				for (num = 0; num < rs2.Length; num++)
				{
					rs2[num].enabled = false;
				}
			}
			return;
		case 1:
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = true;
			}
			for (num = 0; num < rs0.Length; num++)
			{
				rs0[num].enabled = false;
				if (!LodOnly && realtimeShadows)
				{
					rs0[num].shadowCastingMode = originalShadowCastingMode[num];
				}
			}
			if (lods == 3)
			{
				for (num = 0; num < rs2.Length; num++)
				{
					rs2[num].enabled = false;
				}
			}
			return;
		case 2:
			for (num = 0; num < rs2.Length; num++)
			{
				rs2[num].enabled = true;
			}
			for (num = 0; num < rs0.Length; num++)
			{
				rs0[num].enabled = false;
				if (!LodOnly && realtimeShadows)
				{
					rs0[num].shadowCastingMode = originalShadowCastingMode[num];
				}
			}
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			return;
		}
		if (!LodOnly && rs[0].enabled)
		{
			for (num = 0; num < rs.Length; num++)
			{
				rs[num].shadowCastingMode = originalShadowCastingMode[num];
			}
		}
		else
		{
			for (num = 0; num < rs.Length; num++)
			{
				rs[num].enabled = true;
			}
		}
	}

	public void Hide()
	{
		int num = 0;
		hidden = true;
		switch (currentLod)
		{
		case 0:
			if (realtimeShadows && distanceFromCam <= shadowDistance)
			{
				for (num = 0; num < rs0.Length; num++)
				{
					rs0[num].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
			}
			else
			{
				for (num = 0; num < rs0.Length; num++)
				{
					rs0[num].enabled = false;
				}
			}
			break;
		case 1:
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			break;
		case 2:
			for (num = 0; num < rs2.Length; num++)
			{
				rs2[num].enabled = false;
			}
			break;
		default:
			if (realtimeShadows && distanceFromCam <= shadowDistance)
			{
				for (num = 0; num < rs.Length; num++)
				{
					rs[num].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
			}
			else
			{
				for (num = 0; num < rs.Length; num++)
				{
					rs[num].enabled = false;
				}
			}
			break;
		}
		if (Occludee)
		{
			base.gameObject.layer = currentLayer;
		}
	}

	public void HideAll()
	{
		int num = 0;
		switch (LodOnly)
		{
		case false:
			hidden = true;
			if (lods == 0 && rs != null)
			{
				for (num = 0; num < rs.Length; num++)
				{
					rs[num].enabled = false;
					if (realtimeShadows)
					{
						rs[num].shadowCastingMode = originalShadowCastingMode[num];
					}
				}
				break;
			}
			for (num = 0; num < rs0.Length; num++)
			{
				rs0[num].enabled = false;
				if (realtimeShadows)
				{
					rs0[num].shadowCastingMode = originalShadowCastingMode[num];
				}
			}
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			if (rs2 != null)
			{
				for (num = 0; num < rs2.Length; num++)
				{
					rs2[num].enabled = false;
				}
			}
			break;
		case true:
			prevHitTime -= 3f;
			ShowLod(3000f);
			break;
		}
	}

	private BoundingSphere GetBoundingSphere(Renderer[] renderers)
	{
		BoundingSphere result = default(BoundingSphere);
		Bounds bounds = default(Bounds);
		bounds.center = renderers[0].transform.position;
		foreach (Renderer renderer in renderers)
		{
			bounds.Encapsulate(renderer.bounds);
		}
		result.position = bounds.center;
		result.radius = bounds.extents.magnitude;
		return result;
	}

	private float GetLODmargin(Renderer[] renderers)
	{
		Bounds bounds = default(Bounds);
		bounds.center = renderers[0].transform.position;
		foreach (Renderer renderer in renderers)
		{
			bounds.Encapsulate(renderer.bounds);
		}
		return bounds.extents.magnitude;
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__0(Renderer x)
	{
		return x.gameObject.GetComponent<Light>() == null;
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__1(Renderer x)
	{
		return x.gameObject.GetComponent<Light>() == null;
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__2(Renderer x)
	{
		return x.gameObject.GetComponent<Light>() == null;
	}

	[CompilerGenerated]
	private static bool _003CStart_003Em__3(Renderer x)
	{
		return x.gameObject.GetComponent<Light>() == null;
	}
}
