/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace MusicMaster.Extensions;

internal static class SizesExt {
	private const MethodImplOptions Aggressive =
		MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

	internal const ulong KiB = 1024UL;
	internal const ulong MiB = KiB * 1024UL;
	internal const ulong GiB = MiB * 1024UL;

	#region AsKiB

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static int AsKiB(sbyte size) =>
		size * SizesExt<int>.KiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static long AsKiB(int size) =>
		size * SizesExt<long>.KiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static long AsKiB(long size) =>
		size * SizesExt<long>.KiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static uint AsKiB(byte size) =>
		size * SizesExt<uint>.KiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static ulong AsKiB(uint size) =>
		size * SizesExt<ulong>.KiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static ulong AsKiB(ulong size) =>
		size * SizesExt<ulong>.KiB;

	#endregion

	#region AsMiB

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static int AsMiB(sbyte size) =>
		size * SizesExt<int>.MiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static long AsMiB(int size) =>
		size * SizesExt<long>.MiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static long AsMiB(long size) =>
		size * SizesExt<long>.MiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static uint AsMiB(byte size) =>
		size * SizesExt<uint>.MiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static ulong AsMiB(uint size) =>
		size * SizesExt<ulong>.MiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static ulong AsMiB(ulong size) =>
		size * SizesExt<ulong>.MiB;

	#endregion

	#region AsGiB

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static int AsGiB(sbyte size) =>
		size * SizesExt<int>.GiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static long AsGiB(int size) =>
		size * SizesExt<long>.GiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static long AsGiB(long size) =>
		size * SizesExt<long>.GiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static uint AsGiB(byte size) =>
		size * SizesExt<uint>.GiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static ulong AsGiB(uint size) =>
		size * SizesExt<ulong>.GiB;

	[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
	internal static ulong AsGiB(ulong size) =>
		size * SizesExt<ulong>.GiB;

	#endregion
}

internal static class SizesExt<T> where T : unmanaged {
	private const MethodImplOptions Aggressive =
		MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

	internal static T KiB {
		[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
		get {
			if (typeof(T) == typeof(sbyte)) {
				return (T)(object)unchecked((sbyte)SizesExt.KiB);
			}
			if (typeof(T) == typeof(byte)) {
				return (T)(object)unchecked((byte)SizesExt.KiB);
			}
			if (typeof(T) == typeof(short)) {
				return (T)(object)unchecked((short)SizesExt.KiB);
			}
			if (typeof(T) == typeof(ushort)) {
				return (T)(object)unchecked((ushort)SizesExt.KiB);
			}
			if (typeof(T) == typeof(int)) {
				return (T)(object)unchecked((int)SizesExt.KiB);
			}
			if (typeof(T) == typeof(uint)) {
				return (T)(object)unchecked((uint)SizesExt.KiB);
			}
			if (typeof(T) == typeof(long)) {
				return (T)(object)unchecked((long)SizesExt.KiB);
			}
			if (typeof(T) == typeof(ulong)) {
				return (T)(object)unchecked((ulong)SizesExt.KiB);
			}
			if (typeof(T) == typeof(float)) {
				return (T)(object)unchecked((float)SizesExt.KiB);
			}
			if (typeof(T) == typeof(double)) {
				return (T)(object)unchecked((double)SizesExt.KiB);
			}
			return (T)(object)SizesExt.KiB;
		}
	}

	internal static T MiB {
		[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
		get {
			if (typeof(T) == typeof(sbyte)) {
				return (T)(object)unchecked((sbyte)SizesExt.MiB);
			}
			if (typeof(T) == typeof(byte)) {
				return (T)(object)unchecked((byte)SizesExt.MiB);
			}
			if (typeof(T) == typeof(short)) {
				return (T)(object)unchecked((short)SizesExt.MiB);
			}
			if (typeof(T) == typeof(ushort)) {
				return (T)(object)unchecked((ushort)SizesExt.MiB);
			}
			if (typeof(T) == typeof(int)) {
				return (T)(object)unchecked((int)SizesExt.MiB);
			}
			if (typeof(T) == typeof(uint)) {
				return (T)(object)unchecked((uint)SizesExt.MiB);
			}
			if (typeof(T) == typeof(long)) {
				return (T)(object)unchecked((long)SizesExt.MiB);
			}
			if (typeof(T) == typeof(ulong)) {
				return (T)(object)unchecked((ulong)SizesExt.MiB);
			}
			if (typeof(T) == typeof(float)) {
				return (T)(object)unchecked((float)SizesExt.MiB);
			}
			if (typeof(T) == typeof(double)) {
				return (T)(object)unchecked((double)SizesExt.MiB);
			}
			return (T)(object)SizesExt.MiB;
		}
	}

	internal static T GiB {
		[Pure, MustUseReturnValue, MethodImpl(Aggressive)]
		get {
			if (typeof(T) == typeof(sbyte)) {
				return (T)(object)unchecked((sbyte)SizesExt.GiB);
			}
			if (typeof(T) == typeof(byte)) {
				return (T)(object)unchecked((byte)SizesExt.GiB);
			}
			if (typeof(T) == typeof(short)) {
				return (T)(object)unchecked((short)SizesExt.GiB);
			}
			if (typeof(T) == typeof(ushort)) {
				return (T)(object)unchecked((ushort)SizesExt.GiB);
			}
			if (typeof(T) == typeof(int)) {
				return (T)(object)unchecked((int)SizesExt.GiB);
			}
			if (typeof(T) == typeof(uint)) {
				return (T)(object)unchecked((uint)SizesExt.GiB);
			}
			if (typeof(T) == typeof(long)) {
				return (T)(object)unchecked((long)SizesExt.GiB);
			}
			if (typeof(T) == typeof(ulong)) {
				return (T)(object)unchecked((ulong)SizesExt.GiB);
			}
			if (typeof(T) == typeof(float)) {
				return (T)(object)unchecked((float)SizesExt.GiB);
			}
			if (typeof(T) == typeof(double)) {
				return (T)(object)unchecked((double)SizesExt.GiB);
			}
			return (T)(object)SizesExt.GiB;
		}
	}
}

