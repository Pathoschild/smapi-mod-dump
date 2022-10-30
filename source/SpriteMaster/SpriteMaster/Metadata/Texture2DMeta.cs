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
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using SpriteMaster.Mitigations.PyTK;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SpriteDictionary = System.Collections.Generic.Dictionary<ulong, SpriteMaster.ManagedSpriteInstance>;

namespace SpriteMaster.Metadata;

// TODO : This needs a Finalize thread dispatcher, and needs to remove cached data for it from the ResidentCache.
internal sealed class Texture2DMeta : IDisposable {
	private const string CursorsPath = @"LooseSprites\Cursors";

	private static readonly ConcurrentDictionary<ulong, SpriteDictionary> SpriteDictionaries = new();
	private readonly SpriteDictionary SpriteInstanceTable;

	[Flags]
	private enum SpriteFlag : uint {
		None =				0U,
		NoResample =	1U << 0,
		Animated =		1U << 1,
	}

	[Flags]
	internal enum TextureFlag : uint {
		None				= 0U,
		Populated		= 1U << 0,
		IsLargeFont = 1U << 1,
		IsSmallFont = 1U << 2,
	}

	// Class and not a struct because we want to avoid a plethora of dictionary accesses to mutate them.
	// TODO : use an object cache/allocator
	private sealed class SpriteData {
		internal ulong? Hash = null;
		internal SpriteFlag Flags = SpriteFlag.None;
	}

	private readonly ConcurrentDictionary<Bounds, SpriteData> SpriteDataMap = new();
	private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Bounds, SpriteData>> GlobalSpriteDataMaps = new();

	internal readonly record struct InFlightData(long Revision, ulong SpriteHash, WeakReference<Task> ResampleTask);
	internal readonly ConcurrentDictionary<Bounds, InFlightData> InFlightTasks = new();
	internal TextureFlag Flags = TextureFlag.None; // TODO use properties for this

	internal enum SpriteType {
		Unknown = 0,
		Sprite,
		SmallText,
		LargeText,
		Portrait,
	}

	internal SpriteType Type = SpriteType.Sprite;

	internal IReadOnlyDictionary<ulong, ManagedSpriteInstance> GetSpriteInstanceTable() => SpriteInstanceTable;

	internal void SetSpriteHash(Bounds bounds, ulong hash) {
		SpriteDataMap.GetOrAdd(bounds, _ => new()).Hash = hash;
	}

	internal bool TryGetSpriteHash(Bounds bounds, out ulong hash) {
		if (SpriteDataMap.TryGetValue(bounds, out var data) && data.Hash.HasValue) {
			hash = data.Hash.Value;
			return true;
		}
		hash = 0UL;
		return false;
	}

	internal void ClearSpriteHashes(bool animated = false) {
		var conditionalFlag = SpriteFlag.Animated.ConditionalFlag(animated);

		foreach (var pair in SpriteDataMap) {
			var spriteData = pair.Value;
			spriteData.Hash = null;
			spriteData.Flags |= conditionalFlag;
		}
	}

