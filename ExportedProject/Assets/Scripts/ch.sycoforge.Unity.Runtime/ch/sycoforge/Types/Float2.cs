using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ch.sycoforge.Types
{
	[Serializable]
	[ComVisible(true)]
	public struct Float2
	{
		public const float kEpsilon = 1E-05f;

		[MarshalAs(UnmanagedType.R4)]
		public float x;

		[MarshalAs(UnmanagedType.R4)]
		public float y;

		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return x;
				default:
					throw new IndexOutOfRangeException("Invalid Float2 index!");
				case 1:
					return y;
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					x = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Float2 index!");
				case 1:
					y = value;
					break;
				}
			}
		}

		public float magnitude
		{
			get
			{
				return (float)Math.Sqrt(x * x + y * y);
			}
		}

		public Float2 normalized
		{
			get
			{
				Float2 result = new Float2(x, y);
				result.Normalize();
				return result;
			}
		}

		public static Float2 one
		{
			get
			{
				return new Float2(1f, 1f);
			}
		}

		public static Float2 right
		{
			get
			{
				return new Float2(1f, 0f);
			}
		}

		public float sqrMagnitude
		{
			get
			{
				return x * x + y * y;
			}
		}

		public static Float2 up
		{
			get
			{
				return new Float2(0f, 1f);
			}
		}

		public static Float2 zero
		{
			get
			{
				return new Float2(0f, 0f);
			}
		}

		public Float2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static float Angle(Float2 from, Float2 to)
		{
			return (float)Math.Acos(FloatMath.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
		}

		public static float AngleRad(Float2 from, Float2 to)
		{
			return (float)Math.Acos(FloatMath.Clamp(Dot(from.normalized, to.normalized), -1f, 1f));
		}

		public static Float2 ClampMagnitude(Float2 vector, float maxLength)
		{
			if (vector.sqrMagnitude <= maxLength * maxLength)
			{
				return vector;
			}
			return vector.normalized * maxLength;
		}

		public static float Distance(Float2 a, Float2 b)
		{
			return (a - b).magnitude;
		}

		public static float Dot(Float2 lhs, Float2 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		public override bool Equals(object other)
		{
			if (!(other is Float2))
			{
				return false;
			}
			Float2 @float = (Float2)other;
			return x.Equals(@float.x) && y.Equals(@float.y);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ (y.GetHashCode() << 2);
		}

		public static Float2 Lerp(Float2 from, Float2 to, float t)
		{
			t = FloatMath.Clamp01(t);
			return new Float2(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t);
		}

		public static Float2 Max(Float2 lhs, Float2 rhs)
		{
			return new Float2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
		}

		public static Float2 Min(Float2 lhs, Float2 rhs)
		{
			return new Float2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
		}

		public static Float2 MoveTowards(Float2 current, Float2 target, float maxDistanceDelta)
		{
			Float2 @float = target - current;
			float num = @float.magnitude;
			if (num <= maxDistanceDelta || num == 0f)
			{
				return target;
			}
			return current + @float / num * maxDistanceDelta;
		}

		public void Normalize()
		{
			float num = magnitude;
			if (num <= 1E-05f)
			{
				this = zero;
			}
			else
			{
				this /= num;
			}
		}

		public static Float2 operator +(Float2 a, Float2 b)
		{
			return new Float2(a.x + b.x, a.y + b.y);
		}

		public static Float2 operator /(Float2 a, float d)
		{
			return new Float2(a.x / d, a.y / d);
		}

		public static Float2 operator /(Float2 a, Float2 b)
		{
			return new Float2(a.x / b.x, a.y / b.y);
		}

		public static Float2 operator /(Float2 a, Int2 b)
		{
			return new Float2(a.x / (float)b.x, a.y / (float)b.y);
		}

		public static Float2 operator *(Float2 a, Float2 b)
		{
			return new Float2(a.x * b.x, a.y * b.y);
		}

		public static bool operator ==(Float2 lhs, Float2 rhs)
		{
			return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
		}

		public static implicit operator Float2(Float3 v)
		{
			return new Float2(v.x, v.y);
		}

		public static implicit operator Float3(Float2 v)
		{
			return new Float3(v.x, v.y, 0f);
		}

		public static bool operator !=(Float2 lhs, Float2 rhs)
		{
			return SqrMagnitude(lhs - rhs) >= 9.9999994E-11f;
		}

		public static Float2 operator *(Float2 a, float d)
		{
			return new Float2(a.x * d, a.y * d);
		}

		public static Float2 operator *(float d, Float2 a)
		{
			return new Float2(a.x * d, a.y * d);
		}

		public static Float2 operator -(Float2 a, Float2 b)
		{
			return new Float2(a.x - b.x, a.y - b.y);
		}

		public static Float2 operator -(Float2 a)
		{
			return new Float2(0f - a.x, 0f - a.y);
		}

		public static Float2 operator %(Float2 a, float f)
		{
			return new Float2(a.x % f, a.y % f);
		}

		public static Float2 Scale(Float2 a, Float2 b)
		{
			return new Float2(a.x * b.x, a.y * b.y);
		}

		public static Float2 Rotate(float angle, Float2 vector)
		{
			angle *= (float)Math.PI / 180f;
			float num = (float)Math.Cos(angle);
			float num2 = (float)Math.Sin(angle);
			return new Float2(num * vector.x - num2 * vector.y, num2 * vector.x + num * vector.y);
		}

		public void Rotate(float angle)
		{
			Float2 @float = Rotate(angle, this);
			x = @float.x;
			x = @float.y;
		}

		public void Scale(Float2 scale)
		{
			Float2 @float = this;
			@float.x *= scale.x;
			Float2 float2 = this;
			float2.y *= scale.y;
		}

		public void Set(float new_x, float new_y)
		{
			x = new_x;
			y = new_y;
		}

		public static Float2 Step(Float2 a, Float2 x)
		{
			Float2 result = zero;
			result.x = FloatMath.Step(a.x, x.x);
			result.y = FloatMath.Step(a.y, x.y);
			return result;
		}

		public static float SqrMagnitude(Float2 a)
		{
			return a.x * a.x + a.y * a.y;
		}

		public float SqrMagnitude()
		{
			return x * x + y * y;
		}

		public override string ToString()
		{
			return string.Format("({0:F3}, {1:F3})", new object[2] { x, y });
		}

		public string ToString(string format)
		{
			return string.Format("({0}, {1})", new object[2]
			{
				x.ToString(format),
				y.ToString(format)
			});
		}

		public static implicit operator Vector2(Float2 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static implicit operator Float2(Vector2 v)
		{
			return new Float2(v.x, v.y);
		}
	}
}
