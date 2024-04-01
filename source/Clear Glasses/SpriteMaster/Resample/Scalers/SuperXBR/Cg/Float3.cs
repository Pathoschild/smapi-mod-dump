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

[StructLayout(LayoutKind.Sequential, Pack = sizeof(float), Size = sizeof(float) * 3)]
partial struct Float3 {
	private Vector3 Value = default;

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

	public Float3() { }

	internal Float3(Float3 value) : this(value.Value) { }
	internal Float3(Vector3 value) => Value = value;
	internal Float3(float x, float y, float z) : this(new Vector3(x, y, z)) { }
	internal Float3((float X, float Y, float Z) value) : this(value.X, value.Y, value.Z) { }

	internal readonly float Dot(Float3 other) => Vector3.Dot(Value, other.Value);

	internal readonly Float3 Min(Float3 other) => Vector3.Min(Value, other.Value);
	internal readonly Float3 Max(Float3 other) => Vector3.Max(Value, other.Value);
	internal readonly Float3 Clamp(Float3 min, Float3 max) => Vector3.Clamp(Value, min.Value, max.Value);

	public static Float3 operator -(Float3 vec) => Vector3.Negate(vec.Value);
	public static Float3 operator +(Float3 vec) => vec;

	public static Float3 operator +(Float3 a, Float3 b) => Vector3.Add(a.Value, b.Value);
	public static Float3 operator -(Float3 a, Float3 b) => Vector3.Subtract(a.Value, b.Value);

	public static Float3 operator *(Float3 a, Float3 b) => Vector3.Multiply(a.Value, b.Value);
	public static Float3 operator *(Float3 a, float b) => Vector3.Multiply(a.Value, b);
	public static Float3 operator *(float a, Float3 b) => Vector3.Multiply(a, b.Value);
	public static Float3 operator /(Float3 a, Float3 b) => Vector3.Divide(a.Value, b.Value);
	public static Float3 operator /(Float3 a, float b) => Vector3.Divide(a.Value, b);

	public static implicit operator Float3(Vector3 value) => new(value);
	public static implicit operator Float3((float X, float Y, float Z) value) => new(value);
}
#endif
