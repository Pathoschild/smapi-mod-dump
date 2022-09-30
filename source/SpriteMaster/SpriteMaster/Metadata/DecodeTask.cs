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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Resample;

internal static class DecodeTask {
	private static readonly TaskFactory Factory = new(ThreadedTaskScheduler.Instance);

	private readonly record struct DecodeTaskData(Texture2DMeta? Metadata, ulong Revision);

	private static void Decode(object? data) => DecodeFunction((DecodeTaskData)data!);

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowDecompressionFailureException() =>
		throw new InvalidOperationException("Compressed data failed to decompress");

	private static void DecodeFunction(DecodeTaskData data) {
		(var metadata, var revision) = data;

		if (metadata is null) {
			return;
		}

		var rawData = metadata.CachedRawData;
		if (rawData is null) {
			return;
		}

		var uncompressedData = TextureDecode.DecodeBlockCompressedTexture(metadata.Format, metadata.Size, rawData);
		if (uncompressedData.IsEmpty) {
			ThrowDecompressionFailureException();
			return;
		}

		if (Interlocked.Read(ref metadata.DecodingTaskRevision) == revision) {
			metadata.SetCachedDataUnsafe(uncompressedData);
		}
	}

	internal static Task Dispatch(Texture2DMeta metadata, ulong revision) => Factory.StartNew(Decode, new DecodeTaskData(metadata, revision));
}
