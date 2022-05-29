/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#define WITH_SPRITE_REFERENCE_SET

using LinqFasterer;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

// TODO : This class, and Texture2DMeta, have a _lot_ of inter-play and it makes it very confusing.
// This needs to be cleaned up badly.
internal static class SpriteMap {
#if WITH_SPRITE_REFERENCE_SET
	private static readonly WeakSet<ManagedSpriteInstance> SpriteInstanceReferences = new();

	internal static WeakSet<ManagedSpriteInstance> SpriteInstanceReferencesGet => SpriteInstanceReferences;
#else
	private static WeakSet<ManagedSpriteInstance> SpriteInstanceReferencesGet => null!;
#endif

	[Conditional("WITH_SPRITE_REFERENCE_SET")]
	private static void AddInternal(ManagedSpriteInstance instance) => SpriteInstanceReferencesGet.AddOrIgnore(instance);

	[Conditional("WITH_SPRITE_REFERENCE_SET")]
	private static void RemoveInternal(ManagedSpriteInstance instance) => SpriteInstanceReferencesGet.Remove(instance);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong SpriteHash(XTexture2D texture, Bounds source, uint expectedScale, bool preview) {
		return HashUtility.Combine(source.Hash(), expectedScale.GetSafeHash(), preview.GetSafeHash());
	}

	internal static bool Add(XTexture2D reference, ManagedSpriteInstance instance, out ManagedSpriteInstance? current) {
		AddInternal(instance);

		var meta = reference.Meta();

		using (meta.Lock.Write) {
			var result = meta.TryAddToSpriteInstanceTable(instance.SpriteMapHash, instance);
			if (result) {
				current = null;
			}
			else {
				current = null;
				meta.GetSpriteInstanceTable().TryGetValue(instance.SpriteMapHash, out current);
			}
			return result;
		}
	}

	internal static bool AddReplaceInvalidated(XTexture2D reference, ManagedSpriteInstance instance) {
		AddInternal(instance);

		var meta = reference.Meta();

		using (meta.Lock.Write) {
			var result = meta.TryAddToSpriteInstanceTable(instance.SpriteMapHash, instance);
			if (!result) {
				meta.GetSpriteInstanceTable().TryGetValue(instance.SpriteMapHash, out var current);
				if (current is null || current.Invalidated) {
					meta.ReplaceInSpriteInstanceTable(instance.SpriteMapHash, instance);
					result = true;
				}
			}
			return result;
		}
	}

	internal static void AddReplace(XTexture2D reference, ManagedSpriteInstance instance) {
		AddInternal(instance);

		var meta = reference.Meta();

		using (meta.Lock.Write) {
			var result = meta.TryAddToSpriteInstanceTable(instance.SpriteMapHash, instance);
			if (!result) {
				meta.ReplaceInSpriteInstanceTable(instance.SpriteMapHash, instance);
			}
		}
	}

	internal static bool TryGetReady(XTexture2D texture, Bounds source, uint expectedScale, [NotNullWhen(true)] out ManagedSpriteInstance? result) {
		if (TryGet(texture, source, expectedScale, out var internalResult)) {
			if (internalResult.IsReady) {
				result = internalResult;
				return true;
			}

			if (internalResult.PreviousSpriteInstance?.IsReady ?? false) {
				result = internalResult.PreviousSpriteInstance;
				result.Resurrect(texture, internalResult.SpriteMapHash);
				return true;
			}
		}
		result = null;
		return false;
	}

