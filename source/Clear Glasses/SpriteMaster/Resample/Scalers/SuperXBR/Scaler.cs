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
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.SuperXBR;

internal sealed partial class Scaler : AbstractScaler<Config, Scaler.ValueScale> {
	private const uint MinScale = 2;
	private const uint MaxScale = Config.MaxScale;

	internal readonly struct ValueScale : IScale {
		public readonly uint Minimum => MinScale;
		public readonly uint Maximum => MaxScale;
	}

	private static uint ClampScale(uint scale) => 2;// Math.Clamp((uint)MathExt.RoundToInt(Math.Pow(Math.Ceiling(Math.Log2(scale)), 2)), MinScale, MaxScale);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static Span<Color16> Apply(
		Config config,
		uint scaleMultiplier,
		ReadOnlySpan<Color16> sourceData,
		Vector2I sourceSize,
		Span<Color16> targetData,
		Vector2I targetSize
	) {
		Common.ApplyValidate(config, scaleMultiplier, sourceData, sourceSize, ref targetData, targetSize);

		var scalerInstance = new Scaler(
			configuration: config,
			scaleMultiplier: scaleMultiplier,
			sourceSize: sourceSize,
			targetSize: targetSize
		);

		scalerInstance.Scale(sourceData, targetData);
		return targetData;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private Scaler(
		Config configuration,
		uint scaleMultiplier,
		Vector2I sourceSize,
		Vector2I targetSize
	) : base(configuration, scaleMultiplier, sourceSize, targetSize) {
	}

	private void Scale(ReadOnlySpan<Color16> source, Span<Color16> target) {
		if (ScaleMultiplier == 1) {
			source.CopyTo(target);
			return;
		}

		ReadOnlySpan<Color16> currentSource = source;
		Vector2I currentSourceSize = SourceSize;
		Vector2I currentTargetSize = SourceSize;
		// Run the scaling algorithm into a temporary buffer for each scaling up until the final one
		for (uint currentScale = ScaleMultiplier; currentScale > 2U; currentScale >>= 1) {
			currentTargetSize <<= 1;
			var currentTarget = SpanExt.Make<Color16>(currentTargetSize.Area);

			Scale(currentSource, currentSourceSize, currentTarget, currentTargetSize);

			currentSource = currentTarget;
			currentSourceSize = currentTargetSize;
		}

		// Once the scale multiplier is just 2, we end up here.
		Scale(currentSource, currentSourceSize, target, TargetSize);
	}
}
#endif
