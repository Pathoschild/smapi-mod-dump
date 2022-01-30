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
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

// TODO : This class, and Texture2DMeta, have a _lot_ of inter-play and it makes it very confusing.
// This needs to be cleaned up badly.
static class SpriteMap {
	private static readonly SharedLock Lock = new();
	private static readonly WeakCollection<ManagedSpriteInstance> SpriteInstanceReferences = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong SpriteHash(Texture2D texture, in Bounds source, uint expectedScale) {
		return Hashing.Combine(source.Hash(), expectedScale.GetSafeHash());
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Add(Texture2D reference, ManagedSpriteInstance instance, out ManagedSpriteInstance? current) {
		var meta = reference.Meta();
		using (Lock.Write) {
			SpriteInstanceReferences.Add(instance);  
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
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool AddReplaceInvalidated(Texture2D reference, ManagedSpriteInstance instance) {
		var meta = reference.Meta();
		using (Lock.Write) {
			SpriteInstanceReferences.Add(instance);
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
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryGetReady(Texture2D texture, in Bounds source, uint expectedScale, [NotNullWhen(true)] out ManagedSpriteInstance? result) {
		if (TryGet(texture, source, expectedScale, out var internalResult)) {
			if (internalResult.IsReady) {
				result = internalResult;
				return true;
			}
			else if (internalResult.PreviousSpriteInstance?.IsReady ?? false) {
				result = internalResult.PreviousSpriteInstance;
				return true;
			}
		}
		result = null;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryGet(Texture2D texture, in Bounds source, uint expectedScale, [NotNullWhen(true)] out ManagedSpriteInstance? result) {
		var rectangleHash = SpriteHash(texture: texture, source: source, expectedScale: expectedScale);

		var meta = texture.Meta();
		var spriteTable = meta.GetSpriteInstanceTable();

		using (meta.Lock.ReadWrite) {
			if (spriteTable.TryGetValue(rectangleHash, out var spriteInstance)) {
				if (spriteInstance.Texture?.IsDisposed == true) {
					var removeList = new List<ulong>();
					using (meta.Lock.Write) {
						foreach (var skv in spriteTable) {
							if (skv.Value?.Texture?.IsDisposed ?? false) {
								removeList.Add(skv.Key);
							}
						}
						foreach (var key in removeList) {
							meta.RemoveFromSpriteInstanceTable(key);
						}
					}
				}
				else {
					result = spriteInstance;
					return true;
				}
			}
		}

		result = null;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Remove(ManagedSpriteInstance spriteInstance, Texture2D texture) {
		try {
			var meta = texture.Meta();
			var spriteTable = meta.GetSpriteInstanceTable();

			using (Lock.Write) {
				try {
					SpriteInstanceReferences.Purge();
					var removeElements = new List<ManagedSpriteInstance>();
					foreach (var element in SpriteInstanceReferences) {
						if (element == spriteInstance) {
							removeElements.Add(element);
						}
					}

					foreach (var element in removeElements) {
						SpriteInstanceReferences.Remove(element);
					}
				}
				catch { }
			}
			using (meta.Lock.Write) {
				if (spriteTable.TryGetValue(spriteInstance.SpriteMapHash, out var currentValue) && currentValue == spriteInstance) {
					meta.RemoveFromSpriteInstanceTable(spriteInstance.SpriteMapHash);
				}
			}
		}
		finally {
			if (spriteInstance.Texture != null && !spriteInstance.Texture.IsDisposed) {
				Debug.TraceLn($"Disposing Active HD Texture: {spriteInstance.SafeName()}");

				//spriteInstance.Texture.Dispose();
			}
		}
	}

	// TODO : CP-A support - we hit here repeatedly for animated textures.
	// This obviously prevents things from caching or functioning for sprites that are animated.
	// The logic needs to be overridden and previously-cached textures stored in some fashion for sprites
	// that are determined to be animated
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Purge(Texture2D reference, in Bounds? sourceRectangle = null) {
		try {
			var meta = reference.Meta();
			var spriteTable = meta.GetSpriteInstanceTable();

			using (meta.Lock.ReadWrite) {
				if (spriteTable.Count == 0) {
					return;
				}

				// TODO : handle sourceRectangle meaningfully.
				Debug.TraceLn($"Purging Texture {reference.SafeName()}");

				bool hasSourceRect = sourceRectangle.HasValue;

				var removeTexture = hasSourceRect ? new List<ulong>() : null!;

				foreach (var pairs in spriteTable) {
					var spriteInstance = pairs.Value;
					lock (spriteInstance) {
						if (sourceRectangle.HasValue && !spriteInstance.OriginalSourceRectangle.Overlaps(sourceRectangle.Value)) {
							continue;
						}
						if (spriteInstance.Texture is not null) {
							// TODO : should this be locked?
							spriteInstance.Texture.Dispose();
						}
						spriteInstance.Texture = null;
						if (hasSourceRect) {
							removeTexture.Add(pairs.Key);
						}
					}
				}

				using (meta.Lock.Write) {
					if (hasSourceRect) {
						foreach (var hash in removeTexture) {
							meta.RemoveFromSpriteInstanceTable(hash);
						}
					}
					else {
						meta.ClearSpriteInstanceTable();
					}
				}
				// : TODO dispose sprites?
			}
		}
		catch { }
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Invalidate(Texture2D reference, in Bounds? sourceRectangle = null) {
		try {
			var meta = reference.Meta();
			var spriteTable = meta.GetSpriteInstanceTable();

			using (meta.Lock.Read) {
				if (spriteTable.Count == 0) {
					return;
				}

				// TODO : handle sourceRectangle meaningfully.
				Debug.TraceLn($"Invalidating Texture {reference.SafeName()}");

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
		catch { }
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void SeasonPurge(string season) {
		try {
			var purgeList = new List<ManagedSpriteInstance>();
			using (Lock.Read) {
				foreach (var spriteInstance in SpriteInstanceReferences) {
					if (spriteInstance is null || spriteInstance.Anonymous()) {
						continue;
					}

					var textureName = spriteInstance.SafeName().ToLowerInvariant();
					if (
						!textureName.Contains(season) &&
						(
							textureName.Contains("spring") ||
							textureName.Contains("summer") ||
							textureName.Contains("fall") ||
							textureName.Contains("winter")
						)
					) {
						purgeList.Add(spriteInstance);
					}
				}
			}
			foreach (var purgable in purgeList) {
				if (purgable.Reference.TryGetTarget(out var reference)) {
					purgable.Dispose();
					var meta = reference.Meta();
					using (meta.Lock.Write) {
						meta.ClearSpriteInstanceTable();
					}
				}
			}
		}
		catch { }
	}

	internal static Dictionary<Texture2D, List<ManagedSpriteInstance>> GetDump() {
		var result = new Dictionary<Texture2D, List<ManagedSpriteInstance>>();

		foreach (var spriteInstance in SpriteInstanceReferences) {
			if (spriteInstance is not null && spriteInstance.Reference.TryGetTarget(out var referenceTexture)) {
				if (!result.TryGetValue(referenceTexture, out var resultList)) {
					resultList = new List<ManagedSpriteInstance>();
					result.Add(referenceTexture, resultList);
				}
				resultList.Add(spriteInstance);
			}
		}

		return result;
	}
}
