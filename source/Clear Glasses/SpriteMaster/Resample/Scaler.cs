/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Resample;

internal enum Scaler : int {
	None = -1,
	xBRZ = 0,
#if !SHIPPING
	SuperXBR,
#endif
	EPX,
	ScaleX = EPX,
	EPXLegacy,
	xBREPX
}
