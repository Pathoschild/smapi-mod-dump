/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize.Patches;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster;

internal sealed class ManagedTexture2D : InternalTexture2D {
	private static ulong TotalAllocatedSize = 0L;
	private static volatile uint TotalManagedTextures = 0;
	private const bool UseMips = false;
	private const bool UseShared = false;

	internal readonly WeakReference<XTexture2D> Reference;
	internal readonly ManagedSpriteInstance SpriteInstance;
	internal readonly Vector2I Dimensions;
	private volatile bool Disposed = false;

	internal static void DumpStats(List<string> output) {
		output.AddRange(new[]{
			"\tManagedTexture2D:",
			$"\t\tTotal Managed Textures : {TotalManagedTextures}",
			$"\t\tTotal Texture Size     : {Interlocked.Read(ref TotalAllocatedSize).AsDataSize()}"
			});
	}

	internal ManagedTexture2D(
		ReadOnlyPinnedSpan<byte>.FixedSpan data,
		ManagedSpriteInstance instance,
		XTexture2D reference,
		Vector2I dimensions,
		SurfaceFormat format,
		string? name = null
	) : base(
		graphicsDevice: reference.GraphicsDevice.IsDisposed ? DrawState.Device : reference.GraphicsDevice,
		width: dimensions.Width,
		height: dimensions.Height,
		mipmap: UseMips,
		format: format,
		type: PTexture2D.PlatformConstruct is null ? SurfaceType.Texture : SurfaceType.SwapChainRenderTarget, // this prevents the texture from being constructed immediately
		shared: UseShared,
		arraySize: 1
	) {
		if (PTexture2D.PlatformConstruct is not null && !GL.Texture2DExt.Construct(this, data, dimensions, UseMips, format, SurfaceType.Texture, UseShared)) {
			PTexture2D.PlatformConstruct(this, dimensions.X, dimensions.Y, UseMips, format, SurfaceType.Texture, UseShared);
			SetData(data.Array);
		}

		Name = name ?? $"{reference.NormalizedName()} [internal managed <{format}>]";

		Reference = reference.MakeWeak();
		SpriteInstance = instance;
		Dimensions = dimensions - instance.BlockPadding;

		reference.Disposing += OnParentDispose;

		Interlocked.Add(ref TotalAllocatedSize, (ulong)this.SizeBytes());
		Interlocked.Increment(ref TotalManagedTextures);

		Garbage.MarkOwned(format, dimensions.Area);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	~ManagedTexture2D() {
		if (!IsDisposed) {
			//Debug.Error($"Memory leak: ManagedTexture2D '{Name}' was finalized without the Dispose method called");
			Dispose(false);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void OnParentDispose(object? resource, EventArgs args) => OnParentDispose(resource as XTexture2D);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void OnParentDispose(XTexture2D? referenceTexture) {
		if (!IsDisposed) {
			Debug.Trace($"Disposing ManagedTexture2D '{Name}'");
			Dispose();
		}

		referenceTexture?.Meta().Dispose();
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);

		if (Disposed) {
			return;
		}
		Disposed = true;

		if (Reference.TryGet(out var reference)) {
			reference.Disposing -= OnParentDispose;
		}

		Garbage.UnmarkOwned(Format, Width * Height);
		Interlocked.Add(ref TotalAllocatedSize, (ulong)-this.SizeBytes());
		Interlocked.Decrement(ref TotalManagedTextures);
	}
}