	internal void ClearSpriteInstanceTable(bool allowSuspend = false) {
		using (Lock.Write) {
			if (allowSuspend) {
				SuspendSpriteInstanceTable();
			}
			else {
				DisposeSpriteInstanceTable();
			}
			SpriteInstanceTable.Clear();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SuspendSpriteInstanceTable() {
		foreach (var spriteInstance in SpriteInstanceTable.Values) {
			spriteInstance.Suspend();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DisposeSpriteInstanceTable() {
		foreach (var spriteInstance in SpriteInstanceTable.Values) {
			spriteInstance.Dispose();
		}
	}

	internal bool RemoveFromSpriteInstanceTable(ulong key, bool dispose, out ManagedSpriteInstance? instance) {
		using (Lock.Write) {
			if (SpriteInstanceTable.Remove(key, out instance)) {
				return true;
			}
		}

		instance = null;
		return false;
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

	internal void AddNoResample(Bounds bounds) {
		SpriteDataMap.GetOrAdd(bounds, _ => new()).Flags |= SpriteFlag.NoResample;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool SpriteHasFlag(Bounds bounds, SpriteFlag flag) =>
		SpriteDataMap.TryGetValue(bounds, out var data) && data.Flags.HasFlag(flag);

	internal bool IsNoResample(Bounds bounds) =>
		SpriteHasFlag(bounds, SpriteFlag.NoResample);

	internal bool IsAnimated(Bounds bounds) =>
		SpriteHasFlag(bounds, SpriteFlag.Animated);

	private readonly Dictionary<Bounds, Config.TextureRef?> SlicedCache = new();

	internal bool CheckSliced(Bounds bounds, [NotNullWhen(true)] out Config.TextureRef? result) {
		if (SlicedCache.TryGetValue(bounds, out var textureRef)) {
			result = textureRef;
			return textureRef.HasValue;
		}

		if (NormalizedName is not null) {
			foreach (var slicedTexture in Config.Resample.SlicedTexturesS) {
				if (!slicedTexture.Pattern.IsMatch(NormalizedName)) {
					continue;
				}

				if (!slicedTexture.Bounds.IsEmpty && !slicedTexture.Bounds.Contains(bounds)) {
					continue;
				}

				SlicedCache.Add(bounds, slicedTexture);
				result = slicedTexture;
				return true;
			}
		}
		SlicedCache.Add(bounds, null);
		result = null;
		return false;
	}

	private string? NormalizedNameInternal = null;
	private string? NormalizedName {
		get {
			if (NormalizedNameInternal is {} normalizedName) {
				return normalizedName;
			}

			if (Owner.TryGetTarget(out var owner)) {
				normalizedName = NormalizedNameInternal = owner.NormalizedNameOrNull();
			}
			else {
				normalizedName = null;
			}

			return normalizedName;
		}
	}

	/// <summary>The current (static) ID, incremented every time a new <see cref="Texture2DMeta"/> is created.</summary>
	private static ulong CurrentId = 0U;


	internal readonly SharedLock Lock = new(LockRecursionPolicy.SupportsRecursion);
	internal readonly WeakReference<XTexture2D> Owner;
	// TODO : this presently is not thread-safe.
	internal readonly WeakReference<byte[]> CachedDataInternal = new(null!);
	internal readonly WeakReference<byte[]> CachedRawDataInternal = new(null!);

	/// <summary>Whenever a new <see cref="Texture2DMeta"/> is created, <see cref="CurrentId"/> is incremented and this is set to that.</summary>
	private readonly ulong MetaId = Interlocked.Increment(ref CurrentId);
	internal long Revision { get; private set; } = 0;
	internal InterlockedULong LastAccessFrame { get; private set; } = DrawState.CurrentFrame;
	internal InterlockedULong Hash { get; private set; } = 0;
	internal readonly Vector2I Size;
	internal readonly uint ExpectedByteSizeRaw;
	internal readonly uint ExpectedByteSize;
	internal ReportOnceErrors ReportedErrors = 0;
	internal readonly SurfaceFormat Format;

	internal volatile bool TracePrinted = false;
	internal bool? Validation = null;
	internal bool IsSystemRenderTarget = false;
	internal readonly bool IsCompressed = false;

	private volatile Task DecodingTask = Task.CompletedTask;
	internal ulong DecodingTaskRevision = 0ul;

	internal readonly bool IsManagedTexture;

	internal bool IsCursors { get; private set; }

	private bool CheckIsCursors(XTexture2D texture) {
		if (texture.NormalizedName() == CursorsPath) {
			return true;
		}

		if (IsManagedTexture) {
			var underlyingTexture = texture.GetUnderlyingTexture(out bool isManaged);
			if (!isManaged) {
				underlyingTexture = null;
			}

			return underlyingTexture?.NormalizedName() == CursorsPath;
		}
		else {
			return false;
		}
	}

	private string? PreviousName;

	internal bool CheckNameChange(XTexture2D texture) {
		while (true) {
			Contract.Assert(Owner.TryGetTarget(out var target) && target == texture);

			var previousName = PreviousName;
			var currentName = texture.Name;
			if (previousName == currentName) {
				if (IsManagedTexture && texture.GetUnderlyingTexture(out _) is { } innerTexture && innerTexture != texture) {
					texture = innerTexture;
					continue;
				}

				return false;
			}

			if (!IsCursors) {
				IsCursors = CheckIsCursors(texture);
			}

			PreviousName = currentName;
			return true;
		}
	}

	internal void IncrementRevision() => ++Revision;

	internal Texture2DMeta(XTexture2D texture) {
		Owner = texture.MakeWeak();
		IsCompressed = texture.Format.IsCompressed();
		Format = texture.Format;
		Size = texture.Extent();
		SpriteInstanceTable = SpriteDictionaries.GetOrAdd(MetaId, _ => new());
		if (!texture.Anonymous()) {
			SpriteDataMap = GlobalSpriteDataMaps.GetOrAdd(texture.NormalizedName(), _ => new());
		}

		ExpectedByteSizeRaw = (uint)texture.Format.SizeBytes(Size);
		if (Format.IsBlock() || Format.IsCompressed()) {
			ExpectedByteSize = (uint)SurfaceFormat.Color.SizeBytes(Size);
		}
		else {
			ExpectedByteSize = ExpectedByteSizeRaw;
		}

		IsManagedTexture = Mitigations.PyTK.Textures.IsPyTKTexture(texture);

		IsCursors = CheckIsCursors(texture);
	}

	internal bool HasCachedData {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get {
			if (!Config.ResidentCache.Enabled) {
				return false;
			}

			using (Lock.ReadWrite) {
				bool isPopulated = Flags.HasFlag(TextureFlag.Populated);
				return isPopulated && CachedData is not null;
			}
		}
	}

	// Flushes the Metadata altogether, and all cached textures and related instances
	internal void Purge() {
		using (Lock.Write) {
			ClearSpriteInstanceTable();
			CachedRawData = null;
			InFlightTasks.Clear();
		}
	}

	internal bool IsAnyAnimated(XTexture2D reference, Bounds? bounds) {
		bounds ??= reference.Bounds();

		using (Lock.Read) {
			foreach (var dataPair in SpriteDataMap) {
				if (dataPair.Value.Flags.HasFlag(SpriteFlag.Animated) && dataPair.Key.Overlaps(bounds.Value)) {
					return true;
				}
			}
		}

		return false;
	}

	// TODO : This is currently an all-or-nothing task.
	// That is, if it cannot update the cache, it purges it. It has no ability to mark
	// regions of the cache as invalidated.
	// This should be changed.
	internal void Purge(XTexture2D reference, Bounds? bounds, in DataRef<byte> data, bool animated) {
		// Bounds is meaningless if it encompasses the entire texture
		if (bounds == reference.Bounds()) {
			bounds = null;
		}

		using (Lock.Write) {
			bool hasCachedData = CachedRawData is not null;

			if (data.IsNull) {
				if (!hasCachedData) {
					Debug.Trace($"Clearing '{reference.NormalizedName(DrawingColor.LightYellow)}' Cache");
				}
				CachedRawData = null;
				ClearSpriteHashes(animated);
				return;
			}

			var refSize = reference.SizeBytes();

			bool forcePurge = false;

			if (bounds.HasValue) {
				foreach (var dataPair in SpriteDataMap) {
					if (dataPair.Key.Overlaps(bounds.Value)) {
						dataPair.Value.Hash = null;
						if (animated) {
							dataPair.Value.Flags |= SpriteFlag.Animated;
						}
					}
				}
			}
			else {
				ClearSpriteHashes(animated);
			}


			try {
				// TODO : lock isn't granular enough.
				if (Config.ResidentCache.Enabled && Config.ResidentCache.AlwaysFlush) {
					forcePurge = true;
				}
				else if (!bounds.HasValue && data.Length == refSize) {
					Debug.Trace($"{(hasCachedData ? "Overriding" : "Setting")} '{reference.NormalizedName(DrawingColor.LightYellow)}' Cache in Purge: {bounds.HasValue}, {data.Length}");

					if (data.HasData) {
						CachedRawData = data.DataCopy;
						Flags |= TextureFlag.Populated;
					}
					else {
						if (hasCachedData) {
							Debug.Trace($"Forcing full '{reference.NormalizedName(DrawingColor.LightYellow)}' Purge");
						}
						forcePurge = true;
					}
				}
				else if (!IsCompressed) {
					Debug.Trace($"{(hasCachedData ? "Updating" : "Setting")} '{reference.NormalizedName(DrawingColor.LightYellow)}' Cache in Purge: {bounds.HasValue}");

					if (CachedRawData is { } currentData) {
						if (data.HasData) {
							if (bounds.HasValue && (bounds.Value.Extent != Vector2I.Zero || bounds.Value.Size != Size)) {
								if (!Flags.HasFlag(TextureFlag.Populated)) {
									return;
								}

								int elementSize = Format.GetSize();

								//int referenceStride = reference.Width * elementSize;
								int boundsStride = bounds.Value.Width * elementSize;

								var source = data.Data;
								var dest = currentData.AsSpan();
								int sourceOffset = 0;
								for (int y = bounds.Value.Top; y < bounds.Value.Bottom; ++y) {
									int destOffset = (y * reference.Width) + bounds.Value.Left;
									var sourceSlice = source.Slice(sourceOffset * elementSize, boundsStride);
									var destSlice = dest.Slice(destOffset * elementSize, boundsStride);
									sourceSlice.CopyTo(destSlice);
									sourceOffset += bounds.Value.Width;
								}
							}
							else {
								//var source = data.Data;
								//var length = Math.Min(currentData.Length - data.Offset, data.Length);
								//source.CopyTo(currentData.AsSpan(data.Offset, length));

								data.Data.CopyTo(currentData);
								Flags |= TextureFlag.Populated;
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
					else {
						if (!bounds.HasValue || (bounds.Value.Extent == Vector2I.Zero && bounds.Value.Size == Size)) {
							var newCachedData = data.DataCopy;
							Hash = 0;
							CachedRawData = newCachedData; // Force it to update the global cache.
							Flags |= TextureFlag.Populated;
						}
						else {
							if (hasCachedData) {
								Debug.Trace($"Forcing full '{reference.NormalizedName(DrawingColor.LightYellow)}' Purge");
							}
							forcePurge = true;
						}
					}
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

			if (forcePurge) {
				Flags &= ~TextureFlag.Populated;
				if (hasCachedData) {
					CachedRawData = null;
				}
			}
		}
	}

	private void InvalidateCachedRawData() {
		using (Lock.Write) {
			CachedRawDataInternal.SetTarget(null!);
			CachedDataInternal.SetTarget(null!);
			if (Config.ResidentCache.Enabled) {
				ResidentCache.RemoveFast(MetaId);
			}

			Flags &= ~TextureFlag.Populated;
		}
	}

	internal byte[]? CachedRawData {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get {
			if (!Config.ResidentCache.Enabled) {
				return null;
			}

			using (Lock.Read) {
				if (!CachedRawDataInternal.TryGetTarget(out var target)) {
					// Attempt to pull the value out of the cache if the cache is a compressed cache.
					target = ResidentCache.Get(MetaId);
				}
				return target;
			}
		}
		set {
			try {
				if (!Config.ResidentCache.Enabled) {
					return;
				}

				TracePrinted = false;

				//if (CachedRawDataInternal.TryGetTarget(out var target) && target == value) {
				//	return;
				//}
				if (value is null) {
					InvalidateCachedRawData();
				}
				else {
					if (ExpectedByteSizeRaw != (uint)value.Length) {
						Console.Error.WriteLine($"Cached Raw Data length mismatch: {ExpectedByteSizeRaw} != {value.Length}");
						Debug.Break();
						Debug.Error(Environment.StackTrace);
						InvalidateCachedRawData();
						return;
					}

					using (Lock.Write) {
						ResidentCache.SetOrTouchFast(MetaId, value);
						CachedRawDataInternal.SetTarget(value);
						if (!IsCompressed) {
							CachedDataInternal.SetTarget(value);
						}
						else {
							CachedDataInternal.SetTarget(null!);
							DispatchDecodeTask(force: true);
						}
						Flags |= TextureFlag.Populated;
					}
				}
			}
			finally {
				Hash = default;
			}
		}
	}

	internal byte[]? CachedData {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get {
			using (Lock.ReadWrite) {
				if (!Flags.HasFlag(TextureFlag.Populated)) {
					return null;
				}

				if (!CachedDataInternal.TryGetTarget(out var target)) {
					// Attempt to pull the value out of the cache if the cache is a compressed cache.
					var rawData = ResidentCache.Get(MetaId);

					if (rawData is not null && rawData.Length != ExpectedByteSizeRaw) {
						Debug.Error($"Size Mismatch in CachedData ResidentCache pull: {rawData.Length} != {ExpectedByteSizeRaw}");
						if (Config.ResidentCache.Enabled) {
							ResidentCache.Remove(MetaId);
						}

						return null;
					}

					using (Lock.Write) {
						if (rawData is null) {
							InvalidateCachedRawData();
						}
						else {
							CachedRawDataInternal.SetTarget(rawData);
							if (!IsCompressed) {
								CachedDataInternal.SetTarget(rawData);
								target = rawData;
							}
							else {
								CachedDataInternal.SetTarget(null!);
								DispatchDecodeTask();
							}
						}
					}
				}

				if (target is not null && target.Length != ExpectedByteSize) {
					Debug.Error($"Size Mismatch in target pull: {target.Length} != {ExpectedByteSize}");
					InvalidateCachedRawData();
				}

				return target;
			}
		}
	}

	private void DispatchDecodeTask(bool force = false) {
		if (!force && !DecodingTask.IsCompleted) {
			return;
		}

		if (CachedRawData is not null) {
			DecodingTask = DecodeTask.Dispatch(this, Interlocked.Increment(ref DecodingTaskRevision));
		}
	}

	internal static readonly byte[] BlockedSentinel = { 0xFF };

	internal byte[]? CachedDataNonBlocking {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get {
			if (!Config.ResidentCache.Enabled) {
				return null;
			}

			using (var locked = Lock.TryRead) if (locked) {
				if (!Flags.HasFlag(TextureFlag.Populated)) {
					return null;
				}

				if (!CachedDataInternal.TryGetTarget(out var target) && !IsCompressed) {
					// Attempt to pull the value out of the cache if the cache is a compressed cache.
					target = ResidentCache.Get(MetaId);
				}
				return target;
			}
			return BlockedSentinel;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void SetCachedDataUnsafe(Span<byte> data) {
		if (ExpectedByteSize != (uint)data.Length) {
			Console.Error.WriteLine($"Cached Data length mismatch: {ExpectedByteSize} != {data.Length}");
			Debug.Break();
			Debug.Error(Environment.StackTrace);
			InvalidateCachedRawData();

			return;
		}

		using (Lock.Write) {
			CachedDataInternal.SetTarget(data.ToArray());
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void UpdateLastAccess() {
		LastAccessFrame = DrawState.CurrentFrame;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void PushToCache() {
		if (!Config.ResidentCache.Enabled) {
			return;
		}

		if (CachedRawDataInternal.TryGetTarget(out var rawData)) {
			ResidentCache.TrySet(MetaId, rawData);
		}
	}

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowNullReferenceException<T>(string name) =>
		throw new NullReferenceException(name);

	internal ulong GetHash(SpriteInfo info) {
		using (Lock.ReadWrite) {
			ulong hash = Hash;
			if (hash != 0) {
				return hash;
			}

			if (info.ReferenceData is null) {
				return ThrowNullReferenceException<ulong>(nameof(info.ReferenceData));
			}
			hash = info.ReferenceData.Hash();
			using (Lock.Write) {
				Hash = hash;
			}
			return hash;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal bool ShouldReportError(ReportOnceErrors error) {
		if (ReportedErrors.HasFlag(error)) {
			return false;
		}

		ReportedErrors |= error;
		return true;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	~Texture2DMeta() => Dispose(false);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public void Dispose() => Dispose(true);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void Dispose(bool disposing) {
		ResourceManager.ReleasedTextureMetas.Push(MetaId);
		if (disposing) {
			GC.SuppressFinalize(this);
		}
	}

	internal static void Cleanup(in ulong id) {
		if (Config.ResidentCache.Enabled) {
			ResidentCache.RemoveFast(id);
		}

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
