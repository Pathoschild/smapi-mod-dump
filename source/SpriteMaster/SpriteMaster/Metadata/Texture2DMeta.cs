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
using SpriteMaster.Caching;
using SpriteMaster.Extensions;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SpriteDictionary = System.Collections.Generic.Dictionary<ulong, SpriteMaster.ManagedSpriteInstance>;

namespace SpriteMaster.Metadata;

// TODO : This needs a Finalize thread dispatcher, and needs to remove cached data for it from the ResidentCache.
sealed class Texture2DMeta : IDisposable {
	private static readonly ConcurrentDictionary<ulong, SpriteDictionary> SpriteDictionaries = new();
	private readonly SpriteDictionary SpriteInstanceTable;
	private readonly ConcurrentDictionary<Bounds, bool> NoResampleSet = new();

	internal IReadOnlyDictionary<ulong, ManagedSpriteInstance> GetSpriteInstanceTable() => SpriteInstanceTable;

	internal void ClearSpriteInstanceTable(bool allowSuspend = false) {
		using (Lock.Write) {
			foreach (var spriteInstance in SpriteInstanceTable.Values) {
				if (allowSuspend) {
					spriteInstance?.Suspend();
				}
				else {
					spriteInstance?.Dispose();
				}
			}
			SpriteInstanceTable.Clear();
		}
	}

	internal bool RemoveFromSpriteInstanceTable(ulong key, bool dispose, out ManagedSpriteInstance? instance) {
		using (Lock.Write) {
			if (SpriteInstanceTable.Remove(key, out instance)) {
				return true;
			}
			return false;
		}
	}

	internal void ReplaceInSpriteInstanceTable(ulong key, ManagedSpriteInstance instance) {
		using (Lock.Write) {
			SpriteInstanceTable[key] = instance;
		}
	}

	internal bool TryAddToSpriteInstanceTable(ulong key, ManagedSpriteInstance instance) {
		using (Lock.Write) {
			return SpriteInstanceTable.TryAdd(key, instance);
		}
	}

	internal void AddNoResample(in Bounds bounds) {
		NoResampleSet.TryAdd(bounds, true);
	}

	internal bool IsNoResample(in Bounds bounds) {
		return NoResampleSet.ContainsKey(bounds);
	}

	/// <summary>The current (static) ID, incremented every time a new <see cref="Texture2DMeta"/> is created.</summary>
	private static ulong CurrentID = 0U;
	/// <summary>Whenever a new <see cref="Texture2DMeta"/> is created, <see cref="CurrentID"/> is incremented and <see cref="UniqueIDString"/> is set to a string representation of it.</summary>
	private readonly ulong MetaID = Interlocked.Increment(ref CurrentID);
	private readonly string UniqueIDString;

	internal readonly SharedLock Lock = new(LockRecursionPolicy.SupportsRecursion);

	internal volatile bool TracePrinted = false;

	internal bool? Validation = null;
	internal bool IsSystemRenderTarget = false;
	internal bool IsCompressed = false;
	internal ReportOnceErrors ReportedErrors = 0;
	internal SurfaceFormat Format;
	internal Vector2I Size;

	internal InterlockedULong LastAccessFrame { get; private set; } = (ulong)DrawState.CurrentFrame;
	internal InterlockedULong Hash { get; private set; } = 0;

	internal Texture2DMeta(Texture2D texture) {
		UniqueIDString = MetaID.ToString64();
		IsCompressed = texture.Format.IsCompressed();
		Format = texture.Format;
		Size = texture.Extent();
		SpriteInstanceTable = SpriteDictionaries.GetOrAdd(MetaID, id => new());
	}

	// TODO : this presently is not threadsafe.
	private readonly WeakReference<byte[]> _CachedData = (Config.MemoryCache.Enabled) ? new(null!) : null!;
	private readonly WeakReference<byte[]> _CachedRawData = (Config.MemoryCache.Enabled) ? new(null!) : null!;

