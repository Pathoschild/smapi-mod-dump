using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;

using System.Linq;
using System.Collections.Generic;

using WeakTexture = System.WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>;
using SpriteMaster.Types;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal sealed partial class ScaledTexture : IDisposable {
		// TODO : This can grow unbounded. Should fix.
		public static readonly SpriteMap SpriteMap = new SpriteMap();

		private static readonly LinkedList<WeakReference<ScaledTexture>> MostRecentList = new LinkedList<WeakReference<ScaledTexture>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool LegalFormat(Texture2D texture) {
			return AllowedFormats.Contains(texture.Format);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static internal ScaledTexture Fetch (Texture2D texture, Bounds source, uint expectedScale) {
			if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
				return scaleTexture;
			}

			return null;
		}

		private static readonly TexelTimer TexelAverage = new TexelTimer();
		private static readonly TexelTimer TexelAverageCached = new TexelTimer();
		private static readonly TexelTimer TexelAverageSync = new TexelTimer();
		private static readonly TexelTimer TexelAverageCachedSync = new TexelTimer();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TexelTimer GetTimer(bool cached, bool async) {
			if (async) {
				return cached ? TexelAverageCached : TexelAverage;
			}
			else {
				return cached ? TexelAverageCachedSync : TexelAverageSync;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TexelTimer GetTimer(Texture2D texture, bool async) {
			var IsCached = SpriteInfo.IsCached(texture);
			return GetTimer(IsCached, async);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static internal ScaledTexture Get (Texture2D texture, Bounds source, uint expectedScale) {
			using var _ = Performance.Track();

			if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
				return scaleTexture;
			}

			if (!Validate(texture)) {
				return null;
			}

			bool useAsync = (Config.AsyncScaling.EnabledForUnknownTextures || !texture.Anonymous()) && (texture.Area() >= Config.AsyncScaling.MinimumSizeTexels);
			// !texture.Meta().HasCachedData

			var estimatedDuration = GetTimer(texture: texture, async: useAsync).Estimate(texture.Area());
			const float multiplier = 0.75f;
			var remainingTime = DrawState.RemainingFrameTime(multiplier: multiplier);
			if (DrawState.PushedUpdateWithin(1) && estimatedDuration >= remainingTime) {
				return null;
			}

			// TODO : We should really only populate the average when we are performing an expensive operation like GetData.
			var begin = DateTime.Now;

			bool isSprite = (!source.Offset.IsZero || source.Extent != texture.Extent());
			SpriteInfo textureWrapper;
			ulong hash;

			using (Performance.Track("new SpriteInfo"))
				textureWrapper = new SpriteInfo(reference: texture, dimensions: source, expectedScale: expectedScale);

			// If this is null, it can only happen due to something being blocked, so we should try again later.
			if (textureWrapper.Data == null) {
				return null;
			}

			DrawState.PushedUpdateThisFrame = true;

			try {
				using (Performance.Track("Upscaler.GetHash"))
					hash = Upscaler.GetHash(textureWrapper, isSprite);

				var newTexture = new ScaledTexture(
					assetName: texture.SafeName(),
					textureWrapper: textureWrapper,
					sourceRectangle: source,
					isSprite: isSprite,
					hash: hash,
					async: useAsync,
					expectedScale: expectedScale
				);
				if (useAsync && Config.AsyncScaling.Enabled) {
					// It adds itself to the relevant maps.
					if (newTexture.IsReady && newTexture.Texture != null) {
						return newTexture;
					}
					return null;
				}
				else {
					return newTexture;
				}
			}
			finally {
				var averager = GetTimer(cached: textureWrapper.WasCached, async: useAsync);
				averager.Add(texture.Area(), DateTime.Now - begin);
			}
		}

		internal static readonly SurfaceFormat[] AllowedFormats = {
			SurfaceFormat.Color,
			//SurfaceFormat.Dxt3 // fonts
		};

		internal ManagedTexture2D Texture = null;
		internal readonly string Name;
		internal Vector2 Scale;
		internal readonly bool IsSprite;
		internal volatile bool IsReady = false;

		internal Vector2B Wrapped = new Vector2B(false);

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		~ScaledTexture() {
			if (!Disposed) {
				Dispose();
			}
		}

		internal static volatile uint TotalMemoryUsage = 0;

		internal long MemorySize {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				if (!IsReady || Texture == null) {
					return 0;
				}
				return Texture.SizeBytes();
			}
		}

		internal long OriginalMemorySize {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return originalSize.Width * originalSize.Height * sizeof(int);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Purge (Texture2D reference) {
			Purge(reference, null, DataRef<byte>.Null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Purge (Texture2D reference, Bounds? bounds, DataRef<byte> data) {
			SpriteInfo.Purge(reference, bounds, data);
			SpriteMap.Purge(reference, bounds);
			Upscaler.PurgeHash(reference);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void PurgeTextures(long _purgeTotalBytes) {
			Contract.AssertPositive(_purgeTotalBytes);

			Debug.TraceLn($"Attempting to purge {_purgeTotalBytes.AsDataSize()} from currently loaded textures");

			// For portability purposes
			if (IntPtr.Size == 8) {
				var purgeTotalBytes = _purgeTotalBytes;
				lock (MostRecentList) {
					long totalPurge = 0;
					while (purgeTotalBytes > 0 && MostRecentList.Count > 0) {
						if (MostRecentList.Last().TryGetTarget(out var target)) {
							var textureSize = unchecked((long)target.MemorySize);
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
				var purgeTotalBytes = unchecked((uint)_purgeTotalBytes);
				lock (MostRecentList) {
					uint totalPurge = 0;
					while (purgeTotalBytes > 0 && MostRecentList.Count > 0) {
						if (MostRecentList.Last().TryGetTarget(out var target)) {
							var textureSize = unchecked((uint)target.MemorySize);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ScaledTexture (string assetName, SpriteInfo textureWrapper, Bounds sourceRectangle, ulong hash, bool isSprite, bool async, uint expectedScale) {
			using var _ = Performance.Track();

			IsSprite = isSprite;
			Hash = hash;
			var source = textureWrapper.Reference;

			this.OriginalSourceRectangle = new Bounds(sourceRectangle);
			this.Reference = source.MakeWeak();
			this.sourceRectangle = sourceRectangle;
			this.refScale = expectedScale;
			SpriteMap.Add(source, this, sourceRectangle, expectedScale);

			this.Name = source.Anonymous() ? assetName.SafeName() : source.SafeName();
			originalSize = IsSprite ? sourceRectangle.Extent : new Vector2I(source);

			if (async && Config.AsyncScaling.Enabled) {
				ThreadQueue.Queue((SpriteInfo wrapper) => {
					Thread.CurrentThread.Priority = ThreadPriority.Lowest;
					using var _ = new AsyncTracker($"Resampling {Name} [{sourceRectangle}]");
					Thread.CurrentThread.Name = "Texture Resampling Thread";
					Upscaler.Upscale(
						texture: this,
						scale: ref refScale,
						input: wrapper,
						desprite: IsSprite,
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
					desprite: IsSprite,
					hash: Hash,
					wrapped: ref Wrapped,
					async: false
				);

				if (this.Texture != null) {
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Finish () {
			ManagedTexture2D texture;
			lock (this) {
				texture = Texture;
			}

			if (texture?.IsDisposed == true) {
				return;
			}

			UpdateReferenceFrame();

			TotalMemoryUsage += (uint)texture.SizeBytes();
			texture.Disposing += (object sender, EventArgs args) => { TotalMemoryUsage -= (uint)texture.SizeBytes(); };

			if (IsSprite) {
				Debug.TraceLn($"Creating HD Sprite [{texture.Format} x{refScale}]: {this.SafeName()} {sourceRectangle}");
			}
			else {
				Debug.TraceLn($"Creating HD Spritesheet [{texture.Format} x{refScale}]: {this.SafeName()}");
			}

			this.Scale = (Vector2)texture.Dimensions / new Vector2(originalSize.Width, originalSize.Height);

			IsReady = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void UpdateReferenceFrame () {
			if (Disposed) {
				return;
			}

			this.LastReferencedFrame = DrawState.CurrentFrame;

			lock (MostRecentList) {
				if (CurrentRecentNode != null) {
					MostRecentList.Remove(CurrentRecentNode);
					MostRecentList.AddFirst(CurrentRecentNode);
				}
				else {
					CurrentRecentNode = MostRecentList.AddFirst(this.MakeWeak());
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose (bool disposeChildren) {
			if (disposeChildren && Texture != null) {
				if (!Texture.IsDisposed) {
					Texture.Dispose();
				}
				Texture = null;
			}
			Dispose();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose () {
			if (Disposed) {
				return;
			}

			if (Reference.TryGetTarget(out var reference)) {
				SpriteMap.Remove(this, reference);
			}
			if (CurrentRecentNode != null) {
				lock (MostRecentList) {
					MostRecentList.Remove(CurrentRecentNode);
				}
				CurrentRecentNode = null;
			}
			Disposed = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnParentDispose (Texture2D texture) {
			Debug.TraceLn($"Parent Texture Disposing: {texture.SafeName()}");

			Dispose();
		}
	}
}
