using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	public class StaticProxyCollection : MonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1
		{
			public GameObject obj;

			public bool _003CGetMesh_003Eb__0(ProxyMesh o)
			{
				return o.ID == obj.GetInstanceID();
			}
		}

		public List<GameObject> Proxies = new List<GameObject>();

		[HideInInspector]
		[SerializeField]
		public List<ProxyMesh> MeshProxies = new List<ProxyMesh>();

		public int GetStaticVertexCount()
		{
			int num = 0;
			foreach (ProxyMesh meshProxy in MeshProxies)
			{
				if (!(meshProxy.MeshProxy == null))
				{
					num += meshProxy.MeshProxy.vertexCount;
				}
			}
			return num;
		}

		public Mesh GetMesh(GameObject obj)
		{
			_003C_003Ec__DisplayClass1 _003C_003Ec__DisplayClass = new _003C_003Ec__DisplayClass1();
			_003C_003Ec__DisplayClass.obj = obj;
			ProxyMesh proxyMesh = MeshProxies.Find(_003C_003Ec__DisplayClass._003CGetMesh_003Eb__0);
			return (proxyMesh != null) ? proxyMesh.MeshProxy : null;
		}

		public void InitializeProxies()
		{
			MeshProxies = new List<ProxyMesh>();
			foreach (GameObject proxy in Proxies)
			{
				if (proxy == null)
				{
					continue;
				}
				MeshFilter component = proxy.GetComponent<MeshFilter>();
				if (component != null)
				{
					ProxyMesh proxyMesh = new ProxyMesh();
					proxyMesh.ID = proxy.GetInstanceID();
					proxyMesh.transform = proxy.transform;
					proxyMesh.MeshProxy = CloneMesh(component.sharedMesh);
					Bounds bounds = default(Bounds);
					bounds.center = proxy.transform.position;
					Vector3[] vertices = component.sharedMesh.vertices;
					foreach (Vector3 position in vertices)
					{
						bounds.Encapsulate(proxy.transform.TransformPoint(position));
					}
					proxyMesh.bounds = bounds;
					MeshProxies.Add(proxyMesh);
				}
			}
		}

		public static Mesh CloneMesh(Mesh mesh)
		{
			Mesh mesh2 = new Mesh();
			if (mesh != null)
			{
				mesh2.name = mesh.name;
				mesh2.bounds = mesh.bounds;
				mesh2.vertices = mesh.vertices;
				mesh2.triangles = mesh.triangles;
				mesh2.uv = mesh.uv;
				mesh2.colors = mesh.colors;
				mesh2.name = mesh2.name + "_" + GetMeshHash(mesh);
			}
			return mesh2;
		}

		private static int GetMeshHash(Mesh clone)
		{
			int num = 790237721;
			int[] triangles = clone.triangles;
			int[] array = triangles;
			foreach (int num2 in array)
			{
				num = num * 13 + num2.GetHashCode();
			}
			return num;
		}
	}
}
