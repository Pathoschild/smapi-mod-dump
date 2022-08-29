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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers;

internal interface IScaler {
	Config CreateConfig(
		Vector2B wrapped,
		bool hasAlpha,
		bool gammaCorrected
	);

	IScalerInfo Info { get; }

	uint MinScale { get; }
	uint MaxScale { get; }
	uint ClampScale(uint scale);

	Span<Color16> Apply(
		Config configuration,
		uint scaleMultiplier,
		ReadOnlySpan<Color16> sourceData,
		Vector2I sourceSize,
		Span<Color16> targetData,
		Vector2I targetSize
	);

	internal static IScalerInfo DefaultInfo => DefaultScaler.ScalerInfo.Instance;

	internal static IScaler Default => new DefaultScaler.Scaler.ScalerInterface();

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowUnknownScalerTypeException<T>() =>
		throw new InvalidOperationException($"Unknown Scaler Type: {SMConfig.Resample.Scaler}");

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowBilinearNotImplementedException<T>() =>
		throw new NotImplementedException("Bilinear scaling is not implemented");

	internal static IScalerInfo? GetScalerInfo(Scaler scaler) => scaler switch {
		Scaler.xBRZ => xBRZ.ScalerInfo.Instance,
#if !SHIPPING
		Resample.Scaler.SuperXBR => Resample.Scalers.SuperXBR.ScalerInfo.Instance,
#endif
		Scaler.EPX => EPX.ScalerInfo.Instance,
		Scaler.EPXLegacy => EPX.ScalerInfo.InstanceLegacy,
		Scaler.xBREPX => xBREPX.ScalerInfo.Instance,
		Scaler.None => null,
		_ => ThrowUnknownScalerTypeException<IScalerInfo>()
	};

	internal static IScalerInfo? CurrentInfo => GetScalerInfo(SMConfig.Resample.Scaler);

	internal static IScaler? Current => SMConfig.Resample.Scaler switch {
		Scaler.xBRZ => xBRZ.Scaler.ScalerInterface.Instance,
#if !SHIPPING
		Resample.Scaler.SuperXBR => Resample.Scalers.SuperXBR.Scaler.ScalerInterface.Instance,
#endif
		Scaler.EPX => EPX.Scaler.ScalerInterface.Instance,
		Scaler.EPXLegacy => EPX.Scaler.ScalerInterface.InstanceLegacy,
		Scaler.xBREPX => xBREPX.Scaler.ScalerInterface.Instance,
		Scaler.None => null,
		_ => ThrowUnknownScalerTypeException<IScaler>()
	};
}
