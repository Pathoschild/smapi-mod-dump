/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using LinqFasterer;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static SpriteMaster.Runtime;

namespace SpriteMaster;

internal static partial class Debug {
	[MethodImpl(MethodImpl.IgnoreOptimization)]
	internal static void DumpMemory() {
		static string DisposedString(bool disposed) => disposed ? "[DISPOSED]" : string.Empty;

		var dumpBuilder = new StringBuilder();

		var duplicates = new Dictionary<string, List<XTexture2D>>();
		bool haveDuplicates = false;

		var textureDump = SpriteMap.GetDump();
		long totalSize = 0;
		long totalOriginalSize = 0;

		dumpBuilder.AppendLine();
		dumpBuilder.AppendLine("╒═══════════════════════╕");
		dumpBuilder.AppendLine("│ Managed Resource Dump │");
		dumpBuilder.AppendLine("╞═══════════════════════╛");
		foreach (var list in textureDump) {
			var referenceTexture = list.Key;
			long originalSize = (referenceTexture.Area() * sizeof(int));
			bool referenceDisposed = referenceTexture.IsDisposed;
			totalOriginalSize += referenceDisposed ? 0 : originalSize;

			long resampledSize = 0;
			foreach (var sprite in list.Value) {
				if (sprite.IsReady && sprite.Texture is not null) {
					resampledSize += sprite.MemorySize;
				}
			}

			void PrintSpritesheetProperties(IEnumerable<KeyValuePair<string, string>> properties) {
				var keyValuePairs = properties as KeyValuePair<string, string>[] ?? properties.ToArray();
				int maxKeyLen = keyValuePairs.Max(prop => prop.Key.Length);
				foreach (var kvp in keyValuePairs) {
					dumpBuilder.AppendLine($"│ ├ {kvp.Key.PadRight(maxKeyLen)}: {kvp.Value}");
				}
			}

			var systemHashVal = referenceTexture.GetHashCode();
			var systemHash = systemHashVal.ToString64();
			var metaHashVal = referenceTexture.Meta().Hash.Value;
			var metaHash = metaHashVal.ToString64();
			var metaHashStr = (metaHashVal == 0UL) ? "unhashed!" : metaHash; // We want to print something indicating that it's unhashed rather than 0.
			dumpBuilder.AppendLine( "│");
			dumpBuilder.AppendLine($"├ SpriteSheet: '{referenceTexture.NormalizedName()}' {DisposedString(referenceDisposed)}");
			var spriteSheetProps = new KeyValuePair<string, string>[] {
				new("Hash", metaHashStr),
				new("System Hash", systemHash),
				new("Disposed", referenceDisposed.ToString()),
				new("Original Size", originalSize.AsDataSize()),
				new("Resampled Size", resampledSize.AsDataSize()),
				new("Total Size", (originalSize + resampledSize).AsDataSize())
			};
			PrintSpritesheetProperties(spriteSheetProps);
			dumpBuilder.AppendLine("│ │");

			if (!referenceTexture.Anonymous() && !referenceTexture.IsDisposed) {
				if (!duplicates.TryGetValue(referenceTexture.NormalizedName(), out var duplicateList)) {
					duplicateList = new List<XTexture2D>();
					duplicates.Add(referenceTexture.NormalizedName(), duplicateList);
				}
				duplicateList.Add(referenceTexture);
				haveDuplicates = haveDuplicates || (duplicateList.Count > 1);
			}

			var sortedSprites = new List<ManagedSpriteInstance>(list.Value);
			sortedSprites.Sort((a, b) => {
				var cmp = a.OriginalSourceRectangle.Left - b.OriginalSourceRectangle.Left;
				if (cmp != 0) {
					return cmp;
				}
				cmp = a.OriginalSourceRectangle.Top - b.OriginalSourceRectangle.Top;
				if (cmp != 0) {
					return cmp;
				}
				cmp = a.OriginalSourceRectangle.Width - b.OriginalSourceRectangle.Width;
				if (cmp != 0) {
					return cmp;
				}
				cmp = a.OriginalSourceRectangle.Height - b.OriginalSourceRectangle.Height;
				if (cmp != 0) {
					return cmp;
				}

				return a.GetHashCode().CompareTo(b.GetHashCode());
			});

			foreach (var sprite in sortedSprites) {
				if (!sprite.IsReady || sprite.Texture is null) {
					continue;
				}

				bool last = ReferenceEquals(list.Value.LastF(), sprite);
				var spriteDisposed = sprite.Texture.IsDisposed;
				dumpBuilder.AppendLine($"│ {(last ? '└' : '├')} sprite: {sprite.OriginalSourceRectangle} :: {sprite.MemorySize.AsDataSize()} {DisposedString(spriteDisposed)}");
				totalSize += spriteDisposed ? 0 : sprite.MemorySize;
			}
		}
		dumpBuilder.AppendLine( "│");
		dumpBuilder.AppendLine( "├ Total Sizes:");
		dumpBuilder.AppendLine( "│ │");
		dumpBuilder.AppendLine($"│ ├ Resampled: {totalSize.AsDataSize()}");
		dumpBuilder.AppendLine($"│ ├ Original : {totalOriginalSize.AsDataSize()}");
		dumpBuilder.AppendLine($"│ └ All      : {(totalOriginalSize + totalSize).AsDataSize()}");
		dumpBuilder.AppendLine( "│");
		if (!haveDuplicates) {
			dumpBuilder.AppendLine( "└ Duplicates: none");
		}
		else {
			dumpBuilder.AppendLine( "└ Duplicates:");
			dumpBuilder.AppendLine( "  │");
			foreach (var duplicate in duplicates.Where(kv => kv.Value.Count > 1)) {
				bool last = duplicate.Equals(duplicates.Last());
				long size = 0;
				foreach (var subDuplicate in duplicate.Value) {
					size += subDuplicate.Area() * sizeof(int);
				}

				dumpBuilder.AppendLine($"  {(last ? '└' : '├')} '{duplicate.Key}' :: {duplicate.Value.Count.Delimit()} duplicates :: Total Size: {size.AsDataSize()}");
			}
		}

		lock (Console.Error) {
			MessageLn(dumpBuilder.ToString());
			Console.Error.Flush();
		}
	}
}
