using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	[SelectionBase]
	[ExecuteInEditMode]
	public class ObjectPlacer : SerializedMonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003CRepositionObjects_003Ec__AnonStorey0
		{
			internal System.Random rnd;

			internal double _003C_003Em__0(PlaceableObject x)
			{
				return rnd.NextDouble() - (double)x.SpawnChance;
			}
		}

		[SerializeField]
		[HideInInspector]
		private Matrix4x4 prevMatrix;

		[SerializeField]
		[InlineEditor(InlineEditorModes.GUIAndPreview, PreviewWidth = 55f)]
		public List<PlaceableObject> Prefabs;

		[SerializeField]
		[TabGroup("Placement", false, 2)]
		[HideLabel]
		public ObjectPlacementFunction ObjectPlacementFunction = new CirclePlacementFunction();

		[TabGroup("General", false, 0)]
		public bool KeepPrefabReference;

		[Range(2f, 400f)]
		[TabGroup("General", false, 0)]
		public int NumberOfObjects = 30;

		[Range(0f, 30f)]
		[TabGroup("General", false, 0)]
		public float Radius = 4f;

		[TabGroup("General", false, 0)]
		public Vector3 Rotation;

		[Range(0f, 1f)]
		[TabGroup("General", false, 0)]
		public float TerrainRotationFactor;

		[TabGroup("General", false, 0)]
		public LayerMask TerrainLayer;

		[LabelText("X")]
		[TabGroup("Randomization", false, 0)]
		[MinMaxSlider(-0.5f, 0.5f, false)]
		public Vector2 OffsetX;

		[LabelText("Y")]
		[TabGroup("Randomization", false, 0)]
		[MinMaxSlider(-1f, 1f, false)]
		public Vector2 OffsetY;

		[LabelText("Z")]
		[TabGroup("Randomization", false, 0)]
		[MinMaxSlider(-0.1f, 0.1f, false)]
		public Vector2 OffsetZ;

		[Range(0f, 360f)]
		[TabGroup("Randomization", false, 0)]
		[LabelText("Angle")]
		public float AngleJitter;

		[TabGroup("Randomization", false, 0)]
		[LabelText("Axis")]
		public Axis AngleJitterAxis = Axis.Y;

		[CompilerGenerated]
		private static Func<PlaceableObject, bool> _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static Func<PlaceableObject, bool> _003C_003Ef__am_0024cache1;

		[Button("Update", ButtonSizes.Small)]
		[PropertyOrder(3)]
		public void ClearAndRepositionObjects()
		{
			for (int num = base.transform.childCount - 1; num >= 0; num--)
			{
				GameObject obj = base.transform.GetChild(num).gameObject;
				UnityEngine.Object.DestroyImmediate(obj);
			}
			RepositionObjects();
		}

		private void Update()
		{
			if (prevMatrix != base.transform.localToWorldMatrix)
			{
				prevMatrix = base.transform.localToWorldMatrix;
				RepositionObjects();
			}
		}

		private void OnValidate()
		{
			RepositionObjects();
		}

		public void RepositionObjects()
		{
			_003CRepositionObjects_003Ec__AnonStorey0 _003CRepositionObjects_003Ec__AnonStorey = new _003CRepositionObjects_003Ec__AnonStorey0();
			if (ObjectPlacementFunction == null || Prefabs == null)
			{
				return;
			}
			List<PlaceableObject> prefabs = Prefabs;
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CRepositionObjects_003Em__0;
			}
			if (!prefabs.Any(_003C_003Ef__am_0024cache0))
			{
				return;
			}
			while (base.transform.childCount > NumberOfObjects && base.transform.childCount > 0)
			{
				GameObject obj = base.transform.GetChild(0).gameObject;
				UnityEngine.Object.DestroyImmediate(obj);
			}
			_003CRepositionObjects_003Ec__AnonStorey.rnd = new System.Random(GetInstanceID());
			for (int i = base.transform.childCount; i < NumberOfObjects; i++)
			{
				List<PlaceableObject> prefabs2 = Prefabs;
				if (_003C_003Ef__am_0024cache1 == null)
				{
					_003C_003Ef__am_0024cache1 = _003CRepositionObjects_003Em__1;
				}
				PlaceableObject placeableObject = prefabs2.Where(_003C_003Ef__am_0024cache1).OrderBy(_003CRepositionObjects_003Ec__AnonStorey._003C_003Em__0).FirstOrDefault();
				GameObject gameObject = UnityEngine.Object.Instantiate(placeableObject.gameObject);
				gameObject.transform.parent = base.transform;
			}
			for (int j = 0; j < base.transform.childCount; j++)
			{
				GameObject gameObject2 = base.transform.GetChild(j).gameObject;
				PlaceableObject component = gameObject2.GetComponent<PlaceableObject>();
				if (!(component == null))
				{
					float t = (float)j / (float)base.transform.childCount;
					Vector3 binormal = ObjectPlacementFunction.GetBinormal(t);
					Vector3 tangent = ObjectPlacementFunction.GetTangent(t);
					Vector3 vector = ObjectPlacementFunction.GetPosition(t) * Radius;
					Vector3 one = Vector3.one;
					Quaternion quaternion = base.transform.rotation * Quaternion.Euler(new Vector3(0f, Mathf.Atan2(binormal.x, binormal.z) * 57.29578f, 0f) + Rotation);
					int value = TerrainLayer.value;
					Vector3 b = new Vector3(((int)AngleJitterAxis >> 1) & 1, ((int)AngleJitterAxis >> 2) & 1, ((int)AngleJitterAxis >> 3) & 1);
					Vector3 vector2 = new Vector3((float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble() - 0.5f, (float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble() - 0.5f, (float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble() - 0.5f);
					vector += binormal * Radius * Mathf.Lerp(OffsetX.x, OffsetX.y, (float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble());
					vector += tangent * Radius * Mathf.Lerp(OffsetZ.x, OffsetZ.y, (float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble());
					one *= Mathf.Lerp(component.MinMaxScaleSize.x, component.MinMaxScaleSize.y, (float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble());
					quaternion *= Quaternion.Euler(Vector3.Scale(vector2 * AngleJitter, b));
					RaycastHit hitInfo;
					if (Physics.Raycast(base.transform.TransformPoint(vector) + base.transform.up * 100f, -base.transform.up, out hitInfo, float.MaxValue, (value >= 0) ? value : (-1)))
					{
						Vector3 vector3 = Vector3.Lerp(Vector3.up, hitInfo.normal.normalized, TerrainRotationFactor);
						float num = Mathf.Lerp(OffsetY.x, OffsetY.y, (float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble()) * 2f;
						vector = base.transform.InverseTransformPoint(hitInfo.point + vector3 * num);
						quaternion = Quaternion.Lerp(quaternion, Quaternion.FromToRotation(Vector3.up, hitInfo.normal.normalized) * quaternion, TerrainRotationFactor);
					}
					else
					{
						vector.y += Mathf.Lerp(OffsetY.x, OffsetY.y, (float)_003CRepositionObjects_003Ec__AnonStorey.rnd.NextDouble()) * 2f;
					}
					gameObject2.transform.localPosition = vector;
					gameObject2.transform.localScale = one;
					gameObject2.transform.rotation = quaternion;
				}
			}
		}

		[CompilerGenerated]
		private static bool _003CRepositionObjects_003Em__0(PlaceableObject x)
		{
			return (bool)x && x.Enabled;
		}

		[CompilerGenerated]
		private static bool _003CRepositionObjects_003Em__1(PlaceableObject x)
		{
			return (bool)x && x.Enabled;
		}
	}
}
