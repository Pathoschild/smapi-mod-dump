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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[StructLayout(LayoutKind.Auto)]
internal readonly struct Limit<T> where T : struct, IComparable<T> {
	internal readonly T Min { get; init; }
	internal readonly T Max { get; init; }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Limit(in T min, in T max) {
		Min = min;
		Max = max;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool IsWithin(in T value) =>
		DetailImpl.IsWithin(this, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool IsWithinInclusive(in T value) =>
		DetailImpl.IsWithinInclusive(this, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly bool IsWithinExclusive(in T value) =>
		DetailImpl.IsWithinExclusive(this, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly T Clamp(in T value) =>
		DetailImpl.Clamp(this, value);

	private static class DetailImpl {
		// Validated that this inlines as expected with LINQPad.
		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static bool IsWithin(in Limit<T> limit, in T value) =>
			value switch {
				ulong val => val.Within((ulong)(object)limit.Min, (ulong)(object)limit.Max),
				long val => val.Within((long)(object)limit.Min, (long)(object)limit.Max),
				uint val => val.Within((uint)(object)limit.Min, (uint)(object)limit.Max),
				int val => val.Within((int)(object)limit.Min, (int)(object)limit.Max),
				ushort val => val.Within((ushort)(object)limit.Min, (ushort)(object)limit.Max),
				short val => val.Within((short)(object)limit.Min, (short)(object)limit.Max),
				byte val => val.Within((byte)(object)limit.Min, (byte)(object)limit.Max),
				sbyte val => val.Within((sbyte)(object)limit.Min, (sbyte)(object)limit.Max),
				Vector2I val => val.Within((Vector2I)(object)limit.Min, (Vector2I)(object)limit.Max),
				Vector2F val => val.Within((Vector2F)(object)limit.Min, (Vector2F)(object)limit.Max),
				TimeSpan val => val.Within((TimeSpan)(object)limit.Min, (TimeSpan)(object)limit.Max),
				_ => value.CompareTo(limit.Min) >= 0 && value.CompareTo(limit.Max) <= 0
			};

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static bool IsWithinInclusive(in Limit<T> limit, in T value) =>
			IsWithin(limit, value);

		// Validated that this inlines as expected with LINQPad.
		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static bool IsWithinExclusive(in Limit<T> limit, in T value) =>
			value switch {
				ulong val => val.WithinExclusive((ulong)(object)limit.Min, (ulong)(object)limit.Max),
				long val => val.WithinExclusive((long)(object)limit.Min, (long)(object)limit.Max),
				uint val => val.WithinExclusive((uint)(object)limit.Min, (uint)(object)limit.Max),
				int val => val.WithinExclusive((int)(object)limit.Min, (int)(object)limit.Max),
				ushort val => val.WithinExclusive((ushort)(object)limit.Min, (ushort)(object)limit.Max),
				short val => val.WithinExclusive((short)(object)limit.Min, (short)(object)limit.Max),
				byte val => val.WithinExclusive((byte)(object)limit.Min, (byte)(object)limit.Max),
				sbyte val => val.WithinExclusive((sbyte)(object)limit.Min, (sbyte)(object)limit.Max),
				Vector2I val => val.WithinExclusive((Vector2I)(object)limit.Min, (Vector2I)(object)limit.Max),
				Vector2F val => val.WithinExclusive((Vector2F)(object)limit.Min, (Vector2F)(object)limit.Max),
				TimeSpan val => val.WithinExclusive((TimeSpan)(object)limit.Min, (TimeSpan)(object)limit.Max),
				_ => value.CompareTo(limit.Min) > 0 && value.CompareTo(limit.Max) < 0
			};

		// Validated that this inlines as expected with LINQPad.
		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static T Clamp(in Limit<T> limit, in T value) =>
			value switch {
				ulong val => (T)(object)val.Clamp((ulong)(object)limit.Min, (ulong)(object)limit.Max),
				long val => (T)(object)val.Clamp((long)(object)limit.Min, (long)(object)limit.Max),
				uint val => (T)(object)val.Clamp((uint)(object)limit.Min, (uint)(object)limit.Max),
				int val => (T)(object)val.Clamp((int)(object)limit.Min, (int)(object)limit.Max),
				ushort val => (T)(object)val.Clamp((ushort)(object)limit.Min, (ushort)(object)limit.Max),
				short val => (T)(object)val.Clamp((short)(object)limit.Min, (short)(object)limit.Max),
				byte val => (T)(object)val.Clamp((byte)(object)limit.Min, (byte)(object)limit.Max),
				sbyte val => (T)(object)val.Clamp((sbyte)(object)limit.Min, (sbyte)(object)limit.Max),
				Vector2I val => (T)(object)val.Clamp((Vector2I)(object)limit.Min, (Vector2I)(object)limit.Max),
				Vector2F val => (T)(object)val.Clamp((Vector2F)(object)limit.Min, (Vector2F)(object)limit.Max),
				TimeSpan val => (T)(object)val.Clamp((TimeSpan)(object)limit.Min, (TimeSpan)(object)limit.Max),
				_ => (value.CompareTo(limit.Min) < 0) ? limit.Min : (value.CompareTo(limit.Max) > 0) ? limit.Max : value
			};
	}
}
