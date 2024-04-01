/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[DebuggerDisplay("[{Bytes[0]}, {Bytes[1]}, {Bytes[2]}, {Bytes[3]}]")]
[DebuggerDisplay("[{Shorts[0]}, {Shorts[1]}]")]
[DebuggerDisplay("{Packed}")]
[StructLayout(LayoutKind.Sequential, Pack = sizeof(uint), Size = sizeof(uint))]
internal struct PackedUInt {
	internal uint Packed;

	internal readonly unsafe Span<byte> Bytes => Reinterpret.ReinterpretAsSpanUnsafe<uint, byte>(Packed);

	internal readonly unsafe Span<ushort> Shorts => Reinterpret.ReinterpretAsSpanUnsafe<uint, ushort>(Packed);

	// TODO : this isn't ideal as it will initialize 'Packed' before setting its values
	internal PackedUInt(byte b0, byte b1, byte b2, byte b3) : this() {
		Bytes[0] = b0;
		Bytes[1] = b1;
		Bytes[2] = b2;
		Bytes[3] = b3;
	}

	internal PackedUInt(ushort s0, ushort s1) : this() {
		Shorts[0] = s0;
		Shorts[1] = s1;
	}

	internal PackedUInt(uint packed) {
		Packed = packed;
	}

	public static implicit operator uint(PackedUInt value) => value.Packed;
	public static implicit operator PackedUInt(uint value) => new(value);
}
