using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ch.sycoforge.Types
{
	[Serializable]
	public struct FloatRect
	{
		public static FloatRect Empty = default(FloatRect);

		[MarshalAs(UnmanagedType.R4)]
		private float m_XMin;

		[MarshalAs(UnmanagedType.R4)]
		private float m_YMin;

		[MarshalAs(UnmanagedType.R4)]
		private float m_Width;

		[MarshalAs(UnmanagedType.R4)]
		private float m_Height;

		public Float2 center
		{
			get
			{
				return new Float2(x + m_Width / 2f, y + m_Height / 2f);
			}
			set
			{
				m_XMin = value.x - m_Width / 2f;
				m_YMin = value.y - m_Height / 2f;
			}
		}

		public float height
		{
			get
			{
				return m_Height;
			}
			set
			{
				m_Height = value;
			}
		}

		[Obsolete("use xMin")]
		public float left
		{
			get
			{
				return m_XMin;
			}
		}

		[Obsolete("use yMax")]
		public float bottom
		{
			get
			{
				return m_YMin + m_Height;
			}
		}

		public Float2 max
		{
			get
			{
				return new Float2(xMax, yMax);
			}
			set
			{
				xMax = value.x;
				yMax = value.y;
			}
		}

		public Float2 min
		{
			get
			{
				return new Float2(xMin, yMin);
			}
			set
			{
				xMin = value.x;
				yMin = value.y;
			}
		}

		public Float2 position
		{
			get
			{
				return new Float2(m_XMin, m_YMin);
			}
			set
			{
				m_XMin = value.x;
				m_YMin = value.y;
			}
		}

		[Obsolete("use xMax")]
		public float right
		{
			get
			{
				return m_XMin + m_Width;
			}
		}

		public Float2 size
		{
			get
			{
				return new Float2(m_Width, m_Height);
			}
			set
			{
				m_Width = value.x;
				m_Height = value.y;
			}
		}

		[Obsolete("use yMin")]
		public float top
		{
			get
			{
				return m_YMin;
			}
		}

		public float width
		{
			get
			{
				return m_Width;
			}
			set
			{
				m_Width = value;
			}
		}

		public float x
		{
			get
			{
				return m_XMin;
			}
			set
			{
				m_XMin = value;
			}
		}

		public float xMax
		{
			get
			{
				return m_Width + m_XMin;
			}
			set
			{
				m_Width = value - m_XMin;
			}
		}

		public float xMin
		{
			get
			{
				return m_XMin;
			}
			set
			{
				float num = xMax;
				m_XMin = value;
				m_Width = num - m_XMin;
			}
		}

		public float y
		{
			get
			{
				return m_YMin;
			}
			set
			{
				m_YMin = value;
			}
		}

		public float yMax
		{
			get
			{
				return m_Height + m_YMin;
			}
			set
			{
				m_Height = value - m_YMin;
			}
		}

		public float yMin
		{
			get
			{
				return m_YMin;
			}
			set
			{
				float num = yMax;
				m_YMin = value;
				m_Height = num - m_YMin;
			}
		}

		public FloatRect(float left, float top, float width, float height)
		{
			m_XMin = left;
			m_YMin = top;
			m_Width = width;
			m_Height = height;
		}

		public FloatRect(Float2 position, Float2 size)
		{
			m_XMin = position.x;
			m_YMin = position.y;
			m_Width = size.x;
			m_Height = size.y;
		}

		public FloatRect(FloatRect source)
		{
			m_XMin = source.m_XMin;
			m_YMin = source.m_YMin;
			m_Width = source.m_Width;
			m_Height = source.m_Height;
		}

		public bool Contains(Float2 point)
		{
			return !(point.x < xMin) && !(point.x >= xMax) && !(point.y < yMin) && point.y < yMax;
		}

		public bool Contains(Float3 point)
		{
			return Contains(new Float2(point.x, point.y));
		}

		public bool Contains(Float3 point, bool allowInverse)
		{
			if (!allowInverse)
			{
				return Contains(point);
			}
			bool flag = false;
			if ((width < 0f && point.x <= xMin && point.x > xMax) || (width >= 0f && point.x >= xMin && point.x < xMax))
			{
				flag = true;
			}
			if (flag && ((height < 0f && point.y <= yMin && point.y > yMax) || (height >= 0f && point.y >= yMin && point.y < yMax)))
			{
				return true;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			if (!(other is FloatRect))
			{
				return false;
			}
			FloatRect floatRect = (FloatRect)other;
			return x.Equals(floatRect.x) && y.Equals(floatRect.y) && width.Equals(floatRect.width) && height.Equals(floatRect.height);
		}

		public override int GetHashCode()
		{
			int hashCode = x.GetHashCode();
			float num = width;
			float num2 = y;
			float num3 = height;
			return hashCode ^ (num.GetHashCode() << 2) ^ (num2.GetHashCode() >> 2) ^ (num3.GetHashCode() >> 1);
		}

		public static FloatRect MinMaxRect(float left, float top, float right, float bottom)
		{
			return new FloatRect(left, top, right - left, bottom - top);
		}

		public static Float2 NormalizedToPoint(FloatRect rectangle, Float2 normalizedRectCoordinates)
		{
			return new Float2(FloatMath.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), FloatMath.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
		}

		public static bool operator ==(FloatRect lhs, FloatRect rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
		}

		public static bool operator !=(FloatRect lhs, FloatRect rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.width != rhs.width || lhs.height != rhs.height;
		}

		private static FloatRect OrderMinMax(FloatRect rect)
		{
			if (rect.xMin > rect.xMax)
			{
				float num = rect.xMin;
				rect.xMin = rect.xMax;
				rect.xMax = num;
			}
			if (rect.yMin > rect.yMax)
			{
				float num2 = rect.yMin;
				rect.yMin = rect.yMax;
				rect.yMax = num2;
			}
			return rect;
		}

		public bool Overlaps(FloatRect other)
		{
			return !(other.xMax <= xMin) && !(other.xMin >= xMax) && !(other.yMax <= yMin) && other.yMin < yMax;
		}

		public bool Overlaps(FloatRect other, bool allowInverse)
		{
			FloatRect rect = this;
			if (allowInverse)
			{
				rect = OrderMinMax(rect);
				other = OrderMinMax(other);
			}
			return rect.Overlaps(other);
		}

		public static Float2 PointToNormalized(FloatRect rectangle, Float2 point)
		{
			return new Float2(FloatMath.InverseLerp(rectangle.x, rectangle.xMax, point.x), FloatMath.InverseLerp(rectangle.y, rectangle.yMax, point.y));
		}

		public void Set(float x, float y, float width, float height)
		{
			m_XMin = x;
			m_YMin = y;
			m_Width = width;
			m_Height = height;
		}

		public void Set(Float2 position, float width, float height)
		{
			m_XMin = position.x;
			m_YMin = position.y;
			m_Width = width;
			m_Height = height;
		}

		public override string ToString()
		{
			return string.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", x, y, width, height);
		}

		public static implicit operator Rect(FloatRect margin)
		{
			return new Rect(margin.xMin, margin.yMin, margin.width, margin.height);
		}

		public static implicit operator FloatRect(Rect margin)
		{
			return new FloatRect(margin.xMin, margin.yMin, margin.width, margin.height);
		}
	}
}
