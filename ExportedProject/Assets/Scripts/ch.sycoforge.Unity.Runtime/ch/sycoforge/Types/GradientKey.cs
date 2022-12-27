using System;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	[ComVisible(true)]
	public struct GradientKey
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.Struct)]
		public Float4 Color;

		[FieldOffset(16)]
		[MarshalAs(UnmanagedType.R4)]
		public float Position;

		[FieldOffset(20)]
		[MarshalAs(UnmanagedType.Struct)]
		public Float3 Data;

		public GradientKey(Float4 color, float position)
		{
			Color = color;
			Position = position;
			Data = default(Float3);
		}
	}
}
