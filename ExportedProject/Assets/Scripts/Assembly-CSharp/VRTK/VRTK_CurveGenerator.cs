using UnityEngine;
using UnityEngine.Rendering;

namespace VRTK
{
	public class VRTK_CurveGenerator : MonoBehaviour
	{
		public enum BezierControlPointMode
		{
			Free = 0,
			Aligned = 1,
			Mirrored = 2
		}

		protected Vector3[] points;

		protected GameObject[] items;

		protected BezierControlPointMode[] modes;

		protected bool loop;

		protected int frequency;

		protected bool customTracer;

		protected bool rescalePointerTracer;

		protected GameObject tracerLineRenderer;

		protected LineRenderer customLineRenderer;

		protected bool lineRendererAndItem;

		protected virtual bool Loop
		{
			get
			{
				return loop;
			}
			set
			{
				loop = value;
				if (value)
				{
					modes[modes.Length - 1] = modes[0];
					SetControlPoint(0, points[0]);
				}
			}
		}

		protected virtual int ControlPointCount
		{
			get
			{
				return points.Length;
			}
		}

		protected virtual int CurveCount
		{
			get
			{
				return (points.Length - 1) / 3;
			}
		}

		public virtual void Create(int setFrequency, float radius, GameObject tracer, bool rescaleTracer = false)
		{
			float num = radius / 8f;
			frequency = setFrequency;
			customLineRenderer = ((!(tracer != null)) ? null : tracer.GetComponent<LineRenderer>());
			lineRendererAndItem = customLineRenderer != null && (bool)tracer.GetComponentInChildren<MeshFilter>();
			if (customLineRenderer != null)
			{
				tracerLineRenderer = Object.Instantiate(tracer);
				tracerLineRenderer.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, base.name, "LineRenderer");
				for (int i = 0; i < tracerLineRenderer.transform.childCount; i++)
				{
					Object.Destroy(tracerLineRenderer.transform.GetChild(i).gameObject);
				}
				customLineRenderer = tracerLineRenderer.GetComponent<LineRenderer>();
				customLineRenderer.positionCount = frequency;
			}
			if (customLineRenderer == null || lineRendererAndItem)
			{
				items = new GameObject[frequency];
				for (int j = 0; j < items.Length; j++)
				{
					customTracer = true;
					items[j] = ((!(tracer != null)) ? CreateSphere() : Object.Instantiate(tracer));
					items[j].transform.SetParent(base.transform);
					items[j].layer = LayerMask.NameToLayer("Ignore Raycast");
					items[j].transform.localScale = new Vector3(num, num, num);
					if (customLineRenderer != null)
					{
						Object.Destroy(items[j].GetComponent<LineRenderer>());
					}
				}
			}
			rescalePointerTracer = rescaleTracer;
		}

		public virtual void SetPoints(Vector3[] controlPoints, Material material, Color color)
		{
			PointsInit(controlPoints);
			SetObjects(material, color);
		}

		public virtual Vector3[] GetPoints(Vector3[] controlPoints)
		{
			PointsInit(controlPoints);
			Vector3[] array = new Vector3[frequency];
			float num = frequency;
			num = ((!Loop && num != 1f) ? (1f / (num - 1f)) : (1f / num));
			for (int i = 0; i < frequency; i++)
			{
				array[i] = GetPoint((float)i * num);
			}
			return array;
		}

		public virtual void TogglePoints(bool state)
		{
			base.gameObject.SetActive(state);
			if (tracerLineRenderer != null)
			{
				tracerLineRenderer.SetActive(state);
			}
		}

		protected virtual void PointsInit(Vector3[] controlPoints)
		{
			points = controlPoints;
			modes = new BezierControlPointMode[2];
		}

