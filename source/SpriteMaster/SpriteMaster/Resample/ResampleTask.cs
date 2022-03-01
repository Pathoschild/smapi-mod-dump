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
using SpriteMaster.Tasking;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpriteMaster.Resample;

static class ResampleTask {
	private static readonly TaskFactory<ManagedSpriteInstance?> Factory = new(ThreadedTaskScheduler.Instance);

	private static ManagedSpriteInstance? Resample(object? parametersObj) => ResampleFunction((TaskParameters)parametersObj!);

	private static ManagedSpriteInstance? ResampleFunction(in TaskParameters parameters) {
		try {
			return new ManagedSpriteInstance(
				assetName: parameters.SpriteInfo.Reference.NormalizedName(),
				spriteInfo: parameters.SpriteInfo,
				sourceRectangle: parameters.SpriteInfo.Bounds,
				textureType: parameters.SpriteInfo.TextureType,
				async: parameters.Async,
				expectedScale: parameters.SpriteInfo.ExpectedScale,
				previous: parameters.PreviousInstance
			);
		}
		finally {
			parameters.SpriteInfo.Dispose();
		}
	}

	private readonly record struct TaskParameters(
		SpriteInfo SpriteInfo,
		bool Async,
		ManagedSpriteInstance? PreviousInstance = null
	);

	internal static Task<ManagedSpriteInstance?> Dispatch(
		SpriteInfo spriteInfo,
		bool async,
		ManagedSpriteInstance? previousInstance = null
	) {
		var parameters = new TaskParameters(
			SpriteInfo: spriteInfo,
			Async: async,
			PreviousInstance: previousInstance
		);

		if (async) {
			return Factory.StartNew(Resample, parameters);
		}
		else {
			return Task<ManagedSpriteInstance?>.FromResult(ResampleFunction(parameters));
		}
	}

}
