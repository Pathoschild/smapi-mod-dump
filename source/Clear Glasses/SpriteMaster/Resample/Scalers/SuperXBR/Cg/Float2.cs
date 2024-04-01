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
using SpriteMaster.Types;
using System.Runtime.InteropServices;

using SystemVector2 = System.Numerics.Vector2;

namespace SpriteMaster.Resample.Scalers.SuperXBR.Cg;

[StructLayout(LayoutKind.Sequential, Pack = sizeof(float), Size = sizeof(float) * 2)]
partial struct Float2 {
	private SystemVector2 Value = default;

	internal float X {
		readonly get => Value.X;
		set => Value.X = value;
	}

	internal float Y {
		readonly get => Value.Y;
		set => Value.Y = value;
	}

	internal readonly Vector2I AsInt => ((int)X, (int)Y);

	public Float2() { }

	internal Float2(Float2 value) : this(value.Value) { }
	internal Float2(SystemVector2 value) => Value = value;
	internal Float2(float x, float y) : this(new SystemVector2(x, y)) { }
	internal Float2((float X, float Y) value) : this(value.X, value.Y) { }

	internal readonly float Dot(Float2 other) => SystemVector2.Dot(Value, other.Value);

	internal readonly Float2 Min(Float2 other) => SystemVector2.Min(Value, other.Value);
	internal readonly Float2 Max(Float2 other) => SystemVector2.Max(Value, other.Value);
	internal readonly Float2 Clamp(Float2 min, Float2 max) => SystemVector2.Clamp(Value, min.Value, max.Value);

	public static Float2 operator -(Float2 vec) => SystemVector2.Negate(vec.Value);
	public static Float2 operator +(Float2 vec) => vec;

	public static Float2 operator +(Float2 a, Float2 b) => SystemVector2.Add(a.Value, b.Value);
	public static Float2 operator -(Float2 a, Float2 b) => SystemVector2.Subtract(a.Value, b.Value);

	public static Float2 operator *(Float2 a, Float2 b) => SystemVector2.Multiply(a.Value, b.Value);
	public static Float2 operator *(Float2 a, float b) => SystemVector2.Multiply(a.Value, b);
	public static Float2 operator *(float a, Float2 b) => SystemVector2.Multiply(a, b.Value);
	public static Float2 operator /(Float2 a, Float2 b) => SystemVector2.Divide(a.Value, b.Value);
	public static Float2 operator /(Float2 a, float b) => SystemVector2.Divide(a.Value, b);

	public static implicit operator Float2(SystemVector2 value) => new(value);

	public static implicit operator Float2((float X, float Y) value) => new(value);
}
#endif
