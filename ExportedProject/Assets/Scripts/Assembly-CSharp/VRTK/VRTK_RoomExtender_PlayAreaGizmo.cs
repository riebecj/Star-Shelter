using UnityEngine;

namespace VRTK
{
	[ExecuteInEditMode]
	public class VRTK_RoomExtender_PlayAreaGizmo : MonoBehaviour
	{
		public Color color = Color.red;

		public float wireframeHeight = 2f;

		public bool drawWireframeWhenSelectedOnly;

		protected Transform playArea;

		protected VRTK_RoomExtender roomExtender;

		protected virtual void Awake()
		{
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			roomExtender = Object.FindObjectOfType<VRTK_RoomExtender>();
			if (playArea == null || roomExtender == null)
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_RoomExtender_PlayAreaGizmo", "PlayArea or VRTK_RoomExtender", "an active"));
			}
		}

		protected virtual void OnDrawGizmos()
		{
			if (!drawWireframeWhenSelectedOnly)
			{
				DrawWireframe();
			}
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (drawWireframeWhenSelectedOnly)
			{
				DrawWireframe();
			}
		}

		protected virtual void DrawWireframe()
		{
			if (!(playArea == null) && !(roomExtender == null))
			{
				Vector3[] playAreaVertices = VRTK_SDK_Bridge.GetPlayAreaVertices(playArea.gameObject);
				if (playAreaVertices != null && playAreaVertices.Length != 0)
				{
					int num = 4;
					int num2 = 5;
					int num3 = 6;
					int num4 = 7;
					Vector3 vector = playAreaVertices[num] * roomExtender.additionalMovementMultiplier;
					Vector3 vector2 = playAreaVertices[num2] * roomExtender.additionalMovementMultiplier;
					Vector3 vector3 = playAreaVertices[num3] * roomExtender.additionalMovementMultiplier;
					Vector3 vector4 = playAreaVertices[num4] * roomExtender.additionalMovementMultiplier;
					Vector3 vector5 = new Vector3(0f, roomExtender.transform.localPosition.y, 0f);
					Vector3 vector6 = vector5 + playArea.TransformVector(Vector3.up * wireframeHeight);
					Gizmos.color = color;
					Gizmos.DrawLine(vector + vector5, vector2 + vector5);
					Gizmos.DrawLine(vector3 + vector5, vector4 + vector5);
					Gizmos.DrawLine(vector + vector5, vector4 + vector5);
					Gizmos.DrawLine(vector2 + vector5, vector3 + vector5);
					Gizmos.DrawLine(vector + vector6, vector2 + vector6);
					Gizmos.DrawLine(vector3 + vector6, vector4 + vector6);
					Gizmos.DrawLine(vector + vector6, vector4 + vector6);
					Gizmos.DrawLine(vector2 + vector6, vector3 + vector6);
					Gizmos.DrawLine(vector + vector5, vector + vector6);
					Gizmos.DrawLine(vector2 + vector5, vector2 + vector6);
					Gizmos.DrawLine(vector4 + vector5, vector4 + vector6);
					Gizmos.DrawLine(vector3 + vector5, vector3 + vector6);
				}
			}
		}
	}
}
