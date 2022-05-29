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
using SpriteMaster.Caching;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static SpriteMaster.ResourceManager;
using WeakInstanceList = System.Collections.Generic.LinkedList<System.WeakReference<SpriteMaster.ManagedSpriteInstance>>;
using WeakInstanceListNode = System.Collections.Generic.LinkedListNode<System.WeakReference<SpriteMaster.ManagedSpriteInstance>>;
using WeakTexture = System.WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>;

namespace SpriteMaster;

internal sealed class ManagedSpriteInstance : IDisposable {
	private static readonly WeakInstanceList RecentAccessList = new();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool HasLegalFormat(XTexture2D texture) => AllowedFormats.ContainsF(texture.Format);

	private static void PurgeInvalidated(XTexture2D texture) {
		// If somehow it passed validation earlier (like a SetData before a name) make sure no cached data
		// or handles are left dangling around.

		var meta = texture.Meta();
		meta.Purge();
	}

	internal static ulong GetHash(SpriteInfo info, TextureType type) => Resampler.GetHash(info, type);

	internal static ulong? GetHash(in SpriteInfo.Initializer info, TextureType type) => Resampler.GetHash(in info, type);

	internal static bool Validate(XTexture2D texture, bool clean = false) {
		var meta = texture.Meta();
		if (meta.Validation.HasValue) {
			return meta.Validation.Value;
		}

		if (texture is InternalTexture2D) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Internal Texture");
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
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is System Render Target");
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
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Render Target");
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
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Too Small: ({texture.Extent().ToString(DrawingColor.Orange)} <= {Config.Resample.MinimumTextureDimensions.ToString(DrawingColor.Orange)})");
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
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Zero Area (Degenerate)");
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
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Zombie");
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
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Multi-Level Textures Unsupported: {texture.LevelCount.ToString(DrawingColor.Orange)} levels");
			}
			meta.Validation = false;
			return false;
		}

		if (!HasLegalFormat(texture)) {
			if (!meta.TracePrinted) {
				meta.TracePrinted = true;
				Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Format Unsupported: {texture.Format.ToString(DrawingColor.Orange)}");
			}
			meta.Validation = false;
			if (clean) {
				PurgeInvalidated(texture);
			}
			return false;
		}

		if (!texture.Anonymous()) {
			foreach (var blacklistPattern in Config.Resample.BlacklistPatterns) {
				if (blacklistPattern.IsMatch(texture.NormalizedName())) {
					if (!meta.TracePrinted) {
						meta.TracePrinted = true;
						Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Blacklisted ({blacklistPattern})");
					}
					meta.Validation = false;
					if (clean) {
						PurgeInvalidated(texture);
					}
					return false;
				}
			}

			bool isText = texture.NormalizedName().StartsWith(@"Fonts\");
			bool isBasicText = texture.Format == SurfaceFormat.Dxt3;

			if (!(Configuration.Preview.Override.Instance?.ResampleText ?? Config.Resample.EnabledText)) {
				if (isText) {
					if (!meta.TracePrinted) {
						meta.TracePrinted = true;
						Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Font (and text resampling is disabled)");
					}
					meta.Validation = false;
					if (clean) {
						PurgeInvalidated(texture);
					}
					return false;
				}
			}
			if (!(Configuration.Preview.Override.Instance?.ResampleBasicText ?? Config.Resample.EnabledBasicText)) {
				// The only BC2 texture that I've _ever_ seen is the internal font
				if (isBasicText) {
					if (!meta.TracePrinted) {
						meta.TracePrinted = true;
						Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Basic Font (and basic text resampling is disabled)");
					}
					meta.Validation = false;
					if (clean) {
						PurgeInvalidated(texture);
					}
					return false;
				}
			}
			if (!(Configuration.Preview.Override.Instance?.ResampleSprites ?? Config.Resample.EnabledSprites)) {
				if (!isText && !isBasicText) {
					if (!meta.TracePrinted) {
						meta.TracePrinted = true;
						Debug.Trace($"Not Scaling Texture '{texture.NormalizedName(DrawingColor.LightYellow)}', Is Sprite (and sprite resampling is disabled)");
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static TexelTimer GetTimer(bool cached, bool async) {
		if (async) {
			return cached ? TexelAverageCached : TexelAverage;
		}
		else {
			return cached ? TexelAverageCachedSync : TexelAverageSync;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static TexelTimer GetTimer(XTexture2D texture, bool async, out bool isCached) {
		var isTextureCached = SpriteInfo.IsCached(texture);
		isCached = isTextureCached;
		return GetTimer(isTextureCached, async);
	}

#if DEBUG
	private static TimeSpan MeanTimeSpan = TimeSpan.Zero;
	private static int TimeSpanSamples = 0;
#endif

	internal static ManagedSpriteInstance? Fetch(XTexture2D texture, Bounds source, uint expectedScale) {
		if (!Config.IsEnabled || !Config.Resample.IsEnabled) {
			return null;
		}

		if (!Validate(texture, clean: true)) {
			return null;
		}

		if (SpriteMap.TryGetReady(texture, source, expectedScale, out var scaleTexture)) {
			if (scaleTexture.NoResample) {
				return null;
			}

			return scaleTexture;
		}

		return null;
	}

	internal static bool TryResurrect(in SpriteInfo.Initializer initializer, [NotNullWhen(true)] out ManagedSpriteInstance? resurrected) {
		// Check for a suspended sprite instance that happens to match.
		if (!Config.SuspendedCache.Enabled) {
			resurrected = null;
			return false;
		}

		var spriteHash = GetHash(initializer, initializer.TextureType);
		if (spriteHash.HasValue && SuspendedSpriteCache.TryFetch(spriteHash.Value, out var instance)) {
			var spriteMapHash = SpriteMap.SpriteHash(initializer.Reference, initializer.Bounds, initializer.ExpectedScale, initializer.IsPreview);
			if (instance.Resurrect(initializer.Reference, spriteMapHash)) {
				resurrected = instance;
				return true;
			}
		}

		resurrected = null;
		return false;
	}

	internal static bool TryResurrect(SpriteInfo info, [NotNullWhen(true)] out ManagedSpriteInstance? resurrected) {
		// Check for a suspended sprite instance that happens to match.
		if (!Config.SuspendedCache.Enabled) {
			resurrected = null;
			return false;
		}

		var spriteHash = GetHash(info, info.TextureType);
		if (SuspendedSpriteCache.TryFetch(spriteHash, out var instance)) {
			var spriteMapHash = SpriteMap.SpriteHash(info.Reference, info.Bounds, info.ExpectedScale, info.IsPreview);
			if (instance.Resurrect(info.Reference, spriteMapHash)) {
				resurrected = instance;
				return true;
			}
		}

		resurrected = null;
		return false;
	}

	internal static ManagedSpriteInstance? FetchOrCreate(XTexture2D texture, Bounds source, uint expectedScale, bool sliced) {
		if (!Config.IsEnabled || !Config.Resample.IsEnabled) {
			return null;
		}

		if (!Validate(texture, clean: true)) {
			return null;
		}

		bool textureChain = false;
		ManagedSpriteInstance? currentInstance = null;

		if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
			if (scaleTexture.Invalidated) {
				currentInstance = scaleTexture;
				textureChain = true;
			}
			else switch (scaleTexture.IsReady)
			{
				case true:
					return scaleTexture;
				case false when scaleTexture.PreviousSpriteInstance is not null && scaleTexture.PreviousSpriteInstance.IsReady:
					currentInstance = scaleTexture.PreviousSpriteInstance;
					textureChain = false;
					break;
				default:
					currentInstance = scaleTexture;
					textureChain = false;
					break;
			}
		}

		if ((currentInstance?.NoResample ?? false) || texture.Meta().IsNoResample(source)) {
			return null;
		}

		bool useStalling = Config.Resample.UseFrametimeStalling && !GameState.IsLoading;

		bool useAsync = Config.AsyncScaling.Enabled && (Config.AsyncScaling.EnabledForUnknownTextures || !texture.Anonymous()) && (source.Area >= Config.AsyncScaling.MinimumSizeTexels);
		// !texture.Meta().HasCachedData

		TimeSpan? remainingTime = null;
		bool? isCached = null;

		string GetMetadataString() {
			return isCached.HasValue ?
				$" ({(useAsync ? "async" : "sync".Pastel(DrawingColor.Orange))} {(isCached.Value ? "cached" : "uncached".Pastel(DrawingColor.Orange))})" :
				$" ({(useAsync ? "async" : "sync".Pastel(DrawingColor.Orange))})";
		}

		string GetNameString() {
			return $"'{texture.NormalizedName(DrawingColor.LightYellow)}'{GetMetadataString()}";
		}

		// TODO : We should really only populate the average when we are performing an expensive operation like GetData.
		var watch = System.Diagnostics.Stopwatch.StartNew();

		TextureType textureType;
		if (sliced) {
			textureType = TextureType.SlicedImage;
		}
		else if (!source.Offset.IsZero || source.Extent != texture.Extent()) {
			textureType = TextureType.Sprite;
		}
		else {
			textureType = TextureType.Image;
		}

		// TODO : break this up somewhat so that we can delay hashing for things by _one_ frame (still deterministic, but offset so we can parallelize the work).
		// Presently, this cannot be done because the initializer is a 'ref struct' and is used immediately. If we want to check the suspended cache, it needs to be jammed away
		// so the hashing can be performed before the next frame.
		SpriteInfo.Initializer spriteInfoInitializer = new SpriteInfo.Initializer(
			reference: texture,
			dimensions: source,
			expectedScale: expectedScale,
			textureType: textureType,
			animated: texture.Meta().IsAnimated(source)
		);

		// Check for a suspended sprite instance that happens to match.
		if (TryResurrect(in spriteInfoInitializer, out var resurrectedInstance)) {
			return resurrectedInstance;
		}

		if (!textureChain && currentInstance is not null) {
			return currentInstance;
		}

		if (useStalling && DrawState.PushedUpdateWithin(0)) {
			remainingTime = DrawState.RemainingFrameTime();
			if (remainingTime <= TimeSpan.Zero) {
				return currentInstance;
			}

			var estimatedDuration = GetTimer(texture: texture, async: useAsync, out bool cached).Estimate(texture.Format.SizeBytes(source.Area));
			isCached = cached;
			if (estimatedDuration > TimeSpan.Zero && estimatedDuration > remainingTime) {
				Debug.Trace($"Not enough frame time left to begin resampling {GetNameString()} ({estimatedDuration.TotalMilliseconds.ToString(DrawingColor.LightBlue)} ms >= {remainingTime?.TotalMilliseconds.ToString(DrawingColor.LightBlue)} ms)");
				return currentInstance;
			}
		}

		string GetRemainingTime() {
			return remainingTime.HasValue
				? $" (remaining time: {remainingTime.Value.TotalMilliseconds.ToString(DrawingColor.LightYellow)} ms)" :
				"";
		}

		// If this is null, it can only happen due to something being blocked, so we should try again later.
		if (spriteInfoInitializer.ReferenceData is null) {
			Debug.Trace($"Texture Data fetch for {GetNameString()} was {"blocked".Pastel(DrawingColor.Red)}; retrying later{GetRemainingTime()}");
			return currentInstance;
		}

		DrawState.IsUpdatedThisFrame = true;

		try {
			bool doDispatch = true;
			var textureMeta = texture.Meta();
			var currentRevision = textureMeta.Revision;
			if (textureMeta.InFlightTasks.TryGetValue(source, out var inFlightTask)) {
				doDispatch = inFlightTask.Revision != currentRevision || inFlightTask.ResampleTask.Status != TaskStatus.WaitingToRun;
			}

			Task<ManagedSpriteInstance> resampleTask;
			if (!useAsync || doDispatch) {
				resampleTask = ResampleTask.Dispatch(
					spriteInfo: new(spriteInfoInitializer),
					async: useAsync,
					previousInstance: currentInstance
				);
				textureMeta.InFlightTasks[source] = new(currentRevision, resampleTask);
			}
			else {
				return null;
			}

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
#if DEBUG
			TimeSpanSamples++;
			MeanTimeSpan += duration;
			var remainingTimeStr = GetRemainingTime();
			if (!string.IsNullOrEmpty(remainingTimeStr)) {
				remainingTimeStr = $"({remainingTimeStr} was remaining)";
			}
			Debug.Trace($"Rescale Duration {GetNameString()}: {(MeanTimeSpan / TimeSpanSamples).TotalMilliseconds.ToString(DrawingColor.LightYellow)} ms {remainingTimeStr}");
#endif
			averager.Add(source.Area, duration);
		}
	}

	internal static readonly SurfaceFormat[] AllowedFormats = {
		SurfaceFormat.Color,
		SurfaceFormat.Dxt5,
		SurfaceFormat.Dxt5SRgb,
		SurfaceFormat.Dxt3 // fonts
	};

	internal ManagedTexture2D? Texture = null;
	internal readonly string Name;
	internal Vector2F Scale;
	internal readonly TextureType TexType;
	private volatile bool IsReadyInternal = false;
	internal bool IsReady => IsReadyInternal && Texture is not null;

	internal readonly Vector2B Wrapped = Vector2B.False;

	internal readonly WeakTexture Reference;
	internal readonly Bounds OriginalSourceRectangle;
	internal readonly ulong Hash = 0U;

	internal PaddingQuad Padding = PaddingQuad.Zero;
	internal Vector2I UnpaddedSize;
	internal Vector2I BlockPadding = Vector2I.Zero;
	private readonly Vector2I OriginalSize;
	private readonly Bounds SourceRectangle;
	internal IScalerInfo? ScalerInfo = null;
	internal ulong SpriteMapHash { get; private set; }
	private readonly uint ReferenceScale;
	internal ManagedSpriteInstance? PreviousSpriteInstance = null;
	internal volatile bool Invalidated = false;
	internal volatile bool Suspended = false;
	internal bool NoResample = false;
	internal readonly bool IsPreview = false;

	internal ulong LastReferencedFrame = DrawState.CurrentFrame;

	internal Vector2F InnerRatio = Vector2F.One;

	/// <summary>
	/// Node into the most-recent accessed instance list.
	/// Should only be <seealso langword="null"/> after the instance is <seealso cref="ManagedSpriteInstance.Dispose">disposed</seealso>
	/// </summary>
	internal WeakInstanceListNode? RecentAccessNode = null;

	private volatile bool IsDisposedInternal = false;

	internal bool IsDisposed {
		get => IsDisposedInternal;
		private set => IsDisposedInternal = value;
	}

	internal static long TotalMemoryUsage = 0U;

	internal long MemorySize {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get {
			if (!IsReady || Texture is null) {
				return 0;
			}
			return Texture.SizeBytes();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Purge(XTexture2D reference, Bounds? bounds, in DataRef<byte> data, bool animated = false) {
		SpriteInfo.Purge(reference, bounds, data, animated: animated);
		if (data.IsNull) {
			SpriteMap.Purge(reference, bounds, animated: animated);
		}
		else {
			SpriteMap.Invalidate(reference, bounds, animated: animated);
		}
		//Resampler.PurgeHash(reference);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void FullPurge(XTexture2D reference, bool animated = false) {
		SpriteInfo.Purge(reference, reference.Bounds, DataRef<byte>.Null, animated: animated);
		SpriteMap.Purge(reference, reference.Bounds, animated: animated);
	}

	internal static void PurgeTextures(long purgeTotalBytes) {
		purgeTotalBytes.AssertPositive();

		Debug.Trace($"Attempting to purge {purgeTotalBytes.AsDataSize()} from currently loaded textures");

		lock (RecentAccessList) {
			long totalPurge = 0;
			while (purgeTotalBytes > 0 && RecentAccessList.Count > 0) {
				if (RecentAccessList.Last?.Value.TryGet(out var target) ?? false) {
					var textureSize = target.MemorySize;
					Debug.Trace($"Purging {target.NormalizedName()} ({textureSize.AsDataSize()})");
					purgeTotalBytes -= textureSize;
					totalPurge += textureSize;
					target.RecentAccessNode = null;
					target.Dispose(true);
				}
				RecentAccessList.RemoveLast();
			}
			Debug.Trace($"Total Purged: {totalPurge.AsDataSize()}");
		}
	}

	internal ManagedSpriteInstance(string assetName, SpriteInfo spriteInfo, Bounds sourceRectangle, TextureType textureType, bool async, uint expectedScale, ManagedSpriteInstance? previous = null) {
		PreviousSpriteInstance = previous;

		TexType = textureType;

		var source = spriteInfo.Reference;

		OriginalSourceRectangle = sourceRectangle;
		Reference = source.MakeWeak();
		SourceRectangle = sourceRectangle;
		ReferenceScale = expectedScale;
		IsPreview = spriteInfo.IsPreview;
		SpriteMapHash = SpriteMap.SpriteHash(source, sourceRectangle, expectedScale, spriteInfo.IsPreview);
		Name = source.Anonymous() ? assetName.NormalizedName() : source.NormalizedName();
		// TODO : I believe we need a lock here until when the texture is _fully created_, preventing new instantiations from starting of a texture
		// already in-flight
		if (!SpriteMap.AddReplaceInvalidated(source, this)) {
			// If false, then the sprite already exists in the map (which can be caused by gap between the Resample task being kicked off, and hitting this, and _another_ sprite getting
			// past the earlier try-block, and getting here.
			// TODO : this should be fixed by making sure that all of the resample tasks _at least_ get to this point before the end of the frame.
			// TODO : That might not be sufficient either if the _same_ draw ends up happening again.
			Dispose();

			return;
		}

		OriginalSize = TexType switch {
			TextureType.Sprite => sourceRectangle.Extent,
			TextureType.Image => source.Extent(),
			TextureType.SlicedImage => sourceRectangle.Extent,
			_ => OriginalSize
		};

		// TODO store the HD Texture in _this_ object instead. Will confuse things like subtexture updates, though.
		Hash = GetHash(spriteInfo, textureType);
		Texture = Resampler.Upscale(
			spriteInstance: this,
			scale: ref ReferenceScale,
			input: spriteInfo,
			hash: Hash,
			wrapped: ref Wrapped,
			async: false
		);

		// TODO : I would love to dispose of this texture _now_, but we rely on it disposing to know if we need to dispose of ours.
		// There is a better way to do this using weak references, I just need to analyze it further. Memory leaks have been a pain so far.
		// TODO 2: This won't get hit if the texture is disposed of via the finalizer.
		source.Disposing += OnParentDispose;

		lock (RecentAccessList) {
			RecentAccessNode = RecentAccessList.AddFirst(this.MakeWeak());
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public void OnParentDispose(object? sender, EventArgs args) => OnParentDispose(sender as XTexture2D);

	// Async Call
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
		// TODO : this won't get hit if the texture is disposed of via the finalizer
		texture.Disposing += (_, _) => { Interlocked.Add(ref TotalMemoryUsage, -texture.SizeBytes()); };

		{
#if DEBUG
			var formatString = texture.Format.ToString(DrawingColor.LightCoral);
			var nameString = this.NormalizedName(DrawingColor.LightYellow);
			switch (TexType) {
				case TextureType.Sprite:
					Debug.Trace($"Creating Sprite [{formatString} x{ReferenceScale}]: {nameString} {SourceRectangle}");
					break;
				case TextureType.Image:
					Debug.Trace($"Creating Image [{formatString} x{ReferenceScale}]: {nameString}");
					break;
				case TextureType.SlicedImage:
					Debug.Trace($"Creating Sliced Image [{formatString} x{ReferenceScale}]: {nameString}");
					break;
				default:
					Debug.Trace($"Creating UNKNOWN [{formatString} x{ReferenceScale}]: {nameString}");
					break;
			}
#endif
		}

		Scale = (Vector2F)texture.Dimensions / (Vector2F)OriginalSize;

		Thread.MemoryBarrier();
		IsReadyInternal = true;
		if (PreviousSpriteInstance is not null) {
			PreviousSpriteInstance.Suspend(true);
			PreviousSpriteInstance = null;
		}
	}

	internal void UpdateReferenceFrame() {
		if (IsDisposed) {
			return;
		}

		LastReferencedFrame = DrawState.CurrentFrame;

		lock (RecentAccessList) {
			if (RecentAccessNode is not null) {
				if (RecentAccessNode.List == RecentAccessList) {
					try {
						RecentAccessList.Remove(RecentAccessNode);
					}
					catch {
						// Failure is unimportant.
					}
				}
				RecentAccessList.AddFirst(RecentAccessNode);
			}
			else {
				RecentAccessNode = RecentAccessList.AddFirst(this.MakeWeak());
			}
		}
	}

	internal readonly struct CleanupData {
		internal readonly ManagedSpriteInstance? PreviousSpriteInstance;
		internal readonly WeakReference<XTexture2D> ReferenceTexture;
		internal readonly LinkedListNode<WeakReference<ManagedSpriteInstance>>? RecentAccessNode;
		internal readonly ulong MapHash;

		internal CleanupData(ManagedSpriteInstance instance) {
			PreviousSpriteInstance = instance.PreviousSpriteInstance;
			ReferenceTexture = instance.Reference;
			RecentAccessNode = instance.RecentAccessNode;
			instance.RecentAccessNode = null;
			MapHash = instance.SpriteMapHash;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	~ManagedSpriteInstance() {
		if (IsDisposed) {
			return;
		}
		//Debug.Trace($"ManagedSpriteInstance '{Name}' reached destructor; was not initially disposed");
		ReleasedSpriteInstances.Push(new(this));
	}

	internal void Dispose(bool disposeChildren) {
		if (disposeChildren && Texture is not null) {
			if (!Texture.IsDisposed) {
				Texture.Dispose();
			}
			Texture = null;
		}
		Dispose();
	}

	public void Dispose() {
		if (IsDisposed) {
			return;
		}

		if (PreviousSpriteInstance is not null) {
			PreviousSpriteInstance.Suspend(true);
			PreviousSpriteInstance = null;
		}

		if (Reference.TryGetTarget(out var reference)) {
			SpriteMap.Remove(this, reference);
			reference.Disposing -= OnParentDispose;
		}
		if (RecentAccessNode is not null) {
			lock (RecentAccessList) {
				try {
					RecentAccessList.Remove(RecentAccessNode);
				}
				catch {
					// Error is unimportant
				}
			}
			RecentAccessNode = null;
		}
		IsDisposed = true;

		if (Suspended) {
			SuspendedSpriteCache.Remove(this);
			Suspended = false;
		}

		GC.SuppressFinalize(this);
	}

	internal static void Cleanup(in CleanupData data) {
		Debug.Trace("Cleaning up finalized ManagedSpriteInstance");

		if (data.PreviousSpriteInstance is not null) {
			data.PreviousSpriteInstance.Suspend(true);
		}

		if (data.ReferenceTexture.TryGetTarget(out var reference)) {
			SpriteMap.Remove(data, reference);
		}

		if (data.RecentAccessNode is not null) {
			lock (RecentAccessList) {
				if (data.RecentAccessNode.List == RecentAccessList) {
					try {
						RecentAccessList.Remove(data.RecentAccessNode);
					}
					catch {
						// Error is unimportant
					}
				}
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void OnParentDispose(XTexture2D? texture) {
		Debug.Trace($"Parent Texture Disposing: {texture?.NormalizedName() ?? "[NULL]"}, suspending/disposing ManagedSpriteInstance");

		Suspend();
	}

	internal void Suspend(bool clearChildrenIfDispose = false) {
		if (IsDisposed || Suspended) {
			return;
		}

		if (StardewValley.Game1.quit) {
			return;
		}

		if (!IsReadyInternal || !Config.SuspendedCache.Enabled) {
			Dispose(clearChildrenIfDispose);
			return;
		}

		// TODO : Handle clearing any reference to _this_
		PreviousSpriteInstance = null;

		if (RecentAccessNode is not null) {
			lock (RecentAccessList) {
				try {
					RecentAccessList.Remove(RecentAccessNode);
				}
				catch {
					// Error is unimportant
				}
			}
			RecentAccessNode = null;
		}
		Invalidated = false;

		Reference.SetTarget(null!);

		Suspended = true;
		SuspendedSpriteCache.Add(this);

		Debug.Trace($"Sprite Instance '{Name}' {"Suspended".Pastel(DrawingColor.LightGoldenrodYellow)}");
	}

	internal bool Resurrect(XTexture2D texture, ulong spriteMapHash) {
		if (StardewValley.Game1.quit) {
			return false;
		}

		if (IsDisposed || !Suspended) {
			SuspendedSpriteCache.Remove(this);
			return false;
		}

		if (!IsReadyInternal || !Config.SuspendedCache.Enabled) {
			SuspendedSpriteCache.Remove(this);
			return false;
		}

		SuspendedSpriteCache.Remove(this);
		Suspended = false;
		Reference.SetTarget(texture);

		SpriteMapHash = spriteMapHash;
		texture.Meta().ReplaceInSpriteInstanceTable(SpriteMapHash, this);
		SpriteMap.AddReplace(texture, this);

		Debug.Trace($"Sprite Instance '{Name}' {"Resurrected".Pastel(DrawingColor.LightCyan)}");

		return true;
	}
}
