/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if !SHIPPING
using SpriteMaster.Types;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample.Scalers.SuperXBR.Cg;

[StructLayout(LayoutKind.Sequential, Pack = sizeof(float), Size = sizeof(float) * 2)]
partial struct Float2 {
	private Vector2 Value = default;

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

	internal Float2(in Float2 value) : this(value.Value) { }
	internal Float2(in Vector2 value) => Value = value;
	internal Float2(float x, float y) : this(new Vector2(x, y)) { }
	internal Float2(in (float X, float Y) value) : this(value.X, value.Y) { }

	internal readonly float Dot(in Float2 other) => Vector2.Dot(Value, other.Value);

	internal readonly Float2 Min(in Float2 other) => Vector2.Min(Value, other.Value);
	internal readonly Float2 Max(in Float2 other) => Vector2.Max(Value, other.Value);
	internal readonly Float2 Clamp(in Float2 min, in Float2 max) => Vector2.Clamp(Value, min.Value, max.Value);

	public static Float2 operator -(in Float2 vec) => Vector2.Negate(vec.Value);
	public static Float2 operator +(in Float2 vec) => vec;

	public static Float2 operator +(in Float2 a, in Float2 b) => Vector2.Add(a.Value, b.Value);
	public static Float2 operator -(in Float2 a, in Float2 b) => Vector2.Subtract(a.Value, b.Value);

	public static Float2 operator *(in Float2 a, in Float2 b) => Vector2.Multiply(a.Value, b.Value);
	public static Float2 operator *(in Float2 a, float b) => Vector2.Multiply(a.Value, b);
	public static Float2 operator *(float a, in Float2 b) => Vector2.Multiply(a, b.Value);
	public static Float2 operator /(in Float2 a, in Float2 b) => Vector2.Divide(a.Value, b.Value);
	public static Float2 operator /(in Float2 a, float b) => Vector2.Divide(a.Value, b);

	public static implicit operator Float2(in Vector2 value) => new(value);

	public static implicit operator Float2(in (float X, float Y) value) => new(value);
}
#endif
