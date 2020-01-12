using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;

using System.Linq;
using System.Collections.Generic;

using WeakTexture = System.WeakReference<Microsoft.Xna.Framework.Graphics.Texture2D>;
using SpriteMaster.Types;
using SpriteMaster.Extensions;
using TeximpNet.Compression;
using System.Diagnostics;
using SpriteMaster.Metadata;

namespace SpriteMaster {
	sealed class SpriteMap {
		private readonly SharedLock Lock = new SharedLock();
		private readonly WeakCollection<ScaledTexture> ScaledTextureReferences = new WeakCollection<ScaledTexture>();

		static private ulong SpriteHash (Texture2D texture, Bounds source, int expectedScale) {
			return Hashing.CombineHash(ScaledTexture.ExcludeSprite(texture) ? 0UL : source.Hash(), expectedScale.GetHashCode());
		}

		internal void Add (Texture2D reference, ScaledTexture texture, Bounds source, int expectedScale) {
			lock (reference.Meta()) {
				var Map = reference.Meta().SpriteTable;
				var rectangleHash = SpriteHash(texture: reference, source: source, expectedScale: expectedScale);

				using (Lock.Exclusive) {
					ScaledTextureReferences.Add(texture);
					Map.Add(rectangleHash, texture);
				}
			}
		}

		internal bool TryGet (Texture2D texture, Bounds source, int expectedScale, out ScaledTexture result) {
			result = null;

			lock (texture.Meta()) {
				var Map = texture.Meta().SpriteTable;

				using (Lock.Shared) {
					var rectangleHash = SpriteHash(texture: texture, source: source, expectedScale: expectedScale);
					if (Map.TryGetValue(rectangleHash, out var scaledTexture)) {
						if (scaledTexture.Texture?.IsDisposed == true) {
							using (Lock.Promote) {
								Map.Clear();
							}
						}
						else {
							if (scaledTexture.IsReady) {
								result = scaledTexture;
							}
							return true;
						}
					}
				}
			}

			return false;
		}

		internal void Remove (ScaledTexture scaledTexture, Texture2D texture) {
			try {
				lock (texture.Meta()) {
					var Map = texture.Meta().SpriteTable;

					using (Lock.Exclusive) {

						try {
							ScaledTextureReferences.Purge();
							var removeElements = new List<ScaledTexture>();
							foreach (var element in ScaledTextureReferences) {
								if (element == scaledTexture) {
									removeElements.Add(element);
								}
							}

							foreach (var element in removeElements) {
								ScaledTextureReferences.Remove(element);
							}
						}
						catch { }

						Map.Clear();
					}
				}
			}
			finally {
				if (scaledTexture.Texture != null && !scaledTexture.Texture.IsDisposed) {
					Debug.TraceLn($"Disposing Active HD Texture: {scaledTexture.SafeName()}");

					//scaledTexture.Texture.Dispose();
				}
			}
		}

		internal void Purge (Texture2D reference, Bounds? sourceRectangle = null) {
			try {
				using (Lock.Shared) {
					lock (reference.Meta()) {
						var Map = reference.Meta().SpriteTable;
						if (Map.Count == 0) {
							return;
						}

						// TODO handle sourceRectangle meaningfully.
						using (Lock.Promote) {
							Debug.TraceLn($"Purging Texture {reference.SafeName()}");

							foreach (var scaledTexture in Map.Values) {
								lock (scaledTexture) {
									if (scaledTexture.Texture != null) {
										scaledTexture.Texture.Dispose();
									}
									scaledTexture.Texture = null;
								}
							}

							Map.Clear();
							// TODO dispose sprites?
						}
					}
				}
			}
			catch { }
		}

