using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CloudParticle
{
	private GameObject CloudObject = new GameObject();

	private Vector3 Ellipsoid;

	private CloudMovement MovScript;

	private CloudAlphaAnimator alphaScript;

	private Transform MyTransform;

	private ParticleSystem PartEmitter;

	private ParticleSystemRenderer PartRenderer;

	private Projector Shadow;

	private GameObject ShadowObject;

	private float minSize;

	private float maxSize;

	private float minEmission;

	private float maxEmission;

	public CloudParticle(Vector3 Pos, Quaternion Rot)
	{
		MyTransform = CloudObject.transform;
		CloudObject.transform.position = Pos;
		CloudObject.transform.rotation = Rot;
		bool flag = true;
		if (LayerMask.NameToLayer("CloudsToy") < 0)
		{
			flag = false;
			Debug.LogWarning("You SHOULD create a Unity layer named 'CloudsToy' or this asset will use the 30th layer by default.");
		}
		CloudObject.layer = (flag ? LayerMask.NameToLayer("CloudsToy") : 30);
		PartEmitter = CloudObject.AddComponent<ParticleSystem>();
		PartEmitter.enableEmission = false;
		PartEmitter.loop = false;
		PartEmitter.playOnAwake = false;
		PartEmitter.startLifetime = float.PositiveInfinity;
		PartRenderer = CloudObject.GetComponent<ParticleSystemRenderer>();
		PartRenderer.maxParticleSize = 100f;
		PartRenderer.shadowCastingMode = ShadowCastingMode.Off;
		PartRenderer.receiveShadows = false;
		PartRenderer.useLightProbes = false;
		PartRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		PartRenderer.sortMode = ParticleSystemSortMode.Distance;
		MovScript = CloudObject.AddComponent<CloudMovement>();
		alphaScript = CloudObject.AddComponent<CloudAlphaAnimator>();
		ShadowObject = new GameObject();
		ShadowObject.name = "CloudProjector";
		Shadow = ShadowObject.AddComponent<Projector>();
		Shadow.ignoreLayers = (flag ? (1 << LayerMask.NameToLayer("CloudsToy")) : 1073741824);
		ShadowObject.transform.position = Vector3.zero;
		ShadowObject.transform.rotation = Quaternion.identity;
		ShadowObject.transform.Rotate(new Vector3(90f, 0f, 0f));
		ShadowObject.transform.parent = CloudObject.transform;
	}

	public void AnimateCloud(bool isCloudAnim, float AnimationVelocity)
	{
		ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = PartEmitter.rotationOverLifetime;
		rotationOverLifetime.enabled = isCloudAnim;
		if (AnimationVelocity == 0f)
		{
			rotationOverLifetime.enabled = false;
		}
		rotationOverLifetime.separateAxes = false;
		rotationOverLifetime.zMultiplier = 100f;
		float min = UnityEngine.Random.Range(AnimationVelocity * 0.75f, AnimationVelocity);
		float max = UnityEngine.Random.Range(AnimationVelocity, AnimationVelocity * 1.25f);
		rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(min, max);
	}

	public void DefineCloudProperties(int Num, int MaxWithCloud, int MaxTallCloud, int MaxDepthCloud, int TypeofCloud, bool FixedSize, bool FirstTime, bool isEllipsoidChanged)
	{
		PartEmitter.enableEmission = false;
		if (FirstTime)
		{
			isEllipsoidChanged = true;
		}
		if (isEllipsoidChanged)
		{
			Ellipsoid.x = UnityEngine.Random.Range(MaxWithCloud / 2, MaxWithCloud);
			if (FixedSize)
			{
				if (Ellipsoid.x <= 10f)
				{
					Ellipsoid.x = 10f;
				}
				else if (Ellipsoid.x > 10f && Ellipsoid.x <= 20f)
				{
					Ellipsoid.x = 20f;
				}
				else if (Ellipsoid.x > 20f && Ellipsoid.x <= 100f)
				{
					Ellipsoid.x = 100f;
				}
				else if (Ellipsoid.x > 100f && Ellipsoid.x <= 200f)
				{
					Ellipsoid.x = 200f;
				}
			}
			Ellipsoid.z = UnityEngine.Random.Range(MaxDepthCloud / 2, MaxDepthCloud);
			if (FixedSize)
			{
				if (Ellipsoid.z <= 10f)
				{
					Ellipsoid.z = 10f;
				}
				else if (Ellipsoid.z > 10f && Ellipsoid.z <= 20f)
				{
					Ellipsoid.z = 20f;
				}
				else if (Ellipsoid.z > 20f && Ellipsoid.z <= 100f)
				{
					Ellipsoid.z = 100f;
				}
				else if (Ellipsoid.z > 100f && Ellipsoid.z <= 200f)
				{
					Ellipsoid.z = 200f;
				}
			}
			switch (TypeofCloud)
			{
			case 0:
				Ellipsoid.y = UnityEngine.Random.Range(MaxTallCloud / 2, MaxTallCloud);
				break;
			case 1:
				Ellipsoid.y = UnityEngine.Random.Range(1, MaxTallCloud);
				break;
			}
			Ellipsoid *= 0.5f;
		}
		SetCloudName(Num);
		if (FirstTime || isEllipsoidChanged)
		{
			Shadow.transform.localPosition = new Vector3(0f, Ellipsoid.x * 2f, 0f);
			Shadow.nearClipPlane = Ellipsoid.x * 4f;
			Shadow.farClipPlane = Shadow.transform.position.y + Ellipsoid.x * 4f;
		}
	}

	public void SetActive(bool _active)
	{
		CloudObject.SetActive(_active);
		if (_active)
		{
			PartEmitter.Clear();
			int count = (int)UnityEngine.Random.Range(minEmission, maxEmission);
			ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
			emitParams.ResetPosition();
			emitParams.ResetRotation();
			emitParams.ResetAngularVelocity();
			emitParams.ResetAxisOfRotation();
			emitParams.ResetStartColor();
			emitParams.ResetStartLifetime();
			emitParams.ResetStartSize();
			emitParams.ResetVelocity();
			PartEmitter.Emit(emitParams, count);
		}
	}

	public void DesactivateRecursively()
	{
		CloudObject.SetActive(false);
	}

	public Vector3 GetCloudPosition()
	{
		return MyTransform.position;
	}

	public bool IsActive()
	{
		return CloudObject.activeSelf;
	}

	public bool IsShadowActive()
	{
		return ShadowObject.activeSelf;
	}

	public void PaintParticlesBelow(Color MainColor, Color TintColor, int TintStrength, float offset, int PaintType)
	{
		int num = 0;
		float t = 0f;
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[PartEmitter.particleCount];
		PartEmitter.GetParticles(array);
		if (array.Length <= 0)
		{
			return;
		}
		for (num = 0; num < array.Length; num++)
		{
			if (TintStrength > 1)
			{
				switch (PaintType)
				{
				case 1:
					t = ((!(array[num].position.y > 0f)) ? ((Ellipsoid.y + array[num].position.y) / Ellipsoid.y - offset) : ((Ellipsoid.y * 2f - array[num].position.y) / Ellipsoid.y - offset));
					break;
				case 0:
					t = (array[num].position.y - Ellipsoid.y) / Ellipsoid.x;
					break;
				}
				if ((float)num < (float)(array.Length * TintStrength) * 0.01f)
				{
					array[num].color = Color.Lerp(TintColor, MainColor, t);
				}
				else
				{
					array[num].color = MainColor;
				}
			}
			else
			{
				array[num].color = MainColor;
			}
		}
		PartEmitter.SetParticles(array, array.Length);
	}

	public void SetCloudEmitter(int Num, Vector3 SpreadDir, bool SoftClouds, float SizeFactorPart, float EmissionMult, Vector3 MaximunVelocity, float VelocityMultipier)
	{
		SetWorldVelocity(SpreadDir);
		PartEmitter.startLifetime = float.PositiveInfinity;
		PartEmitter.simulationSpace = ParticleSystemSimulationSpace.Local;
		ParticleSystem.MainModule main = PartEmitter.main;
		main.startRotation = new ParticleSystem.MinMaxCurve(0f, 6.265732f);
		main.startSpeed = 0f;
		main.startSpeedMultiplier = 0f;
		if (SoftClouds)
		{
			maxSize = Ellipsoid.x * SizeFactorPart;
			minSize = Ellipsoid.y * SizeFactorPart;
			PartEmitter.startSize = UnityEngine.Random.Range(minSize, maxSize);
			minEmission = 1f * EmissionMult;
			maxEmission = UnityEngine.Random.Range(minEmission, minEmission + 10f * EmissionMult);
		}
		else
		{
			maxSize = (Ellipsoid.x + Mathf.Cos(Ellipsoid.x * 0.1f)) * SizeFactorPart;
			minSize = maxSize * 0.75f;
			PartEmitter.startSize = UnityEngine.Random.Range(minSize, maxSize);
			maxEmission = Ellipsoid.x * EmissionMult;
			minEmission = maxEmission * 0.75f;
		}
		SetCloudName(Num);
		SetCloudVelocity(MaximunVelocity, VelocityMultipier);
	}

	public void SetCloudName(int Num)
	{
		if (Ellipsoid.x <= 10f)
		{
			CloudObject.name = "Cloud" + Num + " Little";
		}
		else if (Ellipsoid.x > 10f && Ellipsoid.x <= 20f)
		{
			CloudObject.name = "Cloud" + Num + " Medium";
		}
		else if (Ellipsoid.x > 20f && Ellipsoid.x <= 100f)
		{
			CloudObject.name = "Cloud" + Num + " Big";
		}
		else if (Ellipsoid.x > 100f && Ellipsoid.x <= 200f)
		{
			CloudObject.name = "Cloud" + Num + " Gigant";
		}
		else
		{
			CloudObject.name = "Cloud" + Num + " Massive";
		}
	}

	public void SetCloudParent(Transform ParentTransform)
	{
		MyTransform.parent = ParentTransform;
	}

	public void SetCloudPosition(Vector3 MyPos)
	{
		MyTransform.position = MyPos;
	}

	public void SetCloudShadowMaterial(Material shadowMat)
	{
		Shadow.material = shadowMat;
	}

	public void SetCloudVelocity(Vector3 MaximunVelocity, float VelocityMultipier)
	{
		SetWorldVelocity(MaximunVelocity);
		if (!(Ellipsoid.x <= 10f))
		{
			if (Ellipsoid.x > 10f && Ellipsoid.x <= 20f)
			{
				MovScript.Velocity = MaximunVelocity * 0.5f;
			}
			else if (Ellipsoid.x > 20f && Ellipsoid.x <= 100f)
			{
				MovScript.Velocity = MaximunVelocity * 0.33f;
			}
			else if (Ellipsoid.x > 100f && Ellipsoid.x <= 200f)
			{
				MovScript.Velocity = MaximunVelocity * 0.25f;
			}
			else
			{
				MovScript.Velocity = MaximunVelocity * 0.2f;
			}
			if (VelocityMultipier != 1f)
			{
				MovScript.Velocity *= VelocityMultipier;
			}
		}
	}

	public void SetCloudAlphaAnimation(float _ratio, float _range, bool _isAnimate)
	{
		alphaScript.SetCloudAlphaAnimator(_ratio, _range, _isAnimate);
	}

	public void SetNewCloudColor(Color newColor)
	{
		alphaScript.SetNewCloudColor(newColor);
	}

	public void SetLengthScale(float fLengthSpread)
	{
		PartRenderer.lengthScale = fLengthSpread;
	}

	public void SetMainColor(Color CloudColor)
	{
		CloudObject.GetComponent<Renderer>().material.SetColor("_TintColor", CloudColor);
	}

	public void SetMaterial(Material MyMat)
	{
		CloudObject.GetComponent<Renderer>().material = MyMat;
	}

	public void SetShadowActive(bool _active)
	{
		ShadowObject.SetActive(_active);
	}

	public void SetWorldVelocity(Vector3 vSpreadDir)
	{
		bool flag = PartRenderer.renderMode == ParticleSystemRenderMode.Stretch;
		if (PartEmitter.particleCount > 0)
		{
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[PartEmitter.particleCount];
			PartEmitter.GetParticles(array);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].velocity = (flag ? vSpreadDir : Vector3.zero);
			}
			PartEmitter.SetParticles(array, array.Length);
		}
	}

	public void SoftCloud(bool isSoftCloud)
	{
		PartRenderer.renderMode = (isSoftCloud ? ParticleSystemRenderMode.Stretch : ParticleSystemRenderMode.Billboard);
	}

	public bool UpdateCloudsPosition()
	{
		int num = 0;
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[PartEmitter.particleCount];
		PartEmitter.GetParticles(array);
		if (array.Length > 0)
		{
			Vector3 position = default(Vector3);
			for (num = 0; num < array.Length; num++)
			{
				float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
				float f2 = UnityEngine.Random.Range(0f, (float)Math.PI);
				float num2 = UnityEngine.Random.Range(0f, 1f);
				float num3 = UnityEngine.Random.Range(0f, 1f);
				float num4 = UnityEngine.Random.Range(0f, 1f);
				position.x = num2 * Mathf.Cos(f) * Mathf.Sin(f2) * Ellipsoid.x;
				position.y = num3 * Mathf.Sin(f) * Mathf.Sin(f2) * Ellipsoid.y;
				position.z = num4 * Mathf.Cos(f2) * Ellipsoid.z;
				if (UnityEngine.Random.value > 0.5f)
				{
					position.x = 0f - position.x;
				}
				if (UnityEngine.Random.value > 0.5f)
				{
					position.y = 0f - position.y;
				}
				if (UnityEngine.Random.value > 0.5f)
				{
					position.z = 0f - position.z;
				}
				array[num].position = position;
			}
			PartEmitter.SetParticles(array, array.Length);
			return true;
		}
		return false;
	}
}