	internal bool HasCachedData {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			if (!Config.MemoryCache.Enabled) {
				return false;
			}

			using (Lock.Read) {
				return _CachedData.TryGetTarget(out var target);
			}
		}
	}

	// Flushes the Metadata altogether, and all cached textures and related instances
	internal void Purge() {
		using (Lock.Write) {
			ClearSpriteInstanceTable();
			CachedRawData = null;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Purge(Texture2D reference, in Bounds? bounds, in DataRef<byte> data) {
		using (Lock.Write) {
			bool hasCachedData = CachedRawData is not null;

			if (data.IsNull) {
				if (!hasCachedData) {
					Debug.Trace($"Clearing '{reference.NormalizedName(DrawingColor.LightYellow)}' Cache");
				}
				CachedRawData = null;
				return;
			}

			var refSize = (int)reference.SizeBytes();

			bool forcePurge = false;

			try {
				// TODO : lock isn't granular enough.
				if (Config.MemoryCache.AlwaysFlush) {
					forcePurge = true;
				}
				else if (!bounds.HasValue && data.Offset == 0 && data.Length == refSize) {
					Debug.Trace($"{(hasCachedData ? "Overriding" : "Setting")} '{reference.NormalizedName(DrawingColor.LightYellow)}' Cache in Purge: {bounds.HasValue}, {data.Offset}, {data.Length}");
					CachedRawData = data.Data;
				}
				else if (!IsCompressed && (bounds.HasValue != (data.Offset != 0)) && CachedRawData is var currentData && currentData is not null) {
					Debug.Trace($"{(hasCachedData ? "Updating" : "Setting")} '{reference.NormalizedName(DrawingColor.LightYellow)}' Cache in Purge: {bounds.HasValue}");

					if (bounds.HasValue) {
						var source = data.Data.AsSpan<uint>();
						var dest = currentData.AsSpan<uint>();
						int sourceOffset = 0;
						for (int y = bounds.Value.Top; y < bounds.Value.Bottom; ++y) {
							int destOffset = (y * reference.Width) + bounds.Value.Left;
							var sourceSlice = source.Slice(sourceOffset, bounds.Value.Width);
							var destSlice = dest.Slice(destOffset, bounds.Value.Width);
							sourceSlice.CopyTo(destSlice);
							sourceOffset += bounds.Value.Width;
						}
					}
					else {
						var byteSpan = data.Data;
						var untilOffset = Math.Min(currentData.Length - data.Offset, data.Length);
						for (int i = 0; i < untilOffset; ++i) {
							currentData[i + data.Offset] = byteSpan[i];
						}
					}
					Hash = 0;
					CachedRawData = currentData; // Force it to update the global cache.
				}
				else {
					if (hasCachedData) {
						Debug.Trace($"Forcing full '{reference.NormalizedName(DrawingColor.LightYellow)}' Purge");
					}
					forcePurge = true;
				}
			}
			catch (Exception ex) {
				ex.PrintInfo();
				forcePurge = true;
			}

			// TODO : maybe we need to purge more often?
			if (forcePurge && hasCachedData) {
				CachedRawData = null;
			}
		}
	}

	internal static readonly byte[] BlockedSentinel = new byte[1] { 0xFF };

	internal byte[]? CachedDataNonBlocking {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			if (!Config.MemoryCache.Enabled) {
				return null;
			}

			using (var locked = Lock.TryRead) if (locked) {
				_CachedRawData.TryGetTarget(out var target);
				return target;
			}
			return BlockedSentinel;
		}
	}

	internal byte[]? CachedRawData {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			if (!Config.MemoryCache.Enabled) {
				return null;
			}

			using (Lock.Read) {
				_CachedRawData.TryGetTarget(out var target);
				return target;
			}
		}
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set {
			try {
				if (!Config.MemoryCache.Enabled) {
					return;
				}

				TracePrinted = false;

				//if (_CachedRawData.TryGetTarget(out var target) && target == value) {
				//	return;
				//}
				if (value is null) {
					using (Lock.Write) {
						_CachedRawData.SetTarget(null!);
						_CachedData.SetTarget(null!);
						ResidentCache.Remove(UniqueIDString);
					}
				}
				else {
					using (Lock.Write) {
						ResidentCache.Set(UniqueIDString, value);
						_CachedRawData.SetTarget(value);
						if (!IsCompressed) {
							_CachedData.SetTarget(value);
						}
						else {
							_CachedData.SetTarget(null!);
							DecodeTask.Dispatch(this);
						}
					}
				}
			}
			finally {
				Hash = default;
			}
		}
	}

	internal byte[]? CachedData {
		get {
			using (Lock.Read) {
				if (_CachedData?.TryGetTarget(out var data) ?? false) {
					return data;
				}
				return null;
			}
		}
	}

	internal void SetCachedDataUnsafe(Span<byte> data) {
		using (Lock.Write) {
			_CachedData.SetTarget(data.ToArray());
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void UpdateLastAccess() {
		LastAccessFrame = DrawState.CurrentFrame;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ulong GetHash(SpriteInfo info) {
		using (Lock.ReadWrite) {
			ulong hash = Hash;
			if (hash == 0) {
				if (info.ReferenceData is null) {
					throw new NullReferenceException(nameof(info.ReferenceData));
				}
				hash = info.ReferenceData.Hash();
				using (Lock.Write) {
					Hash = hash;
				}
			}
			return hash;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal bool ShouldReportError(ReportOnceErrors error) {
		if ((ReportedErrors & error) != 0) {
			return false;
		}
		ReportedErrors |= error;
		return true;
	}

	~Texture2DMeta() => Dispose();

	public void Dispose() {
		ResourceManager.ReleasedTextureMetas.Push(MetaID);
		//ResourceManager.SuspendableSpriteInstances.Push(new(SpriteInstanceTable.Values));

		GC.SuppressFinalize(this);
	}

	internal static void Cleanup(in ulong id) {
		ResidentCache.Remove(id.ToString64());
		if (SpriteDictionaries.Remove(id, out var instances)) {
			foreach (var instance in instances.Values) {
				instance.Suspend();
			}
		}
	}

	/*
	internal static void Cleanup(in List<ManagedSpriteInstance> instances) {
		foreach (var instance in instances) {
			SpriteMap.DirectRemove(instance);
		}
	}
	*/
}