		internal void SeasonPurge (string season) {
			try {
				var purgeList = new List<ScaledTexture>();
				using (Lock.Shared) {
					foreach (var scaledTexture in ScaledTextureReferences) {
						if (
							(scaledTexture.Name.ToLower().Contains("spring") ||
							scaledTexture.Name.ToLower().Contains("summer") ||
							scaledTexture.Name.ToLower().Contains("fall") ||
							scaledTexture.Name.ToLower().Contains("winter")) &&
							!scaledTexture.Name.ToLower().Contains(season.ToLower())
						) {
							purgeList.Add(scaledTexture);
						}
					}
				}
				using (Lock.Exclusive) {
					foreach (var purgable in purgeList) {
						if (purgable.Reference.TryGetTarget(out var reference)) {
							purgable.Dispose();
							lock (reference.Meta()) {
								reference.Meta().SpriteTable.Clear();
							}
						}
					}
				}
			}
			catch { }
		}

		internal Dictionary<Texture2D, List<ScaledTexture>> GetDump () {
			var result = new Dictionary<Texture2D, List<ScaledTexture>>();

			foreach (var scaledTexture in ScaledTextureReferences) {
				if (scaledTexture.Reference.TryGetTarget(out var referenceTexture)) {
					List<ScaledTexture> resultList;
					if (!result.TryGetValue(referenceTexture, out resultList)) {
						resultList = new List<ScaledTexture>();
						result.Add(referenceTexture, resultList);
					}
					resultList.Add(scaledTexture);
				}
			}

			return result;
		}
	}

	internal sealed class ScaledTexture : IDisposable {
		// TODO : This can grow unbounded. Should fix.
		public static readonly SpriteMap SpriteMap = new SpriteMap();

		private static readonly List<Action> PendingActions = Config.AsyncScaling.Enabled ? new List<Action>() : null;

		private static readonly Dictionary<string, WeakTexture> DuplicateTable = Config.DiscardDuplicates ? new Dictionary<string, WeakTexture>() : null;

		private static readonly LinkedList<WeakReference<ScaledTexture>> MostRecentList = new LinkedList<WeakReference<ScaledTexture>>();

		static internal bool ExcludeSprite (Texture2D texture) {
			return false;// && (texture.SafeName() == "LooseSprites/Cursors");
		}

		static internal bool HasPendingActions () {
			if (!Config.AsyncScaling.Enabled) {
				return false;
			}
			lock (PendingActions) {
				return PendingActions.Count != 0;
			}
		}

		static internal void AddPendingAction (Action action) {
			lock (PendingActions) {
				PendingActions.Add(action);
			}
		}

		static internal void ProcessPendingActions (int processCount = -1) {
			if (!Config.AsyncScaling.Enabled) {
				return;
			}

			if (processCount < 0) {
				processCount = Config.AsyncScaling.MaxLoadsPerFrame;
				if (processCount < 0) {
					processCount = int.MaxValue;
				}
			}

			// TODO : use GetUpdateToken

			lock (PendingActions) {
				if (processCount >= PendingActions.Count) {
					foreach (var action in PendingActions) {
						action.Invoke();
					}
					PendingActions.Clear();
				}
				else {
					while (processCount-- > 0) {
						PendingActions.Last().Invoke();
						PendingActions.RemoveAt(PendingActions.Count - 1);
					}
				}
			}
		}

		private static bool LegalFormat(Texture2D texture) {
			return AllowedFormats.Contains(texture.Format);
		}

		internal static bool Validate(Texture2D texture) {
			int textureArea = texture.Area();

			if (texture is ManagedTexture2D) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', is already scaled");
				}
				return false;
			}

