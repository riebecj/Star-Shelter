using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace ch.sycoforge.Decal
{
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class DecalRoot : DecalBase
	{
		private const string DIFFUSEBUFFER_RT0 = "_DiffuseAOBuffer";

		private const string SPECSMOOTHBUFFER_RT1 = "_SpecSmoothBuffer";

		private const string NORMALBUFFER_RT2 = "_NormalBuffer";

		private const string LIGTHINGEMISSION_RT2 = "_LightingEmissionBuffer";

		private const string SCENECAMERA = "SceneCamera";

		private const FilterMode filterMode = FilterMode.Point;

		private const string CMDBUFFER = "Easy Decals";

		private Dictionary<Camera, CommandBuffer> cameras = new Dictionary<Camera, CommandBuffer>();

		private readonly CameraEvent evt = CameraEvent.BeforeReflections;

		private int diffuseBufferID;

		private int specSmoothBufferID;

		private int normalsBufferID;

		private int ligthingEmissionBufferID;

		private Camera SceneCamera;

		[CompilerGenerated]
		private static Predicate<CommandBuffer> CS_0024_003C_003E9__CachedAnonymousMethodDelegate2;

		[CompilerGenerated]
		private static Predicate<CommandBuffer> CS_0024_003C_003E9__CachedAnonymousMethodDelegate3;

		protected override void Initialize()
		{
			base.Initialize();
			diffuseBufferID = Shader.PropertyToID("_DiffuseAOBuffer");
			specSmoothBufferID = Shader.PropertyToID("_SpecSmoothBuffer");
			normalsBufferID = Shader.PropertyToID("_NormalBuffer");
			ligthingEmissionBufferID = Shader.PropertyToID("_LightingEmissionBuffer");
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(OnRenderCamera));
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(OnRenderCamera));
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void OnDisable()
		{
			foreach (KeyValuePair<Camera, CommandBuffer> camera in cameras)
			{
				if ((bool)camera.Key)
				{
					camera.Key.RemoveCommandBuffer(evt, camera.Value);
				}
			}
		}

		private void OnRenderCamera(Camera camera)
		{
			if (Equals(null))
			{
				Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(OnRenderCamera));
			}
			else
			{
				RenderDecals(camera);
			}
		}

		private void OnRenderObject()
		{
		}

		private void OnWillRenderObject()
		{
		}

		private void RenderDecals(Camera cam)
		{
			if (base.gameObject == null || !base.gameObject.activeInHierarchy || !base.enabled)
			{
				OnDisable();
			}
			else
			{
				if (cam == null)
				{
					return;
				}
				CommandBuffer buffer = GetBuffer(cam);
				if (buffer == null)
				{
					return;
				}
				BuiltinRenderTextureType builtinRenderTextureType = (cam.allowHDR ? BuiltinRenderTextureType.CameraTarget : BuiltinRenderTextureType.GBuffer3);
				buffer.GetTemporaryRT(diffuseBufferID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
				buffer.GetTemporaryRT(specSmoothBufferID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
				buffer.GetTemporaryRT(normalsBufferID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB2101010);
				if (cam.allowHDR)
				{
					buffer.GetTemporaryRT(ligthingEmissionBufferID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB2101010);
				}
				else
				{
					buffer.GetTemporaryRT(ligthingEmissionBufferID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
				}
				List<DecalBufferGroup> decalBufferGroups = EasyDecalManager.GetDecalBufferGroups();
				bool flag = IsSceneCamera(cam);
				bool isPlaying = Application.isPlaying;
				foreach (DecalBufferGroup item in decalBufferGroups)
				{
					HashSet<EasyDecal> decals = item.Decals;
					if (decals.Count <= 0)
					{
						continue;
					}
					RenderTargetIdentifier[] targets = item.GetTargets(cam);
					if (item.PassIndex < 0)
					{
						continue;
					}
					bool flag2 = (item.Flags | DeferredFlags.Diffuse) != (DeferredFlags)0;
					bool flag3 = (item.Flags | DeferredFlags.SpecSmooth) != (DeferredFlags)0;
					bool flag4 = (item.Flags | DeferredFlags.Normal) != (DeferredFlags)0;
					foreach (EasyDecal item2 in decals)
					{
						bool flag5 = item2.IsVisible || (flag && item2.Projector.IsVisibleBy(cam));
						if (!isPlaying || (flag5 && item2.gameObject.activeInHierarchy))
						{
							if (flag2)
							{
								buffer.Blit(BuiltinRenderTextureType.GBuffer0, diffuseBufferID);
							}
							if (flag3)
							{
								buffer.Blit(BuiltinRenderTextureType.GBuffer1, specSmoothBufferID);
							}
							if (flag4)
							{
								buffer.Blit(BuiltinRenderTextureType.GBuffer2, normalsBufferID);
							}
							buffer.Blit(builtinRenderTextureType, ligthingEmissionBufferID);
							buffer.SetRenderTarget(targets, BuiltinRenderTextureType.CameraTarget);
							buffer.DrawMesh(item2.SharedMesh, item2.LocalToWorldMatrix, item2.material, 0, item.PassIndex);
						}
					}
				}
				buffer.ReleaseTemporaryRT(diffuseBufferID);
				buffer.ReleaseTemporaryRT(specSmoothBufferID);
				buffer.ReleaseTemporaryRT(normalsBufferID);
				buffer.ReleaseTemporaryRT(ligthingEmissionBufferID);
			}
		}

		private bool IsSceneCamera(Camera camera)
		{
			bool result = false;
			if (SceneCamera == null)
			{
				if (Application.isPlaying)
				{
					result = false;
				}
				else if (camera.name == "SceneCamera")
				{
					SceneCamera = camera;
					result = true;
				}
			}
			else
			{
				result = camera.GetInstanceID() == SceneCamera.GetInstanceID();
			}
			return result;
		}

		protected override void InitalizeProjector()
		{
		}

		private CommandBuffer GetBuffer(Camera cam)
		{
			CommandBuffer commandBuffer = null;
			if (cameras.ContainsKey(cam))
			{
				commandBuffer = cameras[cam];
				commandBuffer.Clear();
			}
			else
			{
				bool flag = true;
				if (Application.isEditor)
				{
					List<CommandBuffer> list = new List<CommandBuffer>(cam.GetCommandBuffers(evt));
					if (CS_0024_003C_003E9__CachedAnonymousMethodDelegate2 == null)
					{
						CS_0024_003C_003E9__CachedAnonymousMethodDelegate2 = _003CGetBuffer_003Eb__0;
					}
					if (list.Exists(CS_0024_003C_003E9__CachedAnonymousMethodDelegate2))
					{
						flag = false;
						if (CS_0024_003C_003E9__CachedAnonymousMethodDelegate3 == null)
						{
							CS_0024_003C_003E9__CachedAnonymousMethodDelegate3 = _003CGetBuffer_003Eb__1;
						}
						commandBuffer = list.Find(CS_0024_003C_003E9__CachedAnonymousMethodDelegate3);
					}
				}
				if (flag)
				{
					commandBuffer = new CommandBuffer();
					commandBuffer.name = "Easy Decals";
				}
				cameras[cam] = commandBuffer;
				cam.RemoveCommandBuffer(evt, commandBuffer);
				cam.AddCommandBuffer(evt, commandBuffer);
			}
			commandBuffer.Clear();
			return commandBuffer;
		}

		[CompilerGenerated]
		private static bool _003CGetBuffer_003Eb__0(CommandBuffer p)
		{
			return p.name == "Easy Decals";
		}

		[CompilerGenerated]
		private static bool _003CGetBuffer_003Eb__1(CommandBuffer p)
		{
			return p.name == "Easy Decals";
		}
	}
}
