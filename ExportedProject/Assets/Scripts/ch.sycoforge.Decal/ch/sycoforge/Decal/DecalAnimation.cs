using UnityEngine;

namespace ch.sycoforge.Decal
{
	public class DecalAnimation : MonoBehaviour
	{
		public int Columns = 8;

		public int Rows = 8;

		public float FPS = 30f;

		public bool CorrectTime;

		public TweenType Type;

		public string[] InputTextureNames = new string[1] { "_MainTex" };

		private int frames;

		private int frameNumber = 0;

		private int direction = 1;

		private float duration = 0f;

		private float frameTime;

		private float rowFrac;

		private float colFrac;

		private Vector2 size;

		private Renderer decalRenderer;

		public virtual void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			EasyDecal component = GetComponent<EasyDecal>();
			if (component == null)
			{
				Debug.LogError("No Easy Decal component found.");
			}
			decalRenderer = component.DecalRenderer;
			frames = Rows * Columns;
			size = new Vector2(1f / (float)Columns, 1f / (float)Rows);
			SetTexturesScale(size);
			rowFrac = 1f / (float)Rows;
			colFrac = 1f / (float)Columns;
			duration = (float)frames / FPS;
			frameTime = duration / (float)frames;
			direction = 1;
			Invoke("RenderFrame", 0f);
		}

		protected virtual void RenderFrame()
		{
			int num = frameNumber / Columns;
			int num2 = frameNumber % Columns;
			Vector2 texturesOffsets = new Vector2(colFrac * (float)num2, 1f - rowFrac * (float)(num + 1));
			SetTexturesOffsets(texturesOffsets);
			int num3 = 1;
			if (CorrectTime)
			{
				num3 = Mathf.Max(Mathf.RoundToInt(Time.deltaTime / frameTime), 1);
			}
			frameNumber += num3 * direction;
			if (IsFinished())
			{
				if (Type == TweenType.Loop)
				{
					Reset();
					Invoke("RenderFrame", frameTime);
				}
				else if (Type == TweenType.LoopReverse)
				{
					SwitchDirection();
					frameNumber += 2 * num3 * direction;
					Invoke("RenderFrame", frameTime);
				}
				else if (Type == TweenType.OneTimeReverse && direction > 0)
				{
					SwitchDirection();
					frameNumber += 2 * num3 * direction;
					Invoke("RenderFrame", frameTime);
				}
			}
			else
			{
				Invoke("RenderFrame", frameTime);
			}
			frameNumber = Mathf.Max(Mathf.Min(frames, frameNumber), 0);
		}

		public bool IsFinished()
		{
			if (direction > 0)
			{
				return frameNumber >= frames;
			}
			return frameNumber < 0;
		}

		public void Reset()
		{
			frameNumber = 0;
		}

		protected void SwitchDirection()
		{
			direction *= -1;
		}

		protected void SetTexturesScale(Vector2 size)
		{
			string[] inputTextureNames = InputTextureNames;
			foreach (string text in inputTextureNames)
			{
				decalRenderer.material.SetTextureScale(text, size);
			}
		}

		protected void SetTexturesOffsets(Vector2 offset)
		{
			string[] inputTextureNames = InputTextureNames;
			foreach (string text in inputTextureNames)
			{
				decalRenderer.material.SetTextureOffset(text, offset);
			}
		}
	}
}