			if (texture is RenderTarget2D) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', render targets unsupported");
				}
				return false;
			}

			if (Math.Max(texture.Width, texture.Height) <= Config.Resample.MinimumTextureDimensions) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', texture is too small to qualify");
				}
				return false;
			}

			if (textureArea == 0) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', zero area");
				}
				return false;
			}

			// TODO pComPtr check?
			if (texture.IsDisposed || texture.GraphicsDevice.IsDisposed) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Is Zombie");
				}
				return false;
			}

			if (Config.IgnoreUnknownTextures && texture.Name.IsBlank()) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Is Unknown Texture");
				}
				return false;
			}


			if (texture.LevelCount > 1) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Multi-Level Textures Unsupported: {texture.LevelCount} levels");
				}
				return false;
			}

			if (!LegalFormat(texture)) {
				if (!texture.Meta().TracePrinted) {
					texture.Meta().TracePrinted = true;
					Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Format Unsupported: {texture.Format}");
				}
				return false;
			}

			if (!texture.Name.IsBlank()) {
				foreach (var blacklisted in Config.Resample.Blacklist) {
					if (texture.SafeName().StartsWith(blacklisted)) {
						if (!texture.Meta().TracePrinted) {
							texture.Meta().TracePrinted = true;
							Debug.TraceLn($"Not Scaling Texture '{texture.SafeName()}', Is Blacklisted");
						}
						return false;
					}
				}
			}

			return true;
		}

		static internal ScaledTexture Fetch (Texture2D texture, Bounds source, int expectedScale) {
			if (!Validate(texture)) {
				return null;
			}

			if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
				return scaleTexture;
			}

			return null;
		}

		static internal ScaledTexture Get (Texture2D texture, Bounds source, int expectedScale) {
			if (!Validate(texture)) {
				return null;
			}

			if (SpriteMap.TryGet(texture, source, expectedScale, out var scaleTexture)) {
				return scaleTexture;
			}

			bool useAsync = (Config.AsyncScaling.EnabledForUnknownTextures || !texture.Name.IsBlank()) && (texture.Area() >= Config.AsyncScaling.MinimumSizeTexels);

			if (useAsync && Config.AsyncScaling.Enabled && !DrawState.GetUpdateToken(texture.Area()) && !texture.Meta().HasCachedData) {
				return null;
			}

			if (Config.DiscardDuplicates) {
				// Check for duplicates with the same name.
				// TODO : We do have a synchronity issue here. We could purge before an asynchronous task adds the texture.
				// DiscardDuplicatesFrameDelay
				if (!texture.Name.IsBlank() && !Config.DiscardDuplicatesBlacklist.Contains(texture.SafeName())) {
					try {
						lock (DuplicateTable) {
							if (DuplicateTable.TryGetValue(texture.SafeName(), out var weakTexture)) {
								if (weakTexture.TryGetTarget(out var strongTexture)) {
									// Is it not the same texture, and the previous texture has not been accessed for at least 2 frames?
									if (strongTexture != texture && (DrawState.CurrentFrame - strongTexture.Meta().LastAccessFrame) > 2) {
										Debug.TraceLn($"Purging Duplicate Texture '{strongTexture.SafeName()}'");
										DuplicateTable[texture.SafeName()] = texture.MakeWeak();
										Purge(strongTexture);
									}
								}
								else {
									DuplicateTable[texture.SafeName()] = texture.MakeWeak();
								}
							}
							else {
								DuplicateTable.Add(texture.SafeName(), texture.MakeWeak());
							}
						}
					}
					catch (Exception ex) {
						ex.PrintWarning();
					}
				}
			}

			bool isSprite = !ExcludeSprite(texture) && (!source.Offset.IsZero || source.Extent != texture.Extent());
			var textureWrapper = new SpriteInfo(reference: texture, dimensions: source, expectedScale: expectedScale);
			ulong hash = Upscaler.GetHash(textureWrapper, isSprite);

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

		internal static readonly SurfaceFormat[] AllowedFormats = new [] {
			SurfaceFormat.Color
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
		private int refScale;

		internal long LastReferencedFrame = DrawState.CurrentFrame;

		internal Vector2 AdjustedScale = Vector2.One;

		private LinkedListNode<WeakReference<ScaledTexture>> CurrentRecentNode = null;
		private bool Disposed = false;

		~ScaledTexture() {
			if (!Disposed) {
				Dispose();
			}
		}

		internal static volatile uint TotalMemoryUsage = 0;

		internal long MemorySize {
			get {
				if (!IsReady || Texture == null) {
					return 0;
				}
				return Texture.SizeBytes();
			}
		}

		internal long OriginalMemorySize {
			get {
				return originalSize.Width * originalSize.Height * sizeof(int);
			}
		}

		internal static void Purge (Texture2D reference) {
			Purge(reference, null, DataRef<byte>.Null);
		}

		internal static void Purge (Texture2D reference, Bounds? bounds, DataRef<byte> data) {
			SpriteInfo.Purge(reference, bounds, data);
			SpriteMap.Purge(reference, bounds);
			Upscaler.PurgeHash(reference);
		}

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

		internal sealed class ManagedTexture2D : Texture2D {
			private static long TotalAllocatedSize = 0;
			private static int TotalManagedTextures = 0;
			private const bool UseMips = false;

			public readonly WeakReference<Texture2D> Reference;
			public readonly ScaledTexture Texture;
			public readonly Vector2I Dimensions;

			internal static void DumpStats() {
				var currentProcess = Process.GetCurrentProcess();
				var workingSet = currentProcess.WorkingSet64;
				var vmem = currentProcess.VirtualMemorySize64;
				var gca = GC.GetTotalMemory(false);
				Debug.InfoLn($"Total Managed Textures : {TotalManagedTextures}");
				Debug.InfoLn($"Total Texture Size     : {TotalAllocatedSize.AsDataSize()}");
				Debug.InfoLn($"Process Working Set    : {workingSet.AsDataSize()}");
				Debug.InfoLn($"Process Virtual Memory : {vmem.AsDataSize()}");
				Debug.InfoLn($"GC Allocated Memory    : {gca.AsDataSize()}");
			}

			public ManagedTexture2D (
				ScaledTexture texture,
				Texture2D reference,
				Vector2I dimensions,
				SurfaceFormat format,
				int[] data = null,
				string name = null
			) : base(reference.GraphicsDevice.IsDisposed ? DrawState.Device : reference.GraphicsDevice, dimensions.Width, dimensions.Height, UseMips, format) {
				this.Name = name ?? $"{reference.SafeName()} [RESAMPLED {(CompressionFormat)format}]";

				Reference = reference.MakeWeak();
				Texture = texture;
				Dimensions = dimensions - texture.BlockPadding;

				reference.Disposing += (_, _1) => OnParentDispose();

				TotalAllocatedSize += this.SizeBytes();
				++TotalManagedTextures;

				Garbage.MarkOwned(format, dimensions.Area);
				Disposing += (_, _1) => {
					Garbage.UnmarkOwned(format, dimensions.Area);
					TotalAllocatedSize -= this.SizeBytes();
					--TotalManagedTextures;
				};

				if (data != null) {
					this.SetDataEx(data);
				}
			}

			~ManagedTexture2D() {
				if (!IsDisposed) {
					Dispose(false);
				}
			}

			private void OnParentDispose() {
				if (!IsDisposed) {
					Debug.TraceLn($"Disposing ManagedTexture2D '{Name}'");
					Dispose();
				}
			}
		}

		internal ScaledTexture (string assetName, SpriteInfo textureWrapper, Bounds sourceRectangle, ulong hash, bool isSprite, bool async, int expectedScale) {
			IsSprite = isSprite;
			Hash = hash;
			var source = textureWrapper.Reference;

			this.OriginalSourceRectangle = new Bounds(sourceRectangle);
			this.Reference = source.MakeWeak();
			this.sourceRectangle = sourceRectangle;
			this.refScale = expectedScale;
			SpriteMap.Add(source, this, sourceRectangle, expectedScale);

			this.Name = source.Name.IsBlank() ? assetName : source.SafeName();
			originalSize = IsSprite ? sourceRectangle.Extent : new Vector2I(source);

			if (async && Config.AsyncScaling.Enabled) {
				ThreadPool.QueueUserWorkItem((object wrapper) => {
					Thread.CurrentThread.Priority = ThreadPriority.Lowest;
					Thread.CurrentThread.Name = "Texture Resampling Thread";
					Upscaler.Upscale(
						texture: this,
						scale: ref refScale,
						input: (SpriteInfo)wrapper,
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

		public void Dispose (bool disposeChildren) {
			if (disposeChildren && Texture != null) {
				if (!Texture.IsDisposed) {
					Texture.Dispose();
				}
				Texture = null;
			}
			Dispose();
		}

		public void Dispose () {
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

		private void OnParentDispose (Texture2D texture) {
			Debug.TraceLn($"Parent Texture Disposing: {texture.SafeName()}");

			Dispose();
		}
	}
}
