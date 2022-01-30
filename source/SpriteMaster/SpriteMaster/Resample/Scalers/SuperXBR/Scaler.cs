/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.SuperXBR;

sealed partial class Scaler {
	private const uint MinScale = 2;
	private const uint MaxScale = Config.MaxScale;

	private static uint ClampScale(uint scale) => 2;// Math.Clamp((uint)MathExt.RoundToInt(Math.Pow(Math.Ceiling(Math.Log2(scale)), 2)), MinScale, MaxScale);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static Span<Color16> Apply(
		Config? config,
		uint scaleMultiplier,
		ReadOnlySpan<Color16> sourceData,
		Vector2I sourceSize,
		Span<Color16> targetData,
		Vector2I targetSize
	) {
		if (config is null) {
			throw new ArgumentNullException(nameof(config));
		}

		if (scaleMultiplier < MinScale || scaleMultiplier > MaxScale || !NumericsExt.IsPow2(scaleMultiplier)) {
			throw new ArgumentOutOfRangeException(nameof(scaleMultiplier));
		}

		if (sourceSize.X * sourceSize.Y > sourceData.Length) {
			throw new ArgumentOutOfRangeException(nameof(sourceData));
		}

		var targetSizeCalculated = sourceSize * scaleMultiplier;
		if (targetSize != targetSizeCalculated) {
			throw new ArgumentOutOfRangeException(nameof(targetSize));
		}

		if (targetData == Span<Color16>.Empty) {
			targetData = SpanExt.MakeUninitialized<Color16>(targetSize.Area);
		}
		else {
			if (targetSize.Area > targetData.Length) {
				throw new ArgumentOutOfRangeException(nameof(targetData));
			}
		}

		var scalerInstance = new Scaler(
			configuration: in config,
			scaleMultiplier: scaleMultiplier,
			sourceSize: sourceSize,
			targetSize: targetSize
		);

		scalerInstance.Scale(sourceData, targetData);
		return targetData;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private Scaler(
		in Config configuration,
		uint scaleMultiplier,
		Vector2I sourceSize,
		Vector2I targetSize
	) {
		/*
		if (sourceData == null) {
			throw new ArgumentNullException(nameof(sourceData));
		}
		if (targetData == null) {
			throw new ArgumentNullException(nameof(targetData));
		}
		*/
		if (sourceSize.X <= 0 || sourceSize.Y <= 0) {
			throw new ArgumentOutOfRangeException(nameof(sourceSize));
		}

		ScaleMultiplier = scaleMultiplier;
		Configuration = configuration;
		SourceSize = sourceSize;
		TargetSize = targetSize;
	}

	private readonly Config Configuration;

	private readonly uint ScaleMultiplier;
	private readonly Vector2I SourceSize;
	private readonly Vector2I TargetSize;

	[MethodImpl(Runtime.MethodImpl.Hot)]
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
			var currentTarget = SpanExt.MakeUninitialized<Color16>(currentTargetSize.Area);

			Scale(currentSource, currentSourceSize, currentTarget, currentTargetSize);

			currentSource = currentTarget;
			currentSourceSize = currentTargetSize;
		}

		// Once the scale multiplier is just 2, we end up here.
		Scale(currentSource, currentSourceSize, target, TargetSize);
	}
}
