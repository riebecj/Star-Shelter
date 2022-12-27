using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VR;

public class OVROverlay : MonoBehaviour
{
	public enum OverlayShape
	{
		Quad = 0,
		Cylinder = 1,
		Cubemap = 2,
		OffcenterCubemap = 4
	}

	public enum OverlayType
	{
		None = 0,
		Underlay = 1,
		Overlay = 2
	}

	private struct LayerTexture
	{
		public Texture appTexture;

		public IntPtr appTexturePtr;

		public Texture[] swapChain;

		public IntPtr[] swapChainPtr;
	}

	public OverlayType currentOverlayType = OverlayType.Overlay;

	public bool isDynamic;

	public OverlayShape currentOverlayShape;

	private OverlayShape prevOverlayShape;

	public Texture[] textures = new Texture[2];

	internal const int maxInstances = 15;

	internal static OVROverlay[] instances = new OVROverlay[15];

	private static Material premultiplyMaterial;

	private OVRPlugin.LayerLayout layout = OVRPlugin.LayerLayout.Mono;

	private LayerTexture[] layerTextures;

	private OVRPlugin.LayerDesc layerDesc;

	private int stageCount = -1;

	private int layerIndex = -1;

	private int layerId;

	private GCHandle layerIdHandle;

	private IntPtr layerIdPtr = IntPtr.Zero;

	private int frameIndex;

	private int prevFrameIndex = -1;

	private Renderer rend;

	private int texturesPerStage
	{
		get
		{
			return (layout != 0) ? 1 : 2;
		}
	}

	public void OverrideOverlayTextureInfo(Texture srcTexture, IntPtr nativePtr, VRNode node)
	{
		int num = ((node == VRNode.RightEye) ? 1 : 0);
		if (textures.Length > num)
		{
			stageCount = 3;
			CreateLayerTextures(true, true, new OVRPlugin.Sizei
			{
				w = srcTexture.width,
				h = srcTexture.height
			}, false);
			textures[num] = srcTexture;
			layerTextures[num].appTexture = srcTexture;
			layerTextures[num].appTexturePtr = nativePtr;
		}
	}

	private bool CreateLayer(int mipLevels, int sampleCount, OVRPlugin.EyeTextureFormat etFormat, int flags, OVRPlugin.Sizei size, OVRPlugin.OverlayShape shape)
	{
		if (!layerIdHandle.IsAllocated || layerIdPtr == IntPtr.Zero)
		{
			layerIdHandle = GCHandle.Alloc(layerId, GCHandleType.Pinned);
			layerIdPtr = layerIdHandle.AddrOfPinnedObject();
		}
		if (layerIndex == -1)
		{
			for (int i = 0; i < 15; i++)
			{
				if (instances[i] == null || instances[i] == this)
				{
					layerIndex = i;
					instances[i] = this;
					break;
				}
			}
		}
		if (layerDesc.MipLevels == mipLevels && layerDesc.SampleCount == sampleCount && layerDesc.Format == etFormat && layerDesc.LayerFlags == flags && layerDesc.TextureSize.Equals(size) && layerDesc.Shape == shape)
		{
			return false;
		}
		OVRPlugin.LayerDesc desc = OVRPlugin.CalculateLayerDesc(shape, layout, size, mipLevels, sampleCount, etFormat, flags);
		OVRPlugin.EnqueueSetupLayer(desc, layerIdPtr);
		layerId = (int)layerIdHandle.Target;
		if (layerId > 0)
		{
			layerDesc = desc;
			stageCount = OVRPlugin.GetLayerTextureStageCount(layerId);
		}
		return true;
	}

