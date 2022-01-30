/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Resample.Scalers.SuperXBR.Cg;

partial struct Float3 {
	internal readonly Float4 XYZ0 => new(Value, 0.0f);
	internal readonly Float4 RGB0 => new(Value, 0.0f);
	internal readonly Float4 XYZ1 => new(Value, 1.0f);
	internal readonly Float4 RGB1 => new(Value, 1.0f);
}
