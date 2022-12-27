using System;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	[Serializable]
	public struct IntRect
	{
		public static IntRect Empty = default(IntRect);

		[MarshalAs(UnmanagedType.R4)]
		private int m_XMin;

		[MarshalAs(UnmanagedType.R4)]
		private int m_YMin;

		[MarshalAs(UnmanagedType.R4)]
		private int m_Width;

		[MarshalAs(UnmanagedType.R4)]
		private int m_Height;

		public Int2 center
		{
			get
			{
				return new Int2(x + m_Width / 2, y + m_Height / 2);
			}
			set
			{
				m_XMin = value.x - m_Width / 2;
				m_YMin = value.y - m_Height / 2;
			}
		}

		public int height
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
		public int left
		{
			get
			{
				return m_XMin;
			}
		}

		[Obsolete("use yMax")]
		public int bottom
		{
			get
			{
				return m_YMin + m_Height;
			}
		}

		public Int2 max
		{
			get
			{
				return new Int2(xMax, yMax);
			}
			set
			{
				xMax = value.x;
				yMax = value.y;
			}
		}

		public Int2 min
		{
			get
			{
				return new Int2(xMin, yMin);
			}
			set
			{
				xMin = value.x;
				yMin = value.y;
			}
		}

		public Int2 position
		{
			get
			{
				return new Int2(m_XMin, m_YMin);
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

		public Int2 size
		{
			get
			{
				return new Int2(m_Width, m_Height);
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

		public int width
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

		public int x
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

		public int xMax
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

		public int xMin
		{
			get
			{
				return m_XMin;
			}
			set
			{
				int num = xMax;
				m_XMin = value;
				m_Width = num - m_XMin;
			}
		}

		public int y
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

		public int yMax
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

		public int yMin
		{
			get
			{
				return m_YMin;
			}
			set
			{
				int num = yMax;
				m_YMin = value;
				m_Height = num - m_YMin;
			}
		}

		public IntRect(int left, int top, int width, int height)
		{
			m_XMin = left;
			m_YMin = top;
			m_Width = width;
			m_Height = height;
		}

		public IntRect(IntRect source)
		{
			m_XMin = source.m_XMin;
			m_YMin = source.m_YMin;
			m_Width = source.m_Width;
			m_Height = source.m_Height;
		}

		public bool Contains(Float2 point)
		{
			return !(point.x < (float)xMin) && !(point.x >= (float)xMax) && !(point.y < (float)yMin) && point.y < (float)yMax;
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
			if (((float)width < 0f && point.x <= (float)xMin && point.x > (float)xMax) || ((float)width >= 0f && point.x >= (float)xMin && point.x < (float)xMax))
			{
				flag = true;
			}
			if (flag && (((float)height < 0f && point.y <= (float)yMin && point.y > (float)yMax) || ((float)height >= 0f && point.y >= (float)yMin && point.y < (float)yMax)))
			{
				return true;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			if (!(other is IntRect))
			{
				return false;
			}
			IntRect intRect = (IntRect)other;
			return x.Equals(intRect.x) && y.Equals(intRect.y) && width.Equals(intRect.width) && height.Equals(intRect.height);
		}

		public override int GetHashCode()
		{
			int hashCode = x.GetHashCode();
			float num = width;
			float num2 = y;
			float num3 = height;
			return hashCode ^ (num.GetHashCode() << 2) ^ (num2.GetHashCode() >> 2) ^ (num3.GetHashCode() >> 1);
		}

		public static IntRect MinMaxRect(int left, int top, int right, int bottom)
		{
			return new IntRect(left, top, right - left, bottom - top);
		}

		public static Float2 NormalizedToPoint(IntRect rectangle, Float2 normalizedRectCoordinates)
		{
			return new Float2(FloatMath.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), FloatMath.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
		}

		public static bool operator ==(IntRect lhs, IntRect rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
		}

		public static bool operator !=(IntRect lhs, IntRect rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.width != rhs.width || lhs.height != rhs.height;
		}

		private static IntRect OrderMinMax(IntRect rect)
		{
			if (rect.xMin > rect.xMax)
			{
				int num = rect.xMin;
				rect.xMin = rect.xMax;
				rect.xMax = num;
			}
			if (rect.yMin > rect.yMax)
			{
				int num2 = rect.yMin;
				rect.yMin = rect.yMax;
				rect.yMax = num2;
			}
			return rect;
		}

		public bool Overlaps(IntRect other)
		{
			return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
		}

		public bool Overlaps(IntRect other, bool allowInverse)
		{
			IntRect rect = this;
			if (allowInverse)
			{
				rect = OrderMinMax(rect);
				other = OrderMinMax(other);
			}
			return rect.Overlaps(other);
		}

		public static Float2 PointToNormalized(IntRect rectangle, Float2 point)
		{
			return new Float2(FloatMath.InverseLerp(rectangle.x, rectangle.xMax, point.x), FloatMath.InverseLerp(rectangle.y, rectangle.yMax, point.y));
		}

		public void Set(int x, int y, int width, int height)
		{
			m_XMin = x;
			m_YMin = y;
			m_Width = width;
			m_Height = height;
		}

		public void Set(Int2 position, int width, int height)
		{
			m_XMin = position.x;
			m_YMin = position.y;
			m_Width = width;
			m_Height = height;
		}

		public override string ToString()
		{
			return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", x, y, width, height);
		}
	}
}