	private bool CreateLayerTextures(bool isSrgb, bool useMipmaps, OVRPlugin.Sizei size, bool isHdr)
	{
		bool result = false;
		if (stageCount <= 0)
		{
			return false;
		}
		if (layerTextures == null)
		{
			frameIndex = 0;
			layerTextures = new LayerTexture[texturesPerStage];
		}
		for (int i = 0; i < texturesPerStage; i++)
		{
			if (layerTextures[i].swapChain == null)
			{
				layerTextures[i].swapChain = new Texture[stageCount];
			}
			if (layerTextures[i].swapChainPtr == null)
			{
				layerTextures[i].swapChainPtr = new IntPtr[stageCount];
			}
			for (int j = 0; j < stageCount; j++)
			{
				Texture texture = layerTextures[i].swapChain[j];
				IntPtr intPtr = layerTextures[i].swapChainPtr[j];
				if (!(texture != null) || !(intPtr != IntPtr.Zero))
				{
					if (intPtr == IntPtr.Zero)
					{
						intPtr = OVRPlugin.GetLayerTexture(layerId, j, (OVRPlugin.Eye)i);
					}
					if (!(intPtr == IntPtr.Zero))
					{
						TextureFormat format = ((!isHdr) ? TextureFormat.RGBA32 : TextureFormat.RGBAHalf);
						texture = ((currentOverlayShape == OverlayShape.Cubemap || currentOverlayShape == OverlayShape.OffcenterCubemap) ? ((Texture)Cubemap.CreateExternalTexture(size.w, format, useMipmaps, intPtr)) : ((Texture)Texture2D.CreateExternalTexture(size.w, size.h, format, useMipmaps, isSrgb, intPtr)));
						layerTextures[i].swapChain[j] = texture;
						layerTextures[i].swapChainPtr[j] = intPtr;
						result = true;
					}
				}
			}
		}
		return result;
	}

	private void DestroyLayerTextures()
	{
		int num = 0;
		while (layerTextures != null && num < texturesPerStage)
		{
			if (layerTextures[num].swapChain != null)
			{
				for (int i = 0; i < stageCount; i++)
				{
					UnityEngine.Object.DestroyImmediate(layerTextures[num].swapChain[i]);
				}
			}
			num++;
		}
		layerTextures = null;
	}

	private void DestroyLayer()
	{
		if (layerIndex != -1)
		{
			OVRPlugin.EnqueueSubmitLayer(true, false, IntPtr.Zero, IntPtr.Zero, -1, 0, OVRPose.identity.ToPosef(), Vector3.one.ToVector3f(), layerIndex, (OVRPlugin.OverlayShape)prevOverlayShape);
			instances[layerIndex] = null;
			layerIndex = -1;
		}
		if (layerIdPtr != IntPtr.Zero)
		{
			OVRPlugin.EnqueueDestroyLayer(layerIdPtr);
			layerIdPtr = IntPtr.Zero;
			layerIdHandle.Free();
			layerId = 0;
		}
		layerDesc = default(OVRPlugin.LayerDesc);
	}

	private bool LatchLayerTextures()
	{
		for (int i = 0; i < texturesPerStage; i++)
		{
			if ((textures[i] != layerTextures[i].appTexture || layerTextures[i].appTexturePtr == IntPtr.Zero) && textures[i] != null)
			{
				layerTextures[i].appTexturePtr = textures[i].GetNativeTexturePtr();
				if (layerTextures[i].appTexturePtr != IntPtr.Zero)
				{
					layerTextures[i].appTexture = textures[i];
				}
			}
			if (currentOverlayShape == OverlayShape.Cubemap && textures[i] as Cubemap == null)
			{
				Debug.LogError("Need Cubemap texture for cube map overlay");
				return false;
			}
		}
		if (currentOverlayShape == OverlayShape.OffcenterCubemap)
		{
			Debug.LogWarning(string.Concat("Overlay shape ", currentOverlayShape, " is not supported on current platform"));
			return false;
		}
		if (layerTextures[0].appTexture == null || layerTextures[0].appTexturePtr == IntPtr.Zero)
		{
			return false;
		}
		return true;
	}

