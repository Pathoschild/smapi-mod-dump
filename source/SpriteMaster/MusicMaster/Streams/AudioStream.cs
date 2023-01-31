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
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MusicMaster.Streams;

internal abstract class AudioStream : Stream {
	private readonly Stream? _baseStream;
	protected abstract Stream DelegatingStream { get; }

	internal abstract AudioChannels Channels { get; init; }
	internal abstract int Frequency { get; init; }

	protected AudioStream(Stream? baseStream) {
		_baseStream = baseStream;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public sealed override void Flush() =>
		DelegatingStream.Flush();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public sealed override int Read(byte[] buffer, int offset, int count) =>
		DelegatingStream.Read(buffer, offset, count);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public sealed override long Seek(long offset, SeekOrigin origin) =>
		DelegatingStream.Seek(offset, origin);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public sealed override void SetLength(long value) =>
		DelegatingStream.SetLength(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public sealed override void Write(byte[] buffer, int offset, int count) =>
		DelegatingStream.Write(buffer, offset, count);

	public sealed override bool CanRead => DelegatingStream.CanRead;

	public sealed override bool CanSeek => DelegatingStream.CanSeek;

	public sealed override bool CanWrite => DelegatingStream.CanWrite;

	public sealed override long Length => DelegatingStream.Length;

	public sealed override long Position {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => DelegatingStream.Position;
		[MethodImpl(Runtime.MethodImpl.Inline)]
		set => DelegatingStream.Position = value;
	}

	protected override void Dispose(bool disposing) {
		DelegatingStream.Dispose();

		_baseStream?.Dispose();

		base.Dispose(disposing);
	}

	public override async ValueTask DisposeAsync() {
		await DelegatingStream.DisposeAsync();


		if (_baseStream is {} baseStream) {
			await baseStream.DisposeAsync();
		}

		await base.DisposeAsync();
	}
}
