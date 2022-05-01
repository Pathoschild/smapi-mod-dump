/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.InteropServices;

namespace SpriteMaster.Types.Volatile;

[StructLayout(LayoutKind.Sequential, Pack = sizeof(bool), Size = sizeof(bool))]
partial struct VolatileBool : ILongHash {
	private volatile bool _Value = default;

	internal bool Value {
		readonly get => _Value;
		set => _Value = value;
	}

	public VolatileBool() { }
	internal VolatileBool(bool value) => _Value = value;
	internal VolatileBool(in VolatileBool value) : this(value._Value) { }

	public static implicit operator bool(in VolatileBool value) => value._Value;
	public static implicit operator VolatileBool(bool value) => new(value);

	public static bool operator ==(in VolatileBool lhs, in VolatileBool rhs) => lhs.Value == rhs.Value;
	public static bool operator !=(in VolatileBool lhs, in VolatileBool rhs) => lhs.Value != rhs.Value;

	public static bool operator ==(in VolatileBool lhs, bool rhs) => lhs.Value == rhs;
	public static bool operator !=(in VolatileBool lhs, bool rhs) => lhs.Value != rhs;

	public static bool operator ==(bool lhs, in VolatileBool rhs) => lhs == rhs.Value;
	public static bool operator !=(bool lhs, in VolatileBool rhs) => lhs != rhs.Value;

	public static bool operator !(in VolatileBool value) => !value.Value;

	public override readonly bool Equals(object? obj) => obj switch {
		VolatileBool other => this == other,
		bool other => this == other,
		_ => false
	};

	internal readonly bool Equals(in VolatileBool other) => this == other;
	internal readonly bool Equals(bool other) => this == other;

	public override readonly int GetHashCode() => Value.GetHashCode();
	readonly ulong ILongHash.GetLongHashCode() => Value.GetLongHashCode();

	public override readonly string ToString() => Value.ToString();
}
