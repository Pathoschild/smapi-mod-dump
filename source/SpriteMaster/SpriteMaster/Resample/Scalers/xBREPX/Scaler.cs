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

namespace SpriteMaster.Resample.Scalers.xBREPX;

internal sealed partial class Scaler : AbstractScaler<Config, Scaler.ValueScale> {
	private const uint MinScale = 2;
	private const uint MaxScale = Config.MaxScale;

	internal readonly struct ValueScale : IScale {
		public readonly uint Minimum => MinScale;
		public readonly uint Maximum => MaxScale;
	}

	private static uint ClampScale(uint scale) => Math.Clamp(scale, MinScale, MaxScale);

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
			configuration: in config,
			scaleMultiplier: scaleMultiplier,
			sourceSize: sourceSize,
			targetSize: targetSize
		);

		scalerInstance.Scale(sourceData, targetData);
		return targetData;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private Scaler(
		in Config configuration,
		uint scaleMultiplier,
		Vector2I sourceSize,
		Vector2I targetSize
	) : base(configuration, scaleMultiplier, sourceSize, targetSize) {
	}

	// https://en.wikipedia.org/wiki/Pixel-art_scaling_algorithms#EPX/Scale2%C3%97/AdvMAME2%C3%97
	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void Scale(ReadOnlySpan<Color16> source, Span<Color16> destination) {
		{
			var scaler = xBRZ.Scaler.ScalerInterface.Instance;
			var config = scaler.CreateConfig(
				wrapped: Configuration.Wrapped,
				hasAlpha: Configuration.HasAlpha,
				gammaCorrected: Configuration.GammaCorrected
			);

			destination = scaler.Apply(
				configuration: config,
				scaleMultiplier: ScaleMultiplier,
				sourceData: source,
				sourceSize: SourceSize,
				targetSize: TargetSize,
				targetData: destination
			);
		}

		var epxData = SpanExt.Make<Color16>(destination.Length);

		{
			var scaler = EPX.Scaler.ScalerInterface.Instance;
			var config = scaler.CreateConfig(
				wrapped: Configuration.Wrapped,
				hasAlpha: Configuration.HasAlpha,
				gammaCorrected: Configuration.GammaCorrected
			);

			epxData = scaler.Apply(
				configuration: config,
				scaleMultiplier: ScaleMultiplier,
				sourceData: source,
				sourceSize: SourceSize,
				targetSize: TargetSize,
				targetData: epxData
			);
		}

		ushort maxAlpha = 0;

		for (int i = 0; i < destination.Length; ++i) {
			var destItem = destination[i];
			var epxItem = epxData[i];
			
			var newAlpha = Math.Min(destItem.A.Value, epxItem.A.Value);
			//var newAlpha = destination[i].A * epxData[i].A;
			//var newAlpha = epxData[i].A;

			destination[i] = new Color16(
				(ushort)((destItem.R.Value + epxItem.R.Value) >> 1),
				(ushort)((destItem.G.Value + epxItem.G.Value) >> 1),
				(ushort)((destItem.B.Value + epxItem.B.Value) >> 1),
				newAlpha
			);

			//destination[i].A = newAlpha;
			if (newAlpha > maxAlpha) {
				maxAlpha = newAlpha;
			}
		}

		for (int i = 0; i < destination.Length; ++i) {
			var item = destination[i];

			if (item.A.Value >= maxAlpha) {
				continue;
			}

			destination[i].SetRgb(
				item.R * item.A,
				item.G * item.A,
				item.B * item.A
			);
		}
	}
}
