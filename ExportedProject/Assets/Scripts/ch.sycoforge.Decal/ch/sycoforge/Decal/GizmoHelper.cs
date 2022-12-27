using System.Globalization;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	internal static class GizmoHelper
	{
		private static Vector3[] unitCubeVertices;

		public static Vector3[] UnitCubeVertices
		{
			get
			{
				if (unitCubeVertices == null)
				{
					unitCubeVertices = GetCubicVertices();
				}
				return unitCubeVertices;
			}
		}

		public static Color ColorFromHex(string hex)
		{
			int num = int.Parse(hex.Replace("#", string.Empty), NumberStyles.HexNumber);
			float r = (float)((num & 0xFF0000) >> 16) / 255f;
			float g = (float)((num & 0xFF00) >> 8) / 255f;
			float b = (float)(num & 0xFF) / 255f;
			return new Color(r, g, b);
		}

		public static void DrawBox(Transform transform, Color color, bool selected)
		{
			Color color2 = color;
			color2.a = (selected ? 0.05f : 0.025f);
			Gizmos.color = color2;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Color color3 = color;
			color3.a = (selected ? 0.8f : 0.5f);
			Gizmos.color = color3;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}

		public static void DrawWireBox(Transform transform, Color color)
		{
			Vector3 position = UnitCubeVertices[0];
			Vector3 position2 = UnitCubeVertices[1];
			Vector3 position3 = UnitCubeVertices[2];
			Vector3 position4 = UnitCubeVertices[3];
			Vector3 position5 = UnitCubeVertices[4];
			Vector3 position6 = UnitCubeVertices[5];
			Vector3 position7 = UnitCubeVertices[6];
			Vector3 position8 = UnitCubeVertices[7];
			position = transform.TransformPoint(position);
			position2 = transform.TransformPoint(position2);
			position3 = transform.TransformPoint(position3);
			position4 = transform.TransformPoint(position4);
			position5 = transform.TransformPoint(position5);
			position6 = transform.TransformPoint(position6);
			position7 = transform.TransformPoint(position7);
			position8 = transform.TransformPoint(position8);
			Gizmos.color = color;
			Gizmos.DrawLine(position, position2);
			Gizmos.DrawLine(position2, position4);
			Gizmos.DrawLine(position4, position3);
			Gizmos.DrawLine(position3, position);
			Gizmos.DrawLine(position5, position6);
			Gizmos.DrawLine(position6, position8);
			Gizmos.DrawLine(position8, position7);
			Gizmos.DrawLine(position7, position5);
			Gizmos.DrawLine(position, position5);
			Gizmos.DrawLine(position2, position6);
			Gizmos.DrawLine(position4, position8);
			Gizmos.DrawLine(position3, position7);
		}

		private static Vector3[] GetCubicVertices()
		{
			Vector3 zero = Vector3.zero;
			Vector3 vector = Vector3.one * 0.5f;
			Vector3 vector2 = new Vector3(zero.x - vector.x, zero.y + vector.y, zero.z - vector.z);
			Vector3 vector3 = new Vector3(zero.x + vector.x, zero.y + vector.y, zero.z - vector.z);
			Vector3 vector4 = new Vector3(zero.x - vector.x, zero.y - vector.y, zero.z - vector.z);
			Vector3 vector5 = new Vector3(zero.x + vector.x, zero.y - vector.y, zero.z - vector.z);
			Vector3 vector6 = new Vector3(zero.x - vector.x, zero.y + vector.y, zero.z + vector.z);
			Vector3 vector7 = new Vector3(zero.x + vector.x, zero.y + vector.y, zero.z + vector.z);
			Vector3 vector8 = new Vector3(zero.x - vector.x, zero.y - vector.y, zero.z + vector.z);
			Vector3 vector9 = new Vector3(zero.x + vector.x, zero.y - vector.y, zero.z + vector.z);
			return new Vector3[8] { vector2, vector3, vector4, vector5, vector6, vector7, vector8, vector9 };
		}
	}
}
