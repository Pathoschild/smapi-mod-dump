/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[CLSCompliant(false)]
[DebuggerDisplay("[{Min} <-> {Max}}")]
[StructLayout(LayoutKind.Sequential, Pack = Vector2I.Alignment, Size = Vector2I.ByteSize)]
internal readonly struct ExtentI : IEquatable<ExtentI>, ILongHash {
	private readonly Vector2I Value;

	internal int Min => Value.X;
	internal int Max => Value.Y;

	internal bool IsValid => Min <= Max;

	internal int Length => Max - Min;

	internal ExtentI(int min, int max) : this() {
		min.AssertLessEqual(max);
		Value = new(min, max);
	}

	internal ExtentI(ExtentI value) : this(value.Min, value.Max) { }

	internal ExtentI((int Min, int Max) value) : this(value.Min, value.Max) { }

	internal bool ContainsInclusive(int value) => value.WithinInclusive(Min, Max);

	internal bool ContainsExclusive(int value) => value.WithinExclusive(Min, Max);

	internal bool Contains(int value) => value.Within(Min, Max);

	internal bool ContainsInclusive(ExtentI value) => value.Min >= Min && value.Max <= Max;

	internal bool ContainsExclusive(ExtentI value) => value.Min > Min && value.Max < Max;

	internal bool Contains(ExtentI value) => ContainsInclusive(value);

	public bool Equals(ExtentI other) => Value == other.Value;

	public override bool Equals(object? other) {
		switch (other) {
			case ExtentI value: return Equals(value);
			case ValueTuple<int, int> value: return Equals(new ExtentI(value));
			default: return false;
		}
	}

	public override int GetHashCode() => Value.GetHashCode();

	ulong ILongHash.GetLongHashCode() => Value.GetLongHashCode();

	public static bool operator ==(ExtentI left, ExtentI right) => Equals(left, right);

	public static bool operator !=(ExtentI left, ExtentI right) => !Equals(left, right);
}
