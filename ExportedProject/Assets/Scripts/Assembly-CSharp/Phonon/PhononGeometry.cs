using UnityEngine;

namespace Phonon
{
	[AddComponentMenu("Phonon/Phonon Geometry")]
	public class PhononGeometry : MonoBehaviour
	{
		[Range(0f, 10f)]
		public int TerrainSimplificationLevel;

		public bool exportAllChildren;

		public int GetNumVertices()
		{
			int num = GetNumVerticesForGameObject(base.gameObject);
			if (exportAllChildren)
			{
				MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
				MeshFilter[] array = componentsInChildren;
				foreach (MeshFilter meshFilter in array)
				{
					if (meshFilter.gameObject.GetComponent<PhononGeometry>() == null)
					{
						num += GetNumVerticesForMesh(meshFilter);
					}
				}
				Terrain[] componentsInChildren2 = GetComponentsInChildren<Terrain>();
				Terrain[] array2 = componentsInChildren2;
				foreach (Terrain terrain in array2)
				{
					if (terrain.gameObject.GetComponent<PhononGeometry>() == null)
					{
						num += GetNumVerticesForTerrain(terrain);
					}
				}
			}
			return num;
		}

		public int GetNumTriangles()
		{
			int num = GetNumTrianglesForGameObject(base.gameObject);
			if (exportAllChildren)
			{
				MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
				MeshFilter[] array = componentsInChildren;
				foreach (MeshFilter meshFilter in array)
				{
					if (meshFilter.gameObject.GetComponent<PhononGeometry>() == null)
					{
						num += GetNumTrianglesForMesh(meshFilter);
					}
				}
				Terrain[] componentsInChildren2 = GetComponentsInChildren<Terrain>();
				Terrain[] array2 = componentsInChildren2;
				foreach (Terrain terrain in array2)
				{
					if (terrain.gameObject.GetComponent<PhononGeometry>() == null)
					{
						num += GetNumTrianglesForTerrain(terrain);
					}
				}
			}
			return num;
		}

		public void GetGeometry(Vector3[] vertices, int vertexOffset, Triangle[] triangles, int triangleOffset)
		{
			int num = 0;
			int num2 = 0;
			int verticesForGameObject = GetVerticesForGameObject(base.gameObject, vertices, vertexOffset, num);
			int trianglesForGameObject = GetTrianglesForGameObject(base.gameObject, triangles, triangleOffset, num2);
			FixupTriangleIndices(triangles, triangleOffset, num2, trianglesForGameObject, vertexOffset + num);
			num += verticesForGameObject;
			num2 += trianglesForGameObject;
			if (!exportAllChildren)
			{
				return;
			}
			MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
			MeshFilter[] array = componentsInChildren;
			foreach (MeshFilter meshFilter in array)
			{
				if (meshFilter.gameObject.GetComponent<PhononGeometry>() == null)
				{
					verticesForGameObject = GetVerticesForMesh(meshFilter, vertices, vertexOffset, num);
					trianglesForGameObject = GetTrianglesForMesh(meshFilter, triangles, triangleOffset, num2);
					FixupTriangleIndices(triangles, triangleOffset, num2, trianglesForGameObject, vertexOffset + num);
					num += verticesForGameObject;
					num2 += trianglesForGameObject;
				}
			}
			Terrain[] componentsInChildren2 = GetComponentsInChildren<Terrain>();
			Terrain[] array2 = componentsInChildren2;
			foreach (Terrain terrain in array2)
			{
				if (terrain.gameObject.GetComponent<PhononGeometry>() == null)
				{
					verticesForGameObject = GetVerticesForTerrain(terrain, vertices, vertexOffset, num);
					trianglesForGameObject = GetTrianglesForTerrain(terrain, triangles, triangleOffset, num2);
					FixupTriangleIndices(triangles, triangleOffset, num2, trianglesForGameObject, vertexOffset + num);
					num += verticesForGameObject;
					num2 += trianglesForGameObject;
				}
			}
		}

