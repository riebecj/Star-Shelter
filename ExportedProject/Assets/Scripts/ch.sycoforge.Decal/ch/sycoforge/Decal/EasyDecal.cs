using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using ch.sycoforge.Decal.Projectors;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal
{
	[Serializable]
	[ExecuteInEditMode]
	public class EasyDecal : DecalBase
	{
		public const int MaxQuality = 6;

		internal const string CHILD_NAME = "PROC_PLANE";

		internal const string SKINNED_CHILD_NAME = "SKINNED_PROC_PLANE";

		public LayerMask Mask = -1;

		private int id;

		internal bool evenID;

		private Action<EasyDecal> OnFadedOut_;

		private Action<EasyDecal> OnFadedIn_;

		private Action<EasyDecal> OnFadeOutStarted_;

		private Action<EasyDecal, Mesh> OnProjectionFinished_;

		private Action<EasyDecal> OnAtlasIndexChanged_;

		private Action<EasyDecal> OnSourceChanged_;

		[SerializeField]
		[HideInInspector]
		private ProjectionMode mode = ProjectionMode.SurfaceNormal;

		[HideInInspector]
		[SerializeField]
		private SourceMode source = SourceMode.Material;

		[SerializeField]
		[HideInInspector]
		private AspectMode aspectCorrectionMode = AspectMode.None;

		[SerializeField]
		[HideInInspector]
		private LookupMode recursiveMode = LookupMode.Up;

		private GameObject receiver;

		private GameObject meshContainer;

		private GameObject skinnedMeshContainer;

		[SerializeField]
		[HideInInspector]
		private SkinQuality skinningQuality = SkinQuality.Auto;

		[HideInInspector]
		[SerializeField]
		internal Material material;

		[SerializeField]
		[HideInInspector]
		private DecalTextureAtlas atlas;

		[HideInInspector]
		[SerializeField]
		private int atlasRegionIndex;

		[HideInInspector]
		[SerializeField]
		private float maxDistance = 5f;

		[SerializeField]
		[HideInInspector]
		private float projectionDistance = 1f;

		[SerializeField]
		[HideInInspector]
		private bool smoothNormals;

		[HideInInspector]
		[SerializeField]
		private float normalSmoothFactor;

		[HideInInspector]
		[SerializeField]
		private float normalSmoothThreshold;

		[SerializeField]
		[HideInInspector]
		internal int resolution = 4;

		[HideInInspector]
		[SerializeField]
		private bool multiMeshEnabled;

		[HideInInspector]
		[SerializeField]
		private bool cullInvisible;

		[SerializeField]
		[HideInInspector]
		private bool cullUnreachable;

		[HideInInspector]
		[SerializeField]
		private bool showVertices;

		[SerializeField]
		[HideInInspector]
		private bool showDir;

		[HideInInspector]
		[SerializeField]
		private bool showNormals;

		[SerializeField]
		[HideInInspector]
		private bool enableVertexColorFade;

		[HideInInspector]
		[SerializeField]
		private bool onlyColliders = true;

		[SerializeField]
		[HideInInspector]
		private bool tangents;

		[SerializeField]
		[HideInInspector]
		private bool calculateNormals;

		[SerializeField]
		[HideInInspector]
		private MeshChannelMode normalChannelMode;

		[SerializeField]
		[HideInInspector]
		private MeshChannelMode tangentChannelMode;

		[HideInInspector]
		[SerializeField]
		private bool recursiceLookup;

		[HideInInspector]
		[SerializeField]
		private bool randomIndexEnabled;

		[SerializeField]
		[HideInInspector]
		private int recursiceLookupSteps;

		private List<IMeshProcessor> meshProcessors = new List<IMeshProcessor>();

		[HideInInspector]
		[SerializeField]
		private bool autoDestroy;

		[HideInInspector]
		[SerializeField]
		private OpacityMode opacityMode = OpacityMode.Geometry;

		[SerializeField]
		[HideInInspector]
		private bool fadeOnly;

		[HideInInspector]
		[SerializeField]
		private bool useLightProbes;

		[HideInInspector]
		[SerializeField]
		private float lifetime = 5f;

		[HideInInspector]
		[SerializeField]
		private float fadeoutTime = 1f;

		private float creationTime;

		[SerializeField]
		[HideInInspector]
		private AnimationCurve fadeoutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		[HideInInspector]
		[SerializeField]
		private float surfaceRotation;

		[HideInInspector]
		[SerializeField]
		private int quality = 1;

		private float alpha = 1f;

		private int creationFrameNumber;

		private int bakeInFrameNumber = -1;

		private bool needUVRecreation = true;

		private bool needGeometryRecreation = true;

		[HideInInspector]
		[SerializeField]
		private Mesh sharedMesh;

		[HideInInspector]
		[SerializeField]
		private int instanceID;

		internal bool projectBake;

		public static StaticProxyCollection ProxyCollection;

		public static bool HideMesh = true;

		public static InstantiationDelegate Instantiation;

		public int ID
		{
			get
			{
				return id;
			}
			internal set
			{
				id = value;
				evenID = id % 2 == 0;
			}
		}

		public SkinQuality SkinningQuality
		{
			get
			{
				return skinningQuality;
			}
			set
			{
				skinningQuality = value;
			}
		}

		public Mesh SharedMesh
		{
			get
			{
				return sharedMesh;
			}
		}

		public bool IsVisible
		{
			get
			{
				return projector != null && projector.IsVisible;
			}
		}

		public int Quality
		{
			get
			{
				return quality;
			}
			set
			{
				if (quality != value)
				{
					int num = Mathf.Clamp(value, 0, 6);
					resolution = (int)Mathf.Pow(2f, num);
				}
				CallOnPropertyChanged(quality, value);
				quality = value;
			}
		}

		public bool MultiMeshEnabled
		{
			get
			{
				return multiMeshEnabled;
			}
			set
			{
				multiMeshEnabled = value;
			}
		}

		public bool CullInvisibles
		{
			get
			{
				return cullInvisible;
			}
			set
			{
				cullInvisible = value;
			}
		}

		public bool CullUnreachable
		{
			get
			{
				return cullUnreachable;
			}
			set
			{
				cullUnreachable = value;
			}
		}

		public GameObject Receiver
		{
			get
			{
				return receiver;
			}
			protected set
			{
				receiver = value;
			}
		}

		public bool CalculateTangents
		{
			get
			{
				return tangents;
			}
			set
			{
				tangents = value;
			}
		}

		public MeshChannelMode TangentChannelMode
		{
			get
			{
				return tangentChannelMode;
			}
			set
			{
				tangentChannelMode = value;
			}
		}

		public bool CalculateNormals
		{
			get
			{
				return calculateNormals;
			}
			set
			{
				calculateNormals = value;
			}
		}

		public MeshChannelMode NormalChannelMode
		{
			get
			{
				return normalChannelMode;
			}
			set
			{
				normalChannelMode = value;
			}
		}

		public bool ShowDir
		{
			get
			{
				return showDir;
			}
			set
			{
				showDir = value;
			}
		}

		public bool ShowVertices
		{
			get
			{
				return showVertices;
			}
			set
			{
				showVertices = value;
			}
		}

		public bool ShowNormals
		{
			get
			{
				return showNormals;
			}
			set
			{
				showNormals = value;
			}
		}

		public float MaxDistance
		{
			get
			{
				return maxDistance;
			}
			set
			{
				maxDistance = value;
			}
		}

		public float ProjectionDistance
		{
			get
			{
				return projectionDistance;
			}
			set
			{
				projectionDistance = value;
			}
		}

		public bool SmoothNormals
		{
			get
			{
				return smoothNormals;
			}
			set
			{
				smoothNormals = value;
			}
		}

		public float NormalSmoothFactor
		{
			get
			{
				return normalSmoothFactor;
			}
			set
			{
				normalSmoothFactor = value;
			}
		}

		public float NormalSmoothThreshold
		{
			get
			{
				return normalSmoothThreshold;
			}
			set
			{
				normalSmoothThreshold = value;
			}
		}

		public bool DontDestroy
		{
			get
			{
				return fadeOnly;
			}
			set
			{
				fadeOnly = value;
			}
		}

		public bool FadeOut
		{
			get
			{
				return autoDestroy;
			}
			set
			{
				autoDestroy = value;
			}
		}

		public OpacityMode OpacityMode
		{
			get
			{
				return opacityMode;
			}
			set
			{
				opacityMode = value;
			}
		}

		public float Lifetime
		{
			get
			{
				return lifetime;
			}
			set
			{
				lifetime = value;
			}
		}

		public float RemainingLifetime
		{
			get
			{
				return Mathf.Max(lifetime + fadeoutTime - (Time.timeSinceLevelLoad - creationTime), 0f);
			}
		}

		public float CurrentFadeoutTime { get; private set; }

		public float FadeoutTime
		{
			get
			{
				return fadeoutTime;
			}
			set
			{
				fadeoutTime = value;
			}
		}

		public AnimationCurve FadeoutCurve
		{
			get
			{
				return fadeoutCurve;
			}
			set
			{
				fadeoutCurve = value;
			}
		}

		public Vector3 InverseProjectionDirection { get; private set; }

		public float SurfaceRotation
		{
			get
			{
				return surfaceRotation;
			}
			set
			{
				surfaceRotation = value;
			}
		}

		public Material DecalMaterial
		{
			get
			{
				return material;
			}
			set
			{
				material = value;
				if (MeshContainer != null)
				{
					base.DecalRenderer.material = value;
				}
			}
		}

		public DecalTextureAtlas Atlas
		{
			get
			{
				return atlas;
			}
			set
			{
				if (MeshContainer != null && atlas != null && base.DecalRenderer != null)
				{
					DecalMaterial = atlas.Material;
				}
				atlas = value;
			}
		}

		public int AtlasRegionIndex
		{
			get
			{
				return atlasRegionIndex;
			}
			set
			{
				if (value == atlasRegionIndex && value >= 0)
				{
					return;
				}
				atlasRegionIndex = value;
				if (atlas != null && atlas.Regions.Count > 0)
				{
					int count = atlas.Regions.Count;
					if (atlasRegionIndex < 0)
					{
						atlasRegionIndex = count - Math.Abs(atlasRegionIndex) % count;
					}
					else
					{
						atlasRegionIndex %= count;
					}
					CallOnAtlasIndexChanged();
				}
				CorrectAspect();
			}
		}

		public GameObject MeshContainer
		{
			get
			{
				return (base.Technique == ProjectionTechnique.SkinnedBox) ? skinnedMeshContainer : meshContainer;
			}
		}

		public ProjectionMode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
			}
		}

		public SourceMode Source
		{
			get
			{
				return source;
			}
			set
			{
				if (source != value)
				{
					CallOnSourceChanged();
				}
				source = value;
			}
		}

		public AspectMode AspectCorrectionMode
		{
			get
			{
				return aspectCorrectionMode;
			}
			set
			{
				aspectCorrectionMode = value;
			}
		}

		public LookupMode RecursiveMode
		{
			get
			{
				return recursiveMode;
			}
			set
			{
				recursiveMode = value;
			}
		}

		public float Alpha
		{
			get
			{
				return alpha;
			}
			private set
			{
				bool flag = !MathPlane.FloatEquals(value, alpha);
				alpha = value;
				if (flag)
				{
					CallOnAlphaChanged();
				}
			}
		}

		[Obsolete("Use EasyDecal.IsFading instead.")]
		public bool IsFadingOut
		{
			get
			{
				return alpha < 1f;
			}
		}

		public bool IsFading
		{
			get
			{
				return alpha < 1f;
			}
		}

		public bool OnlyColliders
		{
			get
			{
				return onlyColliders;
			}
			set
			{
				onlyColliders = value;
			}
		}

		public bool EnableVertexColorBleed
		{
			get
			{
				return enableVertexColorFade;
			}
			set
			{
				enableVertexColorFade = value;
			}
		}

		public bool RecursiveLookup
		{
			get
			{
				return recursiceLookup;
			}
			set
			{
				recursiceLookup = value;
			}
		}

		public bool RandomIndexEnabled
		{
			get
			{
				return randomIndexEnabled;
			}
			set
			{
				randomIndexEnabled = value;
			}
		}

		public int RecursiveLookupSteps
		{
			get
			{
				return recursiceLookupSteps;
			}
			set
			{
				recursiceLookupSteps = value;
			}
		}

		public List<IMeshProcessor> MeshProcessors
		{
			get
			{
				return meshProcessors;
			}
			set
			{
				meshProcessors = value;
			}
		}

		public bool UseLightProbes
		{
			get
			{
				return useLightProbes;
			}
			set
			{
				useLightProbes = value;
			}
		}

		public event Action<EasyDecal> OnFadedOut
		{
			add
			{
				OnFadedOut_ = (Action<EasyDecal>)Delegate.Combine(OnFadedOut_, value);
			}
			remove
			{
				OnFadedOut_ = (Action<EasyDecal>)Delegate.Remove(OnFadedOut_, value);
			}
		}

		public event Action<EasyDecal> OnFadedIn
		{
			add
			{
				OnFadedIn_ = (Action<EasyDecal>)Delegate.Combine(OnFadedIn_, value);
			}
			remove
			{
				OnFadedIn_ = (Action<EasyDecal>)Delegate.Remove(OnFadedIn_, value);
			}
		}

		public event Action<EasyDecal> OnFadeOutStarted
		{
			add
			{
				OnFadeOutStarted_ = (Action<EasyDecal>)Delegate.Combine(OnFadeOutStarted_, value);
			}
			remove
			{
				OnFadeOutStarted_ = (Action<EasyDecal>)Delegate.Remove(OnFadeOutStarted_, value);
			}
		}

		public event Action<EasyDecal, Mesh> OnProjectionFinished
		{
			add
			{
				OnProjectionFinished_ = (Action<EasyDecal, Mesh>)Delegate.Combine(OnProjectionFinished_, value);
			}
			remove
			{
				OnProjectionFinished_ = (Action<EasyDecal, Mesh>)Delegate.Remove(OnProjectionFinished_, value);
			}
		}

		public event Action<EasyDecal> OnAtlasIndexChanged
		{
			add
			{
				OnAtlasIndexChanged_ = (Action<EasyDecal>)Delegate.Combine(OnAtlasIndexChanged_, value);
			}
			remove
			{
				OnAtlasIndexChanged_ = (Action<EasyDecal>)Delegate.Remove(OnAtlasIndexChanged_, value);
			}
		}

		public event Action<EasyDecal> OnSourceChanged
		{
			add
			{
				OnSourceChanged_ = (Action<EasyDecal>)Delegate.Combine(OnSourceChanged_, value);
			}
			remove
			{
				OnSourceChanged_ = (Action<EasyDecal>)Delegate.Remove(OnSourceChanged_, value);
			}
		}

		public void Reset()
		{
			Reset(false);
		}

		public void Reset(bool unbake)
		{
			Reset(unbake, false);
		}

		public void Reset(bool unbake, bool recreate)
		{
			creationFrameNumber = Time.frameCount;
			CancelFade();
			if (recreate)
			{
				Initialize();
				ResetMesh();
				InitalizeProjector();
				if (projector != null)
				{
					projector.Reset();
					ConvertAndAssign(projector.Mesh, true);
				}
			}
			if (unbake)
			{
				base.Baked = false;
			}
			if (autoDestroy && Application.isPlaying)
			{
				Alpha = 1f;
				creationTime = Time.timeSinceLevelLoad;
				StartFade();
			}
			AskForUVRecreation();
			AskForGeometryRecreation();
		}

		protected override void Awake()
		{
			base.Awake();
			int num = GetInstanceID();
			bool recreate = num != instanceID && num != 0;
			instanceID = num;
			Reset(false, recreate);
			if (bakeOnAwake && Application.isPlaying)
			{
				LateBake();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			EasyDecalManager.Add(this);
			Initialize();
			InitalizeProjector();
			if (Source == SourceMode.Atlas && Atlas != null)
			{
				DecalMaterial = Atlas.Material;
			}
			else
			{
				DecalMaterial = material;
			}
			if (!baked)
			{
				AskForUVRecreation();
			}
		}

		protected override void Start()
		{
			base.Start();
			EasyDecalManager.Add(this);
			if (!base.Baked)
			{
				if (randomIndexEnabled)
				{
					SelectRandom();
				}
				CorrectAspect();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			EasyDecalManager.Remove(this);
			if (MeshContainer != null)
			{
				UnityEngine.Object.DestroyImmediate(MeshContainer);
			}
		}

		public void StartFade()
		{
			if (IsInvoking("StartFadeOut"))
			{
				CancelInvoke("StartFadeOut");
			}
			Invoke("StartFadeOut", lifetime);
		}

		public void CancelFade()
		{
			CancelInvoke("StartFadeOut");
		}

		public void SelectRandom()
		{
			if (Source == SourceMode.Atlas && Atlas != null && Atlas.Regions.Count > 0)
			{
				int value = UnityEngine.Random.Range(0, Atlas.Regions.Count);
				AtlasRegionIndex = Mathf.Clamp(value, 0, Atlas.Regions.Count - 1);
			}
		}

		public void CorrectAspect()
		{
			if (Source == SourceMode.Atlas && AspectCorrectionMode != 0 && Atlas != null && atlasRegionIndex < atlas.Regions.Count)
			{
				AtlasRegion atlasRegion = atlas.Regions[AtlasRegionIndex];
				Vector3 localScale = base.transform.localScale;
				if (AspectCorrectionMode == AspectMode.Height)
				{
					base.transform.localScale = new Vector3(localScale.z * atlasRegion.AspectRatio, localScale.y, localScale.z);
				}
				else if (AspectCorrectionMode == AspectMode.Width)
				{
					base.transform.localScale = new Vector3(localScale.x, localScale.y, localScale.x / atlasRegion.AspectRatio);
				}
			}
		}

		private void StartFadeOut()
		{
			StartFade(FadingMode.Out);
		}

		public void StartFade(FadingMode mode = FadingMode.Out)
		{
			if (base.gameObject.activeInHierarchy)
			{
				StopAllCoroutines();
				StopCoroutine("ProcessFadeOut");
				StopCoroutine("ProcessFadeIn");
				switch (mode)
				{
				case FadingMode.Out:
					CurrentFadeoutTime = fadeoutTime * alpha;
					CallOnFadeOutStarted();
					StartCoroutine("ProcessFadeOut");
					break;
				case FadingMode.In:
					CurrentFadeoutTime = fadeoutTime * (1f - alpha);
					StartCoroutine("ProcessFadeIn");
					break;
				case FadingMode.ImmediateHide:
					Alpha = 0f;
					break;
				case FadingMode.ImmediateShow:
					Alpha = 1f;
					break;
				}
			}
		}

		private IEnumerator ProcessFadeOut()
		{
			while (CurrentFadeoutTime > 0f)
			{
				float t = 1f - CurrentFadeoutTime / fadeoutTime;
				Alpha = fadeoutCurve.Evaluate(t);
				CurrentFadeoutTime -= Time.deltaTime;
				if (CurrentFadeoutTime <= 0f)
				{
					if (!fadeOnly)
					{
						UnityEngine.Object.Destroy(base.gameObject, 0.1f);
					}
					CallOnFadedOut();
				}
				yield return null;
			}
		}

		private IEnumerator ProcessFadeIn()
		{
			while (CurrentFadeoutTime > 0f)
			{
				float t = CurrentFadeoutTime / fadeoutTime;
				Alpha = fadeoutCurve.Evaluate(t);
				CurrentFadeoutTime -= Time.deltaTime;
				if (CurrentFadeoutTime <= 0f)
				{
					CallOnFadedIn();
				}
				yield return null;
			}
		}

		public Vector3[] GetBoundingVertices()
		{
			Vector3[] array = new Vector3[8];
			Vector3 vector = Vector3.right * base.transform.lossyScale.x;
			Vector3 vector2 = Vector3.left * base.transform.lossyScale.x;
			Vector3 vector3 = Vector3.up * base.transform.lossyScale.y;
			Vector3 vector4 = Vector3.down * base.transform.lossyScale.y;
			Vector3 vector5 = Vector3.forward * base.transform.lossyScale.z;
			Vector3 vector6 = Vector3.back * base.transform.lossyScale.z;
			array[0] = base.transform.position + vector + vector5 + vector4;
			array[1] = base.transform.position + vector2 + vector5 + vector4;
			array[2] = base.transform.position + vector + vector6 + vector4;
			array[3] = base.transform.position + vector2 + vector6 + vector4;
			array[4] = base.transform.position + vector + vector5 + vector3;
			array[5] = base.transform.position + vector2 + vector5 + vector3;
			array[6] = base.transform.position + vector + vector6 + vector3;
			array[7] = base.transform.position + vector2 + vector6 + vector3;
			return array;
		}

		public Bounds GetBounds()
		{
			Bounds result = default(Bounds);
			Vector3 zero = Vector3.zero;
			Vector3 vector = Vector3.one * 0.5f;
			Vector3 position = new Vector3(zero.x - vector.x, zero.y + vector.y, zero.z - vector.z);
			Vector3 position2 = new Vector3(zero.x + vector.x, zero.y + vector.y, zero.z - vector.z);
			Vector3 position3 = new Vector3(zero.x - vector.x, zero.y - vector.y, zero.z - vector.z);
			Vector3 position4 = new Vector3(zero.x + vector.x, zero.y - vector.y, zero.z - vector.z);
			Vector3 position5 = new Vector3(zero.x - vector.x, zero.y + vector.y, zero.z + vector.z);
			Vector3 position6 = new Vector3(zero.x + vector.x, zero.y + vector.y, zero.z + vector.z);
			Vector3 position7 = new Vector3(zero.x - vector.x, zero.y - vector.y, zero.z + vector.z);
			Vector3 position8 = new Vector3(zero.x + vector.x, zero.y - vector.y, zero.z + vector.z);
			position = base.transform.TransformPoint(position);
			position2 = base.transform.TransformPoint(position2);
			position3 = base.transform.TransformPoint(position3);
			position4 = base.transform.TransformPoint(position4);
			position5 = base.transform.TransformPoint(position5);
			position6 = base.transform.TransformPoint(position6);
			position7 = base.transform.TransformPoint(position7);
			position8 = base.transform.TransformPoint(position8);
			result.center = base.transform.position;
			result.Encapsulate(position);
			result.Encapsulate(position2);
			result.Encapsulate(position3);
			result.Encapsulate(position4);
			result.Encapsulate(position5);
			result.Encapsulate(position6);
			result.Encapsulate(position7);
			result.Encapsulate(position8);
			return result;
		}

		protected virtual void OnDrawGizmos()
		{
			if (SharedMesh != null && base.Technique != ProjectionTechnique.SSD)
			{
				DrawMeshGeometryGizmos(SharedMesh.vertices, SharedMesh.normals);
			}
			if (base.Baked)
			{
				Gizmos.DrawIcon(base.transform.position, "EasyDecalBaked Icon.png", true);
			}
			else
			{
				Gizmos.DrawIcon(base.transform.position, "EasyDecal Icon.png", true);
			}
			if (projector != null)
			{
				projector.DrawGizmos(false);
			}
		}

		public void OnDrawGizmosSelected()
		{
			if (projector != null)
			{
				projector.DrawGizmos(true);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (projector == null)
			{
				return;
			}
			if (!baked)
			{
				InverseProjectionDirection = (base.Rotation * Vector3.up).normalized;
				projector.Project();
				if (projectBake && Time.frameCount == bakeInFrameNumber)
				{
					base.Baked = true;
					projectBake = false;
				}
			}
			projector.Update();
		}

		protected override void Initialize()
		{
			base.Initialize();
			CreateDecalGameObject();
		}

		protected override void InitalizeProjector()
		{
			if (base.Technique == ProjectionTechnique.Plane)
			{
				projector = new RayProjector(this);
			}
			else if (base.Technique == ProjectionTechnique.Box)
			{
				projector = new BoxProjector(this);
			}
			else if (base.Technique == ProjectionTechnique.SkinnedBox)
			{
				projector = new SkinnedBoxProjector(this);
			}
			else if (base.Technique == ProjectionTechnique.SSD || base.Technique == ProjectionTechnique.Deferred)
			{
				projector = new SSDProjector(this);
			}
			if (projector != null)
			{
				if (sharedMesh == null)
				{
					sharedMesh = FindSharedMesh();
				}
				if (sharedMesh != null && baked && projector.Mesh != null && projector.Mesh.VerticesAmount == 0 && sharedMesh.vertexCount > 0)
				{
					DynamicMesh.Assign(projector.Mesh, sharedMesh);
				}
			}
			else
			{
				Debug.LogError("Easy Decal: Couldn't instantiate projector: " + base.Technique);
			}
		}

		private void Log(string msg)
		{
			if (technique == ProjectionTechnique.Box)
			{
				Debug.Log(msg);
			}
		}

		protected override void OnChangedTechnique()
		{
			base.OnChangedTechnique();
			if (base.DecalRenderer != null)
			{
				base.DecalRenderer.enabled = base.DeferredFlags == (DeferredFlags)0;
			}
			Initialize();
		}

		protected virtual void OnChangedAtlasIndex()
		{
			AskForUVRecreation();
		}

		protected virtual void OnChangedSource()
		{
			AskForGeometryRecreation();
			AskForUVRecreation();
		}

		protected virtual void OnChangedAlpha()
		{
			if (projector != null)
			{
				projector.OnAlphaChanged();
			}
		}

		protected override void OnChangedBakeStatus()
		{
			base.OnChangedBakeStatus();
			if (projector != null)
			{
				projector.OnBakeStatusChanged();
			}
		}

		protected override void AskForGeometryRecreation()
		{
			base.AskForGeometryRecreation();
			needGeometryRecreation = true;
		}

		protected override void AskForUVRecreation()
		{
			base.AskForUVRecreation();
			if (baked)
			{
				if (projector != null)
				{
					if (source == SourceMode.Atlas)
					{
						TransformUV(projector.Mesh);
					}
					ConvertAndAssign(projector.Mesh, false);
				}
			}
			else
			{
				needUVRecreation = true;
			}
		}

		private void CreateDecalGameObject()
		{
			meshContainer = Search("PROC_PLANE");
			skinnedMeshContainer = Search("SKINNED_PROC_PLANE");
			if (meshContainer == null)
			{
				meshContainer = new GameObject("PROC_PLANE", typeof(MeshRenderer), typeof(MeshFilter));
			}
			if (skinnedMeshContainer == null)
			{
				skinnedMeshContainer = new GameObject("SKINNED_PROC_PLANE", typeof(SkinnedMeshRenderer));
			}
			SetChildTransform(meshContainer);
			SetChildTransform(skinnedMeshContainer);
			if (base.Technique == ProjectionTechnique.SkinnedBox)
			{
				meshContainer.SetActive(false);
				skinnedMeshContainer.SetActive(true);
			}
			else
			{
				meshContainer.SetActive(true);
				skinnedMeshContainer.SetActive(false);
			}
			base.MeshFilter = MeshContainer.GetComponent<MeshFilter>();
			base.DecalRenderer = MeshContainer.GetComponent<Renderer>();
			base.DecalRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
		}

		private void SetChildTransform(GameObject child)
		{
			child.transform.parent = base.transform;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
			child.transform.localPosition = Vector3.zero;
			if (HideMesh)
			{
				child.hideFlags = HideFlags.HideInHierarchy;
			}
		}

		private void ResetMesh()
		{
			if (technique == ProjectionTechnique.SkinnedBox)
			{
				if (base.SkinnedDecalRenderer != null && base.SkinnedDecalRenderer.sharedMesh != null)
				{
					base.SkinnedDecalRenderer.sharedMesh = UnityEngine.Object.Instantiate(base.SkinnedDecalRenderer.sharedMesh);
				}
			}
			else if (base.MeshFilter != null && base.MeshFilter.sharedMesh != null)
			{
				base.MeshFilter.sharedMesh = UnityEngine.Object.Instantiate(base.MeshFilter.sharedMesh);
			}
			sharedMesh = null;
		}

		private Mesh FindSharedMesh()
		{
			Mesh result = null;
			if (technique == ProjectionTechnique.SkinnedBox)
			{
				if (base.SkinnedDecalRenderer != null)
				{
					result = base.SkinnedDecalRenderer.sharedMesh;
				}
			}
			else if (base.MeshFilter != null)
			{
				result = base.MeshFilter.sharedMesh;
			}
			return result;
		}

		private GameObject Search(string name)
		{
			GameObject result = null;
			Transform transform = base.transform.Find(name);
			if (transform != null)
			{
				result = transform.gameObject;
			}
			return result;
		}

		internal void AddDynamicMesh(DynamicMesh dynamicMesh)
		{
			foreach (IMeshProcessor meshProcessor in meshProcessors)
			{
				if (meshProcessor != null)
				{
					meshProcessor.Process(dynamicMesh);
				}
			}
			if ((source == SourceMode.Atlas && needUVRecreation) || dynamicMesh.Mode == RecreationMode.Always)
			{
				TransformUV(dynamicMesh);
			}
			PostProcessMesh(dynamicMesh);
			ConvertAndAssign(dynamicMesh, needUVRecreation || needGeometryRecreation);
			CallOnProjectionFinished(sharedMesh);
			needUVRecreation = false;
			needGeometryRecreation = false;
		}

		private void ConvertAndAssign(DynamicMesh dynamicMesh, bool forceRecreation)
		{
			sharedMesh = dynamicMesh.ConvertToMesh(sharedMesh, forceRecreation);
			AssignSharedMesh(sharedMesh);
		}

		private void AssignSharedMesh(Mesh mesh)
		{
			if (base.Technique == ProjectionTechnique.SkinnedBox)
			{
				base.SkinnedDecalRenderer.sharedMesh = mesh;
			}
			else
			{
				base.MeshFilter.sharedMesh = mesh;
			}
		}

		private void PostProcessMesh(DynamicMesh mesh)
		{
			if (CalculateNormals && base.Technique != ProjectionTechnique.SSD && base.Technique != ProjectionTechnique.Deferred)
			{
				if (base.Technique == ProjectionTechnique.Plane)
				{
					MeshUtil.CalculateNormals(mesh.Normals, mesh.Vertices, mesh.TriangleIndices);
				}
				if (smoothNormals)
				{
					MeshUtil.SmoothVertexNormals(mesh.Normals, InverseProjectionDirection, normalSmoothFactor, normalSmoothThreshold);
				}
				if (CalculateTangents)
				{
					mesh.Tangents = (List<Vector4>)MeshUtil.CalculateTangents(mesh.Vertices, mesh.Normals, mesh.UV, mesh.TriangleIndices);
				}
			}
		}

		public void PostProcessMesh(Mesh mesh)
		{
		}

		internal void TransformUV(DynamicMesh dynamicMesh)
		{
			if (source != SourceMode.Atlas || !(atlas != null) || atlasRegionIndex >= atlas.Regions.Count || atlas.Regions.Count <= 0)
			{
				return;
			}
			AtlasRegion atlasRegion = Atlas.Regions[AtlasRegionIndex];
			bool flag = base.Technique == ProjectionTechnique.SSD || base.Technique == ProjectionTechnique.Deferred;
			for (int i = 0; i < dynamicMesh.VerticesAmount; i++)
			{
				if (flag)
				{
					dynamicMesh.UV[i] = new Vector2(atlasRegion.Region.width, atlasRegion.Region.height);
					dynamicMesh.UV2[i] = new Vector2(atlasRegion.Region.x, atlasRegion.Region.y);
					continue;
				}
				Vector2 value = dynamicMesh.UV[i];
				value.x *= atlasRegion.Region.width;
				value.y *= atlasRegion.Region.height;
				value.x += atlasRegion.Region.x;
				value.y += atlasRegion.Region.y;
				dynamicMesh.UV[i] = value;
			}
		}

		private void DrawMeshGeometryGizmos(Vector3[] vertices, Vector3[] normals)
		{
			Matrix4x4 localToWorldMatrix = base.LocalToWorldMatrix;
			if (normals.Length != vertices.Length)
			{
				return;
			}
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
				Vector3 vector2 = localToWorldMatrix.MultiplyVector(normals[i]);
				if (ShowVertices)
				{
					Gizmos.DrawCube(vector, Vector3.one * 0.02f);
				}
				if (ShowNormals)
				{
					Gizmos.DrawLine(vector, vector + vector2 * 0.5f);
				}
			}
		}

		private void ResetPlanes()
		{
		}

		protected void UpdateDecal()
		{
			projector.Project();
		}

		[Obsolete("Use LateBake() instead.")]
		protected void ProjectBake()
		{
			LateBake();
		}

		public void LateUnbake()
		{
			projectBake = false;
			base.Baked = false;
		}

		public void LateBake()
		{
			LateBake(1u);
		}

		public void LateBake(uint frames)
		{
			projectBake = true;
			if (frames < 1)
			{
				Debug.LogWarning("Easy Decal: LateBake() the passed argument have to be greater than 0.");
			}
			bakeInFrameNumber = Time.frameCount + (int)frames;
		}

		internal Color GetFadeoutColor()
		{
			return new Color(1f, 1f, 1f, Alpha);
		}

		private void CallOnProjectionFinished(Mesh mesh)
		{
			if (OnProjectionFinished_ != null)
			{
				OnProjectionFinished_(this, mesh);
			}
		}

		private void CallOnFadeOutStarted()
		{
			projector.OnFadeOutStarted();
			if (OnFadeOutStarted_ != null)
			{
				OnFadeOutStarted_(this);
			}
		}

		private void CallOnFadedOut()
		{
			if (OnFadedOut_ != null)
			{
				OnFadedOut_(this);
			}
		}

		private void CallOnFadedIn()
		{
			if (OnFadedIn_ != null)
			{
				OnFadedIn_(this);
			}
		}

		private void CallOnAtlasIndexChanged()
		{
			OnChangedAtlasIndex();
			if (OnAtlasIndexChanged_ != null)
			{
				OnAtlasIndexChanged_(this);
			}
		}

		private void CallOnSourceChanged()
		{
			OnChangedSource();
			if (OnSourceChanged_ != null)
			{
				OnSourceChanged_(this);
			}
		}

		private void CallOnAlphaChanged()
		{
			OnChangedAlpha();
		}

		public static EasyDecal Clone(EasyDecal decal)
		{
			EasyDecal easyDecal = null;
			GameObject gameObject = new GameObject(decal.name, typeof(EasyDecal));
			gameObject.transform.position = decal.Position;
			gameObject.transform.rotation = decal.Rotation;
			gameObject.transform.localScale = decal.Scale;
			gameObject.transform.parent = decal.transform.parent;
			easyDecal = gameObject.GetComponent<EasyDecal>();
			CopyProperties(decal, easyDecal);
			return easyDecal;
		}

		public static void CopyProperties(EasyDecal from, EasyDecal to)
		{
			to.DecalMaterial = from.DecalRenderer.sharedMaterial;
			to.Technique = from.Technique;
			to.Mode = from.Mode;
			to.MultiMeshEnabled = from.MultiMeshEnabled;
			to.NormalSmoothFactor = from.NormalSmoothFactor;
			to.NormalSmoothThreshold = from.NormalSmoothThreshold;
			to.SmoothNormals = from.SmoothNormals;
			to.ShowDir = from.ShowDir;
			to.ShowNormals = from.ShowNormals;
			to.ShowVertices = from.ShowVertices;
			to.Baked = from.Baked;
			to.BakeOnAwake = from.BakeOnAwake;
			to.Quality = from.Quality;
			to.CalculateNormals = from.CalculateNormals;
			to.CalculateTangents = from.CalculateTangents;
			to.AngleConstraint = from.AngleConstraint;
			to.Distance = from.Distance;
			to.Source = from.Source;
			to.Atlas = from.Atlas;
			to.AtlasRegionIndex = from.AtlasRegionIndex;
			to.AspectCorrectionMode = from.AspectCorrectionMode;
			to.MaxDistance = from.MaxDistance;
			to.BackfaceCulling = from.BackfaceCulling;
			to.CullInvisibles = from.CullInvisibles;
			to.RecursiveLookup = from.RecursiveLookup;
			to.RecursiveLookupSteps = from.RecursiveLookupSteps;
			to.FadeOut = from.FadeOut;
			to.OnlyColliders = from.OnlyColliders;
			to.Mask = from.Mask;
			to.DecalMaterial = from.DecalMaterial;
			to.DeferredFlags = from.DeferredFlags;
			to.SurfaceRotation = from.SurfaceRotation;
		}

		public static void SetStaticProxyCollection(StaticProxyCollection proxyCollection)
		{
			ProxyCollection = proxyCollection;
		}

		public static EasyDecal ProjectAt(GameObject decalPrefab, GameObject receiver, Vector3 position, Vector3 forward, Vector3 up)
		{
			Vector3 scale = new Vector3(decalPrefab.transform.localScale.x, decalPrefab.transform.localScale.y, decalPrefab.transform.localScale.z);
			Quaternion rotation = Quaternion.LookRotation(up, -forward);
			return ProjectInternal(decalPrefab, receiver, position, rotation, scale);
		}

		public static EasyDecal ProjectAt(GameObject decalPrefab, GameObject receiver, Vector3 position, Quaternion rotation)
		{
			Vector3 scale = new Vector3(decalPrefab.transform.localScale.x, decalPrefab.transform.localScale.y, decalPrefab.transform.localScale.z);
			return ProjectInternal(decalPrefab, receiver, position, rotation, scale);
		}

		public static EasyDecal ProjectAt(GameObject decalPrefab, GameObject receiver, Vector3 position, Vector3 normal)
		{
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
			Vector3 scale = new Vector3(decalPrefab.transform.localScale.x, decalPrefab.transform.localScale.y, decalPrefab.transform.localScale.z);
			return ProjectInternal(decalPrefab, receiver, position, rotation, scale);
		}

		public static EasyDecal ProjectAt(GameObject decalPrefab, GameObject receiver, Vector3 position, Vector3 normal, float rotation)
		{
			Quaternion rotation2 = Quaternion.AngleAxis(rotation, normal) * Quaternion.FromToRotation(Vector3.up, normal);
			return ProjectAt(decalPrefab, receiver, position, rotation2);
		}

		public static EasyDecal ProjectAt(GameObject decalPrefab, GameObject receiver, Vector3 position, Vector3 normal, float rotation, Vector3 scale)
		{
			Quaternion rotation2 = Quaternion.AngleAxis(rotation, normal) * Quaternion.FromToRotation(Vector3.up, normal);
			return ProjectInternal(decalPrefab, receiver, position, rotation2, scale);
		}

		public static EasyDecal Project(GameObject decalPrefab, Vector3 position, Quaternion rotation)
		{
			Vector3 scale = new Vector3(decalPrefab.transform.localScale.x, decalPrefab.transform.localScale.y, decalPrefab.transform.localScale.z);
			return ProjectInternal(decalPrefab, null, position, rotation, scale);
		}

		public static EasyDecal Project(GameObject decalPrefab, Vector3 position, Vector3 normal)
		{
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
			return Project(decalPrefab, position, rotation);
		}

		public static EasyDecal Project(GameObject decalPrefab, Vector3 position, Vector3 normal, float rotation)
		{
			Quaternion rotation2 = Quaternion.AngleAxis(rotation, normal) * Quaternion.FromToRotation(Vector3.up, normal);
			return Project(decalPrefab, position, rotation2);
		}

		public static EasyDecal Project(GameObject decalPrefab, Vector3 position, Ray ray)
		{
			return Project(decalPrefab, position, -ray.direction.normalized);
		}

		private static EasyDecal ProjectInternal(GameObject decalPrefab, GameObject parent, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			EasyDecal component = decalPrefab.GetComponent<EasyDecal>();
			if (component == null)
			{
				throw new Exception("The prefab you want to instantiate has no 'EasyDecal' script appended.");
			}
			if (parent != null && !ContainsLayer(component.Mask, parent.layer))
			{
				return null;
			}
			float y = ((component.Technique == ProjectionTechnique.Plane) ? 1f : scale.y);
			EasyDecal easyDecal = null;
			if (Instantiation != null)
			{
				easyDecal = Instantiation(decalPrefab, parent, position, rotation);
				if (easyDecal == null)
				{
					Debug.LogError("The assigned instantiation method returned a null object.");
				}
			}
			else
			{
				easyDecal = UnityEngine.Object.Instantiate(decalPrefab, position, rotation).GetComponent<EasyDecal>();
			}
			easyDecal.transform.localScale = new Vector3(scale.x, y, scale.z);
			easyDecal.Receiver = parent;
			easyDecal.LateBake();
			if (parent != null)
			{
				DecalReceiver component2 = parent.GetComponent<DecalReceiver>();
				if (component2 != null)
				{
					easyDecal.Distance = component2.fractionsum;
					component2.Receive(easyDecal);
				}
				easyDecal.gameObject.transform.parent = parent.transform;
			}
			return easyDecal;
		}

		private static List<EasyDecal> SearchChildren(Transform parent)
		{
			List<EasyDecal> list = new List<EasyDecal>();
			foreach (Transform item in parent)
			{
				EasyDecal component = item.GetComponent<EasyDecal>();
				if (component != null)
				{
					list.Add(component);
				}
			}
			return list;
		}

		private static bool ContainsLayer(LayerMask mask, int layer)
		{
			return (mask.value & (1 << layer)) > 0;
		}
	}
}
