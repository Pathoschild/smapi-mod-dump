/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using MusicMaster.Extensions;
using System.IO;

namespace MusicMaster.Streams;
internal sealed class Mp3Stream : AudioStream {
	protected override Stream DelegatingStream { get; }

	internal override AudioChannels Channels { get; init; }
	internal override int Frequency { get; init; }

	internal Mp3Stream(string path) : base(null) {
		var inStream = new MP3Sharp.MP3Stream(path);

		try {
			Channels = inStream.ChannelCount >= 2 ? AudioChannels.Stereo : AudioChannels.Mono;
			Frequency = inStream.Frequency;

			if (inStream.ChannelCount > 2) {
				// Truncate to stereo
				var streamDataArray = new byte[inStream.Length];
				var streamDataStream = new MemoryStream(streamDataArray);
				inStream.CopyTo(streamDataStream);
				inStream.Dispose();
				var rawStreamDataBytes = streamDataStream.GetBuffer();
				var rawStreamData = rawStreamDataBytes.AsReadOnlySpan().Cast<short>();

				long bytesPerChannel = inStream.Length / inStream.ChannelCount;
				var rawTruncatedData = new byte[bytesPerChannel * 2];
				var truncatedData = rawTruncatedData.AsSpan<short>();

				int sourceIndex = 0;
				for (int destIndex = 0;
						destIndex < truncatedData.Length;
						destIndex += 2, sourceIndex += inStream.ChannelCount) {
					truncatedData[destIndex + 0] = rawStreamData[sourceIndex + 0];
					truncatedData[destIndex + 1] = rawStreamData[sourceIndex + 1];
				}

				DelegatingStream = new MemoryStream(rawTruncatedData);
			}
			else {
				DelegatingStream = inStream;
			}
		}
		catch {
			inStream.Dispose();
			throw;
		}
	}
}
