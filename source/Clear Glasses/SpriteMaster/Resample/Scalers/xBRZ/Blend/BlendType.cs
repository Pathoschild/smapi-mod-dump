/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.xBRZ.Blend;

using PreprocessType = Byte;

internal enum BlendType : PreprocessType {
	// These blend types must fit into 2 bits.
	None = 0, //do not blend
	Normal = 1, //a normal indication to blend
	Dominant = 2 //a strong indication to blend
}

internal static class BlendTypeExtension {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PreprocessType Value(this BlendType type) => (byte)type;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BlendType BlendType(this PreprocessType value) => (BlendType)value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static BlendType BlendType(this int value) => (BlendType)value;
}
