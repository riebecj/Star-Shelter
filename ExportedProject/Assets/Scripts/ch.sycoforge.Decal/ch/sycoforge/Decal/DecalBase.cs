using System;
using UnityEngine;
using ch.sycoforge.Decal.Projectors;

namespace ch.sycoforge.Decal
{
	public abstract class DecalBase : MonoBehaviour
	{
		private const string RootName = "[Decal Root]";

		private OrientationChangedHandler OnOrientationChanged_;

		private TechniqueChangedHandler OnTechniqueChangedHandler_;

		private DeferredFlagsChangedHandler OnDeferredFlagsChanged_;

		private BakeStatusChangedHandler OnBakeStatusChanged_;

		private Action<DecalBase> OnDestroyed_;

		private Vector3 lastPosition;

		private Vector3 lastScale;

		private Quaternion lastRotation;

		[HideInInspector]
		[SerializeField]
		protected bool baked;

		[SerializeField]
		[HideInInspector]
		protected bool bakeOnAwake;

		[HideInInspector]
		[SerializeField]
		private bool flipNormals;

		[SerializeField]
		[HideInInspector]
		private bool backfaceCulling;

		[SerializeField]
		[HideInInspector]
		private float distance = 0.01f;

		[SerializeField]
		[HideInInspector]
		private float angleConstraint = 180f;

		[HideInInspector]
		[SerializeField]
		protected ProjectionTechnique technique = ProjectionTechnique.Plane;

		[SerializeField]
		[HideInInspector]
		protected ch.sycoforge.Decal.Projectors.Projector projector;

		[HideInInspector]
		[SerializeField]
		private DeferredFlags deferredFlags = (DeferredFlags)(-1);

		[HideInInspector]
		[SerializeField]
		private TransformObserver transformObserver;

		private Bounds bounds;

		private ProjectionTechnique lastTechnique;

		private DeferredFlags lastDeferredFlags;

		private static GameObject combinedRoot;

		private static DecalRoot decalRoot;

		private static GameObject CombinedRoot
		{
			get
			{
				TryInitializeRoot();
				return combinedRoot;
			}
		}

		public static DecalRoot DecalRoot
		{
			get
			{
				GameObject gameObject = CombinedRoot;
				if (decalRoot == null)
				{
					decalRoot = gameObject.GetComponent<DecalRoot>();
					if (decalRoot == null)
					{
						decalRoot = gameObject.AddComponent<DecalRoot>();
					}
				}
				return decalRoot;
			}
		}

		public bool Baked
		{
			get
			{
				return baked;
			}
			set
			{
				bool flag = baked != value;
				baked = value;
				if (flag)
				{
					CallOnBakeStatusChanged();
				}
			}
		}

		public bool BakeOnAwake
		{
			get
			{
				return bakeOnAwake;
			}
			set
			{
				bakeOnAwake = value;
			}
		}

		public bool FlipNormals
		{
			get
			{
				return flipNormals;
			}
			set
			{
				flipNormals = value;
			}
		}

		public bool BackfaceCulling
		{
			get
			{
				return backfaceCulling;
			}
			set
			{
				backfaceCulling = value;
			}
		}

		public Matrix4x4 WorldToLocalMatrix { get; protected set; }

		public Matrix4x4 LocalToWorldMatrix { get; protected set; }

		public Transform CachedTransform { get; protected set; }

		public Quaternion Rotation { get; private set; }

		public Vector3 Position { get; private set; }

		public Vector3 Scale { get; private set; }

		public float Distance
		{
			get
			{
				return distance;
			}
			set
			{
				distance = value;
			}
		}

		public float AngleConstraint
		{
			get
			{
				return angleConstraint;
			}
			set
			{
				CallOnPropertyChanged(angleConstraint, value);
				angleConstraint = value;
			}
		}

		public ProjectionTechnique Technique
		{
			get
			{
				return technique;
			}
			set
			{
				ProjectionTechnique projectionTechnique = technique;
				technique = value;
				if (technique != ProjectionTechnique.Deferred)
				{
					deferredFlags = (DeferredFlags)0;
				}
			}
		}

