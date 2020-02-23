using SpriteDictionary = System.Collections.Generic.Dictionary<ulong, SpriteMaster.ScaledTexture>;

using System.Runtime.Caching;
using System.Threading;
using System;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Types;
using SpriteMaster.Extensions;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Metadata {
	internal sealed class MTexture2D {
		internal static readonly SharedLock DataCacheLock = new SharedLock();
		private static MemoryCache DataCache = (Config.MemoryCache.Enabled) ? new MemoryCache(name: "DataCache", config: null) : null;
		private static long CurrentID = 0U;

		public readonly SpriteDictionary SpriteTable = new SpriteDictionary();
		private readonly string UniqueIDString = Interlocked.Increment(ref CurrentID).ToString();

		public readonly SharedLock Lock = new SharedLock();

		private volatile int CompressorCount = 0;
		private readonly Semaphore CompressionSemaphore = new Semaphore(int.MaxValue, int.MaxValue);

		public volatile bool TracePrinted = false;
		public Volatile<ulong> UpdateToken { get; private set; } = 0;

		public bool ScaleValid = true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void PurgeDataCache() {
			if (!Config.MemoryCache.Enabled) {
				return;
			}

			using (DataCacheLock.Shared) {
				DataCache.Dispose();
				DataCache = new MemoryCache(name: "DataCache", config: null);
			}
		}

		public Volatile<ulong> LastAccessFrame { get; private set; } = (ulong)DrawState.CurrentFrame;
		internal Volatile<ulong> Hash { get; private set; } = Hashing.Default;

		// TODO : this presently is not threadsafe.
		private readonly WeakReference<byte[]> _CachedData = (Config.MemoryCache.Enabled) ? new WeakReference<byte[]>(null) : null;

		public bool HasCachedData {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				if (!Config.MemoryCache.Enabled)
					return false;

				using (Lock.Shared) {
					return (_CachedData.TryGetTarget(out var target) && target != null);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Purge (Texture2D reference, Bounds? bounds, DataRef<byte> data) {
			using (Lock.Exclusive) {
				if (data.IsNull) {
					CachedData = null;
					return;
				}

				var refSize = unchecked((int)reference.SizeBytes());

				bool forcePurge = false;

				try {
					// TODO : lock isn't granular enough.
					if (Config.MemoryCache.AlwaysFlush) {
						forcePurge = true;
					}
					else if (!bounds.HasValue && data.Offset == 0 && data.Length == refSize) {
						Debug.TraceLn("Overriding MTexture2D Cache in Purge");
						CachedData = data.Data;
					}
					// TODO : This doesn't update the compressed cache.
					else if (!bounds.HasValue && CachedData is var currentData && currentData != null) {
						Debug.TraceLn("Updating MTexture2D Cache in Purge");
						var byteSpan = data.Data;
						using (DataCacheLock.Exclusive) {
							/*using (Lock.Exclusive)*/ {
								var untilOffset = Math.Min(currentData.Length - data.Offset, data.Length);
								foreach (int i in 0..untilOffset) {
									currentData[i + data.Offset] = byteSpan[i];
								}
								Hash = Hashing.Default;
								CachedData = currentData; // Force it to update the global cache.
							}
						}
					}
					else {
						Debug.TraceLn("Forcing full MTexture2D Purge");
						forcePurge = true;
					}
				}
				catch (Exception ex) {
					ex.PrintInfo();
					forcePurge = true;
				}

				// TODO : maybe we need to purge more often?
				if (forcePurge) {
					CachedData = null;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool CheckUpdateToken(ulong referenceToken) {
			using (Lock.Shared) {
				return UpdateToken == referenceToken;
			}
		}

		public static readonly byte[] BlockedSentinel = new byte[1] { 0xFF };

		public byte[] CachedDataNonBlocking {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				if (!Config.MemoryCache.Enabled)
					return null;

				//Lock.Tr
				using (Lock.TryShared) {
					byte[] target = null;
					if (!_CachedData.TryGetTarget(out target) || target == null) {
						byte[] compressedBuffer;
						using (DataCacheLock.TryShared) {
							if (Config.MemoryCache.Compress != Compression.Algorithm.None && Config.MemoryCache.Async) {
								bool handledCompression = false;
								using (Lock.TryPromote) {
									handledCompression = CompressorCount <= 0;
								}
								if (!handledCompression) {
									return BlockedSentinel;
								}
							}

							compressedBuffer = DataCache[UniqueIDString] as byte[];
							if (compressedBuffer != null) {
								target = Compression.Decompress(compressedBuffer, Config.MemoryCache.Compress);
							}
							else {
								target = null;
							}
						}
						if (target != null) {
							bool promoted = false;
							using (Lock.TryPromote) {
								_CachedData.SetTarget(target);
								promoted = true;
							}
							if (!promoted) {
								return BlockedSentinel;
							}
						}
					}
					return target;
				}
				return BlockedSentinel;
			}
		}

		public byte[] CachedData {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				if (!Config.MemoryCache.Enabled)
					return null;

				using (Lock.Shared) {
					byte[] target = null;
					if (!_CachedData.TryGetTarget(out target) || target == null) {
						byte[] compressedBuffer;
						using (DataCacheLock.Shared) {
							if (Config.MemoryCache.Compress != Compression.Algorithm.None && Config.MemoryCache.Async) {
								using (Lock.Promote) {
									int count = CompressorCount;
									if (count > 0) {
										foreach (int i in 0..count) {
											CompressionSemaphore.WaitOne();
										}
										CompressionSemaphore.Release(count);
										CompressorCount = 0;
									}
								}
							}

							compressedBuffer = DataCache[UniqueIDString] as byte[];
							if (compressedBuffer != null) {
								target = Compression.Decompress(compressedBuffer, Config.MemoryCache.Compress);
							}
							else {
								target = null;
							}
						}
						using (Lock.Promote) {
							_CachedData.SetTarget(target);
						}
					}
					return target;
				}
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				try {
					if (!Config.MemoryCache.Enabled)
						return;

					ulong currentUpdateToken;
					using (Lock.Exclusive) {
						currentUpdateToken = UpdateToken;
						UpdateToken = currentUpdateToken + 1;
					}

					TracePrinted = false;

					using (Lock.Shared) {
						//if (_CachedData.TryGetTarget(out var target) && target == value) {
						//	return;
						//}
						if (value == null) {
							using (Lock.Promote) {
								_CachedData.SetTarget(null);
							}
							using (DataCacheLock.Exclusive) {
								DataCache.Remove(UniqueIDString);
							}
						}
						else {
							bool queueCompress = false;

							// I suspect this is completing AFTER we get a call to purge again, and so is overwriting what is the correct data.
							// Doesn't explain why _not_ purging helps, though.
							if (Config.MemoryCache.Compress != Compression.Algorithm.None && Config.MemoryCache.Async) {
								if (queueCompress) {
									using (Lock.Promote) {
										++CompressorCount;
										CompressionSemaphore.WaitOne();
										ThreadQueue.Queue((buffer) => {
											Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
											using var _ = new AsyncTracker($"Texture Cache Compress {UniqueIDString}");
											try {
												if (!CheckUpdateToken(currentUpdateToken)) {
													return;
												}
												var compressedData = Compression.Compress(buffer, Config.MemoryCache.Compress);
												using (DataCacheLock.Exclusive) {
													using (Lock.Exclusive) {
														if (currentUpdateToken != UpdateToken) {
															return;
														}

														DataCache[UniqueIDString] = compressedData;
													}
												}
											}
											finally {
												CompressionSemaphore.Release();
											}
										}, value);
									}
								}
							}
							else {
								DataCache[UniqueIDString] = Compression.Compress(value, Config.MemoryCache.Compress);
							}

							using (Lock.Promote) {
								_CachedData.SetTarget(value);
							}
						}
					}
				}
				finally {
					Hash = default;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UpdateLastAccess() {
			LastAccessFrame = DrawState.CurrentFrame;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ulong GetHash(SpriteInfo info) {
			using (Lock.Shared) {
				ulong hash = Hash;
				if (hash == Hashing.Default) {
					hash = info.Hash;
					using (Lock.Promote) {
						Hash = hash;
					}
				}
				return hash;
			}
		}
	}
}
