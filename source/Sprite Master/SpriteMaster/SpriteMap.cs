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
using System.Runtime.CompilerServices;

namespace SpriteMaster;
sealed class SpriteMap {
	private readonly SharedLock Lock = new();
	private readonly WeakCollection<ScaledTexture> ScaledTextureReferences = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	static private ulong SpriteHash(Texture2D texture, in Bounds source, uint expectedScale) {
		return Hash.Combine(source.Hash(), expectedScale.GetHashCode());
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Add(Texture2D reference, ScaledTexture texture, in Bounds source, uint expectedScale) {
		var rectangleHash = SpriteHash(texture: reference, source: source, expectedScale: expectedScale);

		var meta = reference.Meta();
		using (Lock.Exclusive) {
			ScaledTextureReferences.Add(texture);
			using (meta.Lock.Exclusive) {
				meta.SpriteTable.Add(rectangleHash, texture);
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal bool TryGet(Texture2D texture, in Bounds source, uint expectedScale, out ScaledTexture result) {
		result = null;
		var rectangleHash = SpriteHash(texture: texture, source: source, expectedScale: expectedScale);

		var meta = texture.Meta();
		var Map = meta.SpriteTable;
		using (meta.Lock.Shared) {
			if (Map.TryGetValue(rectangleHash, out var scaledTexture)) {
				if (scaledTexture.Texture?.IsDisposed == true) {
					using (meta.Lock.Promote) {
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

		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Remove(ScaledTexture scaledTexture, Texture2D texture) {
		try {
			var meta = texture.Meta();

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
			}
			using (meta.Lock.Exclusive) {
				meta.SpriteTable.Clear();
			}
		}
		finally {
			if (scaledTexture.Texture != null && !scaledTexture.Texture.IsDisposed) {
				Debug.TraceLn($"Disposing Active HD Texture: {scaledTexture.SafeName()}");

				//scaledTexture.Texture.Dispose();
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void Purge(Texture2D reference, in Bounds? sourceRectangle = null) {
		try {
			var meta = reference.Meta();
			var Map = meta.SpriteTable;
			using (meta.Lock.Shared) {
				if (Map.Count == 0) {
					return;
				}

				// TODO : handle sourceRectangle meaningfully.
				Debug.TraceLn($"Purging Texture {reference.SafeName()}");

				bool hasSourceRect = sourceRectangle.HasValue;

				var removeTexture = hasSourceRect ? new List<ulong>() : null;

				foreach (var pairs in Map) {
					var scaledTexture = pairs.Value;
					lock (scaledTexture) {
						if (sourceRectangle.HasValue && !scaledTexture.OriginalSourceRectangle.Overlaps(sourceRectangle.Value)) {
							continue;
						}
						if (scaledTexture.Texture != null) {
							// TODO : should this be locked?
							scaledTexture.Texture.Dispose();
						}
						scaledTexture.Texture = null;
						if (hasSourceRect) {
							removeTexture.Add(pairs.Key);
						}
					}
				}

				using (meta.Lock.Promote) {
					if (hasSourceRect) {
						foreach (var hash in removeTexture) {
							Map.Remove(hash);
						}
					}
					else {
						Map.Clear();
					}
				}
				// : TODO dispose sprites?
			}
		}
		catch { }
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal void SeasonPurge(string season) {
		try {
			var purgeList = new List<ScaledTexture>();
			using (Lock.Shared) {
				foreach (var scaledTexture in ScaledTextureReferences) {
					if (scaledTexture.Anonymous())
						continue;
					var textureName = scaledTexture.SafeName().ToLowerInvariant();
					if (
						!textureName.Contains(season.ToLowerInvariant()) &&
						(
							textureName.Contains("spring") ||
							textureName.Contains("summer") ||
							textureName.Contains("fall") ||
							textureName.Contains("winter")
						)
					) {
						purgeList.Add(scaledTexture);
					}
				}
			}
			foreach (var purgable in purgeList) {
				if (purgable.Reference.TryGetTarget(out var reference)) {
					purgable.Dispose();
					var meta = reference.Meta();
					using (meta.Lock.Exclusive) {
						meta.SpriteTable.Clear();
					}
				}
			}
		}
		catch { }
	}

	internal Dictionary<Texture2D, List<ScaledTexture>> GetDump() {
		var result = new Dictionary<Texture2D, List<ScaledTexture>>();

		foreach (var scaledTexture in ScaledTextureReferences) {
			if (scaledTexture.Reference.TryGetTarget(out var referenceTexture)) {
				if (!result.TryGetValue(referenceTexture, out var resultList)) {
					resultList = new List<ScaledTexture>();
					result.Add(referenceTexture, resultList);
				}
				resultList.Add(scaledTexture);
			}
		}

		return result;
	}
}