		public void GetMaterial(ref Material material)
		{
			PhononMaterial component = GetComponent<PhononMaterial>();
			if (!(component == null))
			{
				material.absorptionLow = component.Value.LowFreqAbsorption;
				material.absorptionMid = component.Value.MidFreqAbsorption;
				material.absorptionHigh = component.Value.HighFreqAbsorption;
				material.scattering = component.Value.Scattering;
			}
		}

		private int GetNumVerticesForMesh(MeshFilter mesh)
		{
			return mesh.sharedMesh.vertexCount;
		}

		private int GetVerticesForMesh(MeshFilter mesh, Vector3[] vertices, int offset, int localOffset)
		{
			UnityEngine.Vector3[] vertices2 = mesh.sharedMesh.vertices;
			for (int i = 0; i < vertices2.Length; i++)
			{
				vertices[offset + localOffset + i] = Common.ConvertVector(mesh.transform.TransformPoint(vertices2[i]));
			}
			return vertices2.Length;
		}

		private int GetNumVerticesForTerrain(Terrain terrain)
		{
			int heightmapWidth = terrain.terrainData.heightmapWidth;
			int heightmapHeight = terrain.terrainData.heightmapHeight;
			int num = Mathf.Min(heightmapWidth - 1, Mathf.Min(heightmapHeight - 1, (int)Mathf.Pow(2f, TerrainSimplificationLevel)));
			if (num == 0)
			{
				num = 1;
			}
			heightmapWidth = (heightmapWidth - 1) / num + 1;
			heightmapHeight = (heightmapHeight - 1) / num + 1;
			return heightmapWidth * heightmapHeight;
		}

		private int GetVerticesForTerrain(Terrain terrain, Vector3[] vertices, int offset, int localOffset)
		{
			int heightmapWidth = terrain.terrainData.heightmapWidth;
			int heightmapHeight = terrain.terrainData.heightmapHeight;
			int num = Mathf.Min(heightmapWidth - 1, Mathf.Min(heightmapHeight - 1, (int)Mathf.Pow(2f, TerrainSimplificationLevel)));
			if (num == 0)
			{
				num = 1;
			}
			heightmapWidth = (heightmapWidth - 1) / num + 1;
			heightmapHeight = (heightmapHeight - 1) / num + 1;
			UnityEngine.Vector3 position = terrain.transform.position;
			float[,] heights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
			int result = 0;
			for (int i = 0; i < terrain.terrainData.heightmapHeight; i += num)
			{
				for (int j = 0; j < terrain.terrainData.heightmapWidth; j += num)
				{
					float num2 = heights[i, j];
					float x = position.x + (float)j / (float)terrain.terrainData.heightmapWidth * terrain.terrainData.size.x;
					float y = position.y + num2 * terrain.terrainData.size.y;
					float z = position.z + (float)i / (float)terrain.terrainData.heightmapHeight * terrain.terrainData.size.z;
					vertices[offset + localOffset + result++] = Common.ConvertVector(new UnityEngine.Vector3
					{
						x = x,
						y = y,
						z = z
					});
				}
			}
			return result;
		}

		private int GetNumVerticesForGameObject(GameObject obj)
		{
			MeshFilter component = obj.GetComponent<MeshFilter>();
			Terrain component2 = obj.GetComponent<Terrain>();
			if (component != null)
			{
				return GetNumVerticesForMesh(component);
			}
			if (component2 != null)
			{
				return GetNumVerticesForTerrain(component2);
			}
			return 0;
		}

		private int GetVerticesForGameObject(GameObject obj, Vector3[] vertices, int offset, int localOffset)
		{
			MeshFilter component = obj.GetComponent<MeshFilter>();
			Terrain component2 = obj.GetComponent<Terrain>();
			if (component != null)
			{
				return GetVerticesForMesh(component, vertices, offset, localOffset);
			}
			if (component2 != null)
			{
				return GetVerticesForTerrain(component2, vertices, offset, localOffset);
			}
			return 0;
		}

		private int GetNumTrianglesForMesh(MeshFilter mesh)
		{
			return mesh.sharedMesh.triangles.Length / 3;
		}

