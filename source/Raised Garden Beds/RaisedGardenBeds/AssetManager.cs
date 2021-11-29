/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/RaisedGardenBeds
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaisedGardenBeds
{
	public class AssetManager : IAssetLoader, IAssetEditor
	{
		private readonly IModHelper _helper;

		internal static readonly string GameContentAssetPath = Path.Combine("Mods", "blueberry.rgb.Assets");

		internal static readonly string GameContentEndOfNightSpritesPath = Path.Combine(GameContentAssetPath, "EndOfNightSprites");
		internal static readonly string GameContentEventDataPath = Path.Combine(GameContentAssetPath, "EventData");
		internal static readonly string GameContentCommonTranslationDataPath = Path.Combine(GameContentAssetPath, "CommonTranslations");
		internal static readonly string GameContentItemTranslationDataPath = Path.Combine(GameContentAssetPath, "ItemTranslations");

		internal static readonly string LocalAssetPath = "assets";

		internal static readonly string LocalEndOfNightSpritesPath = Path.Combine(LocalAssetPath, "endOfNightSprites.png");
		internal static readonly string LocalEventDataPath = Path.Combine(LocalAssetPath, "eventData.json");

		internal static readonly string ContentPackPath = Path.Combine(LocalAssetPath, "ContentPack");


		public AssetManager(IModHelper helper)
		{
			this._helper = helper;
		}

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(GameContentEndOfNightSpritesPath)
				|| asset.AssetNameEquals(GameContentEventDataPath)
				|| asset.AssetNameEquals(GameContentCommonTranslationDataPath)
				|| asset.AssetNameEquals(GameContentItemTranslationDataPath);
		}

		public T Load<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals(GameContentEndOfNightSpritesPath))
			{
				return (T)(object)_helper.Content.Load
					<Texture2D>
					(LocalEndOfNightSpritesPath);
			}
			if (asset.AssetNameEquals(GameContentEventDataPath))
			{
				return (T)(object)_helper.Content.Load
					<Dictionary<string, object>>
					(LocalEventDataPath);
			}
			if (asset.AssetNameEquals(GameContentCommonTranslationDataPath))
			{
				var data = new Dictionary
					<string, Dictionary<string, string>>
					(StringComparer.InvariantCultureIgnoreCase);

				// Populate all possible language codes for translation pack support
				string[] keys = Enum.GetNames(typeof(StardewValley.LocalizedContentManager.LanguageCode));
				foreach (string key in keys)
				{
					data.Add(key, new Dictionary<string, string>());
				}

				return (T)(object)data;
			}
			if (asset.AssetNameEquals(GameContentItemTranslationDataPath))
			{
				var data = new Dictionary
					<string, Dictionary<string, Dictionary<string, string>>>
					(StringComparer.InvariantCultureIgnoreCase);

				// Populate all possible language codes for translation pack support
				string[] keys = Enum.GetNames(typeof(StardewValley.LocalizedContentManager.LanguageCode));
				foreach (string key in keys)
				{
					data.Add(key, new Dictionary<string, Dictionary<string, string>>());
				}

				return (T)(object)data;
			}
			return (T)(object)null;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(GameContentEventDataPath)
				|| asset.AssetNameEquals(Path.Combine("TileSheets", "Craftables"))
				|| asset.AssetNameEquals(Path.Combine("Data", "BigCraftablesInformation"))
				|| asset.AssetNameEquals(Path.Combine("Data", "CraftingRecipes"))
				// Also patch the event dictionary for any locations with an entry in our event definitions
				|| (asset.AssetName.StartsWith(Path.Combine("Data", "Events"))
					&& Path.GetFileNameWithoutExtension(asset.AssetName) is string where
					&& ModEntry.EventData != null && ModEntry.EventData.Any(e => e["Where"] == where));
		}

		public void Edit<T>(IAssetData asset)
		{
			/*********
			Local data
			*********/

			if (asset.AssetNameEquals(GameContentEventDataPath))
			{
				var events = ((Newtonsoft.Json.Linq.JArray)asset
					.AsDictionary<string, object>()
					.Data["Events"])
					.ToObject<List<Dictionary<string, string>>>();

				// Events are populated with preset tokens and script dialogues depending on game locale.

				// Root event tokenisation
				for (int i = 0; i < events.Count; ++i)
				{
					// Format event script with event NPC name, as well as their dialogue strings
					string[] args = new string[] { events[i]["Who"] }
						.Concat(new int[] { 1, 2, 3, 4 }
							.Select(j => Translations.GetTranslation($"event.{i}.dialogue.{j}")))
						.ToArray();
					events[i]["Script"] = string.Format(
						format: events[i]["Script"],
						args: args);
					events[i]["Conditions"] = string.Format(
						format: events[i]["Conditions"],
						events[i]["Who"]);
				}

				Log.T($"Loaded {events.Count} event(s).{Environment.NewLine}Root event: {events[0]["Where"]}/{events[0]["Conditions"]}");

				ModEntry.EventData = events;

				return;
			}

			/********
			Game data
			********/

			int id = OutdoorPot.BaseParentSheetIndex;

			if (asset.AssetNameEquals(Path.Combine("Data", "BigCraftablesInformation")))
			{
				if (ModEntry.ItemDefinitions == null)
					return;

				string[] fields;
				var data = asset.AsDictionary<int, string>().Data;

				// Set or reset the item ID for the generic object to some first best available index
				id = OutdoorPot.BaseParentSheetIndex = data.Keys.Max() + 2;

				string name, description;

				// Patch generic object entry into bigcraftables file, including display name and description from localisations file
				name = Translations.GetTranslation("item.name"); 
				description = Translations.GetTranslation("item.description.default");
				fields = data.First().Value.Split('/');	// Use existing data as a template; most fields are common or unused
				fields[0] = OutdoorPot.GenericName;
				fields[4] = description;
				fields[8] = name;
				data[id] = string.Join("/", fields);

				// Patch in dummy object entries after generic object entry
				for (int i = 1; i < ModEntry.ItemDefinitions.Count; ++i)
				{
					ItemDefinition d = ModEntry.ItemDefinitions[ModEntry.ItemDefinitions.Keys.ElementAt(i)];
					name = Translations.GetNameTranslation(data: d);
					fields = data[id].Split('/');
					fields[4] = description;
					fields[8] = name;
					data[id + i] = string.Join("/", fields);
				}
				
				// Don't remove the generic craftable from data lookup, since it's used later for crafting recipes and defaults

				return;
			}
			if (asset.AssetNameEquals(Path.Combine("Data", "CraftingRecipes")))
			{
				if (ModEntry.ItemDefinitions == null || id < 0)
					return;

				// As above for the craftables dictionary, the recipes dictionary needs to have
				// our varieties patched in to have them appear.
				// Since all objects share a single ParentSheetIndex, each crafting recipe will normally
				// only produce a generic/wooden object.
				// This is handled in HarmonyPatches.CraftingPage_ClickCraftingRecipe_Prefix, which
				// is also needed to produce an OutdoorPot instance rather than a StardewValley.Object.

				// Add crafting recipes for all object variants
				var data = asset.AsDictionary<string, string>().Data;
				foreach (KeyValuePair<string, ItemDefinition> idAndFields in ModEntry.ItemDefinitions)
				{
					string[] newFields = new string[]
					{	// Crafting ingredients:
						ItemDefinition.ParseRecipeIngredients(data: idAndFields.Value),
						// Unused field:
						"blue berry",
						// Crafted item ID and quantity:
						$"{OutdoorPot.BaseParentSheetIndex} {idAndFields.Value.RecipeCraftedCount}",
						// Recipe is bigCraftable:
						"true",
						// Recipe conditions (we ignore these):
						"blue berry",
						// Recipe display name:
						Translations.GetNameTranslation(data: idAndFields.Value)
					};
					data[OutdoorPot.GetNameFromVariantKey(idAndFields.Key)] = string.Join("/", newFields);
				}

				return;
			}
			if (asset.AssetName.StartsWith(Path.Combine("Data", "Events"))
				&& Path.GetFileNameWithoutExtension(asset.AssetName) is string where)
			{
				// Patch our event data into whatever location happens to match the one specified.
				// Event tokenisation is handled in the Edit block for GameContentEventDataPath.

				if (ModEntry.EventData != null
					&& ModEntry.EventData.FirstOrDefault(e => e["Where"] == where) is Dictionary<string, string> eventData)
				{
					string key = $"{ModEntry.EventRootId}{ModEntry.EventData.IndexOf(eventData)}/{eventData["Conditions"]}";
					asset.AsDictionary<string, string>().Data[key] = eventData["Script"];
				}

				return;
			}
		}
	}
}
