/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomCommunityCentre.Data
{
    public class ContentPack
    {
        public IContentPack Source;
        public Dictionary<string, CustomCommunityCentre.Data.BundleMetadata> Metadata;
        public Dictionary<string, CustomCommunityCentre.Data.BundleDefinition> Definitions;
        public Dictionary<string, Dictionary<string, CustomCommunityCentre.Data.BundleSubstitute>> Substitutes;
		public Dictionary<string, Texture2D> Spritesheets;


		public static void Load(IModHelper helper)
		{
			Log.D("Loading content packs...",
				CustomCommunityCentre.ModEntry.Config.DebugMode);

			CustomCommunityCentre.ModEntry.ContentPacks = new();

			// Fetch content packs loaded by SMAPI:
			List<IContentPack> contentPacks = helper.ContentPacks.GetOwned().ToList();

			// Fetch additional content packs added by other SMAPI mods:
			IEnumerable<string> additionalContentPackPaths = CustomCommunityCentre.Events.Content.InvokeOnLoadingContentPacks();
			foreach (string absoluteDirectoryPath in additionalContentPackPaths)
			{
				// Code based on spacechase0's Json Assets
				// See StardewValleyMods.JsonAssets.Mod.cs:LoadData()
				// https://github.com/spacechase0/StardewValleyMods/

				try
				{
					// Read content pack definition
					var manifest = helper.ContentPacks.CreateFake(directoryPath: absoluteDirectoryPath)
						.ReadJsonFile
						<CustomCommunityCentre.Data.TemporaryManifest>
						($"{CustomCommunityCentre.AssetManager.TemporaryManifestFileName}.json");

					if (manifest == null)
					{
						Log.E($"Content pack at {absoluteDirectoryPath} was not loaded: '{CustomCommunityCentre.AssetManager.TemporaryManifestFileName}.json' could not be read.");
						continue;
					}

					// Load content pack
					IContentPack contentPack = helper.ContentPacks.CreateTemporary(
						directoryPath: absoluteDirectoryPath,
						id: manifest.UniqueID,
						name: manifest.Name,
						description: manifest.Description,
						author: manifest.Author,
						version: new SemanticVersion(manifest.Version));

					// Add to list of content packs to load
					contentPacks.Add(contentPack);

					Log.D($"Found additional content pack: {contentPack.Manifest.UniqueID}",
						CustomCommunityCentre.ModEntry.Config.DebugMode);
				}
				catch (Exception e)
				{
					Log.E($"Exception while loading content pack ({absoluteDirectoryPath}):{Environment.NewLine}{e}");
				}
			}

			// Load all fetched content packs
			foreach (IContentPack source in contentPacks)
			{
				try
				{
					Log.D($"Loading content pack: {source.Manifest.UniqueID}",
						CustomCommunityCentre.ModEntry.Config.DebugMode);

					// Load content packs
					CustomCommunityCentre.Data.ContentPack contentPack = source.ReadJsonFile
					   <CustomCommunityCentre.Data.ContentPack>
					   ($"{CustomCommunityCentre.AssetManager.ContentPackDataFileName}.json");

					if (contentPack == null)
					{
						Log.E($"Content pack {source.Manifest.UniqueID} was not loaded: '{CustomCommunityCentre.AssetManager.ContentPackDataFileName}.json' could not be read.");
						continue;
					}

					contentPack.Source = source;

					// b e g o n e,   n u l l s
					contentPack.Definitions ??= new();
					contentPack.Metadata ??= new();
					contentPack.Substitutes ??= new();
					contentPack.Spritesheets = new();

					// Add bundle spritesheets
					const string ext = ".png";
					IEnumerable<(string sprite, string filename, string absoluteFilename, string assetKey)> filenames = Directory
						.GetFiles(source.DirectoryPath)
						.Where(s => Path.GetExtension(s).Equals(ext, StringComparison.InvariantCultureIgnoreCase))
						.Select(s => (
							sprite: Path.GetFileNameWithoutExtension(s),
							filename: Path.GetFileName(s),
							absoluteFilename: s,
							assetKey: CustomCommunityCentre.AssetManager.PrefixPath(
								asset: $"{source.Manifest.UniqueID}.{Path.GetFileNameWithoutExtension(s)}",
								prefix: CustomCommunityCentre.AssetManager.RootGameContentPath,
								separator: "/")));
					foreach ((string sprite, string filename, string absoluteFilename, string assetKey) in filenames)
                    {
						// Force unique names on bundle spritesheet files
						contentPack.Spritesheets.Add(key: assetKey, value: source.LoadAsset<Texture2D>(Path.GetFileName(absoluteFilename)));
						Log.D($"Found spritesheet: {filename} ({assetKey})",
							CustomCommunityCentre.ModEntry.Config.DebugMode);
					}
					// Replace bundle spritesheet references with unique names
					string getBundleSpriteEntry(string sprite)
					{
						string[] spriteAndNumber = sprite.Split(':');
						sprite = spriteAndNumber.First();
						(string sprite, string filename, string absoluteFilename, string assetKey) tuple = filenames
							.FirstOrDefault(t => t.sprite == sprite);
						if (sprite == null || tuple.assetKey == null)
						{
							Log.E($"Spritesheet for content pack {source.Manifest.UniqueID} was not loaded:"
								+ $" No matching '{sprite}{ext}' asset for entry"
								+ $" \"{nameof(StardewValley.GameData.BundleData.Sprite)}\": \"{sprite}\""
								+ $"{Environment.NewLine}(HINT: check TRACE logs for found spritesheets.)");
							return null;
						}
						return $"{tuple.assetKey}:{spriteAndNumber.Last()}";
                    }
					foreach (string key in contentPack.Definitions.Keys.ToList())
                    {
						foreach (string bundleName in contentPack.Definitions[key].Bundles.Keys.ToList())
						{
							contentPack.Definitions[key].Bundles[bundleName].Sprite
								= getBundleSpriteEntry(contentPack.Definitions[key].Bundles[bundleName].Sprite);
						}
						for (int i = 0; i < contentPack.Definitions[key].BundleSets.Count; ++i)
						{
							foreach (string bundleName in contentPack.Definitions[key].BundleSets[i].Keys.ToList())
							{
								contentPack.Definitions[key].BundleSets[i][bundleName].Sprite
									= getBundleSpriteEntry(contentPack.Definitions[key].BundleSets[i][bundleName].Sprite);
							}
						}
                    }

					// Add area names to metadata entries
					foreach (string key in contentPack.Metadata.Keys.ToList())
					{
						contentPack.Metadata[key].AreaName = key;
					}

					// Add loaded content packs to global data
					CustomCommunityCentre.ModEntry.ContentPacks.Add(contentPack);

					System.Text.StringBuilder message = new();
					message.AppendLine($"Loaded content pack {source.Manifest.UniqueID}:");
					message.AppendLine($"Metadata:    {contentPack.Metadata.Count}");
					message.AppendLine($"Definitions: {contentPack.Definitions.Count}");
					message.AppendLine($"Substitutes: {contentPack.Substitutes.Count} ({contentPack.Substitutes.SelectMany(_ => _.Value).Count()})");
					Log.D(message.ToString(),
						CustomCommunityCentre.ModEntry.Config.DebugMode);
				}
				catch (Exception e)
                {
					Log.E($"Exception while loading content pack:{Environment.NewLine}{e}");
				}
			}
		}
	}
}