		private int GetTrianglesForMesh(MeshFilter mesh, Triangle[] triangles, int offset, int localOffset)
		{
			int[] triangles2 = mesh.sharedMesh.triangles;
			for (int i = 0; i < triangles2.Length / 3; i++)
			{
				triangles[offset + localOffset + i].index0 = triangles2[3 * i];
				triangles[offset + localOffset + i].index1 = triangles2[3 * i + 1];
				triangles[offset + localOffset + i].index2 = triangles2[3 * i + 2];
			}
			return triangles2.Length / 3;
		}

		private int GetNumTrianglesForTerrain(Terrain terrain)
		{
			int heightmapWidth = terrain.terrainData.heightmapWidth;
			int heightmapHeight = terrain.terrainData.heightmapHeight;
			int num = Mathf.Min(heightmapWidth - 1, Mathf.Min(heightmapHeight - 1, (int)Mathf.Pow(2f, TerrainSimplificationLevel)));
			if (num == 0)
			{
				num = 1;
			}
			heightmapWidth = (heightmapWidth - 1) / num + 1;
			heightmapHeight = (heightmapHeight - 1) / num + 1;
			return (heightmapWidth - 1) * (heightmapHeight - 1) * 2;
		}

		private int GetTrianglesForTerrain(Terrain terrain, Triangle[] triangles, int offset, int localOffset)
		{
			int heightmapWidth = terrain.terrainData.heightmapWidth;
			int heightmapHeight = terrain.terrainData.heightmapHeight;
			int num = Mathf.Min(heightmapWidth - 1, Mathf.Min(heightmapHeight - 1, (int)Mathf.Pow(2f, TerrainSimplificationLevel)));
			if (num == 0)
			{
				num = 1;
			}
			heightmapWidth = (heightmapWidth - 1) / num + 1;
			heightmapHeight = (heightmapHeight - 1) / num + 1;
			int result = 0;
			for (int i = 0; i < heightmapHeight - 1; i++)
			{
				for (int j = 0; j < heightmapWidth - 1; j++)
				{
					int index = i * heightmapWidth + j;
					int index2 = (i + 1) * heightmapWidth + j;
					int index3 = i * heightmapWidth + (j + 1);
					triangles[offset + localOffset + result++] = new Triangle
					{
						index0 = index,
						index1 = index2,
						index2 = index3
					};
					index = i * heightmapWidth + (j + 1);
					index2 = (i + 1) * heightmapWidth + j;
					index3 = (i + 1) * heightmapWidth + (j + 1);
					triangles[offset + localOffset + result++] = new Triangle
					{
						index0 = index,
						index1 = index2,
						index2 = index3
					};
				}
			}
			return result;
		}

		private int GetNumTrianglesForGameObject(GameObject obj)
		{
			MeshFilter component = obj.GetComponent<MeshFilter>();
			Terrain component2 = obj.GetComponent<Terrain>();
			if (component != null)
			{
				return GetNumTrianglesForMesh(component);
			}
			if (component2 != null)
			{
				return GetNumTrianglesForTerrain(component2);
			}
			return 0;
		}

		private int GetTrianglesForGameObject(GameObject obj, Triangle[] triangles, int offset, int localOffset)
		{
			MeshFilter component = obj.GetComponent<MeshFilter>();
			Terrain component2 = obj.GetComponent<Terrain>();
			if (component != null)
			{
				return GetTrianglesForMesh(component, triangles, offset, localOffset);
			}
			if (component2 != null)
			{
				return GetTrianglesForTerrain(component2, triangles, offset, localOffset);
			}
			return 0;
		}

		private void FixupTriangleIndices(Triangle[] triangles, int offset, int localOffset, int numTriangles, int indexOffset)
		{
			for (int i = 0; i < numTriangles; i++)
			{
				triangles[offset + localOffset + i].index0 += indexOffset;
				triangles[offset + localOffset + i].index1 += indexOffset;
				triangles[offset + localOffset + i].index2 += indexOffset;
			}
		}
	}
}
