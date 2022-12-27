using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using ch.sycoforge.Decal.Projectors.Geometry;

namespace ch.sycoforge.Decal.Projectors
{
	internal class BoxProjector : Projector
	{
		protected float radius;

		protected HashSet<int> processedIds = new HashSet<int>();

		protected IMeshCutter meshCutter;

		[CompilerGenerated]
		private static Predicate<Collider> CS_0024_003C_003E9__CachedAnonymousMethodDelegate1;

		internal BoxProjector(EasyDecal decal)
			: base(decal, RecreationMode.Always)
		{
			meshCutter = new MeshCutter<DynamicMesh>(decal);
		}

		protected Collider[] FindCandidates()
		{
			Vector3 halfExtents = base.Parent.transform.lossyScale * 0.5f;
			return Physics.OverlapBox(base.Parent.transform.position, halfExtents, base.Parent.transform.rotation, base.Parent.Mask.value);
		}

		internal override void Project()
		{
			if (base.Parent.Baked)
			{
				return;
			}
			processedIds.Clear();
			dynamicMesh.Clear();
			radius = Math.Max(base.Parent.transform.lossyScale.x, Math.Max(base.Parent.transform.lossyScale.y, base.Parent.transform.lossyScale.z));
			Collider[] array = FindCandidates();
			if (CS_0024_003C_003E9__CachedAnonymousMethodDelegate1 == null)
			{
				CS_0024_003C_003E9__CachedAnonymousMethodDelegate1 = _003CProject_003Eb__0;
			}
			TerrainCollider terrainCollider = Array.Find(array, CS_0024_003C_003E9__CachedAnonymousMethodDelegate1) as TerrainCollider;
			if (terrainCollider != null)
			{
				AddTerrain(terrainCollider.gameObject);
			}
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				GameObject gameObject = collider.gameObject;
				processedIds.Add(gameObject.GetInstanceID());
				ProcessReceiver(gameObject, 0, 0);
			}
			if (!base.Parent.OnlyColliders)
			{
				if (EasyDecal.ProxyCollection != null)
				{
					List<ProxyMesh> list = FindInProxies(radius);
					foreach (ProxyMesh item in list)
					{
						Mesh meshProxy = item.MeshProxy;
						AddMesh(meshProxy, item.transform);
					}
				}
				else
				{
					List<GameObject> list2 = Find(radius);
					foreach (GameObject item2 in list2)
					{
						if (!processedIds.Contains(item2.GetInstanceID()))
						{
							AddMesh(item2);
						}
					}
				}
			}
			meshCutter.CutMesh(dynamicMesh);
			base.Parent.AddDynamicMesh(dynamicMesh);
		}

		protected virtual void ProcessReceiver(GameObject receiver, int recursiveStepUp, int recursiveStepDown)
		{
			if (recursiveStepUp > base.Parent.RecursiveLookupSteps || recursiveStepDown > base.Parent.RecursiveLookupSteps)
			{
				return;
			}
			MeshFilter componentInChildren = receiver.GetComponentInChildren<MeshFilter>();
			SkinnedMeshRenderer componentInChildren2 = receiver.GetComponentInChildren<SkinnedMeshRenderer>();
			bool cullInvisibles = base.Parent.CullInvisibles;
			if (componentInChildren != null && componentInChildren.name != "PROC_PLANE")
			{
				Renderer renderer = null;
				if (cullInvisibles)
				{
					renderer = receiver.GetComponent<Renderer>();
				}
				if (!cullInvisibles || (cullInvisibles && renderer != null && renderer.enabled))
				{
					AddMesh(receiver);
				}
			}
			else if (componentInChildren2 != null)
			{
				Renderer renderer = null;
				if (cullInvisibles)
				{
					renderer = receiver.GetComponent<Renderer>();
				}
				if (!cullInvisibles || (cullInvisibles && renderer != null && renderer.enabled))
				{
					AddMesh(componentInChildren2.sharedMesh, receiver.transform);
				}
			}
			else
			{
				if (!base.Parent.RecursiveLookup)
				{
					return;
				}
				if (recursiveStepUp >= 0 && receiver.transform.parent != null && (base.Parent.RecursiveMode == LookupMode.Up || base.Parent.RecursiveMode == LookupMode.Both))
				{
					ProcessReceiver(receiver.transform.parent.gameObject, ++recursiveStepUp, -1);
				}
				if (recursiveStepDown < 0 || (base.Parent.RecursiveMode != LookupMode.Down && base.Parent.RecursiveMode != LookupMode.Both))
				{
					return;
				}
				foreach (Transform item in receiver.transform)
				{
					if (item != null)
					{
						ProcessReceiver(item.gameObject, -1, ++recursiveStepDown);
					}
				}
			}
		}

		protected List<GameObject> Find(float radius)
		{
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			List<GameObject> list = new List<GameObject>();
			radius = radius * radius * 10f;
			radius = Math.Max(radius, 5f);
			foreach (GameObject gameObject in array)
			{
				float sqrMagnitude = (base.Parent.Position - gameObject.transform.position).sqrMagnitude;
				if (sqrMagnitude <= radius && gameObject.GetInstanceID() != base.Parent.gameObject.GetInstanceID() && gameObject.name != "PROC_PLANE" && (base.Parent.Mask.value & (1 << gameObject.layer)) > 0)
				{
					MeshFilter componentInChildren = gameObject.GetComponentInChildren<MeshFilter>();
					if (componentInChildren != null)
					{
						list.Add(gameObject);
					}
				}
			}
			return list;
		}