		protected virtual GameObject CreateSphere()
		{
			customTracer = false;
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "Sphere");
			Object.Destroy(gameObject.GetComponent<SphereCollider>());
			gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
			gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
			return gameObject;
		}

		protected virtual Vector3 GetControlPoint(int index)
		{
			return points[index];
		}

		protected virtual void SetControlPoint(int index, Vector3 point)
		{
			if (index % 3 == 0)
			{
				Vector3 vector = point - points[index];
				if (loop)
				{
					if (index == 0)
					{
						points[1] += vector;
						points[points.Length - 2] += vector;
						points[points.Length - 1] = point;
					}
					else if (index == points.Length - 1)
					{
						points[0] = point;
						points[1] += vector;
						points[index - 1] += vector;
					}
					else
					{
						points[index - 1] += vector;
						points[index + 1] += vector;
					}
				}
				else
				{
					if (index > 0)
					{
						points[index - 1] += vector;
					}
					if (index + 1 < points.Length)
					{
						points[index + 1] += vector;
					}
				}
			}
			points[index] = point;
			EnforceMode(index);
		}

		protected virtual void EnforceMode(int index)
		{
			int num = (index + 1) / 3;
			BezierControlPointMode bezierControlPointMode = modes[num];
			if (bezierControlPointMode == BezierControlPointMode.Free || (!loop && (num == 0 || num == modes.Length - 1)))
			{
				return;
			}
			int num2 = num * 3;
			int num3;
			int num4;
			if (index <= num2)
			{
				num3 = num2 - 1;
				if (num3 < 0)
				{
					num3 = points.Length - 2;
				}
				num4 = num2 + 1;
				if (num4 >= points.Length)
				{
					num4 = 1;
				}
			}
			else
			{
				num3 = num2 + 1;
				if (num3 >= points.Length)
				{
					num3 = 1;
				}
				num4 = num2 - 1;
				if (num4 < 0)
				{
					num4 = points.Length - 2;
				}
			}
			Vector3 vector = points[num2];
			Vector3 vector2 = vector - points[num3];
			if (bezierControlPointMode == BezierControlPointMode.Aligned)
			{
				vector2 = vector2.normalized * Vector3.Distance(vector, points[num4]);
			}
			points[num4] = vector + vector2;
		}

		protected virtual Vector3 GetPoint(float t)
		{
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			return base.transform.TransformPoint(Bezier.GetPoint(points[num], points[num + 1], points[num + 2], points[num + 3], t));
		}

		protected virtual void SetObjects(Material material, Color color)
		{
			float num = frequency;
			num = ((!Loop && num != 1f) ? (1f / (num - 1f)) : (1f / num));
			SetPointData(material, color, num);
		}

		protected virtual void SetPointData(Material material, Color color, float stepSize)
		{
			for (int i = 0; i < frequency; i++)
			{
				Vector3 point = GetPoint((float)i * stepSize);
				if (customLineRenderer != null)
				{
					customLineRenderer.SetPosition(i, point);
					SetMaterial(customLineRenderer.sharedMaterial, color);
				}
				if (customLineRenderer == null || lineRendererAndItem)
				{
					SetItemPosition(i, point, material, color, stepSize);
				}
			}
		}

		protected virtual void SetItemPosition(int currentIndex, Vector3 setPosition, Material material, Color color, float stepSize)
		{
			if (customTracer && currentIndex == frequency - 1)
			{
				items[currentIndex].SetActive(false);
				return;
			}
			SetItemMaterial(items[currentIndex], material, color);
			items[currentIndex].transform.position = setPosition;
			Vector3 point = GetPoint((float)(currentIndex + 1) * stepSize);
			Vector3 vector = point - setPosition;
			Vector3 normalized = vector.normalized;
			if (normalized != Vector3.zero)
			{
				items[currentIndex].transform.rotation = Quaternion.LookRotation(normalized);
				if (rescalePointerTracer)
				{
					Vector3 localScale = items[currentIndex].transform.localScale;
					localScale.z = vector.magnitude / 2f;
					items[currentIndex].transform.localScale = localScale;
				}
			}
		}

		protected virtual void SetItemMaterial(GameObject item, Material material, Color color)
		{
			Renderer[] componentsInChildren = item.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (material != null)
				{
					renderer.material = material;
				}
				SetMaterial(renderer.material, color);
			}
		}

		protected virtual void SetMaterial(Material material, Color color)
		{
			if (material != null)
			{
				material.EnableKeyword("_EMISSION");
				if (material.HasProperty("_Color"))
				{
					material.color = color;
				}
				if (material.HasProperty("_EmissionColor"))
				{
					material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, 50f));
				}
			}
		}
	}
}
