/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

#if !SHIPPING
using System.Numerics;
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample.Scalers.SuperXBR.Cg;

[StructLayout(LayoutKind.Sequential, Pack = sizeof(float), Size = sizeof(float) * 4)]
partial struct Float4 {
	private Vector4 Value = default;

	internal float X {
		readonly get => Value.X;
		set => Value.X = value;
	}

	internal float R {
		readonly get => Value.X;
		set => Value.X = value;
	}

	internal float Y {
		readonly get => Value.Y;
		set => Value.Y = value;
	}

	internal float G {
		readonly get => Value.Y;
		set => Value.Y = value;
	}

	internal float Z {
		readonly get => Value.Z;
		set => Value.Z = value;
	}

	internal float B {
		readonly get => Value.Z;
		set => Value.Z = value;
	}

	internal float W {
		readonly get => Value.W;
		set => Value.W = value;
	}

	internal float A {
		readonly get => Value.W;
		set => Value.W = value;
	}

	public Float4() { }

	internal Float4(Float4 value) : this(value.Value) { }
	internal Float4(Vector4 value) => Value = value;
	internal Float4(Vector3 xyz, float w) : this(new Vector4(xyz, w)) { }
	internal Float4(Float3 xyz, float w) : this(new Vector4(xyz.X, xyz.Y, xyz.Z, w)) { }
	internal Float4(float x, Vector3 yzw) : this(new Vector4(x, yzw.X, yzw.Y, yzw.Z)) { }
	internal Float4(float x, Float3 yzw) : this(new Vector4(x, yzw.X, yzw.Y, yzw.Z)) { }
	internal Float4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w)) { }
	internal Float4((float X, float Y, float Z, float W) value) : this(value.X, value.Y, value.Z, value.W) { }

	internal readonly float Dot(Float4 other) => Vector4.Dot(Value, other.Value);

	internal readonly Float4 Min(Float4 other) => Vector4.Min(Value, other.Value);
	internal readonly Float4 Max(Float4 other) => Vector4.Max(Value, other.Value);
	internal readonly Float4 Clamp(Float4 min, Float4 max) => Vector4.Clamp(Value, min.Value, max.Value);

	public static Float4 operator -(Float4 vec) => Vector4.Negate(vec.Value);
	public static Float4 operator +(Float4 vec) => vec;

	public static Float4 operator +(Float4 a, Float4 b) => Vector4.Add(a.Value, b.Value);
	public static Float4 operator -(Float4 a, Float4 b) => Vector4.Subtract(a.Value, b.Value);

	public static Float4 operator *(Float4 a, Float4 b) => Vector4.Multiply(a.Value, b.Value);
	public static Float4 operator *(Float4 a, float b) => Vector4.Multiply(a.Value, b);
	public static Float4 operator *(float a, Float4 b) => Vector4.Multiply(a, b.Value);
	public static Float4 operator /(Float4 a, Float4 b) => Vector4.Divide(a.Value, b.Value);
	public static Float4 operator /(Float4 a, float b) => Vector4.Divide(a.Value, b);

	public static implicit operator Float4(Vector4 value) => new(value);
	public static implicit operator Float4((float X, float Y, float Z, float W) value) => new(value);
}
#endif
