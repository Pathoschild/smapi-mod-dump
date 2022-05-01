/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Configuration.Preview;

internal class Override {
	internal static Override? Instance = null;

	internal bool Enabled = false;
	internal bool ResampleEnabled = false;
	internal Resample.Scaler Scaler = Resample.Scaler.None;
	internal Resample.Scaler ScalerGradient = Resample.Scaler.None;
	internal bool ResampleSprites = false;
	internal bool ResampleText = false;
	internal bool ResampleBasicText = false;

	internal static Override FromConfig => new() {
		Enabled = Config.IsUnconditionallyEnabled,
#pragma warning disable CS0618 // Type or member is obsolete
		ResampleEnabled = Config.Resample.Enabled,
#pragma warning restore CS0618 // Type or member is obsolete
		Scaler = Config.Resample.Scaler,
		ScalerGradient = Config.Resample.ScalerGradient,
		ResampleSprites = Config.Resample.EnabledSprites,
		ResampleText = Config.Resample.EnabledText,
		ResampleBasicText = Config.Resample.EnabledBasicText
	};
}
