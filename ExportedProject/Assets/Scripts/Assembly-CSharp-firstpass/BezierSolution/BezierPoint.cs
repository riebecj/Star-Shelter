using UnityEngine;

namespace BezierSolution
{
	public class BezierPoint : MonoBehaviour
	{
		public enum HandleMode
		{
			Free = 0,
			Aligned = 1,
			Mirrored = 2
		}

		[SerializeField]
		[HideInInspector]
		private Vector3 m_position;

		[SerializeField]
		[HideInInspector]
		private Vector3 m_precedingControlPointLocalPosition = Vector3.left;

		[SerializeField]
		[HideInInspector]
		private Vector3 m_precedingControlPointPosition;

		[SerializeField]
		[HideInInspector]
		private Vector3 m_followingControlPointLocalPosition = Vector3.right;

		[SerializeField]
		[HideInInspector]
		private Vector3 m_followingControlPointPosition;

		[SerializeField]
		[HideInInspector]
		private HandleMode m_handleMode = HandleMode.Mirrored;

		public Vector3 localPosition
		{
			get
			{
				return base.transform.localPosition;
			}
			set
			{
				base.transform.localPosition = value;
			}
		}

		public Vector3 position
		{
			get
			{
				if (base.transform.hasChanged)
				{
					Revalidate();
				}
				return m_position;
			}
			set
			{
				base.transform.position = value;
			}
		}

		public Quaternion localRotation
		{
			get
			{
				return base.transform.localRotation;
			}
			set
			{
				base.transform.localRotation = value;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return base.transform.rotation;
			}
			set
			{
				base.transform.rotation = value;
			}
		}

		public Vector3 localEulerAngles
		{
			get
			{
				return base.transform.localEulerAngles;
			}
			set
			{
				base.transform.localEulerAngles = value;
			}
		}

		public Vector3 eulerAngles
		{
			get
			{
				return base.transform.eulerAngles;
			}
			set
			{
				base.transform.eulerAngles = value;
			}
		}

		public Vector3 localScale
		{
			get
			{
				return base.transform.localScale;
			}
			set
			{
				base.transform.localScale = value;
			}
		}

		public Vector3 precedingControlPointLocalPosition
		{
			get
			{
				return m_precedingControlPointLocalPosition;
			}
			set
			{
				m_precedingControlPointLocalPosition = value;
				m_precedingControlPointPosition = base.transform.TransformPoint(value);
				if (m_handleMode == HandleMode.Aligned)
				{
					m_followingControlPointLocalPosition = -m_precedingControlPointLocalPosition.normalized * m_followingControlPointLocalPosition.magnitude;
					m_followingControlPointPosition = base.transform.TransformPoint(m_followingControlPointLocalPosition);
				}
				else if (m_handleMode == HandleMode.Mirrored)
				{
					m_followingControlPointLocalPosition = -m_precedingControlPointLocalPosition;
					m_followingControlPointPosition = base.transform.TransformPoint(m_followingControlPointLocalPosition);
				}
			}
		}

		public Vector3 precedingControlPointPosition
		{
			get
			{
				if (base.transform.hasChanged)
				{
					Revalidate();
				}
				return m_precedingControlPointPosition;
			}
			set
			{
				m_precedingControlPointPosition = value;
				m_precedingControlPointLocalPosition = base.transform.InverseTransformPoint(value);
				if (base.transform.hasChanged)
				{
					m_position = base.transform.position;
					base.transform.hasChanged = false;
				}
				if (m_handleMode == HandleMode.Aligned)
				{
					m_followingControlPointPosition = m_position - (m_precedingControlPointPosition - m_position).normalized * (m_followingControlPointPosition - m_position).magnitude;
					m_followingControlPointLocalPosition = base.transform.InverseTransformPoint(m_followingControlPointPosition);
				}
				else if (m_handleMode == HandleMode.Mirrored)
				{
					m_followingControlPointPosition = 2f * m_position - m_precedingControlPointPosition;
					m_followingControlPointLocalPosition = base.transform.InverseTransformPoint(m_followingControlPointPosition);
				}
			}
		}

		public Vector3 followingControlPointLocalPosition
		{
			get
			{
				return m_followingControlPointLocalPosition;
			}
			set
			{
				m_followingControlPointLocalPosition = value;
				m_followingControlPointPosition = base.transform.TransformPoint(value);
				if (m_handleMode == HandleMode.Aligned)
				{
					m_precedingControlPointLocalPosition = -m_followingControlPointLocalPosition.normalized * m_precedingControlPointLocalPosition.magnitude;
					m_precedingControlPointPosition = base.transform.TransformPoint(m_precedingControlPointLocalPosition);
				}
				else if (m_handleMode == HandleMode.Mirrored)
				{
					m_precedingControlPointLocalPosition = -m_followingControlPointLocalPosition;
					m_precedingControlPointPosition = base.transform.TransformPoint(m_precedingControlPointLocalPosition);
				}
			}
		}

		public Vector3 followingControlPointPosition
		{
			get
			{
				if (base.transform.hasChanged)
				{
					Revalidate();
				}
				return m_followingControlPointPosition;
			}
			set
			{
				m_followingControlPointPosition = value;
				m_followingControlPointLocalPosition = base.transform.InverseTransformPoint(value);
				if (base.transform.hasChanged)
				{
					m_position = base.transform.position;
					base.transform.hasChanged = false;
				}
				if (m_handleMode == HandleMode.Aligned)
				{
					m_precedingControlPointPosition = m_position - (m_followingControlPointPosition - m_position).normalized * (m_precedingControlPointPosition - m_position).magnitude;
					m_precedingControlPointLocalPosition = base.transform.InverseTransformPoint(m_precedingControlPointPosition);
				}
				else if (m_handleMode == HandleMode.Mirrored)
				{
					m_precedingControlPointPosition = 2f * m_position - m_followingControlPointPosition;
					m_precedingControlPointLocalPosition = base.transform.InverseTransformPoint(m_precedingControlPointPosition);
				}
			}
		}

		public HandleMode handleMode
		{
			get
			{
				return m_handleMode;
			}
			set
			{
				m_handleMode = value;
				if (value == HandleMode.Aligned || value == HandleMode.Mirrored)
				{
					precedingControlPointLocalPosition = m_precedingControlPointLocalPosition;
				}
			}
		}

		private void Awake()
		{
			base.transform.hasChanged = true;
		}

		public void CopyTo(BezierPoint other)
		{
			other.transform.localPosition = base.transform.localPosition;
			other.transform.localRotation = base.transform.localRotation;
			other.transform.localScale = base.transform.localScale;
			other.m_handleMode = m_handleMode;
			other.m_precedingControlPointLocalPosition = m_precedingControlPointLocalPosition;
			other.m_followingControlPointLocalPosition = m_followingControlPointLocalPosition;
		}

		private void Revalidate()
		{
			m_position = base.transform.position;
			m_precedingControlPointPosition = base.transform.TransformPoint(m_precedingControlPointLocalPosition);
			m_followingControlPointPosition = base.transform.TransformPoint(m_followingControlPointLocalPosition);
			base.transform.hasChanged = false;
		}

		public void Reset()
		{
			localPosition = Vector3.zero;
			localRotation = Quaternion.identity;
			localScale = Vector3.one;
			precedingControlPointLocalPosition = Vector3.left;
			followingControlPointLocalPosition = Vector3.right;
			base.transform.hasChanged = true;
		}
	}
}
