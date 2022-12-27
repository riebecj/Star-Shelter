using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Phonon
{
	public class Scene
	{
		private IntPtr scene = IntPtr.Zero;

		public IntPtr GetScene()
		{
			return scene;
		}

		public Error Export(ComputeDevice computeDevice, SimulationSettings simulationSettings, PhononMaterialValue defaultMaterial, GlobalContext globalContext)
		{
			Error error = Error.None;
			PhononGeometry[] array = UnityEngine.Object.FindObjectsOfType<PhononGeometry>();
			simulationSettings.sceneType = SceneType.Phonon;
			error = PhononCore.iplCreateScene(globalContext, computeDevice.GetDevice(), simulationSettings, array.Length, ref scene);
			if (error != 0)
			{
				throw new Exception("Unable to create scene for export (" + array.Length + " materials): [" + error.ToString() + "]");
			}
			Material[] array2 = new Material[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i].absorptionHigh = defaultMaterial.HighFreqAbsorption;
				array2[i].absorptionMid = defaultMaterial.MidFreqAbsorption;
				array2[i].absorptionLow = defaultMaterial.LowFreqAbsorption;
				array2[i].scattering = defaultMaterial.Scattering;
				array[i].GetMaterial(ref array2[i]);
				PhononCore.iplSetSceneMaterial(scene, i, array2[i]);
			}
			int num = 0;
			int[] array3 = new int[array.Length];
			int num2 = 0;
			int[] array4 = new int[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array3[j] = array[j].GetNumVertices();
				num += array3[j];
				array4[j] = array[j].GetNumTriangles();
				num2 += array4[j];
			}
			IntPtr staticMesh = IntPtr.Zero;
			error = PhononCore.iplCreateStaticMesh(scene, num, num2, ref staticMesh);
			if (error != 0)
			{
				throw new Exception("Unable to create static mesh for export (" + num + " vertices, " + num2 + " triangles): [" + error.ToString() + "]");
			}
			Vector3[] vertices = new Vector3[num];
			int num3 = 0;
			Triangle[] triangles = new Triangle[num2];
			int num4 = 0;
			int[] array5 = new int[num2];
			for (int k = 0; k < array.Length; k++)
			{
				array[k].GetGeometry(vertices, num3, triangles, num4);
				for (int l = 0; l < array4[k]; l++)
				{
					array5[num4 + l] = k;
				}
				num3 += array3[k];
				num4 += array4[k];
			}
			PhononCore.iplSetStaticMeshVertices(scene, staticMesh, vertices);
			PhononCore.iplSetStaticMeshTriangles(scene, staticMesh, triangles);
			PhononCore.iplSetStaticMeshMaterials(scene, staticMesh, array5);
			PhononCore.iplFinalizeScene(scene, null);
			error = PhononCore.iplSaveFinalizedScene(scene, Common.ConvertString(SceneFileName()));
			if (error != 0)
			{
				throw new Exception("Unable to save scene to " + SceneFileName() + " [" + error.ToString() + "]");
			}
			PhononCore.iplDestroyStaticMesh(ref staticMesh);
			PhononCore.iplDestroyScene(ref scene);
			Debug.Log("Scene exported to " + SceneFileName() + ".");
			return error;
		}

		public Error DumpToObj(ComputeDevice computeDevice, SimulationSettings simulationSettings, PhononMaterialValue defaultMaterial, GlobalContext globalContext)
		{
			Error error = Error.None;
			PhononGeometry[] array = UnityEngine.Object.FindObjectsOfType<PhononGeometry>();
			simulationSettings.sceneType = SceneType.Phonon;
			error = PhononCore.iplCreateScene(globalContext, computeDevice.GetDevice(), simulationSettings, array.Length, ref scene);
			if (error != 0)
			{
				throw new Exception("Unable to create scene for export (" + array.Length + " materials): [" + error.ToString() + "]");
			}
			Material[] array2 = new Material[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i].absorptionHigh = defaultMaterial.HighFreqAbsorption;
				array2[i].absorptionMid = defaultMaterial.MidFreqAbsorption;
				array2[i].absorptionLow = defaultMaterial.LowFreqAbsorption;
				array2[i].scattering = defaultMaterial.Scattering;
				array[i].GetMaterial(ref array2[i]);
				PhononCore.iplSetSceneMaterial(scene, i, array2[i]);
			}
			int num = 0;
			int[] array3 = new int[array.Length];
			int num2 = 0;
			int[] array4 = new int[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array3[j] = array[j].GetNumVertices();
				num += array3[j];
				array4[j] = array[j].GetNumTriangles();
				num2 += array4[j];
			}
			IntPtr staticMesh = IntPtr.Zero;
			error = PhononCore.iplCreateStaticMesh(scene, num, num2, ref staticMesh);
			if (error != 0)
			{
				throw new Exception("Unable to create static mesh for export (" + num + " vertices, " + num2 + " triangles): [" + error.ToString() + "]");
			}
			Vector3[] vertices = new Vector3[num];
			int num3 = 0;
			Triangle[] triangles = new Triangle[num2];
			int num4 = 0;
			int[] array5 = new int[num2];
			for (int k = 0; k < array.Length; k++)
			{
				array[k].GetGeometry(vertices, num3, triangles, num4);
				for (int l = 0; l < array4[k]; l++)
				{
					array5[num4 + l] = k;
				}
				num3 += array3[k];
				num4 += array4[k];
			}
			PhononCore.iplSetStaticMeshVertices(scene, staticMesh, vertices);
			PhononCore.iplSetStaticMeshTriangles(scene, staticMesh, triangles);
			PhononCore.iplSetStaticMeshMaterials(scene, staticMesh, array5);
			PhononCore.iplFinalizeScene(scene, null);
			PhononCore.iplDumpSceneToObjFile(scene, Common.ConvertString(ObjFileName()));
			PhononCore.iplDestroyStaticMesh(ref staticMesh);
			PhononCore.iplDestroyScene(ref scene);
			Debug.Log("Scene dumped to " + ObjFileName() + ".");
			return error;
		}

		public Error Create(ComputeDevice computeDevice, SimulationSettings simulationSettings, GlobalContext globalContext)
		{
			string text = SceneFileName();
			if (!File.Exists(text))
			{
				return Error.Fail;
			}
			return PhononCore.iplLoadFinalizedScene(globalContext, simulationSettings, Common.ConvertString(text), computeDevice.GetDevice(), null, ref scene);
		}

		public void Destroy()
		{
			if (scene != IntPtr.Zero)
			{
				PhononCore.iplDestroyScene(ref scene);
			}
		}

		private static string SceneFileName()
		{
			string path = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().name) + ".phononscene";
			return Path.Combine(Application.streamingAssetsPath, path);
		}

		private static string ObjFileName()
		{
			string path = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().name) + ".obj";
			return Path.Combine(Application.streamingAssetsPath, path);
		}
	}
}
