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
namespace SpriteMaster.Resample.Scalers.SuperXBR.Cg;

ref struct DiffTexel {
	internal readonly float YUV;
	internal readonly float Alpha;

	internal DiffTexel(float yuv, float alpha) {
		YUV = yuv;
		Alpha = alpha;
	}
}
#endif
