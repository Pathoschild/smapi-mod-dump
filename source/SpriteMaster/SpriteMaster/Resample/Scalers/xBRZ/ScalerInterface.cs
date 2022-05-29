/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;

namespace SpriteMaster.Resample.Scalers.xBRZ;

internal sealed partial class Scaler {
	internal sealed class ScalerInterface : IScaler {
		internal static readonly ScalerInterface Instance = new();

		public IScalerInfo Info => ScalerInfo.Instance;

		public uint MinScale => Scaler.MinScale;

		public uint MaxScale => Scaler.MaxScale;

		public uint ClampScale(uint scale) => Scaler.ClampScale(scale);

		public Span<Color16> Apply(
			in Resample.Scalers.Config configuration,
			uint scaleMultiplier,
			ReadOnlySpan<Color16> sourceData,
			Vector2I sourceSize,
			Span<Color16> targetData,
			Vector2I targetSize
		) =>
			Scaler.Apply(configuration as Config, scaleMultiplier, sourceData, sourceSize, targetData, targetSize);

		public Resample.Scalers.Config CreateConfig(Vector2B wrapped, bool hasAlpha, bool gammaCorrected) => new Config(
			wrapped: wrapped,
			hasAlpha: hasAlpha,
			luminanceWeight: SMConfig.Resample.Common.LuminanceWeight,
			gammaCorrected: gammaCorrected,
			equalColorTolerance: (uint)SMConfig.Resample.Common.EqualColorTolerance,
			dominantDirectionThreshold: SMConfig.Resample.xBRZ.DominantDirectionThreshold,
			steepDirectionThreshold: SMConfig.Resample.xBRZ.SteepDirectionThreshold,
			centerDirectionBias: SMConfig.Resample.xBRZ.CenterDirectionBias,
			useRedmean: SMConfig.Resample.UseRedmean
		);
	}
}
