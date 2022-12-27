using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB_Utility
	{
		public struct MeshAnalysisResult
		{
			public Rect uvRect;

			public bool hasOutOfBoundsUVs;

			public bool hasOverlappingSubmeshVerts;

			public bool hasOverlappingSubmeshTris;

			public bool hasUVs;

			public float submeshArea;
		}

		private class MB_Triangle
		{
			private int submeshIdx;

			private int[] vs = new int[3];

			public bool isSame(object obj)
			{
				MB_Triangle mB_Triangle = (MB_Triangle)obj;
				if (vs[0] == mB_Triangle.vs[0] && vs[1] == mB_Triangle.vs[1] && vs[2] == mB_Triangle.vs[2] && submeshIdx != mB_Triangle.submeshIdx)
				{
					return true;
				}
				return false;
			}

			public bool sharesVerts(MB_Triangle obj)
			{
				if ((vs[0] == obj.vs[0] || vs[0] == obj.vs[1] || vs[0] == obj.vs[2]) && submeshIdx != obj.submeshIdx)
				{
					return true;
				}
				if ((vs[1] == obj.vs[0] || vs[1] == obj.vs[1] || vs[1] == obj.vs[2]) && submeshIdx != obj.submeshIdx)
				{
					return true;
				}
				if ((vs[2] == obj.vs[0] || vs[2] == obj.vs[1] || vs[2] == obj.vs[2]) && submeshIdx != obj.submeshIdx)
				{
					return true;
				}
				return false;
			}

			public void Initialize(int[] ts, int idx, int sIdx)
			{
				vs[0] = ts[idx];
				vs[1] = ts[idx + 1];
				vs[2] = ts[idx + 2];
				submeshIdx = sIdx;
				Array.Sort(vs);
			}
		}

		public static Texture2D createTextureCopy(Texture2D source)
		{
			Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);
			texture2D.SetPixels(source.GetPixels());
			return texture2D;
		}

		public static bool ArrayBIsSubsetOfA(object[] a, object[] b)
		{
			for (int i = 0; i < b.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < a.Length; j++)
				{
					if (a[j] == b[i])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public static Material[] GetGOMaterials(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			Material[] array = null;
			Mesh mesh = null;
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				array = component.sharedMaterials;
				MeshFilter component2 = go.GetComponent<MeshFilter>();
				if (component2 == null)
				{
					throw new Exception(string.Concat("Object ", go, " has a MeshRenderer but no MeshFilter."));
				}
				mesh = component2.sharedMesh;
			}
			SkinnedMeshRenderer component3 = go.GetComponent<SkinnedMeshRenderer>();
			if (component3 != null)
			{
				array = component3.sharedMaterials;
				mesh = component3.sharedMesh;
			}
			if (array == null)
			{
				Debug.LogError("Object " + go.name + " does not have a MeshRenderer or a SkinnedMeshRenderer component");
				return new Material[0];
			}
			if (mesh == null)
			{
				Debug.LogError("Object " + go.name + " has a MeshRenderer or SkinnedMeshRenderer but no mesh.");
				return new Material[0];
			}
			if (mesh.subMeshCount < array.Length)
			{
				Debug.LogWarning(string.Concat("Object ", go, " has only ", mesh.subMeshCount, " submeshes and has ", array.Length, " materials. Extra materials do nothing."));
				Material[] array2 = new Material[mesh.subMeshCount];
				Array.Copy(array, array2, array2.Length);
				array = array2;
			}
			return array;
		}

		public static Mesh GetMesh(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component != null)
			{
				return component.sharedMesh;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				return component2.sharedMesh;
			}
			Debug.LogError("Object " + go.name + " does not have a MeshFilter or a SkinnedMeshRenderer component");
			return null;
		}

		public static void SetMesh(GameObject go, Mesh m)
		{
			if (go == null)
			{
				return;
			}
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component != null)
			{
				component.sharedMesh = m;
				return;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				component2.sharedMesh = m;
			}
		}

		public static Renderer GetRenderer(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				return component;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				return component2;
			}
			return null;
		}

		public static void DisableRendererInSource(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = false;
				return;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
		}

		public static bool hasOutOfBoundsUVs(Mesh m, ref Rect uvBounds)
		{
			MeshAnalysisResult putResultHere = default(MeshAnalysisResult);
			bool result = hasOutOfBoundsUVs(m, ref putResultHere);
			uvBounds = putResultHere.uvRect;
			return result;
		}

		public static bool hasOutOfBoundsUVs(Mesh m, ref MeshAnalysisResult putResultHere, int submeshIndex = -1, int uvChannel = 0)
		{
			if (m == null)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				return putResultHere.hasOutOfBoundsUVs;
			}
			Vector2[] uvs;
			switch (uvChannel)
			{
			case 0:
				uvs = m.uv;
				break;
			case 1:
				uvs = m.uv2;
				break;
			case 2:
				uvs = new Vector2[0];
				break;
			default:
				uvs = new Vector2[0];
				break;
			}
			return hasOutOfBoundsUVs(uvs, m, ref putResultHere, submeshIndex);
		}

		public static bool hasOutOfBoundsUVs(Vector2[] uvs, Mesh m, ref MeshAnalysisResult putResultHere, int submeshIndex = -1)
		{
			putResultHere.hasUVs = true;
			if (uvs.Length == 0)
			{
				putResultHere.hasUVs = false;
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = default(Rect);
				return putResultHere.hasOutOfBoundsUVs;
			}
			if (submeshIndex >= m.subMeshCount)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = default(Rect);
				return putResultHere.hasOutOfBoundsUVs;
			}
			float num;
			float x;
			float num2;
			float y;
			if (submeshIndex >= 0)
			{
				int[] triangles = m.GetTriangles(submeshIndex);
				if (triangles.Length == 0)
				{
					putResultHere.hasOutOfBoundsUVs = false;
					putResultHere.uvRect = default(Rect);
					return putResultHere.hasOutOfBoundsUVs;
				}
				num = (x = uvs[triangles[0]].x);
				num2 = (y = uvs[triangles[0]].y);
				foreach (int num3 in triangles)
				{
					if (uvs[num3].x < num)
					{
						num = uvs[num3].x;
					}
					if (uvs[num3].x > x)
					{
						x = uvs[num3].x;
					}
					if (uvs[num3].y < num2)
					{
						num2 = uvs[num3].y;
					}
					if (uvs[num3].y > y)
					{
						y = uvs[num3].y;
					}
				}
			}
			else
			{
				num = (x = uvs[0].x);
				num2 = (y = uvs[0].y);
				for (int j = 0; j < uvs.Length; j++)
				{
					if (uvs[j].x < num)
					{
						num = uvs[j].x;
					}
					if (uvs[j].x > x)
					{
						x = uvs[j].x;
					}
					if (uvs[j].y < num2)
					{
						num2 = uvs[j].y;
					}
					if (uvs[j].y > y)
					{
						y = uvs[j].y;
					}
				}
			}
			Rect uvRect = default(Rect);
			uvRect.x = num;
			uvRect.y = num2;
			uvRect.width = x - num;
			uvRect.height = y - num2;
			if (x > 1f || num < 0f || y > 1f || num2 < 0f)
			{
				putResultHere.hasOutOfBoundsUVs = true;
			}
			else
			{
				putResultHere.hasOutOfBoundsUVs = false;
			}
			putResultHere.uvRect = uvRect;
			return putResultHere.hasOutOfBoundsUVs;
		}

		public static void setSolidColor(Texture2D t, Color c)
		{
			Color[] pixels = t.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = c;
			}
			t.SetPixels(pixels);
			t.Apply();
		}

		public static Texture2D resampleTexture(Texture2D source, int newWidth, int newHeight)
		{
			TextureFormat format = source.format;
			if (format == TextureFormat.ARGB32 || format == TextureFormat.RGBA32 || format == TextureFormat.BGRA32 || format == TextureFormat.RGB24 || format == TextureFormat.Alpha8 || format == TextureFormat.DXT1)
			{
				Texture2D texture2D = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, true);
				float num = newWidth;
				float num2 = newHeight;
				for (int i = 0; i < newWidth; i++)
				{
					for (int j = 0; j < newHeight; j++)
					{
						float u = (float)i / num;
						float v = (float)j / num2;
						texture2D.SetPixel(i, j, source.GetPixelBilinear(u, v));
					}
				}
				texture2D.Apply();
				return texture2D;
			}
			Debug.LogError("Can only resize textures in formats ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT");
			return null;
		}

		public static bool AreAllSharedMaterialsDistinct(Material[] sharedMaterials)
		{
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				for (int j = i + 1; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[i] == sharedMaterials[j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int doSubmeshesShareVertsOrTris(Mesh m, ref MeshAnalysisResult mar)
		{
			MB_Triangle mB_Triangle = new MB_Triangle();
			MB_Triangle mB_Triangle2 = new MB_Triangle();
			int[][] array = new int[m.subMeshCount][];
			for (int i = 0; i < m.subMeshCount; i++)
			{
				array[i] = m.GetTriangles(i);
			}
			bool flag = false;
			bool flag2 = false;
			for (int j = 0; j < m.subMeshCount; j++)
			{
				int[] array2 = array[j];
				for (int k = j + 1; k < m.subMeshCount; k++)
				{
					int[] array3 = array[k];
					for (int l = 0; l < array2.Length; l += 3)
					{
						mB_Triangle.Initialize(array2, l, j);
						for (int n = 0; n < array3.Length; n += 3)
						{
							mB_Triangle2.Initialize(array3, n, k);
							if (mB_Triangle.isSame(mB_Triangle2))
							{
								flag2 = true;
								break;
							}
							if (mB_Triangle.sharesVerts(mB_Triangle2))
							{
								flag = true;
								break;
							}
						}
					}
				}
			}
			if (flag2)
			{
				mar.hasOverlappingSubmeshVerts = true;
				mar.hasOverlappingSubmeshTris = true;
				return 2;
			}
			if (flag)
			{
				mar.hasOverlappingSubmeshVerts = true;
				mar.hasOverlappingSubmeshTris = false;
				return 1;
			}
			mar.hasOverlappingSubmeshTris = false;
			mar.hasOverlappingSubmeshVerts = false;
			return 0;
		}

		public static bool GetBounds(GameObject go, out Bounds b)
		{
			if (go == null)
			{
				Debug.LogError("go paramater was null");
				b = new Bounds(Vector3.zero, Vector3.zero);
				return false;
			}
			Renderer renderer = GetRenderer(go);
			if (renderer == null)
			{
				Debug.LogError("GetBounds must be called on an object with a Renderer");
				b = new Bounds(Vector3.zero, Vector3.zero);
				return false;
			}
			if (renderer is MeshRenderer)
			{
				b = renderer.bounds;
				return true;
			}
			if (renderer is SkinnedMeshRenderer)
			{
				b = renderer.bounds;
				return true;
			}
			Debug.LogError("GetBounds must be called on an object with a MeshRender or a SkinnedMeshRenderer.");
			b = new Bounds(Vector3.zero, Vector3.zero);
			return false;
		}

		public static void Destroy(UnityEngine.Object o)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(o);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(o, false);
			}
		}
	}
}
