using System;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	[Serializable]
	public struct Int3
	{
		[MarshalAs(UnmanagedType.I4)]
		public int x;

		[MarshalAs(UnmanagedType.I4)]
		public int y;

		[MarshalAs(UnmanagedType.I4)]
		public int z;

		public static Int3 back
		{
			get
			{
				return new Int3(0, 0, -1);
			}
		}

		public static Int3 down
		{
			get
			{
				return new Int3(0, -1, 0);
			}
		}

		public static Int3 forward
		{
			get
			{
				return new Int3(0, 0, 1);
			}
		}

		public int this[int index]
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
					throw new IndexOutOfRangeException("Invalid Int3 index!");
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
					throw new IndexOutOfRangeException("Invalid Int3 index!");
				}
			}
		}

		public static Int3 left
		{
			get
			{
				return new Int3(-1, 0, 0);
			}
		}

		public float magnitude
		{
			get
			{
				return (float)Math.Sqrt(x * x + y * y + z * z);
			}
		}

		public static Int3 one
		{
			get
			{
				return new Int3(1, 1, 1);
			}
		}

		public static Int3 right
		{
			get
			{
				return new Int3(1, 0, 0);
			}
		}

		public float sqrMagnitude
		{
			get
			{
				return x * x + y * y + z * z;
			}
		}

		public static Int3 up
		{
			get
			{
				return new Int3(0, 1, 0);
			}
		}

		public static Int3 zero
		{
			get
			{
				return new Int3(0, 0, 0);
			}
		}

		public Int3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static Int3 Cross(Int3 lhs, Int3 rhs)
		{
			return new Int3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public static float Distance(Int3 a, Int3 b)
		{
			Int3 @int = new Int3(a.x - b.x, a.y - b.y, a.z - b.z);
			return (float)Math.Sqrt(@int.x * @int.x + @int.y * @int.y + @int.z * @int.z);
		}

		public static float Dot(Int3 lhs, Int3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public override bool Equals(object other)
		{
			if (!(other is Int3))
			{
				return false;
			}
			Int3 @int = (Int3)other;
			return x.Equals(@int.x) && y.Equals(@int.y) && z.Equals(@int.z);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
		}

		public static float Magnitude(Int3 a)
		{
			return (float)Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
		}

		public static Int3 Max(Int3 lhs, Int3 rhs)
		{
			return new Int3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
		}

		public static Int3 Min(Int3 lhs, Int3 rhs)
		{
			return new Int3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
		}

		public static Int3 operator +(Int3 a, Int3 b)
		{
			return new Int3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Int3 operator /(Int3 a, int d)
		{
			return new Int3(a.x / d, a.y / d, a.z / d);
		}

		public static bool operator ==(Int3 lhs, Int3 rhs)
		{
			return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
		}

		public static bool operator !=(Int3 lhs, Int3 rhs)
		{
			return SqrMagnitude(lhs - rhs) >= 9.9999994E-11f;
		}

		public static Int3 operator *(Int3 a, int d)
		{
			return new Int3(a.x * d, a.y * d, a.z * d);
		}

		public static Int3 operator *(int d, Int3 a)
		{
			return new Int3(a.x * d, a.y * d, a.z * d);
		}

		public static Int3 operator -(Int3 a, Int3 b)
		{
			return new Int3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Int3 operator -(Int3 a)
		{
			return new Int3(-a.x, -a.y, -a.z);
		}

		public static Int3 Scale(Int3 a, Int3 b)
		{
			return new Int3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public void Scale(Int3 scale)
		{
			Int3 @int = this;
			@int.x *= scale.x;
			Int3 int2 = this;
			int2.y *= scale.y;
			Int3 int3 = this;
			int3.z *= scale.z;
		}

		public void Set(int new_x, int new_y, int new_z)
		{
			x = new_x;
			y = new_y;
			z = new_z;
		}

		public static float SqrMagnitude(Int3 a)
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
	}
}
