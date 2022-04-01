/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Metadata;
using SpriteMaster.Tasking;
using System;
using System.Threading.Tasks;

namespace SpriteMaster.Resample;

static class DecodeTask {
	private static readonly TaskFactory Factory = new(ThreadedTaskScheduler.Instance);

	private static void Decode(object? metadata) => DecodeFunction(metadata! as Texture2DMeta);

	private static void DecodeFunction(Texture2DMeta? metadata) {
		var rawData = metadata!.CachedRawData;
		if (rawData is null) {
			return;
		}

		var uncompressedData = Resample.TextureDecode.DecodeBlockCompressedTexture(metadata!.Format, metadata!.Size, rawData);
		if (uncompressedData.IsEmpty) {
			throw new InvalidOperationException("Compressed data failed to decompress");
		}
		metadata!.SetCachedDataUnsafe(uncompressedData);
	}

	internal static Task Dispatch(Texture2DMeta metadata) => Factory.StartNew(Decode, metadata);
}
