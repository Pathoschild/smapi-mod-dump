/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Resample.Scalers.xBRZ.Common;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.xBRZ.Blend;

using PreprocessType = Byte;

internal static class BlendInfo {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BlendType GetTopR(this PreprocessType b) => ((PreprocessType)(b >> 2) & 0x3).BlendType();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BlendType GetBottomR(this PreprocessType b) => ((PreprocessType)(b >> 4) & 0x3).BlendType();
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BlendType GetBottomL(this PreprocessType b) => ((PreprocessType)(b >> 6) & 0x3).BlendType();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void SetTopL(this ref PreprocessType b, BlendType bt) => b = (PreprocessType)(b | bt.Value());
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void SetTopR(this ref PreprocessType b, BlendType bt) => b = (PreprocessType)(b | bt.Value() << 2);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void SetBottomR(this ref PreprocessType b, BlendType bt) => b = (PreprocessType)(b | bt.Value() << 4);
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void SetBottomL(this ref PreprocessType b, BlendType bt) => b = (PreprocessType)(b | bt.Value() << 6);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool BlendingNeeded(this PreprocessType b) => b != 0;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PreprocessType Rotate(this PreprocessType b, RotationDegree rotDeg) {
		var l = (PreprocessType)((PreprocessType)rotDeg << 1);
		var r = (PreprocessType)(8 - l);

		return (PreprocessType)(b << l | b >> r);
	}
}
