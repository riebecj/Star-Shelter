using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ch.sycoforge.Types
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	[ComVisible(true)]
	public struct Float4
	{
		[MarshalAs(UnmanagedType.R4)]
		public float x;

		[MarshalAs(UnmanagedType.R4)]
		public float y;

		[MarshalAs(UnmanagedType.R4)]
		public float z;

		[MarshalAs(UnmanagedType.R4)]
		public float w;

		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return x;
				case 1:
					return y;
				case 2:
					return z;
				case 3:
					return w;
				default:
					throw new IndexOutOfRangeException("Invalid Float4 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				case 2:
					z = value;
					break;
				case 3:
					w = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Float4 index!");
				}
			}
		}

		public float magnitude
		{
			get
			{
				return (float)Math.Sqrt(Dot(this, this));
			}
		}

		public Float4 normalized
		{
			get
			{
				return Normalize(this);
			}
		}

		public static Float4 one
		{
			get
			{
				return new Float4(1f, 1f, 1f, 1f);
			}
		}

		public float sqrMagnitude
		{
			get
			{
				return Dot(this, this);
			}
		}

		public static Float4 zero
		{
			get
			{
				return new Float4(0f, 0f, 0f, 0f);
			}
		}

		public Float4(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Float4(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			w = 0f;
		}

		public Float4(float x, float y)
		{
			this.x = x;
			this.y = y;
			z = 0f;
			w = 0f;
		}

		public static float Distance(Float4 a, Float4 b)
		{
			return Magnitude(a - b);
		}

		public static float Dot(Float4 a, Float4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public override bool Equals(object other)
		{
			if (!(other is Float4))
			{
				return false;
			}
			Float4 @float = (Float4)other;
			return x.Equals(@float.x) && y.Equals(@float.y) && z.Equals(@float.z) && w.Equals(@float.w);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
		}

		public static Float4 Lerp(Float4 from, Float4 to, float t)
		{
			t = FloatMath.Clamp01(t);
			return new Float4(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t, from.w + (to.w - from.w) * t);
		}

		public static float Magnitude(Float4 a)
		{
			return (float)Math.Sqrt(Dot(a, a));
		}

		public static Float4 Max(Float4 lhs, Float4 rhs)
		{
			return new Float4(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z), Math.Max(lhs.w, rhs.w));
		}

		public static Float4 Min(Float4 lhs, Float4 rhs)
		{
			return new Float4(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z), Math.Min(lhs.w, rhs.w));
		}

		public static Float4 MoveTowards(Float4 current, Float4 target, float maxDistanceDelta)
		{
			Float4 @float = target - current;
			float num = @float.magnitude;
			if (num <= maxDistanceDelta || num == 0f)
			{
				return target;
			}
			return current + @float / num * maxDistanceDelta;
		}

		public static Float4 Normalize(Float4 a)
		{
			float num = Magnitude(a);
			if (num <= 1E-05f)
			{
				return zero;
			}
			return a / num;
		}

		public void Normalize()
		{
			float num = Magnitude(this);
			if (num <= 1E-05f)
			{
				this = zero;
			}
			else
			{
				this /= num;
			}
		}

		public static Float4 operator +(Float4 a, Float4 b)
		{
			return new Float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Float4 operator /(Float4 a, float d)
		{
			return new Float4(a.x / d, a.y / d, a.z / d, a.w / d);
		}

		public static Float4 operator /(Float4 a, Float4 b)
		{
			return new Float4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}

		public static bool operator ==(Float4 lhs, Float4 rhs)
		{
			return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
		}

		public static implicit operator Float4(Float3 v)
		{
			return new Float4(v.x, v.y, v.z, 0f);
		}

		public static implicit operator Float3(Float4 v)
		{
			return new Float4(v.x, v.y, v.z);
		}

		public static implicit operator Float4(Float2 v)
		{
			return new Float4(v.x, v.x, 0f, 0f);
		}

		public static implicit operator Float2(Float4 v)
		{
			return new Float2(v.x, v.y);
		}

		public static bool operator !=(Float4 lhs, Float4 rhs)
		{
			return SqrMagnitude(lhs - rhs) >= 9.9999994E-11f;
		}

		public static Float4 operator *(Float4 a, float d)
		{
			return new Float4(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Float4 operator *(float d, Float4 a)
		{
			return new Float4(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Float4 operator -(Float4 a, Float4 b)
		{
			return new Float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Float4 operator -(Float4 a)
		{
			return new Float4(0f - a.x, 0f - a.y, 0f - a.z, 0f - a.w);
		}

		public static Float4 Project(Float4 a, Float4 b)
		{
			return b * Dot(a, b) / Dot(b, b);
		}

		public static Float4 Scale(Float4 a, Float4 b)
		{
			return new Float4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		public void Scale(Float4 scale)
		{
			Float4 @float = this;
			@float.x *= scale.x;
			Float4 float2 = this;
			float2.y *= scale.y;
			Float4 float3 = this;
			float3.z *= scale.z;
			Float4 float4 = this;
			float4.w *= scale.w;
		}

		public void Set(float new_x, float new_y, float new_z, float new_w)
		{
			x = new_x;
			y = new_y;
			z = new_z;
			w = new_w;
		}

		public static Float4 Step(Float4 a, Float4 x)
		{
			Float4 result = zero;
			result.x = FloatMath.Step(a.x, x.x);
			result.y = FloatMath.Step(a.y, x.y);
			result.z = FloatMath.Step(a.z, x.z);
			result.w = FloatMath.Step(a.w, x.w);
			return result;
		}

		public static float SqrMagnitude(Float4 a)
		{
			return Dot(a, a);
		}

		public float SqrMagnitude()
		{
			return Dot(this, this);
		}

		public override string ToString()
		{
			return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
		}

		public string ToString(string format)
		{
			return string.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
		}

		public static implicit operator Vector4(Float4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		public static implicit operator Float4(Vector4 v)
		{
			return new Float4(v.x, v.y, v.z, v.w);
		}
	}
}
