/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Resample.Scalers;

namespace SpriteMaster.Resample;

interface IScalerInfo {
	Resample.Scaler Scaler { get; }
	int MinScale { get; }
	int MaxScale { get; }
	XNA.Graphics.TextureFilter Filter { get; }
	bool PremultiplyAlpha { get; }
	bool GammaCorrect { get; }
	bool BlockCompress { get; }

	IScaler Interface { get; }
}
