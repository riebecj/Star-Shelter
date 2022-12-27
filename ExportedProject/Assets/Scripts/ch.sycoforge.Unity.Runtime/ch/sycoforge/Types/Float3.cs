using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ch.sycoforge.Types
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 12)]
	[ComVisible(true)]
	public struct Float3
	{
		public const float kEpsilon = 1E-05f;

		public float x;

		public float y;

		public float z;

		public static Float3 back
		{
			get
			{
				return new Float3(0f, 0f, -1f);
			}
		}

		public static Float3 down
		{
			get
			{
				return new Float3(0f, -1f, 0f);
			}
		}

		public static Float3 forward
		{
			get
			{
				return new Float3(0f, 0f, 1f);
			}
		}

		[Obsolete("Use Float3.forward instead.")]
		public static Float3 fwd
		{
			get
			{
				return new Float3(0f, 0f, 1f);
			}
		}

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
				default:
					throw new IndexOutOfRangeException("Invalid Float3 index!");
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
				default:
					throw new IndexOutOfRangeException("Invalid Float3 index!");
				}
			}
		}

		public static Float3 left
		{
			get
			{
				return new Float3(-1f, 0f, 0f);
			}
		}

		public float magnitude
		{
			get
			{
				return (float)Math.Sqrt(x * x + y * y + z * z);
			}
		}

		public Float3 normalized
		{
			get
			{
				return Normalize(this);
			}
		}

		public static Float3 one
		{
			get
			{
				return new Float3(1f, 1f, 1f);
			}
		}

		public static Float3 right
		{
			get
			{
				return new Float3(1f, 0f, 0f);
			}
		}

		public float sqrMagnitude
		{
			get
			{
				return x * x + y * y + z * z;
			}
		}

		public static Float3 up
		{
			get
			{
				return new Float3(0f, 1f, 0f);
			}
		}

		public static Float3 zero
		{
			get
			{
				return new Float3(0f, 0f, 0f);
			}
		}

		public Float3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Float3(float x, float y)
		{
			this.x = x;
			this.y = y;
			z = 0f;
		}

		public static float Angle(Float3 from, Float3 to)
		{
			float value = Dot(from.normalized, to.normalized);
			return (float)Math.Acos(FloatMath.Clamp(value, -1f, 1f)) * ((float)Math.PI / 180f);
		}

		public static Float3 ClampMagnitude(Float3 vector, float maxLength)
		{
			if (vector.sqrMagnitude <= maxLength * maxLength)
			{
				return vector;
			}
			return vector.normalized * maxLength;
		}

		public static Float3 Cross(Float3 lhs, Float3 rhs)
		{
			return new Float3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public static float Distance(Float3 a, Float3 b)
		{
			Float3 @float = new Float3(a.x - b.x, a.y - b.y, a.z - b.z);
			return (float)Math.Sqrt(@float.x * @float.x + @float.y * @float.y + @float.z * @float.z);
		}

		public static float Dot(Float3 lhs, Float3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public override bool Equals(object other)
		{
			if (!(other is Float3))
			{
				return false;
			}
			Float3 @float = (Float3)other;
			return x.Equals(@float.x) && y.Equals(@float.y) && z.Equals(@float.z);
		}

		[Obsolete("Use Float3.ProjectOnPlane instead.")]
		public static Float3 Exclude(Float3 excludeThis, Float3 fromThat)
		{
			return fromThat - Project(fromThat, excludeThis);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
		}

		public static Float3 Lerp(Float3 from, Float3 to, float t)
		{
			t = FloatMath.Clamp01(t);
			return new Float3(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}

		public static float Magnitude(Float3 a)
		{
			return (float)Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
		}

		public static Float3 Max(Float3 lhs, Float3 rhs)
		{
			return new Float3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
		}

		public static Float3 Min(Float3 lhs, Float3 rhs)
		{
			return new Float3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
		}

		public static Float3 MoveTowards(Float3 current, Float3 target, float maxDistanceDelta)
		{
			Float3 @float = target - current;
			float num = @float.magnitude;
			if (num <= maxDistanceDelta || num == 0f)
			{
				return target;
			}
			return current + @float / num * maxDistanceDelta;
		}

		public static Float3 Normalize(Float3 value)
		{
			float num = Magnitude(value);
			if (num <= 1E-05f)
			{
				return zero;
			}
			return value / num;
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

		public static Float3 operator +(Float3 a, Float3 b)
		{
			return new Float3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Float3 operator /(Float3 a, float d)
		{
			return new Float3(a.x / d, a.y / d, a.z / d);
		}

		public static bool operator ==(Float3 lhs, Float3 rhs)
		{
			return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
		}

		public static bool operator !=(Float3 lhs, Float3 rhs)
		{
			return SqrMagnitude(lhs - rhs) >= 9.9999994E-11f;
		}

		public static Float3 operator *(Float3 a, float d)
		{
			return new Float3(a.x * d, a.y * d, a.z * d);
		}

		public static Float3 operator *(float d, Float3 a)
		{
			return new Float3(a.x * d, a.y * d, a.z * d);
		}

		public static Float3 operator *(Float3 a, Float3 b)
		{
			return new Float3(a.x * a.x, a.y * a.y, a.z * a.z);
		}

		public static Float3 operator /(Float3 a, Float3 b)
		{
			return new Float3(a.x / a.x, a.y / a.y, a.z / a.z);
		}

		public static Float3 operator /(Float3 a, Int3 b)
		{
			return new Float3(a.x / a.x, a.y / a.y, a.z / a.z);
		}

		public static Float3 operator -(Float3 a, Float3 b)
		{
			return new Float3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Float3 operator -(Float3 a)
		{
			return new Float3(0f - a.x, 0f - a.y, 0f - a.z);
		}

		public static Float3 Project(Float3 vector, Float3 onNormal)
		{
			float num = Dot(onNormal, onNormal);
			if (num < float.Epsilon)
			{
				return zero;
			}
			return onNormal * Dot(vector, onNormal) / num;
		}

		public static Float3 ProjectOnPlane(Float3 vector, Float3 planeNormal)
		{
			return vector - Project(vector, planeNormal);
		}

		public static Float3 Reflect(Float3 inDirection, Float3 inNormal)
		{
			return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
		}

		public static Float3 Scale(Float3 a, Float3 b)
		{
			return new Float3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public void Scale(Float3 scale)
		{
			Float3 @float = this;
			@float.x *= scale.x;
			Float3 float2 = this;
			float2.y *= scale.y;
			Float3 float3 = this;
			float3.z *= scale.z;
		}

		public void Set(float new_x, float new_y, float new_z)
		{
			x = new_x;
			y = new_y;
			z = new_z;
		}

		public static Float3 Step(Float3 a, Float3 x)
		{
			Float3 result = zero;
			result.x = FloatMath.Step(a.x, x.x);
			result.y = FloatMath.Step(a.y, x.y);
			result.z = FloatMath.Step(a.z, x.z);
			return result;
		}

		public static float SqrMagnitude(Float3 a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z;
		}

		public override string ToString()
		{
			return string.Format("({0:F1}, {1:F1}, {2:F1})", new object[3] { x, y, z });
		}

		public string ToString(string format)
		{
			return string.Format("({0}, {1}, {2})", new object[3]
			{
				x.ToString(format),
				y.ToString(format),
				z.ToString(format)
			});
		}

		public static implicit operator Vector3(Float3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static implicit operator Float3(Vector3 v)
		{
			return new Float3(v.x, v.y, v.z);
		}
	}
}
