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
using Microsoft.Xna.Framework.Graphics;
using Pastel;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using SpriteMaster.Types.Volatile;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using WeakInstanceList = System.Collections.Generic.LinkedList<System.WeakReference<SpriteMaster.ManagedSpriteInstance>>;
using WeakInstanceListNode = System.Collections.Generic.LinkedListNode<System.WeakReference<SpriteMaster.ManagedSpriteInstance>>;
using WeakTexture = System.WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>;

namespace SpriteMaster;

sealed partial class ManagedSpriteInstance : IDisposable {
	private static readonly WeakInstanceList RecentAccessList = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool HasLegalFormat(Texture2D texture) => AllowedFormats.ContainsF(texture.Format);

	private static void PurgeInvalidated(Texture2D texture) {
		// If somehow it passed validation earlier (like a SetData before a name) make sure no cached data
		// or handles are left dangling around.

		var meta = texture.Meta();
		meta.Purge();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Validate(Texture2D texture, bool clean = false) {
		var meta = texture.Meta();
		if (meta.Validation.HasValue) {
			return meta.Validation.Value;
		}

		if (texture is InternalTexture2D) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Is Internal Texture");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		if (texture is RenderTarget2D && (
				StardewValley.GameRunner.instance.gameInstances.AnyF(game => texture == game.screen || texture == game.uiScreen) ||
				texture.Name is ("UI Screen" or "Screen") ||
				texture.Meta().IsSystemRenderTarget
			)
		) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Is System Render Target");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		// For now, render targets are disabled. General ones _can_ be made to work - they need to be invalidated once they are bound to a render target slot,
		// and a hold set on using them until they are no longer bound to a render target slot. Or on clear calls. If the render target is not a persist-type,
		// it should be processed _immediately_. If it is, it should be processed synchronously on first use.
		if (texture is RenderTarget2D) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Is Render Target");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		if (Math.Max(texture.Width, texture.Height) <= Config.Resample.MinimumTextureDimensions) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Is Too Small: ({texture.Extent().ToString(DrawingColor.Orange)} < {Config.Resample.MinimumTextureDimensions.ToString(DrawingColor.Orange)})");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		if (texture.Area() == 0) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Zero Area (Degenerate)");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		// TODO pComPtr check?
		if (texture.IsDisposed || texture.GraphicsDevice.IsDisposed) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Is Zombie");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		// TODO : this can invalidate sprites that haven't had their Name/Tag set prior to SetData!
		if (Config.IgnoreUnknownTextures && texture.Anonymous()) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Is Unknown Texture");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		if (texture.LevelCount > 1) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Multi-Level Textures Unsupported: {texture.LevelCount.ToString(DrawingColor.Orange)} levels");
			}
			meta.Validation = false;
			return false;
		}

		if (!HasLegalFormat(texture)) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Format Unsupported: {texture.Format.ToString(DrawingColor.Orange)}");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		if (!texture.Anonymous()) {
			foreach (var blacklisted in Config.Resample.Blacklist) {
				if (texture.SafeName().StartsWith(blacklisted)) {
					if (!meta.TracePrinted) {
						meta.TracePrinted = true;
						Debug.TraceLn($"Not Scaling Texture '{texture.SafeName(DrawingColor.LightYellow)}', Is Blacklisted");
					}
					meta.Validation = false;
					if (clean) {
						PurgeInvalidated(texture);
					}
					return false;
				}
			}
		}

		if (!texture.Anonymous()) {
			meta.Validation = true;
		}

		return true;
	}

	private static readonly TexelTimer TexelAverage = new();
	private static readonly TexelTimer TexelAverageCached = new();
	private static readonly TexelTimer TexelAverageSync = new();
	private static readonly TexelTimer TexelAverageCachedSync = new();

	internal static void ClearTimers() {
		TexelAverage.Reset();
		TexelAverageCached.Reset();
		TexelAverageSync.Reset();
		TexelAverageCachedSync.Reset();
	}

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
	private static TexelTimer GetTimer(Texture2D texture, bool async, out bool isCached) {
		var IsCached = SpriteInfo.IsCached(texture);
		isCached = IsCached;
		return GetTimer(IsCached, async);
	}

	static TimeSpan MeanTimeSpan = TimeSpan.Zero;
	static int TimeSpanSamples = 0;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	static internal ManagedSpriteInstance? Fetch(Texture2D texture, in Bounds source, uint expectedScale) {
		if (!Validate(texture, clean: true)) {
			return null;
		}

		if (SpriteMap.TryGetReady(texture, source, expectedScale, out var scaleTexture)) {
			return scaleTexture;
		}

		return null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	static internal ManagedSpriteInstance? FetchOrCreate(Texture2D texture, in Bounds source, uint expectedScale) {
		using var _ = Performance.Track();

		if (!Validate(texture, clean: true)) {
			return null;
		}

		ManagedSpriteInstance? currentInstance = null;

		if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
			if (scaleTexture.Invalidated) {
				currentInstance = scaleTexture;
			}
			else if (!scaleTexture.IsReady && scaleTexture.PreviousSpriteInstance is not null && scaleTexture.PreviousSpriteInstance.IsReady) {
				return scaleTexture.PreviousSpriteInstance;
			}
			else {
				return scaleTexture;
			}
		}

		bool useStalling = Config.Resample.UseFrametimeStalling && !GameState.IsLoading;

		bool useAsync = Config.AsyncScaling.Enabled && (Config.AsyncScaling.EnabledForUnknownTextures || !texture.Anonymous()) && (source.Area >= Config.AsyncScaling.MinimumSizeTexels);
		// !texture.Meta().HasCachedData

		TimeSpan? remainingTime = null;
		bool? isCached = null;

		string getMetadataString() {
			if (isCached.HasValue) {
				return $" ({(useAsync ? "async" : "sync".Pastel(DrawingColor.Orange))} {(isCached.Value ? "cached" : "uncached".Pastel(DrawingColor.Orange))})";
			}
			else {
				return $" ({(useAsync ? "async" : "sync".Pastel(DrawingColor.Orange))})";
			}
		}

		string getNameString() {
			return $"'{texture.SafeName(DrawingColor.LightYellow)}'{getMetadataString()}";
		}

		if (useStalling && DrawState.PushedUpdateWithin(0)) {
			remainingTime = DrawState.RemainingFrameTime();
			if (remainingTime <= TimeSpan.Zero) {
				return currentInstance;
			}

			var estimatedDuration = GetTimer(texture: texture, async: useAsync, out bool cached).Estimate((int)texture.Format.SizeBytes(source.Area));
			isCached = cached;
			if (estimatedDuration > TimeSpan.Zero && estimatedDuration > remainingTime) {
				Debug.TraceLn($"Not enough frame time left to begin resampling {getNameString()} ({estimatedDuration.TotalMilliseconds.ToString(DrawingColor.LightBlue)} ms >= {remainingTime?.TotalMilliseconds.ToString(DrawingColor.LightBlue)} ms)");
				return currentInstance;
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

		if (SpriteOverrides.IsWater(source, texture)) {
			//textureType = TextureType.Image;
		}

		SpriteInfo.Initializer spriteInfoInitializer = new SpriteInfo.Initializer(reference: texture, dimensions: source, expectedScale: expectedScale, textureType: textureType);

		string getRemainingTime() {
			if (!remainingTime.HasValue) {
				return "";
			}
			return $" (remaining time: {remainingTime?.TotalMilliseconds.ToString(DrawingColor.LightYellow)} ms)";
		}

		// If this is null, it can only happen due to something being blocked, so we should try again later.
		if (spriteInfoInitializer.ReferenceData is null) {
			Debug.TraceLn($"Texture Data fetch for {getNameString()} was {"blocked".Pastel(DrawingColor.Red)}; retrying later#{getRemainingTime()}");
			return currentInstance;
		}

		Debug.TraceLn($"Beginning Rescale Process for {getNameString()} #{getRemainingTime()}");

		DrawState.PushedUpdateThisFrame = true;

		try {
			var resampleTask = ResampleTask.Dispatch(
				spriteInfo: new(spriteInfoInitializer),
				async: useAsync,
				previousInstance: currentInstance
			);

			var result = resampleTask.IsCompletedSuccessfully ? resampleTask.Result : currentInstance;

			if (useAsync) {
				// It adds itself to the relevant maps.
				return (result?.IsReady ?? false) ? result : null;
			}
			else {
				return result;
			}
		}
		finally {
			watch.Stop();
			var duration = watch.Elapsed;
			var averager = GetTimer(cached: spriteInfoInitializer.WasCached, async: useAsync);
			TimeSpanSamples++;
			MeanTimeSpan += duration;
			Debug.TraceLn($"Duration {getNameString()}: {(MeanTimeSpan / TimeSpanSamples).TotalMilliseconds.ToString(DrawingColor.LightYellow)} ms");
			averager.Add(source.Area, duration);
		}
	}

	internal static readonly SurfaceFormat[] AllowedFormats = {
		SurfaceFormat.Color,
		SurfaceFormat.Dxt5,
		SurfaceFormat.Dxt5SRgb
		//SurfaceFormat.Dxt3 // fonts
	};

	internal ManagedTexture2D? Texture = null;
	internal readonly string Name;
	internal Vector2F Scale;
	internal readonly TextureType TexType;
	private volatile bool _isReady = false;
	internal bool IsReady => _isReady && Texture is not null;

	internal readonly Vector2B Wrapped = Vector2B.False;

	internal readonly WeakTexture Reference;
	internal readonly Bounds OriginalSourceRectangle;
	internal readonly ulong Hash = 0U;

	internal PaddingQuad Padding = PaddingQuad.Zero;
	internal Vector2I UnpaddedSize;
	internal Vector2I BlockPadding = Vector2I.Zero;
	private readonly Vector2I originalSize;
	private readonly Bounds SourceRectangle;
	private readonly uint ExpectedScale;
	internal readonly ulong SpriteMapHash;
	private readonly uint ReferenceScale;
	internal ManagedSpriteInstance? PreviousSpriteInstance = null;
	internal volatile bool Invalidated = false;

	internal ulong LastReferencedFrame = DrawState.CurrentFrame;

	internal Vector2F InnerRatio = Vector2F.One;

	/// <summary>
	/// Node into the most-recent accessed instance list.
	/// Should only be <seealso cref="null"/> after the instance is <seealso cref="ManagedSpriteInstance.Dispose">disposed</seealso>
	/// </summary>
	private WeakInstanceListNode? RecentAccessNode = null;
	internal VolatileBool IsDisposed { get; private set; } = false;

	internal static long TotalMemoryUsage = 0U;

	internal long MemorySize {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			if (!IsReady || Texture is null) {
				return 0;
			}
			return Texture.SizeBytes();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	~ManagedSpriteInstance() {
		Debug.Trace($"ManagedSpriteInstance '{Name}' reached destructor; was not initially disposed");
		Dispose();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Purge(Texture2D reference, in Bounds? bounds, in DataRef<byte> data) {
		SpriteInfo.Purge(reference, bounds, data);
		if (data.IsNull) {
			SpriteMap.Purge(reference, bounds);
		}
		else {
			SpriteMap.Invalidate(reference, bounds);
		}
		//Resampler.PurgeHash(reference);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void PurgeTextures(long purgeTotalBytes) {
		Contracts.AssertPositive(purgeTotalBytes);

		Debug.TraceLn($"Attempting to purge {purgeTotalBytes.AsDataSize()} from currently loaded textures");

		lock (RecentAccessList) {
			long totalPurge = 0;
			while (purgeTotalBytes > 0 && RecentAccessList.Count > 0) {
				if (RecentAccessList.Last?.Value.TryGetTarget(out var target) ?? false) {
					var textureSize = (long)target.MemorySize;
					Debug.TraceLn($"Purging {target.SafeName()} ({textureSize.AsDataSize()})");
					purgeTotalBytes -= textureSize;
					totalPurge += textureSize;
					target.RecentAccessNode = null;
					target.Dispose(true);
				}
				RecentAccessList.RemoveLast();
			}
			Debug.TraceLn($"Total Purged: {totalPurge.AsDataSize()}");
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ManagedSpriteInstance(string assetName, SpriteInfo textureWrapper, in Bounds sourceRectangle, TextureType textureType, bool async, uint expectedScale, ManagedSpriteInstance? previous = null) {
		using var _ = Performance.Track();

		PreviousSpriteInstance = previous;

		TexType = textureType;

		ulong GetHash() {
			using (Performance.Track("Upscaler.GetHash")) {
				return Resampler.GetHash(textureWrapper, textureType);
			}
		}

		var source = textureWrapper.Reference;

		OriginalSourceRectangle = sourceRectangle;
		Reference = source.MakeWeak();
		SourceRectangle = sourceRectangle;
		ExpectedScale = expectedScale;
		ReferenceScale = expectedScale;
		SpriteMapHash = SpriteMap.SpriteHash(source, sourceRectangle, expectedScale);
		Name = source.Anonymous() ? assetName.SafeName() : source.SafeName();
		// TODO : I believe we need a lock here until when the texture is _fully created_, preventing new instantiations from starting of a texture
		// already in-flight
		if (!SpriteMap.AddReplaceInvalidated(source, this)) {
			// If false, then the sprite already exists in the map (which can be caused by gap between the Resample task being kicked off, and hitting this, and _another_ sprite getting
			// past the earlier try-block, and getting here.
			// TODO : this should be fixed by making sure that all of the resample tasks _at least_ get to this point before the end of the frame.
			// TODO : That might not be sufficient either if the _same_ draw ends up happening again.
			Dispose();
		}
		else {
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

			// TODO store the HD Texture in _this_ object instead. Will confuse things like subtexture updates, though.
			Hash = GetHash();
			this.Texture = Resampler.Upscale(
				spriteInstance: this,
				scale: ref ReferenceScale,
				input: textureWrapper,
				hash: Hash,
				wrapped: ref Wrapped,
				async: false
			);

			// TODO : I would love to dispose of this texture _now_, but we rely on it disposing to know if we need to dispose of ours.
			// There is a better way to do this using weak references, I just need to analyze it further. Memory leaks have been a pain so far.
			source.Disposing += (object? sender, EventArgs args) => { OnParentDispose(sender as Texture2D); };

			lock (RecentAccessList) {
				RecentAccessNode = RecentAccessList.AddFirst(this.MakeWeak());
			}
		}
	}

	// Async Call
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Finish() {
		ManagedTexture2D? texture;
		lock (this) {
			texture = Texture;
		}

		if (texture is null || texture.IsDisposed || IsDisposed) {
			return;
		}

		UpdateReferenceFrame();

		Interlocked.Add(ref TotalMemoryUsage, texture.SizeBytes());
		texture.Disposing += (object? sender, EventArgs args) => { Interlocked.Add(ref TotalMemoryUsage, -texture.SizeBytes()); };

		{
			var formatString = texture.Format.ToString(DrawingColor.LightCoral);
			var nameString = this.SafeName(DrawingColor.LightYellow);
			switch (TexType) {
				case TextureType.Sprite:
					Debug.TraceLn($"Creating Sprite [{formatString} x{ReferenceScale}]: {nameString} {SourceRectangle}");
					break;
				case TextureType.Image:
					Debug.TraceLn($"Creating Image [{formatString} x{ReferenceScale}]: {nameString}");
					break;
				case TextureType.SlicedImage:
					Debug.TraceLn($"Creating Sliced Image [{formatString} x{ReferenceScale}]: {nameString}");
					break;
				default:
					Debug.TraceLn($"Creating UNKNOWN [{formatString} x{ReferenceScale}]: {nameString}");
					break;
			}
		}

		this.Scale = (Vector2F)texture.Dimensions / (Vector2F)originalSize;

		Thread.MemoryBarrier();
		_isReady = true;
		if (PreviousSpriteInstance is not null) {
			PreviousSpriteInstance.Dispose(true);
			PreviousSpriteInstance = null;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void UpdateReferenceFrame() {
		if (IsDisposed) {
			return;
		}

		this.LastReferencedFrame = DrawState.CurrentFrame;

		lock (RecentAccessList) {
			if (RecentAccessNode is not null) {
				RecentAccessList.Remove(RecentAccessNode);
				RecentAccessList.AddFirst(RecentAccessNode);
			}
			else {
				RecentAccessNode = RecentAccessList.AddFirst(this.MakeWeak());
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
		if (IsDisposed) {
			return;
		}

		if (PreviousSpriteInstance is not null) {
			PreviousSpriteInstance.Dispose(true);
			PreviousSpriteInstance = null;
		}

		if (Reference.TryGetTarget(out var reference)) {
			SpriteMap.Remove(this, reference);
		}
		if (RecentAccessNode is not null) {
			lock (RecentAccessList) {
				RecentAccessList.Remove(RecentAccessNode);
			}
			RecentAccessNode = null;
		}
		IsDisposed = true;

		GC.SuppressFinalize(this);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void OnParentDispose(Texture2D? texture) {
		Debug.TraceLn($"Parent Texture Disposing: {texture?.SafeName() ?? "[NULL]"}, disposing ManagedSpriteInstance");

		Dispose();
	}
}
