using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal.Projectors.Geometry
{
	public struct Triangle
	{
		public int TriangleIndex;

		public int IndexA;

		public int IndexB;

		public int IndexC;

		public int this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return IndexA;
				case 1:
					return IndexB;
				case 2:
					return IndexC;
				default:
					return -1;
				}
			}
		}

		public Triangle(int triangleIndex, int indexA, int indexB, int indexC)
		{
			TriangleIndex = triangleIndex;
			IndexA = indexA;
			IndexB = indexB;
			IndexC = indexC;
		}

		public bool IsValid(IList<Vector3> vertices, float value)
		{
			Vector3 vector = vertices[IndexA];
			Vector3 vector2 = vertices[IndexB];
			Vector3 vector3 = vertices[IndexC];
			float num = Vector3.Distance(vector, vector2);
			float num2 = Vector3.Distance(vector, vector3);
			float num3 = Vector3.Distance(vector2, vector3);
			Vector3 vector4 = vector2 - vector;
			Vector3 vector5 = vector3 - vector;
			Vector3 vector6 = vector - vector2;
			Vector3 vector7 = vector3 - vector2;
			Vector3 vector8 = vector - vector3;
			Vector3 vector9 = vector2 - vector3;
			float num4 = Vector3.Angle(vector4.normalized, vector5.normalized);
			float num5 = Vector3.Angle(vector6.normalized, vector7.normalized);
			float num6 = Vector3.Angle(vector8.normalized, vector9.normalized);
			if (num4 < value)
			{
				return false;
			}
			if (num5 < value)
			{
				return false;
			}
			if (num6 < value)
			{
				return false;
			}
			return true;
		}
	}
}
