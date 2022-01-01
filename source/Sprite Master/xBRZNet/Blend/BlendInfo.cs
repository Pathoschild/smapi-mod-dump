/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.xBRZ.Common;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Blend;

static class BlendInfo {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BlendType GetTopL(this byte b) => (b & 0x3).BlendType();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BlendType GetTopR(this byte b) => ((byte)(b >> 2) & 0x3).BlendType();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BlendType GetBottomR(this byte b) => ((byte)(b >> 4) & 0x3).BlendType();
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static BlendType GetBottomL(this byte b) => ((byte)(b >> 6) & 0x3).BlendType();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte SetTopL(this byte b, BlendType bt) => (byte)(b | bt.Byte());
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte SetTopR(this byte b, BlendType bt) => (byte)(b | (bt.Byte() << 2));
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte SetBottomR(this byte b, BlendType bt) => (byte)(b | (bt.Byte() << 4));
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte SetBottomL(this byte b, BlendType bt) => (byte)(b | (bt.Byte() << 6));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool BlendingNeeded(this byte b) => b != 0;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte Rotate(this byte b, RotationDegree rotDeg) {
		var l = (byte)((byte)rotDeg << 1);
		var r = (byte)(8 - l);

		return (byte)(b << l | b >> r);
	}
}
