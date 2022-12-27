using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
	[ExecuteInEditMode]
	public class BezierSpline : MonoBehaviour
	{
		private static Material gizmoMaterial;

		private Color gizmoColor = Color.white;

		private float gizmoStep = 0.05f;

		private List<BezierPoint> endPoints = new List<BezierPoint>();

		public bool loop;

		public bool drawGizmos;

		public int Count
		{
			get
			{
				return endPoints.Count;
			}
		}

		public float Length
		{
			get
			{
				return GetLengthApproximately(0f, 1f);
			}
		}

		public BezierPoint this[int index]
		{
			get
			{
				if (index < Count)
				{
					return endPoints[index];
				}
				Debug.LogError("Bezier index " + index + " is out of range: " + Count);
				return null;
			}
		}

		private void Awake()
		{
			Refresh();
		}

		public void Initialize(int endPointsCount)
		{
			if (endPointsCount < 2)
			{
				Debug.LogError("Can't initialize spline with " + endPointsCount + " point(s). At least 2 points are needed");
				return;
			}
			Refresh();
			for (int num = endPoints.Count - 1; num >= 0; num--)
			{
				Object.DestroyImmediate(endPoints[num].gameObject);
			}
			endPoints.Clear();
			for (int i = 0; i < endPointsCount; i++)
			{
				InsertNewPointAt(i);
			}
			Refresh();
		}

		public void Refresh()
		{
			endPoints.Clear();
			GetComponentsInChildren(endPoints);
		}

		public BezierPoint InsertNewPointAt(int index)
		{
			if (index < 0 || index > endPoints.Count)
			{
				Debug.LogError("Index " + index + " is out of range: [0," + endPoints.Count + "]");
				return null;
			}
			int count = endPoints.Count;
			BezierPoint bezierPoint = new GameObject("Point").AddComponent<BezierPoint>();
			bezierPoint.transform.SetParent((endPoints.Count == 0) ? base.transform : ((index != 0) ? endPoints[index - 1].transform.parent : endPoints[0].transform.parent), false);
			bezierPoint.transform.SetSiblingIndex((index != 0) ? (endPoints[index - 1].transform.GetSiblingIndex() + 1) : 0);
			if (endPoints.Count == count)
			{
				endPoints.Insert(index, bezierPoint);
			}
			return bezierPoint;
		}

		public BezierPoint DuplicatePointAt(int index)
		{
			if (index < 0 || index >= endPoints.Count)
			{
				Debug.LogError("Index " + index + " is out of range: [0," + (endPoints.Count - 1) + "]");
				return null;
			}
			BezierPoint bezierPoint = InsertNewPointAt(index + 1);
			endPoints[index].CopyTo(bezierPoint);
			return bezierPoint;
		}

		public void RemovePointAt(int index)
		{
			if (endPoints.Count <= 2)
			{
				Debug.LogError("Can't remove point: spline must consist of at least two points!");
			}
			else if (index < 0 || index >= endPoints.Count)
			{
				Debug.LogError("Index " + index + " is out of range: [0," + endPoints.Count + ")");
			}
			else
			{
				BezierPoint bezierPoint = endPoints[index];
				endPoints.RemoveAt(index);
				Object.DestroyImmediate(bezierPoint.gameObject);
			}
		}

		public void SwapPointsAt(int index1, int index2)
		{
			if (index1 == index2)
			{
				Debug.LogError("Indices can't be equal to each other");
				return;
			}
			if (index1 < 0 || index1 >= endPoints.Count || index2 < 0 || index2 >= endPoints.Count)
			{
				Debug.LogError("Indices must be in range [0," + (endPoints.Count - 1) + "]");
				return;
			}
			BezierPoint bezierPoint = endPoints[index1];
			int siblingIndex = bezierPoint.transform.GetSiblingIndex();
			endPoints[index1] = endPoints[index2];
			endPoints[index2] = bezierPoint;
			bezierPoint.transform.SetSiblingIndex(endPoints[index1].transform.GetSiblingIndex());
			endPoints[index1].transform.SetSiblingIndex(siblingIndex);
		}

		public int IndexOf(BezierPoint point)
		{
			return endPoints.IndexOf(point);
		}

		public void DrawGizmos(Color color, int smoothness = 4)
		{
			drawGizmos = true;
			gizmoColor = color;
			gizmoStep = 1f / (float)(endPoints.Count * Mathf.Clamp(smoothness, 1, 30));
		}

		public void HideGizmos()
		{
			drawGizmos = false;
		}

		public Vector3 GetPoint(float normalizedT)
		{
			if (!loop)
			{
				if (normalizedT <= 0f)
				{
					return endPoints[0].position;
				}
				if (normalizedT >= 1f)
				{
					return endPoints[endPoints.Count - 1].position;
				}
			}
			else if (normalizedT < 0f)
			{
				normalizedT += 1f;
			}
			else if (normalizedT >= 1f)
			{
				normalizedT -= 1f;
			}
			float num = normalizedT * (float)((!loop) ? (endPoints.Count - 1) : endPoints.Count);
			int num2 = (int)num;
			int num3 = num2 + 1;
			if (num3 == endPoints.Count)
			{
				num3 = 0;
			}
			BezierPoint bezierPoint = endPoints[num2];
			BezierPoint bezierPoint2 = endPoints[num3];
			float num4 = num - (float)num2;
			float num5 = 1f - num4;
			return num5 * num5 * num5 * bezierPoint.position + 3f * num5 * num5 * num4 * bezierPoint.followingControlPointPosition + 3f * num5 * num4 * num4 * bezierPoint2.precedingControlPointPosition + num4 * num4 * num4 * bezierPoint2.position;
		}

		public Vector3 GetTangent(float normalizedT)
		{
			if (!loop)
			{
				if (normalizedT <= 0f)
				{
					return 3f * (endPoints[0].followingControlPointPosition - endPoints[0].position);
				}
				if (normalizedT >= 1f)
				{
					int index = endPoints.Count - 1;
					return 3f * (endPoints[index].position - endPoints[index].precedingControlPointPosition);
				}
			}
			else if (normalizedT < 0f)
			{
				normalizedT += 1f;
			}
			else if (normalizedT >= 1f)
			{
				normalizedT -= 1f;
			}
			float num = normalizedT * (float)((!loop) ? (endPoints.Count - 1) : endPoints.Count);
			int num2 = (int)num;
			int num3 = num2 + 1;
			if (num3 == endPoints.Count)
			{
				num3 = 0;
			}
			BezierPoint bezierPoint = endPoints[num2];
			BezierPoint bezierPoint2 = endPoints[num3];
			float num4 = num - (float)num2;
			float num5 = 1f - num4;
			return 3f * num5 * num5 * (bezierPoint.followingControlPointPosition - bezierPoint.position) + 6f * num5 * num4 * (bezierPoint2.precedingControlPointPosition - bezierPoint.followingControlPointPosition) + 3f * num4 * num4 * (bezierPoint2.position - bezierPoint2.precedingControlPointPosition);
		}

		public float GetLengthApproximately(float startNormalizedT, float endNormalizedT, float accuracy = 50f)
		{
			if (endNormalizedT < startNormalizedT)
			{
				float num = startNormalizedT;
				startNormalizedT = endNormalizedT;
				endNormalizedT = num;
			}
			if (startNormalizedT < 0f)
			{
				startNormalizedT = 0f;
			}
			if (endNormalizedT > 1f)
			{
				endNormalizedT = 1f;
			}
			float num2 = AccuracyToStepSize(accuracy) * (endNormalizedT - startNormalizedT);
			float num3 = 0f;
			Vector3 vector = GetPoint(startNormalizedT);
			for (float num4 = startNormalizedT + num2; num4 < endNormalizedT; num4 += num2)
			{
				Vector3 point = GetPoint(num4);
				num3 += Vector3.Distance(point, vector);
				vector = point;
			}
			return num3 + Vector3.Distance(vector, GetPoint(endNormalizedT));
		}

		public Vector3 FindNearestPointTo(Vector3 worldPos, float accuracy = 100f)
		{
			float normalizedT;
			return FindNearestPointTo(worldPos, out normalizedT, accuracy);
		}

		public Vector3 FindNearestPointTo(Vector3 worldPos, out float normalizedT, float accuracy = 100f)
		{
			Vector3 result = Vector3.zero;
			normalizedT = -1f;
			float num = AccuracyToStepSize(accuracy);
			float num2 = float.PositiveInfinity;
			for (float num3 = 0f; num3 < 1f; num3 += num)
			{
				Vector3 point = GetPoint(num3);
				float sqrMagnitude = (worldPos - point).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					result = point;
					normalizedT = num3;
				}
			}
			return result;
		}

		public Vector3 MoveAlongSpline(ref float normalizedT, float deltaMovement, int accuracy = 3)
		{
			float num = deltaMovement / (float)(((!loop) ? (endPoints.Count - 1) : endPoints.Count) * accuracy);
			for (int i = 0; i < accuracy; i++)
			{
				normalizedT += num / GetTangent(normalizedT).magnitude;
			}
			return GetPoint(normalizedT);
		}

		public void ConstructLinearPath()
		{
			for (int i = 0; i < endPoints.Count; i++)
			{
				endPoints[i].handleMode = BezierPoint.HandleMode.Free;
				if (i < endPoints.Count - 1)
				{
					Vector3 vector = (endPoints[i].position + endPoints[i + 1].position) * 0.5f;
					endPoints[i].followingControlPointPosition = vector;
					endPoints[i + 1].precedingControlPointPosition = vector;
				}
				else
				{
					Vector3 vector2 = (endPoints[i].position + endPoints[0].position) * 0.5f;
					endPoints[i].followingControlPointPosition = vector2;
					endPoints[0].precedingControlPointPosition = vector2;
				}
			}
		}

		public void AutoConstructSpline()
		{
			for (int i = 0; i < endPoints.Count; i++)
			{
				endPoints[i].handleMode = BezierPoint.HandleMode.Mirrored;
			}
			int num = endPoints.Count - 1;
			if (num == 1)
			{
				endPoints[0].followingControlPointPosition = (2f * endPoints[0].position + endPoints[1].position) / 3f;
				endPoints[1].precedingControlPointPosition = 2f * endPoints[0].followingControlPointPosition - endPoints[0].position;
				return;
			}
			Vector3[] array = ((!loop) ? new Vector3[num] : new Vector3[num + 1]);
			for (int j = 1; j < num - 1; j++)
			{
				array[j] = 4f * endPoints[j].position + 2f * endPoints[j + 1].position;
			}
			array[0] = endPoints[0].position + 2f * endPoints[1].position;
			if (!loop)
			{
				array[num - 1] = (8f * endPoints[num - 1].position + endPoints[num].position) * 0.5f;
			}
			else
			{
				array[num - 1] = 4f * endPoints[num - 1].position + 2f * endPoints[num].position;
				array[num] = (8f * endPoints[num].position + endPoints[0].position) * 0.5f;
			}
			Vector3[] firstControlPoints = GetFirstControlPoints(array);
			for (int k = 0; k < num; k++)
			{
				endPoints[k].followingControlPointPosition = firstControlPoints[k];
				if (loop)
				{
					endPoints[k + 1].precedingControlPointPosition = 2f * endPoints[k + 1].position - firstControlPoints[k + 1];
				}
				else if (k < num - 1)
				{
					endPoints[k + 1].precedingControlPointPosition = 2f * endPoints[k + 1].position - firstControlPoints[k + 1];
				}
				else
				{
					endPoints[k + 1].precedingControlPointPosition = (endPoints[num].position + firstControlPoints[num - 1]) * 0.5f;
				}
			}
			if (loop)
			{
				float num2 = Vector3.Distance(endPoints[0].followingControlPointPosition, endPoints[0].position);
				Vector3 vector = Vector3.Normalize(endPoints[num].position - endPoints[1].position);
				endPoints[0].precedingControlPointPosition = endPoints[0].position + vector * num2;
				endPoints[0].followingControlPointLocalPosition = -endPoints[0].precedingControlPointLocalPosition;
			}
		}

		private static Vector3[] GetFirstControlPoints(Vector3[] rhs)
		{
			int num = rhs.Length;
			Vector3[] array = new Vector3[num];
			float[] array2 = new float[num];
			float num2 = 2f;
			array[0] = rhs[0] / num2;
			for (int i = 1; i < num; i++)
			{
				float num3 = (array2[i] = 1f / num2);
				num2 = ((i >= num - 1) ? 3.5f : 4f) - num3;
				array[i] = (rhs[i] - array[i - 1]) / num2;
			}
			for (int j = 1; j < num; j++)
			{
				array[num - j - 1] -= array2[num - j] * array[num - j];
			}
			return array;
		}

		public void AutoConstructSpline2()
		{
			for (int i = 0; i < endPoints.Count; i++)
			{
				Vector3 vector = ((i != 0) ? endPoints[i - 1].position : ((!loop) ? endPoints[0].position : endPoints[endPoints.Count - 1].position));
				Vector3 position;
				Vector3 position2;
				if (loop)
				{
					position = endPoints[(i + 1) % endPoints.Count].position;
					position2 = endPoints[(i + 2) % endPoints.Count].position;
				}
				else if (i < endPoints.Count - 2)
				{
					position = endPoints[i + 1].position;
					position2 = endPoints[i + 2].position;
				}
				else if (i == endPoints.Count - 2)
				{
					position = endPoints[i + 1].position;
					position2 = endPoints[i + 1].position;
				}
				else
				{
					position = endPoints[i].position;
					position2 = endPoints[i].position;
				}
				endPoints[i].followingControlPointPosition = endPoints[i].position + (position - vector) / 6f;
				endPoints[i].handleMode = BezierPoint.HandleMode.Mirrored;
				if (i < endPoints.Count - 1)
				{
					endPoints[i + 1].precedingControlPointPosition = position - (position2 - endPoints[i].position) / 6f;
				}
				else if (loop)
				{
					endPoints[0].precedingControlPointPosition = position - (position2 - endPoints[i].position) / 6f;
				}
			}
		}

		private float AccuracyToStepSize(float accuracy)
		{
			if (accuracy <= 0f)
			{
				return 0.2f;
			}
			return Mathf.Clamp(1f / accuracy, 0.001f, 0.2f);
		}

		private void OnRenderObject()
		{
			if (drawGizmos && endPoints.Count >= 2)
			{
				if (!gizmoMaterial)
				{
					Shader shader = Shader.Find("Hidden/Internal-Colored");
					Material material = new Material(shader);
					material.hideFlags = HideFlags.HideAndDontSave;
					gizmoMaterial = material;
					gizmoMaterial.SetInt("_SrcBlend", 5);
					gizmoMaterial.SetInt("_DstBlend", 10);
					gizmoMaterial.SetInt("_Cull", 0);
					gizmoMaterial.SetInt("_ZWrite", 0);
				}
				gizmoMaterial.SetPass(0);
				GL.Begin(1);
				GL.Color(gizmoColor);
				Vector3 vector = endPoints[0].position;
				for (float num = gizmoStep; num < 1f; num += gizmoStep)
				{
					GL.Vertex3(vector.x, vector.y, vector.z);
					vector = GetPoint(num);
					GL.Vertex3(vector.x, vector.y, vector.z);
				}
				GL.Vertex3(vector.x, vector.y, vector.z);
				vector = GetPoint(1f);
				GL.Vertex3(vector.x, vector.y, vector.z);
				GL.End();
			}
		}
	}
}
