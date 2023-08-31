/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SeedsUnboxed
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace SeedsUnpacked
{
	public sealed class ModEntry : Mod
	{
		public class SeedDataModel
		{
			public SeedDataEntry Include;
			public SeedDataEntry Exclude;
		}

		public class SeedDataEntry
		{
			public string[] Prefix;
			public string[] Suffix;
			public string[] Name;
		}

		///      v v v v v v v v v v v v

		// Target used for edits to mod seed data from other mods
		public const string GameSeedDataAssetKey = @"Mods/blueberry.SeedsUnpacked/Seeds";

		//      ^ ^ ^ ^ ^ ^ ^ ^ ^ ^ ^ ^

		private const string LocalSeedDataAssetKey = @"assets/seeds.json";
		private const string TargetAssetKey = @"Maps/springobjects";

		public override void Entry(IModHelper helper)
		{
			this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
			this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
		}

		private void OnSaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
		{
			// Ensure seed assets are edited on load
			this.Helper.GameContent.InvalidateCache(ModEntry.TargetAssetKey);
		}

		private void OnAssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo(ModEntry.GameSeedDataAssetKey))
			{
				// Load seed data
				e.LoadFromModFile<SeedDataModel>(relativePath: ModEntry.LocalSeedDataAssetKey, priority: StardewModdingAPI.Events.AssetLoadPriority.Medium);
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(ModEntry.TargetAssetKey)
				&& Game1.objectInformation is not null
				&& Game1.cropSpriteSheet is not null
				)
			{
				// Edit seed assets
				e.Edit(apply: this.Edit);
			}
		}

		private void Edit(IAssetData asset)
		{
			try
			{
				// Reload seed data on each edit to allow for input from other mods
				SeedDataModel seeds = this.Helper.ModContent.Load<SeedDataModel>(ModEntry.LocalSeedDataAssetKey);
				Dictionary<int, string> crops = this.Helper.GameContent.Load<Dictionary<int, string>>(@"Data/Crops");

				// Replace each valid seed in object spritesheet with matching source from crops spritesheet
				foreach (int id in Game1.objectInformation.Keys.Where((int id) => Game1.objectInformation[id].Split('/') is string[] split
					// Crop seeds
					&& crops.ContainsKey(id)
					// Inclusions
					&& (seeds.Include.Name.Any((string name) => split[0].Equals(name, StringComparison.InvariantCultureIgnoreCase))
						|| seeds.Include.Prefix.Any((string prefix) => split[0].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
						|| seeds.Include.Suffix.Any((string suffix) => split[0].EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase)))
					// Exclusions
					&& seeds.Exclude.Name.All((string name) => !split[0].Equals(name, StringComparison.InvariantCultureIgnoreCase))
					&& seeds.Exclude.Prefix.All((string prefix) => !split[0].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
					&& seeds.Exclude.Suffix.All((string suffix) => !split[0].EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))))
				{
					try
					{
						// Create dummy crop to fetch source area
						Crop crop = new(seedIndex: id, tileX: 0, tileY: 0);

						// Adjust source area for probable position of seed sprite
						Rectangle source = crop.getSourceRect(number: 0);
						source.Y += source.Height - Game1.smallestTileSize;
						source.Height = Game1.smallestTileSize;
						source.Width = Game1.smallestTileSize;

						// Fetch matching target area in object spritesheet
						Rectangle target = Game1.getSourceRectForStandardTileSheet(
							tileSheet: Game1.objectSpriteSheet,
							tilePosition: id,
							width: source.Width,
							height: source.Height);

						// Replace object sprite with crop sprite
						asset.AsImage().PatchImage(
							source: Game1.cropSpriteSheet,
							sourceArea: source,
							targetArea: target,
							patchMode: PatchMode.Replace);
					}
					catch (Exception e)
					{
						this.Monitor.Log($"Error editing crop {id}:{Environment.NewLine}{e}", LogLevel.Error);
						continue;
					}
				}
			}
			catch (Exception e)
			{
				this.Monitor.Log($"Error editing crops. Some sprites may not be changed.{Environment.NewLine}{e}", LogLevel.Error);
			}
		}
	}
}
