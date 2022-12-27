using System;
using UnityEngine;

public static class OctahedronSphereCreator
{
	private static Vector3[] directions = new Vector3[4]
	{
		Vector3.left,
		Vector3.back,
		Vector3.right,
		Vector3.forward
	};

	public static Mesh Create(int subdivisions, float radius)
	{
		if (subdivisions < 0)
		{
			subdivisions = 0;
			Debug.LogWarning("Octahedron Sphere subdivisions increased to minimum, which is 0.");
		}
		else if (subdivisions > 6)
		{
			subdivisions = 6;
			Debug.LogWarning("Octahedron Sphere subdivisions decreased to maximum, which is 6.");
		}
		int num = 1 << subdivisions;
		Vector3[] array = new Vector3[(num + 1) * (num + 1) * 4 - (num * 2 - 1) * 3];
		int[] triangles = new int[(1 << subdivisions * 2 + 3) * 3];
		CreateOctahedron(array, triangles, num);
		Vector3[] normals = new Vector3[array.Length];
		Normalize(array, normals);
		Vector2[] uv = new Vector2[array.Length];
		CreateUV(array, uv);
		Vector4[] tangents = new Vector4[array.Length];
		CreateTangents(array, tangents);
		if (radius != 1f)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] *= radius;
			}
		}
		Mesh mesh = new Mesh();
		mesh.name = "Octahedron Sphere";
		mesh.vertices = array;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.tangents = tangents;
		mesh.triangles = triangles;
		return mesh;
	}

	private static void CreateOctahedron(Vector3[] vertices, int[] triangles, int resolution)
	{
		int num = 0;
		int num2 = 0;
		int t = 0;
		for (int i = 0; i < 4; i++)
		{
			vertices[num++] = Vector3.down;
		}
		for (int j = 1; j <= resolution; j++)
		{
			float t2 = (float)j / (float)resolution;
			Vector3 vector = (vertices[num++] = Vector3.Lerp(Vector3.down, Vector3.forward, t2));
			for (int k = 0; k < 4; k++)
			{
				Vector3 from = vector;
				vector = Vector3.Lerp(Vector3.down, directions[k], t2);
				t = CreateLowerStrip(j, num, num2, t, triangles);
				num = CreateVertexLine(from, vector, j, num, vertices);
				num2 += ((j <= 1) ? 1 : (j - 1));
			}
			num2 = num - 1 - j * 4;
		}
		for (int num3 = resolution - 1; num3 >= 1; num3--)
		{
			float t3 = (float)num3 / (float)resolution;
			Vector3 vector2 = (vertices[num++] = Vector3.Lerp(Vector3.up, Vector3.forward, t3));
			for (int l = 0; l < 4; l++)
			{
				Vector3 from2 = vector2;
				vector2 = Vector3.Lerp(Vector3.up, directions[l], t3);
				t = CreateUpperStrip(num3, num, num2, t, triangles);
				num = CreateVertexLine(from2, vector2, num3, num, vertices);
				num2 += num3 + 1;
			}
			num2 = num - 1 - num3 * 4;
		}
		for (int m = 0; m < 4; m++)
		{
			triangles[t++] = num2;
			triangles[t++] = num;
			num2 = (triangles[t++] = num2 + 1);
			vertices[num++] = Vector3.up;
		}
	}

	private static int CreateVertexLine(Vector3 from, Vector3 to, int steps, int v, Vector3[] vertices)
	{
		for (int i = 1; i <= steps; i++)
		{
			vertices[v++] = Vector3.Lerp(from, to, (float)i / (float)steps);
		}
		return v;
	}

	private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
	{
		for (int i = 1; i < steps; i++)
		{
			triangles[t++] = vBottom;
			triangles[t++] = vTop - 1;
			triangles[t++] = vTop;
			triangles[t++] = vBottom++;
			triangles[t++] = vTop++;
			triangles[t++] = vBottom;
		}
		triangles[t++] = vBottom;
		triangles[t++] = vTop - 1;
		triangles[t++] = vTop;
		return t;
	}

	private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
	{
		triangles[t++] = vBottom;
		triangles[t++] = vTop - 1;
		triangles[t++] = ++vBottom;
		for (int i = 1; i <= steps; i++)
		{
			triangles[t++] = vTop - 1;
			triangles[t++] = vTop;
			triangles[t++] = vBottom;
			triangles[t++] = vBottom;
			triangles[t++] = vTop++;
			triangles[t++] = ++vBottom;
		}
		return t;
	}

	private static void Normalize(Vector3[] vertices, Vector3[] normals)
	{
		for (int i = 0; i < vertices.Length; i++)
		{
			normals[i] = (vertices[i] = vertices[i].normalized);
		}
	}

	private static void CreateUV(Vector3[] vertices, Vector2[] uv)
	{
		float num = 1f;
		Vector2 vector2 = default(Vector2);
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			if (vector.x == num)
			{
				uv[i - 1].x = 1f;
			}
			num = vector.x;
			vector2.x = Mathf.Atan2(vector.x, vector.z) / ((float)Math.PI * -2f);
			if (vector2.x < 0f)
			{
				vector2.x += 1f;
			}
			vector2.y = Mathf.Asin(vector.y) / (float)Math.PI + 0.5f;
			uv[i] = vector2;
		}
		uv[vertices.Length - 4].x = (uv[0].x = 0.125f);
		uv[vertices.Length - 3].x = (uv[1].x = 0.375f);
		uv[vertices.Length - 2].x = (uv[2].x = 0.625f);
		uv[vertices.Length - 1].x = (uv[3].x = 0.875f);
	}

	private static void CreateTangents(Vector3[] vertices, Vector4[] tangents)
	{
		Vector4 vector2 = default(Vector4);
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			vector.y = 0f;
			vector = vector.normalized;
			vector2.x = 0f - vector.z;
			vector2.y = 0f;
			vector2.z = vector.x;
			vector2.w = -1f;
			tangents[i] = vector2;
		}
		tangents[vertices.Length - 4] = (tangents[0] = new Vector3(-1f, 0f, -1f).normalized);
		tangents[vertices.Length - 3] = (tangents[1] = new Vector3(1f, 0f, -1f).normalized);
		tangents[vertices.Length - 2] = (tangents[2] = new Vector3(1f, 0f, 1f).normalized);
		tangents[vertices.Length - 1] = (tangents[3] = new Vector3(-1f, 0f, 1f).normalized);
		for (int j = 0; j < 4; j++)
		{
			tangents[vertices.Length - 1 - j].w = (tangents[j].w = -1f);
		}
	}
}