	private OVRPlugin.LayerDesc GetCurrentLayerDesc()
	{
		OVRPlugin.LayerDesc layerDesc = default(OVRPlugin.LayerDesc);
		layerDesc.Format = OVRPlugin.EyeTextureFormat.Default;
		layerDesc.LayerFlags = 8;
		layerDesc.Layout = layout;
		layerDesc.MipLevels = 1;
		layerDesc.SampleCount = 1;
		layerDesc.Shape = (OVRPlugin.OverlayShape)currentOverlayShape;
		layerDesc.TextureSize = new OVRPlugin.Sizei
		{
			w = textures[0].width,
			h = textures[0].height
		};
		OVRPlugin.LayerDesc result = layerDesc;
		Texture2D texture2D = textures[0] as Texture2D;
		if (texture2D != null)
		{
			if (texture2D.format == TextureFormat.RGBAHalf || texture2D.format == TextureFormat.RGBAFloat)
			{
				result.Format = OVRPlugin.EyeTextureFormat.R16G16B16A16_FP;
			}
			result.MipLevels = texture2D.mipmapCount;
		}
		Cubemap cubemap = textures[0] as Cubemap;
		if (cubemap != null)
		{
			result.MipLevels = cubemap.mipmapCount;
		}
		RenderTexture renderTexture = textures[0] as RenderTexture;
		if (renderTexture != null)
		{
			isDynamic = true;
			result.SampleCount = renderTexture.antiAliasing;
			if (renderTexture.format == RenderTextureFormat.ARGBHalf)
			{
				result.Format = OVRPlugin.EyeTextureFormat.R16G16B16A16_FP;
			}
		}
		return result;
	}

