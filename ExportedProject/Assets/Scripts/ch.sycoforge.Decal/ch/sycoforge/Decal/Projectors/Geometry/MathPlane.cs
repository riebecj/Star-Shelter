using System;
using UnityEngine;

namespace ch.sycoforge.Decal.Projectors.Geometry
{
	public struct MathPlane
	{
		private Vector3 normal;

		private Vector3 origin;

		private float distance;

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

		public Vector3 Normal
		{
			get
			{
				return normal;
			}
			set
			{
				normal = value;
			}
		}

		public Vector3 Origin
		{
			get
			{
				return origin;
			}
			set
			{
				origin = value;
			}
		}

		public MathPlane(Vector3 inNormal, Vector3 inPoint)
		{
			normal = Vector3.Normalize(inNormal);
			distance = 0f - Vector3.Dot(inNormal, inPoint);
			origin = normal * distance;
		}

		public MathPlane(Vector3 inNormal, float d)
		{
			normal = Vector3.Normalize(inNormal);
			distance = d;
			origin = normal * distance;
		}

		public MathPlane(Vector3 a, Vector3 b, Vector3 c)
		{
			normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
			distance = 0f - Vector3.Dot(normal, a);
			origin = normal * distance;
		}

		public float GetDistanceToPoint(Vector3 inPt)
		{
			return Vector3.Dot(Normal, inPt) + Distance;
		}

		public bool GetSide(Vector3 inPt)
		{
			return Vector3.Dot(Normal, inPt) + Distance > 0f;
		}

		public bool Raycast(Ray ray, out float enter)
		{
			float num = Vector3.Dot(ray.direction, Normal);
			float num2 = 0f - Vector3.Dot(ray.origin, Normal) - Distance;
			if (Mathf.Approximately(num, 0f))
			{
				enter = 0f;
				return false;
			}
			enter = num2 / num;
			return enter > 0f;
		}

		public bool SameSide(Vector3 inPt0, Vector3 inPt1)
		{
			float distanceToPoint = GetDistanceToPoint(inPt0);
			float distanceToPoint2 = GetDistanceToPoint(inPt1);
			if (distanceToPoint > 0f && distanceToPoint2 > 0f)
			{
				return true;
			}
			if (distanceToPoint <= 0f && distanceToPoint2 <= 0f)
			{
				return true;
			}
			return false;
		}

		public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
		{
			Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
			Distance = 0f - Vector3.Dot(Normal, a);
		}

		public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
		{
			Normal = Vector3.Normalize(inNormal);
			Distance = 0f - Vector3.Dot(inNormal, inPoint);
			origin = inPoint;
		}

		public bool Intersection(Ray ray, out Vector3 point)
		{
			bool result = false;
			point = Vector3.zero;
			float a = Vector3.Dot(Normal, ray.direction);
			if (!Mathf.Approximately(a, 0f))
			{
				float num = Vector3.Dot(Normal, origin - ray.origin) / Vector3.Dot(Normal, ray.direction);
				point = ray.origin + ray.direction * num;
				result = num >= 0f;
			}
			return result;
		}

		public float StretchFactor(Ray ray)
		{
			Vector3 direction = ray.direction;
			float num = Vector3.Dot(normal, direction);
			return Vector3.Dot(normal, origin - ray.origin) / Vector3.Dot(normal, direction);
		}

		public static MathPlane Transform(MathPlane plane, Transform transform)
		{
			Vector3 inNormal = transform.TransformDirection(plane.Normal);
			Vector3 inPoint = transform.TransformPoint(plane.origin);
			return new MathPlane(inNormal, inPoint);
		}

		internal static bool FloatEquals(float a, float b)
		{
			return Math.Abs(a - b) <= float.Epsilon;
		}
	}
}