		private List<ProxyMesh> FindInProxies(float radius)
		{
			Bounds bounds = base.Parent.GetBounds();
			List<ProxyMesh> list = new List<ProxyMesh>();
			int count = EasyDecal.ProxyCollection.MeshProxies.Count;
			for (int i = 0; i < count; i++)
			{
				ProxyMesh proxyMesh = EasyDecal.ProxyCollection.MeshProxies[i];
				bool flag = false;
				if (proxyMesh.bounds.Intersects(bounds) && (base.Parent.Mask.value & (1 << proxyMesh.transform.gameObject.layer)) > 0)
				{
					list.Add(proxyMesh);
				}
			}
			return list;
		}

		private void AddTerrain(GameObject receiver)
		{
			Terrain component = receiver.GetComponent<Terrain>();
			Matrix4x4 localToWorld = Matrix4x4.TRS(receiver.transform.position, Quaternion.identity, Vector3.one);
			dynamicMesh.Add(component, localToWorld);
		}

		private void AddMesh(GameObject receiver)
		{
			MeshFilter component = receiver.GetComponent<MeshFilter>();
			if (!(component != null))
			{
				return;
			}
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh == null)
			{
				return;
			}
			bool flag = sharedMesh != null;
			if (!sharedMesh.isReadable)
			{
				flag = false;
				MeshCollider component2 = receiver.GetComponent<MeshCollider>();
				if (component2 != null)
				{
					AddMesh(component2, receiver.transform);
				}
				else
				{
					flag = false;
					BoxCollider component3 = receiver.GetComponent<BoxCollider>();
					AddMesh(component3, receiver.transform);
				}
			}
			if (flag)
			{
				dynamicMesh.Add(sharedMesh, receiver.transform.localToWorldMatrix);
			}
		}

		private void AddMesh(Mesh mesh, Transform receiver)
		{
			if (mesh != null && receiver != null)
			{
				dynamicMesh.Add(mesh, receiver.localToWorldMatrix);
			}
		}

		private void AddMesh(MeshCollider meshCollider, Transform receiver)
		{
			if (meshCollider != null && receiver != null && meshCollider.sharedMesh != null)
			{
				dynamicMesh.Add(meshCollider.sharedMesh, receiver.localToWorldMatrix);
			}
		}

		private void AddMesh(BoxCollider boxCollider, Transform receiver)
		{
			if (boxCollider != null && receiver != null)
			{
				Mesh mesh = MeshUtil.MeshFromBoxCollider24(boxCollider);
				dynamicMesh.Add(mesh, receiver.localToWorldMatrix);
			}
		}

		protected Mesh PrepareCut(Mesh input, Transform receiver)
		{
			Mesh mesh = new Mesh();
			List<int> list = new List<int>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector3> list3 = new List<Vector3>();
			List<Vector2> list4 = new List<Vector2>();
			List<Vector4> list5 = new List<Vector4>();
			Vector3 localCenter = receiver.InverseTransformPoint(base.Parent.transform.position);
			Vector3[] vertices = input.vertices;
			Vector3[] normals = input.normals;
			Vector2[] uv = input.uv;
			Vector4[] tangents = input.tangents;
			int[] triangles = input.triangles;
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int num = triangles[i];
				int num2 = triangles[i + 1];
				int num3 = triangles[i + 2];
				Vector3[] array = new Vector3[3]
				{
					vertices[num],
					vertices[num2],
					vertices[num3]
				};
				Vector2[] array2 = new Vector2[3]
				{
					uv[num],
					uv[num2],
					uv[num3]
				};
				for (int j = 0; j < 3; j++)
				{
					Vector3 p = array[j];
					if (InsideSphere(p, localCenter))
					{
						int[] array3 = new int[3] { num, num2, num3 };
						int count = list2.Count;
						for (int k = 0; k < 3; k++)
						{
							Vector3 item = vertices[array3[k]];
							Vector3 item2 = normals[array3[k]];
							Vector2 item3 = uv[array3[k]];
							Vector4 item4 = tangents[array3[k]];
							list.Add(count + k);
							list2.Add(item);
							list3.Add(item2);
							list4.Add(item3);
							list5.Add(item4);
						}
						break;
					}
				}
			}
			mesh.vertices = list2.ToArray();
			mesh.uv = list4.ToArray();
			mesh.normals = list3.ToArray();
			mesh.triangles = list.ToArray();
			mesh.tangents = list5.ToArray();
			return mesh;
		}

		protected bool InsideSphere(Vector3 p, Vector3 localCenter)
		{
			float num = localCenter.x - p.x;
			float num2 = localCenter.y - p.y;
			float num3 = localCenter.z - p.z;
			return num * num + num2 * num2 + num3 * num3 <= radius * radius;
		}

		internal override void Dispose()
		{
		}

		internal override void DrawGizmos(bool selected)
		{
			base.DrawGizmos(selected);
			GizmoHelper.DrawBox(base.Parent.CachedTransform, GizmoHelper.ColorFromHex("00baff"), selected);
		}

		[CompilerGenerated]
		private static bool _003CProject_003Eb__0(Collider c)
		{
			return c is TerrainCollider;
		}
	}
}