		public DeferredFlags DeferredFlags
		{
			get
			{
				return deferredFlags;
			}
			set
			{
				deferredFlags = value;
			}
		}

		public Renderer DecalRenderer { get; set; }

		internal SkinnedMeshRenderer SkinnedDecalRenderer
		{
			get
			{
				return DecalRenderer as SkinnedMeshRenderer;
			}
		}

		public MeshFilter MeshFilter { get; protected set; }

		public ch.sycoforge.Decal.Projectors.Projector Projector
		{
			get
			{
				return projector;
			}
			private set
			{
				projector = value;
			}
		}

		public event OrientationChangedHandler OnOrientationChanged
		{
			add
			{
				OnOrientationChanged_ = (OrientationChangedHandler)Delegate.Combine(OnOrientationChanged_, value);
			}
			remove
			{
				OnOrientationChanged_ = (OrientationChangedHandler)Delegate.Remove(OnOrientationChanged_, value);
			}
		}

		internal event TechniqueChangedHandler OnTechniqueChanged
		{
			add
			{
				OnTechniqueChangedHandler_ = (TechniqueChangedHandler)Delegate.Combine(OnTechniqueChangedHandler_, value);
			}
			remove
			{
				OnTechniqueChangedHandler_ = (TechniqueChangedHandler)Delegate.Remove(OnTechniqueChangedHandler_, value);
			}
		}

		internal event DeferredFlagsChangedHandler OnDeferredFlagsChanged
		{
			add
			{
				OnDeferredFlagsChanged_ = (DeferredFlagsChangedHandler)Delegate.Combine(OnDeferredFlagsChanged_, value);
			}
			remove
			{
				OnDeferredFlagsChanged_ = (DeferredFlagsChangedHandler)Delegate.Remove(OnDeferredFlagsChanged_, value);
			}
		}

		internal event BakeStatusChangedHandler OnBakeStatusChanged
		{
			add
			{
				OnBakeStatusChanged_ = (BakeStatusChangedHandler)Delegate.Combine(OnBakeStatusChanged_, value);
			}
			remove
			{
				OnBakeStatusChanged_ = (BakeStatusChangedHandler)Delegate.Remove(OnBakeStatusChanged_, value);
			}
		}

		internal event Action<DecalBase> OnDestroyed
		{
			add
			{
				OnDestroyed_ = (Action<DecalBase>)Delegate.Combine(OnDestroyed_, value);
			}
			remove
			{
				OnDestroyed_ = (Action<DecalBase>)Delegate.Remove(OnDestroyed_, value);
			}
		}

