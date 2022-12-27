using System;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
	[ExecuteInEditMode]
	public class UICircle : Graphic
	{
		[Range(0f, 100f)]
		public int fillPercent;

		public bool fill = true;

		public int thickness = 5;

		[Range(0f, 360f)]
		public int segments = 360;

		[SerializeField]
		protected Texture setTexture;

		public override Texture mainTexture
		{
			get
			{
				return (!(setTexture == null)) ? setTexture : Graphic.s_WhiteTexture;
			}
		}

		public Texture texture
		{
			get
			{
				return setTexture;
			}
			set
			{
				if (!(setTexture == value))
				{
					setTexture = value;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		[Obsolete("Use OnPopulateMesh(VertexHelper vh) instead.")]
		protected override void OnPopulateMesh(Mesh toFill)
		{
			float num = (0f - base.rectTransform.pivot.x) * base.rectTransform.rect.width;
			float num2 = (0f - base.rectTransform.pivot.x) * base.rectTransform.rect.width + (float)thickness;
			toFill.Clear();
			VertexHelper vertexHelper = new VertexHelper(toFill);
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			Vector2 vector3 = new Vector2(0f, 0f);
			Vector2 vector4 = new Vector2(0f, 1f);
			Vector2 vector5 = new Vector2(1f, 1f);
			Vector2 vector6 = new Vector2(1f, 0f);
			float num3 = (float)fillPercent / 100f;
			float num4 = 360f / (float)segments;
			int num5 = (int)((float)(segments + 1) * num3);
			for (int i = -1 - num5 / 2; i < num5 / 2 + 1; i++)
			{
				float f = (float)Math.PI / 180f * ((float)i * num4);
				float num6 = Mathf.Cos(f);
				float num7 = Mathf.Sin(f);
				vector3 = new Vector2(0f, 1f);
				vector4 = new Vector2(1f, 1f);
				vector5 = new Vector2(1f, 0f);
				vector6 = new Vector2(0f, 0f);
				Vector2 vector7 = vector;
				Vector2 vector8 = new Vector2(num * num6, num * num7);
				Vector2 vector9;
				Vector2 vector10;
				if (fill)
				{
					vector9 = Vector2.zero;
					vector10 = Vector2.zero;
				}
				else
				{
					vector9 = new Vector2(num2 * num6, num2 * num7);
					vector10 = vector2;
				}
				vector = vector8;
				vector2 = vector9;
				vertexHelper.AddUIVertexQuad(SetVbo(new Vector2[4] { vector7, vector8, vector9, vector10 }, new Vector2[4] { vector3, vector4, vector5, vector6 }));
			}
			if (vertexHelper.currentVertCount > 3)
			{
				vertexHelper.FillMesh(toFill);
			}
		}

		protected virtual void Update()
		{
			thickness = (int)Mathf.Clamp(thickness, 0f, base.rectTransform.rect.width / 2f);
		}

		protected virtual UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] array = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}
	}
}