	internal static bool TryGet(XTexture2D texture, Bounds source, uint expectedScale, [NotNullWhen(true)] out ManagedSpriteInstance? result) {
		var rectangleHash = SpriteHash(texture: texture, source: source, expectedScale: expectedScale, preview: Configuration.Preview.Override.Instance is not null);

		var meta = texture.Meta();
		var spriteTable = meta.GetSpriteInstanceTable();

		var instanceDisposeList = new List<ManagedSpriteInstance>();

		using (meta.Lock.ReadWrite) {
			if (spriteTable.TryGetValue(rectangleHash, out var spriteInstance)) {
				if (spriteInstance.Texture?.IsDisposed == true) {
					var removeList = new List<ulong>();
					using (meta.Lock.Write) {
						foreach (var skv in spriteTable) {
							if (skv.Value.Texture?.IsDisposed ?? false) {
								removeList.Add(skv.Key);
							}
						}
						foreach (var key in removeList) {
							meta.RemoveFromSpriteInstanceTable(key, dispose: false, out var instance);
							if (instance is not null) {
								instanceDisposeList.Add(instance);
							}
						}
					}
				}
				else {
					result = spriteInstance;
					return true;
				}
			}
		}

		foreach (var instance in instanceDisposeList) {
			instance.Suspend();
		}

		result = null;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void DirectRemove(ManagedSpriteInstance instance) {
		RemoveInternal(instance);
		instance.Suspend();
	}

	internal static void Remove(ManagedSpriteInstance spriteInstance, XTexture2D texture) {
		try {
			RemoveInternal(spriteInstance);

			var meta = texture.Meta();
			var spriteTable = meta.GetSpriteInstanceTable();

			ManagedSpriteInstance? instance = null;
			using (meta.Lock.Write) {
				if (spriteTable.TryGetValue(spriteInstance.SpriteMapHash, out var currentValue) && currentValue == spriteInstance) {
					meta.RemoveFromSpriteInstanceTable(spriteInstance.SpriteMapHash, dispose: false, out instance);
				}
			}

			instance?.Suspend();
		}
		finally {
			if (spriteInstance.Texture is not null && !spriteInstance.Texture.IsDisposed) {
				Debug.Trace($"Disposing Active HD Texture: {spriteInstance.NormalizedName()}");

				//spriteInstance.Texture.Dispose();
			}
		}
	}

	internal static void Remove(in ManagedSpriteInstance.CleanupData instanceData, XTexture2D texture) {
		var meta = texture.Meta();
		var spriteTable = meta.GetSpriteInstanceTable();

		ManagedSpriteInstance? instance = null;
		using (meta.Lock.Write) {
			if (!spriteTable.TryGetValue(instanceData.MapHash, out instance)) {
				meta.RemoveFromSpriteInstanceTable(instanceData.MapHash, dispose: false, out instance);
			}
		}

		instance?.Suspend();
	}

	// TODO : CP-A support - we hit here repeatedly for animated textures.
	// This obviously prevents things from caching or functioning for sprites that are animated.
	// The logic needs to be overridden and previously-cached textures stored in some fashion for sprites
	// that are determined to be animated
	internal static void Purge(XTexture2D reference, Bounds? sourceRectangle = null, bool animated = false) {
		try {
			var meta = reference.Meta();
			var spriteTable = meta.GetSpriteInstanceTable();

			using (meta.Lock.ReadWrite) {
				if (spriteTable.Count == 0) {
					return;
				}

				// TODO : handle sourceRectangle meaningfully.
				Debug.Trace($"Purging Texture {reference.NormalizedName()}");

				bool hasSourceRect = sourceRectangle.HasValue;

				var removeTexture = hasSourceRect ? new List<ulong>() : null!;
				var nullTextures = new List<ManagedTexture2D>();

				foreach (var pairs in spriteTable) {
					var spriteInstance = pairs.Value;
					lock (spriteInstance) {
						if (sourceRectangle.HasValue && !spriteInstance.OriginalSourceRectangle.Overlaps(sourceRectangle.Value)) {
							continue;
						}
						if (spriteInstance.Texture is not null) {
							nullTextures.Add(spriteInstance.Texture);
						}
						spriteInstance.Texture = null;
						if (hasSourceRect) {
							removeTexture.Add(pairs.Key);
						}
					}
				}

				var instanceDisposeList = new List<ManagedSpriteInstance>();

				using (meta.Lock.Write) {
					foreach (var instance in nullTextures) {
						instance.Dispose();
					}

					if (hasSourceRect) {
						foreach (var hash in removeTexture) {
							meta.RemoveFromSpriteInstanceTable(hash, dispose: false, out var instance);
							if (instance is not null) {
								instanceDisposeList.Add(instance);
							}
						}
					}
					else {
						meta.ClearSpriteInstanceTable(true);
					}
				}
				// : TODO dispose sprites?

				foreach (var instance in instanceDisposeList) {
					instance.Suspend();
				}
			}
		}
		catch {
			// ignored
		}
	}

	internal static void Invalidate(XTexture2D reference, Bounds? sourceRectangle = null, bool animated = false) {
		try {
			var meta = reference.Meta();
			var spriteTable = meta.GetSpriteInstanceTable();

			using (meta.Lock.Read) {
				if (spriteTable.Count == 0) {
					return;
				}

				// TODO : handle sourceRectangle meaningfully.
				Debug.Trace($"Invalidating Texture {reference.NormalizedName()}");

				foreach (var pairs in spriteTable) {
					var spriteInstance = pairs.Value;
					lock (spriteInstance) {
						if (sourceRectangle.HasValue && !spriteInstance.OriginalSourceRectangle.Overlaps(sourceRectangle.Value)) {
							continue;
						}
						spriteInstance.Invalidated = true;
					}
				}
			}
		}
		catch {
			// ignored
		}
	}

	private static readonly string[] Seasons = {
		"spring",
		"summer",
		"fall",
		"winter"
	};

	internal static void SeasonPurge(string season) {
		if (!Config.Garbage.SeasonalPurge) {
			return;
		}

		try {
			foreach (var spriteInstance in SpriteInstanceReferences) {
				if (spriteInstance.Anonymous()) {
					continue;
				}

				var textureName = spriteInstance.NormalizedName().ToLowerInvariant();
				if (textureName.Contains(season) || !Seasons.AnyF(s => textureName.Contains(s))) {
					continue;
				}

				if (spriteInstance.Reference.TryGetTarget(out _)) {
					spriteInstance.Dispose();
				}
			}
		}
		catch {
			// ignored
		}
	}

	internal static Dictionary<XTexture2D, List<ManagedSpriteInstance>> GetDump() {
		var result = new Dictionary<XTexture2D, List<ManagedSpriteInstance>>();

		foreach (var spriteInstance in SpriteInstanceReferences) {
			if (!spriteInstance.Reference.TryGetTarget(out var referenceTexture)) {
				continue;
			}

			if (!result.TryGetValue(referenceTexture, out var resultList)) {
				resultList = new List<ManagedSpriteInstance>();
				result.Add(referenceTexture, resultList);
			}
			resultList.Add(spriteInstance);
		}

		return result;
	}
}