		protected virtual void Awake()
		{
			CachedTransform = base.transform;
			transformObserver = new TransformObserver(CachedTransform);
			lastDeferredFlags = (DeferredFlags)0;
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void OnDestroy()
		{
			CallOnDestroyed();
		}

		protected virtual void Start()
		{
			TryInitializeRoot();
			SetLastTransforms();
			DecalRoot.Initialize();
		}

		protected virtual void Update()
		{
			if (!baked)
			{
				OrientationChange orientationChange = transformObserver.CheckTransformChange();
				if (orientationChange != 0)
				{
					CallOnOrientationChanged(orientationChange);
				}
			}
			CheckTechniqueChange();
			CheckDeferredFlagsChange();
		}

		protected virtual void Initialize()
		{
			Rotation = CachedTransform.rotation;
			Position = CachedTransform.position;
			Scale = CachedTransform.lossyScale;
			WorldToLocalMatrix = CachedTransform.worldToLocalMatrix;
			LocalToWorldMatrix = CachedTransform.localToWorldMatrix;
			bounds = CalculateBounds();
		}

		protected virtual void OnChangedTechnique()
		{
			InitalizeProjector();
		}

		protected virtual void OnChangedDeferredFlags()
		{
		}

		protected virtual void OnChangedOrientation()
		{
			Initialize();
		}

		protected virtual void OnChangedBakeStatus()
		{
		}

		protected abstract void InitalizeProjector();

		protected void CallOnPropertyChanged(object oldValue, object newValue)
		{
			try
			{
				if (!oldValue.Equals(newValue) && projector != null)
				{
					AskForGeometryRecreation();
					projector.OnGeometryPropertyChanged();
				}
			}
			catch
			{
				Debug.LogError("Easy Decal: OnPropertyChanged event could not be thrown.");
			}
		}

		protected virtual void AskForUVRecreation()
		{
		}

		protected virtual void AskForGeometryRecreation()
		{
		}

		public Bounds Bounds()
		{
			if (bounds == default(Bounds))
			{
				bounds = CalculateBounds();
			}
			return bounds;
		}

		private void CheckTechniqueChange()
		{
			if (technique != lastTechnique)
			{
				CallOnTechniqueChanged(lastTechnique, technique);
			}
			lastTechnique = technique;
		}

		private void CheckDeferredFlagsChange()
		{
			if (deferredFlags != lastDeferredFlags)
			{
				CallOnDeferredFlagsChanged(lastDeferredFlags, deferredFlags);
			}
			lastDeferredFlags = deferredFlags;
		}

		private Bounds CalculateBounds()
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(Position, Rotation, Vector3.one);
			Vector3 vector = 0.5f * Scale;
			Vector3 vector2 = new Vector3(0f, 0f - Mathf.Abs(vector.y), 0f);
			Vector3 center = matrix4x.MultiplyPoint3x4(Vector3.zero);
			Bounds result = new Bounds(center, Vector3.zero);
			center = vector2 + new Vector3(vector.x, vector.y, vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			center = vector2 + new Vector3(vector.x, vector.y, 0f - vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			center = vector2 + new Vector3(vector.x, 0f - vector.y, vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			center = vector2 + new Vector3(vector.x, 0f - vector.y, 0f - vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			center = vector2 + new Vector3(0f - vector.x, vector.y, vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			center = vector2 + new Vector3(0f - vector.x, vector.y, 0f - vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			center = vector2 + new Vector3(0f - vector.x, 0f - vector.y, vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			center = vector2 + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
			center = matrix4x.MultiplyPoint3x4(center);
			result.Encapsulate(center);
			return result;
		}

		private void SetLastTransforms()
		{
			lastPosition = CachedTransform.position;
			lastScale = CachedTransform.localScale;
			lastRotation = CachedTransform.rotation;
		}

		private void CallOnOrientationChanged(OrientationChange change)
		{
			OnChangedOrientation();
			if (OnOrientationChanged_ != null)
			{
				OnOrientationChanged_(this, change);
			}
		}

		private void CallOnTechniqueChanged(ProjectionTechnique last, ProjectionTechnique curr)
		{
			OnChangedTechnique();
			if (OnTechniqueChangedHandler_ != null)
			{
				OnTechniqueChangedHandler_(this, last, curr);
			}
		}

		private void CallOnDeferredFlagsChanged(DeferredFlags last, DeferredFlags curr)
		{
			OnChangedDeferredFlags();
			if (OnDeferredFlagsChanged_ != null)
			{
				OnDeferredFlagsChanged_(this, last, curr);
			}
		}

		private void CallOnDestroyed()
		{
			if (OnDestroyed_ != null)
			{
				OnDestroyed_(this);
			}
		}

		private void CallOnBakeStatusChanged()
		{
			OnChangedBakeStatus();
			if (OnBakeStatusChanged_ != null)
			{
				OnBakeStatusChanged_(this);
			}
		}

		public static void TryInitializeRoot()
		{
			if ((combinedRoot == null || (combinedRoot != null && combinedRoot.Equals(null))) && (combinedRoot = GameObject.Find("[Decal Root]")) == null)
			{
				combinedRoot = new GameObject("[Decal Root]", typeof(DecalRoot));
				Debug.Log("Easy Decal: No Decal root found. Created root.");
			}
		}
	}
}
