/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using WeakTexture = System.WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>;

namespace SpriteMaster;
sealed partial class ScaledTexture : IDisposable {
	// TODO : This can grow unbounded. Should fix.
	internal static readonly SpriteMap SpriteMap = new();

	private static readonly LinkedList<WeakReference<ScaledTexture>> MostRecentList = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool LegalFormat(Texture2D texture) => AllowedFormats.ContainsF(texture.Format);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Validate(Texture2D texture) {
		var meta = texture.Meta();
		if (!meta.ScaleValid) {
			return false;
		}

		if (texture is ManagedTexture2D) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', is already scaled");
			}
			return meta.ScaleValid = false;
		}

		if (texture is RenderTarget2D) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', render targets unsupported");
			}
			return meta.ScaleValid = false;
		}

		if (Math.Max(texture.Width, texture.Height) <= Config.Resample.MinimumTextureDimensions) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', texture is too small to qualify");
			}
			return meta.ScaleValid = false;
		}

		if (texture.Area() == 0) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', zero area");
			}
			return meta.ScaleValid = false;
		}

		// TODO pComPtr check?
		if (texture.IsDisposed || texture.GraphicsDevice.IsDisposed) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Is Zombie");
			}
			return meta.ScaleValid = false;
		}

		if (Config.IgnoreUnknownTextures && texture.Anonymous()) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Is Unknown Texture");
			}
			return meta.ScaleValid = false;
		}


		if (texture.LevelCount > 1) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Multi-Level Textures Unsupported: {texture.LevelCount} levels");
			}
			return meta.ScaleValid = false;
		}

		if (!LegalFormat(texture)) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Format Unsupported: {texture.Format}");
			}
			return meta.ScaleValid = false;
		}

		if (!texture.Anonymous()) {
			foreach (var blacklisted in Config.Resample.Blacklist) {
				if (texture.SafeName().StartsWith(blacklisted)) {
					if (!meta.TracePrinted) {
						meta.TracePrinted = true;
						Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Is Blacklisted");
					}
					return meta.ScaleValid = false;
				}
			}
		}

		return true;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	static internal ScaledTexture Fetch(Texture2D texture, in Bounds source, uint expectedScale) {
		if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
			return scaleTexture;
		}

		return null;
	}

	private static readonly TexelTimer TexelAverage = new();
	private static readonly TexelTimer TexelAverageCached = new();
	private static readonly TexelTimer TexelAverageSync = new();
	private static readonly TexelTimer TexelAverageCachedSync = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static TexelTimer GetTimer(bool cached, bool async) {
		if (async) {
			return cached ? TexelAverageCached : TexelAverage;
		}
		else {
			return cached ? TexelAverageCachedSync : TexelAverageSync;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static TexelTimer GetTimer(Texture2D texture, bool async) {
		var IsCached = SpriteInfo.IsCached(texture);
		return GetTimer(IsCached, async);
	}

	static TimeSpan MeanTimeSpan = TimeSpan.Zero;
	static int TimeSpanSamples = 0;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	static internal ScaledTexture Get(Texture2D texture, in Bounds source, uint expectedScale) {
		using var _ = Performance.Track();

		if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
			return scaleTexture;
		}

		if (!Validate(texture)) {
			return null;
		}

		bool useAsync = Config.AsyncScaling.Enabled && (Config.AsyncScaling.EnabledForUnknownTextures || !texture.Anonymous()) && (source.Area >= Config.AsyncScaling.MinimumSizeTexels);
		// !texture.Meta().HasCachedData

		if (Config.Resample.UseFrametimeStalling && DrawState.PushedUpdateWithin(0)) {
			var remainingTime = DrawState.RemainingFrameTime();
			if (remainingTime <= TimeSpan.Zero) {
				return null;
			}

			var estimatedDuration = GetTimer(texture: texture, async: useAsync).Estimate(source.Area);
			if (estimatedDuration > TimeSpan.Zero && estimatedDuration > remainingTime) {
				Debug.TraceLn($"Not enough frame time left to begin resampling '{texture.SafeName()}' ({estimatedDuration.TotalMilliseconds} >= {remainingTime.TotalMilliseconds})");
				return null;
			}
			else {
				Debug.TraceLn($"Remaining Time: {remainingTime.TotalMilliseconds}");
			}
		}

		// TODO : We should really only populate the average when we are performing an expensive operation like GetData.
		var watch = System.Diagnostics.Stopwatch.StartNew();

		TextureType textureType;
		if (!source.Offset.IsZero || source.Extent != texture.Extent()) {
			textureType = TextureType.Sprite;
		}
		else {
			textureType = TextureType.Image;
		}
		SpriteInfo textureWrapper;

		using (Performance.Track("new SpriteInfo")) {
			textureWrapper = new(reference: texture, dimensions: source, expectedScale: expectedScale);
		}

		// If this is null, it can only happen due to something being blocked, so we should try again later.
		if (textureWrapper.Data is null) {
			Debug.TraceLn($"Texture Data fetch for '{texture.SafeName()}' was blocked; retrying later");
			return null;
		}

		Debug.TraceLn($"Beginning Rescale Process for '{texture.SafeName()}'");

		DrawState.PushedUpdateThisFrame = true;

		try {
			ulong hash;
			using (Performance.Track("Upscaler.GetHash")) {
				hash = Upscaler.GetHash(textureWrapper, textureType);
			}

			var newTexture = new ScaledTexture(
				assetName: texture.SafeName(),
				textureWrapper: textureWrapper,
				sourceRectangle: source,
				textureType: textureType,
				hash: hash,
				async: useAsync,
				expectedScale: expectedScale
			);
			if (useAsync) {
				// It adds itself to the relevant maps.
				if (newTexture.IsReady && newTexture.Texture is not null) {
					return newTexture;
				}
				return null;
			}
			else {
				return newTexture;
			}
		}
		finally {
			watch.Stop();
			var duration = watch.Elapsed;
			var averager = GetTimer(cached: textureWrapper.WasCached, async: useAsync);
			TimeSpanSamples++;
			MeanTimeSpan += duration;
			Debug.TraceLn($"{(MeanTimeSpan / TimeSpanSamples).TotalMilliseconds} ms");
			averager.Add(source.Area, duration);
		}
	}

	internal static readonly SurfaceFormat[] AllowedFormats = {
		SurfaceFormat.Color,
		//SurfaceFormat.Dxt3 // fonts
	};

	internal ManagedTexture2D Texture = null;
	internal readonly string Name;
	internal Vector2 Scale;
	internal readonly TextureType TexType;
	internal volatile bool IsReady = false;

	internal Vector2B Wrapped = Vector2B.False;

	internal readonly WeakTexture Reference;
	internal readonly Bounds OriginalSourceRectangle;
	internal readonly ulong Hash;

	internal Vector2I Padding = Vector2I.Zero;
	internal Vector2I UnpaddedSize;
	internal Vector2I BlockPadding = Vector2I.Zero;
	private readonly Vector2I originalSize;
	private readonly Bounds sourceRectangle;
	private uint refScale;

	internal ulong LastReferencedFrame = DrawState.CurrentFrame;

	internal Vector2 AdjustedScale = Vector2.One;

	private LinkedListNode<WeakReference<ScaledTexture>> CurrentRecentNode = null;
	private volatile bool Disposed = false;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	~ScaledTexture() {
		if (!Disposed) {
			Dispose();
		}
	}

	internal static volatile uint TotalMemoryUsage = 0;

	internal long MemorySize {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			if (!IsReady || Texture is null) {
				return 0;
			}
			return Texture.SizeBytes();
		}
	}

	internal long OriginalMemorySize {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			return originalSize.Width * originalSize.Height * sizeof(int);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Purge(Texture2D reference) {
		Purge(reference, null, DataRef<byte>.Null);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Purge(Texture2D reference, in Bounds? bounds, DataRef<byte> data) {
		SpriteInfo.Purge(reference, bounds, data);
		SpriteMap.Purge(reference, bounds);
		Upscaler.PurgeHash(reference);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void PurgeTextures(long _purgeTotalBytes) {
		Contract.AssertPositive(_purgeTotalBytes);

		Debug.TraceLn($"Attempting to purge {_purgeTotalBytes.AsDataSize()} from currently loaded textures");

		// For portability purposes
		if (IntPtr.Size == 8) {
			var purgeTotalBytes = _purgeTotalBytes;
			lock (MostRecentList) {
				long totalPurge = 0;
				while (purgeTotalBytes > 0 && MostRecentList.Count > 0) {
					if (MostRecentList.Last.Value.TryGetTarget(out var target)) {
						var textureSize = (long)target.MemorySize;
						Debug.TraceLn($"Purging {target.SafeName()} ({textureSize.AsDataSize()})");
						purgeTotalBytes -= textureSize;
						totalPurge += textureSize;
						target.CurrentRecentNode = null;
						target.Dispose(true);
					}
					MostRecentList.RemoveLast();
				}
				Debug.TraceLn($"Total Purged: {totalPurge.AsDataSize()}");
			}
		}
		else {
			// For 32-bit, truncate down to an integer so this operation goes a bit faster.
			Contract.AssertLessEqual(_purgeTotalBytes, (long)uint.MaxValue);
			var purgeTotalBytes = (uint)_purgeTotalBytes;
			lock (MostRecentList) {
				uint totalPurge = 0;
				while (purgeTotalBytes > 0 && MostRecentList.Count > 0) {
					if (MostRecentList.Last.Value.TryGetTarget(out var target)) {
						var textureSize = (uint)target.MemorySize;
						Debug.TraceLn($"Purging {target.SafeName()} ({textureSize.AsDataSize()})");
						purgeTotalBytes -= textureSize;
						totalPurge += textureSize;
						target.CurrentRecentNode = null;
						target.Dispose(true);
					}
					MostRecentList.RemoveLast();
				}
				Debug.TraceLn($"Total Purged: {totalPurge.AsDataSize()}");
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ScaledTexture(string assetName, SpriteInfo textureWrapper, Bounds sourceRectangle, ulong hash, TextureType textureType, bool async, uint expectedScale) {
		using var _ = Performance.Track();

		TexType = textureType;
		Hash = hash;
		var source = textureWrapper.Reference;

		this.OriginalSourceRectangle = new(sourceRectangle);
		this.Reference = source.MakeWeak();
		this.sourceRectangle = sourceRectangle;
		this.refScale = expectedScale;
		SpriteMap.Add(source, this, sourceRectangle, expectedScale);

		this.Name = source.Anonymous() ? assetName.SafeName() : source.SafeName();
		switch (TexType) {
			case TextureType.Sprite:
				originalSize = sourceRectangle.Extent;
				break;
			case TextureType.Image:
				originalSize = source.Extent();
				break;
			case TextureType.SlicedImage:
				throw new NotImplementedException("Sliced Images not yet implemented");
		}

		if (async && Config.AsyncScaling.Enabled) {
			ThreadQueue.Queue((SpriteInfo wrapper) => {
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				using var _ = new AsyncTracker($"Resampling {Name} [{sourceRectangle}]");
				Thread.CurrentThread.Name = "Texture Resampling Thread";
				Upscaler.Upscale(
					texture: this,
					scale: ref refScale,
					input: wrapper,
					textureType: TexType,
					hash: Hash,
					wrapped: ref Wrapped,
					async: true
				);
				// If the upscale fails, the asynchronous action on the render thread automatically cleans up the ScaledTexture.
			}, textureWrapper);
		}
		else {
			// TODO store the HD Texture in _this_ object instead. Will confuse things like subtexture updates, though.
			this.Texture = Upscaler.Upscale(
				texture: this,
				scale: ref refScale,
				input: textureWrapper,
				textureType: TexType,
				hash: Hash,
				wrapped: ref Wrapped,
				async: false
			);

			if (this.Texture is not null) {
				Finish();
			}
			else {
				Dispose();
				return;
			}
		}

		// TODO : I would love to dispose of this texture _now_, but we rely on it disposing to know if we need to dispose of ours.
		// There is a better way to do this using weak references, I just need to analyze it further. Memory leaks have been a pain so far.
		source.Disposing += (object sender, EventArgs args) => { OnParentDispose((Texture2D)sender); };

		lock (MostRecentList) {
			CurrentRecentNode = MostRecentList.AddFirst(this.MakeWeak());
		}
	}

	// Async Call
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Finish() {
		ManagedTexture2D texture;
		lock (this) {
			texture = Texture;
		}

		if (texture is null || texture.IsDisposed) {
			return;
		}

		UpdateReferenceFrame();

		TotalMemoryUsage += (uint)texture.SizeBytes();
		texture.Disposing += (object sender, EventArgs args) => { TotalMemoryUsage -= (uint)texture.SizeBytes(); };

		switch (TexType) {
			case TextureType.Sprite:
				Debug.TraceLn($"Creating Sprite [{texture.Format} x{refScale}]: {this.SafeName()} {sourceRectangle}");
				break;
			case TextureType.Image:
				Debug.TraceLn($"Creating Image [{texture.Format} x{refScale}]: {this.SafeName()}");
				break;
			case TextureType.SlicedImage:
				Debug.TraceLn($"Creating Sliced Image [{texture.Format} x{refScale}]: {this.SafeName()}");
				break;
			default:
				Debug.TraceLn($"Creating UNKNOWN [{texture.Format} x{refScale}]: {this.SafeName()}");
				break;
		}

		this.Scale = (Vector2)texture.Dimensions / new Vector2(originalSize.Width, originalSize.Height);

		IsReady = true;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void UpdateReferenceFrame() {
		if (Disposed) {
			return;
		}

		this.LastReferencedFrame = DrawState.CurrentFrame;

		lock (MostRecentList) {
			if (CurrentRecentNode is not null) {
				MostRecentList.Remove(CurrentRecentNode);
				MostRecentList.AddFirst(CurrentRecentNode);
			}
			else {
				CurrentRecentNode = MostRecentList.AddFirst(this.MakeWeak());
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Dispose(bool disposeChildren) {
		if (disposeChildren && Texture is not null) {
			if (!Texture.IsDisposed) {
				Texture.Dispose();
			}
			Texture = null;
		}
		Dispose();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public void Dispose() {
		if (Disposed) {
			return;
		}

		if (Reference.TryGetTarget(out var reference)) {
			SpriteMap.Remove(this, reference);
		}
		if (CurrentRecentNode is not null) {
			lock (MostRecentList) {
				MostRecentList.Remove(CurrentRecentNode);
			}
			CurrentRecentNode = null;
		}
		Disposed = true;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void OnParentDispose(Texture2D texture) {
		Debug.TraceLn($"Parent Texture Disposing: {texture.SafeName()}");

		Dispose();
	}
}