	private bool PopulateLayer(int mipLevels, bool isHdr, OVRPlugin.Sizei size, int sampleCount)
	{
		bool result = false;
		RenderTextureFormat format = (isHdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
		for (int i = 0; i < texturesPerStage; i++)
		{
			int dstElement = ((layout == OVRPlugin.LayerLayout.Array) ? i : 0);
			int num = frameIndex % stageCount;
			Texture texture = layerTextures[i].swapChain[num];
			if (texture == null)
			{
				continue;
			}
			for (int j = 0; j < mipLevels; j++)
			{
				RenderTexture temporary = RenderTexture.GetTemporary(size.w >> j, size.h >> j, 0, format, RenderTextureReadWrite.Default, sampleCount);
				if (!temporary.IsCreated())
				{
					temporary.Create();
				}
				temporary.DiscardContents();
				if (currentOverlayShape != OverlayShape.Cubemap && currentOverlayShape != OverlayShape.OffcenterCubemap)
				{
					Graphics.Blit(textures[i], temporary, premultiplyMaterial);
					Graphics.CopyTexture(temporary, 0, 0, texture, dstElement, j);
				}
				else
				{
					RenderTexture temporary2 = RenderTexture.GetTemporary(size.w >> j, size.h >> j, 0, format, RenderTextureReadWrite.Default, sampleCount);
					if (!temporary2.IsCreated())
					{
						temporary2.Create();
					}
					temporary2.DiscardContents();
					for (int k = 0; k < 6; k++)
					{
						Graphics.CopyTexture(textures[i], k, j, temporary2, 0, 0);
						Graphics.Blit(temporary2, temporary, premultiplyMaterial);
						Graphics.CopyTexture(temporary, 0, 0, texture, k, j);
					}
					RenderTexture.ReleaseTemporary(temporary2);
				}
				RenderTexture.ReleaseTemporary(temporary);
				result = true;
			}
		}
		return result;
	}

	private bool SubmitLayer(bool overlay, bool headLocked, OVRPose pose, Vector3 scale)
	{
		int num = ((texturesPerStage >= 2) ? 1 : 0);
		bool result = OVRPlugin.EnqueueSubmitLayer(overlay, headLocked, layerTextures[0].appTexturePtr, layerTextures[num].appTexturePtr, layerId, frameIndex, pose.flipZ().ToPosef(), scale.ToVector3f(), layerIndex, (OVRPlugin.OverlayShape)currentOverlayShape);
		if (isDynamic)
		{
			frameIndex++;
		}
		prevOverlayShape = currentOverlayShape;
		return result;
	}

	private void Awake()
	{
		Debug.Log("Overlay Awake");
		if (premultiplyMaterial == null)
		{
			premultiplyMaterial = new Material(Shader.Find("Oculus/Alpha Premultiply"));
		}
		rend = GetComponent<Renderer>();
		if (textures.Length == 0)
		{
			textures = new Texture[1];
		}
		if (rend != null && textures[0] == null)
		{
			textures[0] = rend.material.mainTexture;
		}
	}

	private void OnEnable()
	{
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		DestroyLayerTextures();
		DestroyLayer();
	}

	private void OnDestroy()
	{
		DestroyLayerTextures();
		DestroyLayer();
	}

	private bool ComputeSubmit(ref OVRPose pose, ref Vector3 scale, ref bool overlay, ref bool headLocked)
	{
		overlay = currentOverlayType == OverlayType.Overlay;
		headLocked = false;
		Transform parent = base.transform;
		while (parent != null && !headLocked)
		{
			headLocked |= parent == Camera.current.transform;
			parent = parent.parent;
		}
		pose = ((!headLocked) ? base.transform.ToTrackingSpacePose() : base.transform.ToHeadSpacePose());
		scale = base.transform.lossyScale;
		for (int i = 0; i < 3; i++)
		{
			scale[i] /= Camera.current.transform.lossyScale[i];
		}
		if (currentOverlayShape == OverlayShape.Cubemap)
		{
			pose.position = Camera.current.transform.position;
		}
		if (currentOverlayShape == OverlayShape.OffcenterCubemap)
		{
			pose.position = base.transform.position;
			if (pose.position.magnitude > 1f)
			{
				Debug.LogWarning("Your cube map center offset's magnitude is greater than 1, which will cause some cube map pixel always invisible .");
				return false;
			}
		}
		if (currentOverlayShape == OverlayShape.Cylinder)
		{
			float num = scale.x / scale.z / (float)Math.PI * 180f;
			if (num > 180f)
			{
				Debug.LogWarning("Cylinder overlay's arc angle has to be below 180 degree, current arc angle is " + num + " degree.");
				return false;
			}
		}
		return true;
	}

	private void OnRenderObject()
	{
		if (!Camera.current.CompareTag("MainCamera") || Camera.current.cameraType != CameraType.Game || currentOverlayType == OverlayType.None || textures.Length < texturesPerStage || Time.frameCount <= prevFrameIndex)
		{
			return;
		}
		prevFrameIndex = Time.frameCount;
		OVRPose pose = OVRPose.identity;
		Vector3 scale = Vector3.one;
		bool overlay = false;
		bool headLocked = false;
		if (!ComputeSubmit(ref pose, ref scale, ref overlay, ref headLocked))
		{
			return;
		}
		OVRPlugin.LayerDesc currentLayerDesc = GetCurrentLayerDesc();
		bool isHdr = currentLayerDesc.Format == OVRPlugin.EyeTextureFormat.R16G16B16A16_FP;
		bool flag = CreateLayer(currentLayerDesc.MipLevels, currentLayerDesc.SampleCount, currentLayerDesc.Format, currentLayerDesc.LayerFlags, currentLayerDesc.TextureSize, currentLayerDesc.Shape);
		if (layerIndex == -1 || layerId <= 0)
		{
			return;
		}
		bool isSrgb = currentLayerDesc.Format == OVRPlugin.EyeTextureFormat.B8G8R8A8_sRGB || currentLayerDesc.Format == OVRPlugin.EyeTextureFormat.Default;
		bool useMipmaps = currentLayerDesc.MipLevels > 1;
		flag |= CreateLayerTextures(isSrgb, useMipmaps, currentLayerDesc.TextureSize, isHdr);
		if (layerTextures[0].appTexture as RenderTexture != null)
		{
			isDynamic = true;
		}
		if ((isDynamic || flag) && LatchLayerTextures() && PopulateLayer(currentLayerDesc.MipLevels, isHdr, currentLayerDesc.TextureSize, currentLayerDesc.SampleCount))
		{
			bool flag2 = SubmitLayer(overlay, headLocked, pose, scale);
			if ((bool)rend)
			{
				rend.enabled = !flag2;
			}
		}
	}
}
