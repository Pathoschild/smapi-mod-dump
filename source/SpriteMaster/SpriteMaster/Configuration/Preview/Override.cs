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
	internal Resample.Scaler ScalerPortrait = Resample.Scaler.None;
	internal Resample.Scaler ScalerText = Resample.Scaler.None;
	internal Resample.Scaler ScalerGradient = Resample.Scaler.None;
	internal bool ResampleSprites = false;
	internal bool ResamplePortraits = false;
	internal bool ResampleLargeText = false;
	internal bool ResampleSmallText = false;

	// draw state
	internal bool SetLinearUnresampled = false;
	internal bool SetLinear = true;

#pragma warning disable CS0618 // Type or member is obsolete
	internal static Override FromConfig => new() {
		Enabled = Config.IsUnconditionallyEnabled,
		ResampleEnabled = Config.Resample.Enabled,
		Scaler = Config.Resample.Scaler,
		ScalerPortrait = Config.Resample.ScalerPortrait,
		ScalerText = Config.Resample.ScalerText,
		ScalerGradient = Config.Resample.ScalerGradient,
		ResampleSprites = Config.Resample.EnabledSprites,
		ResamplePortraits = Config.Resample.EnabledPortraits,
		ResampleLargeText = Config.Resample.EnabledLargeText,
		ResampleSmallText = Config.Resample.EnabledSmallText,

		SetLinearUnresampled = Config.DrawState.SetLinearUnresampled,
		SetLinear = Config.DrawState.SetLinear
	};
#pragma warning restore CS0618 // Type or member is obsolete
}
