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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.SuperXBR.Cg;

partial struct Float4 {
	internal readonly unsafe ref Float3 XYZ => ref *(Float3*)Unsafe.AsPointer(ref Unsafe.AsRef(this));
	internal readonly ref Float3 RGB => ref XYZ;

	// 2 component
	internal readonly Float2 XX => (X, X);
	internal readonly Float2 XY => (X, Y);
	internal readonly Float2 XZ => (X, Z);
	internal readonly Float2 XW => (X, W);

	internal readonly Float2 YX => (Y, X);
	internal readonly Float2 YY => (Y, Y);
	internal readonly Float2 YZ => (Y, Z);
	internal readonly Float2 YW => (Y, W);

	internal readonly Float2 ZX => (Z, X);
	internal readonly Float2 ZY => (Z, Y);
	internal readonly Float2 ZZ => (Z, Z);
	internal readonly Float2 ZW => (Z, W);

	internal readonly Float2 WX => (W, X);
	internal readonly Float2 WY => (W, Y);
	internal readonly Float2 WZ => (W, Z);
	internal readonly Float2 WW => (W, W);
}
#endif
