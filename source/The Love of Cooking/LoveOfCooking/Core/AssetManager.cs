/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Object = StardewValley.Object;

namespace LoveOfCooking
{
	public class AssetManager : IAssetEditor, IAssetLoader
	{
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

		private readonly Dictionary<string, int> _buffIndex = new Dictionary<string, int>
		{
			{ "Farming", 0},
			{ "Fishing", 1},
			{ "Mining", 2},
			{ "Luck", 4},
			{ "Foraging", 5},
			{ "Energy", 7},
			{ "Magnetism", 8},
			{ "Speed", 9},
			{ "Defense", 10},
			{ "Attack", 11}
		};
		internal static readonly Rectangle NotificationIconTargetArea = new Rectangle(506, 372, 11, 14);

		// Assets
		// Game content paths: asset keys sent as requests to Game1.content.Load<T>()
		// These can be intercepted and modified by AssetLoaders/Editors, eg. Content Patcher.
		private readonly List<string> _gameContentAssetPaths;
		public static readonly string RootGameContentPath = PathUtilities.NormalizePath("Mods/blueberry.LoveOfCooking.Assets");
		public static readonly string GameContentSpriteSheetPath = Path.Combine(RootGameContentPath, "Sprites");
		public static readonly string GameContentBundleDataPath = Path.Combine(RootGameContentPath, "Bundles");
		public static readonly string GameContentIngredientBuffDataPath = Path.Combine(RootGameContentPath, "IngredientBuffChart");
		public static readonly string GameContentDefinitionsPath = Path.Combine(RootGameContentPath, "ItemDefinitions");
		public static readonly string GameContentSkillValuesPath = Path.Combine(RootGameContentPath, "CookingSkillValues");
		public static readonly string GameContentSkillRecipeTablePath = Path.Combine(RootGameContentPath, "CookingSkillLevelUpRecipes");
		public static readonly string GameContentContextTagDataPath = Path.Combine(RootGameContentPath, "ContextTags");
		// Local paths: filepaths without extension passed to Helper.Content.Load<T>()
		// These are the paths for our default data files bundled with the mod in our assets folder.
		public static readonly string RootLocalContentPath = PathUtilities.NormalizePath("assets");
		public static readonly string LocalSpriteSheetPath = Path.Combine(RootLocalContentPath, "sprites");
		public static readonly string LocalBundleDataPath = Path.Combine(RootLocalContentPath, "bundles");
		public static readonly string LocalIngredientBuffDataPath = Path.Combine(RootLocalContentPath, "ingredientBuffChart");
		public static readonly string LocalDefinitionsPath = Path.Combine(RootLocalContentPath, "itemDefinitions");
		public static readonly string LocalSkillValuesPath = Path.Combine(RootLocalContentPath, "cookingSkillValues");
		public static readonly string LocalSkillRecipeTablePath = Path.Combine(RootLocalContentPath, "cookingSkillLevelUpRecipes");
		public static readonly string LocalContextTagDataPath = Path.Combine(RootLocalContentPath, "contextTags");
		// Content pack paths: filepaths without extension passed to JsonAssets.LoadAssets()
		// These are again bundled with the mod, but aren't intended to be intercepted or changed.
		public static readonly string RootContentPackPath = PathUtilities.NormalizePath("assets");
		public static readonly string BasicObjectsPack = Path.Combine(RootContentPackPath, "BasicObjectsPack");
		public static readonly string NewRecipesPackPath = Path.Combine(RootContentPackPath, "NewRecipesPack");
		public static readonly string NewCropsPackPath = Path.Combine(RootContentPackPath, "NewCropsPack");
		public static readonly string NettlesPackPath = Path.Combine(RootContentPackPath, "NettlesPack");
		public static readonly string ProducerFrameworkPackPath = Path.Combine(RootContentPackPath, "ProducerFrameworkPack");

		private static readonly string[] AssetsToEdit = new string[]
		{
			GameContentSpriteSheetPath,
			@"Data/Bundles",
			@"Data/BigCraftablesInformation",
			@"Data/CookingRecipes",
			@"Data/mail",
			@"Data/ObjectContextTags",
			@"Data/ObjectInformation",
			@"Data/Events/Saloon",
			@"Data/Events/Mountain",
			@"Data/Events/JoshHouse",
			@"Data/Events/Town",
			@"LooseSprites/Cursors",
			@"LooseSprites/JunimoNote",
			@"Maps/Beach",
			@"Maps/springobjects",
			@"Maps/townInterior",
			@"Strings/UI",
			@"Strings/Locations",
			@"TileSheets/tools",
		};


		public AssetManager()
		{
			// Populate all custom asset paths from GameContentPath values
			_gameContentAssetPaths = typeof(AssetManager)
				.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
				.Where(field => field.Name.StartsWith("GameContent") && field.Name.EndsWith("Path"))
				.Select(field => (string)field.GetValue(this)).ToList();
		}

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return _gameContentAssetPaths.Contains(asset.AssetName);
		}

		public T Load<T>(IAssetInfo asset)
		{
			Log.D($"Loading custom asset {asset.AssetName}",
				ModEntry.Config.DebugMode);
			if (asset.AssetNameEquals(GameContentSpriteSheetPath))
			{
				return (T)(object)ModEntry.Instance.Helper.Content.Load<Texture2D>($"{LocalSpriteSheetPath}.png");
			}
			if (asset.AssetNameEquals(GameContentBundleDataPath))
			{
				return (T)(object)ModEntry.Instance.Helper.Content.Load<Dictionary<string, Dictionary<string, List<string>>>>($"{LocalBundleDataPath}.json");
			}
			if (asset.AssetNameEquals(GameContentIngredientBuffDataPath))
			{
				return (T)(object)ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>($"{LocalIngredientBuffDataPath}.json");
			}
			if (asset.AssetNameEquals(GameContentDefinitionsPath))
			{
				return (T)(object)ModEntry.Instance.Helper.Content.Load<Dictionary<string, List<string>>>($"{LocalDefinitionsPath}.json");
			}
			if (asset.AssetNameEquals(GameContentSkillRecipeTablePath))
			{
				return (T)(object)ModEntry.Instance.Helper.Content.Load<Dictionary<string, List<string>>>($"{LocalSkillRecipeTablePath}.json");
			}
			if (asset.AssetNameEquals(GameContentSkillValuesPath))
			{
				return (T)(object)ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>($"{LocalSkillValuesPath}.json");
			}
			if (asset.AssetNameEquals(GameContentContextTagDataPath))
			{
				return (T)(object)ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>($"{LocalContextTagDataPath}.json");
			}
			return (T) (object) null;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return Game1.player != null && AssetsToEdit.Any(s => asset.AssetNameEquals(s));	
		}

		public void Edit<T>(IAssetData asset)
		{
			this.EditAsset(ref asset); // eat that, ENC0036
		}

		private void EditAsset(ref IAssetData asset)
		{
			if (asset.DataType == typeof(Texture2D) && asset.AsImage().Data.IsDisposed)
				return;
			if (asset.AssetNameEquals(@"Data/Bundles"))
			{
				// Make no changes for the new community centre bundle, but set our base values from the data
				// Do this even with community centre changes disabled, in case we join as farmhand to a player who has it enabled
				
				var data = asset.AsDictionary<string, string>().Data;
				Bundles.BundleStartIndex = 1 + data.Keys.ToList().Max(key => int.Parse(key.Split('/')[1]));
			}
			else if (asset.AssetNameEquals(@"Data/BigCraftablesInformation"))
			{
				var data = asset.AsDictionary<int, string>().Data;

				// Add localised names for new craftables
				foreach (KeyValuePair<int, string> pair in data.Where(pair => pair.Value.Split('/')[0].StartsWith(ModEntry.ObjectPrefix)).ToList())
				{
					string[] split = pair.Value.Split('/');
					string[] name = split[0].Split(new[] { '.' }, 3);
					string nameData = data[pair.Key];
					split[4] = i18n.Get($"item.{name[2]}.description").ToString();
					split[8] = i18n.Get($"item.{name[2]}.name").ToString();
					data[pair.Key] = string.Join("/", split);
					if (ModEntry.PrintRename)
						Log.D($"Named craftable {name[2]} ({data[pair.Key].Split('/')[5]})", ModEntry.Config.DebugMode);
				}

				asset.AsDictionary<int, string>().ReplaceWith(data);
			}
			else if (asset.AssetNameEquals(@"Data/CookingRecipes"))
			{
				// Edit fields of vanilla recipes to use new ingredients
				// Do NOT call RebuildBuffs from within this block

				if (Interface.Interfaces.JsonAssets == null || Game1.currentLocation == null)
					return;

				var data = asset.AsDictionary<string, string>().Data;

				// Add localised names for new recipes
				// While this also happens in CookingMenu.ctor for English locales, it's efficient here for other locales
				foreach (KeyValuePair<string, string> pair in data.Where(pair => pair.Key.StartsWith(ModEntry.ObjectPrefix)).ToList())
				{
					string[] name = pair.Key.Split(new[] { '.' }, 3);
					string nameData = data[pair.Key];
					string[] split = new string[6];
					data[pair.Key].Split('/').CopyTo(split, 0);
					split[5] = i18n.Get($"item.{name[2]}.name").ToString();
					data[pair.Key] = string.Join("/", split);
					if (ModEntry.PrintRename)
						Log.D($"Named recipe {name[2]} ({data[pair.Key].Split('/')[5]})", ModEntry.Config.DebugMode);
				}
				asset.AsDictionary<string, string>().ReplaceWith(data);

				try
				{
					// Substitute in the actual custom ingredients for custom recipes if custom ingredients are enabled
					Dictionary<string, string> recipeData = null;

					// Update recipe data for recipes planned to use vanilla objects or best-available common custom objects

					// BASE GAME RECIPES
					if (ModEntry.Config.AddRecipeRebalancing)
					{
						recipeData = new Dictionary<string, string>
						{
							// Maki Roll: Sashimi 1 Seaweed 1 Rice 1
							{
								"Maki Roll",
								"227 1 152 1 423 1"
							},
							// Pizza: Flour 2 Tomato 2 Cheese 2
							{
								"Pizza",
								"246 2 256 2 424 2"
							},
						};

						// New Crops ingredients:
						if (ModEntry.Config.AddNewCropsAndStuff || Interface.Interfaces.UsingPPJACrops)
						{
							recipeData = recipeData.Concat(new Dictionary<string, string>
							{
								// Coleslaw: Vinegar 1 Mayonnaise 1
								{
									"Coleslaw",
									$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.CabbageName)} 1"
									+ " 419 1 306 1"
								},
								// Cookies: Flour 1 Category:Egg 1 Chocolate Bar 1
								{
									"Cookies",
									"246 1 -5 1"
									+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.ChocolateName)} 1"
								},
							}).ToDictionary(pair => pair.Key, pair => pair.Value);
						}

						// Cooking Skill ingredients:
						if (ModEntry.Config.AddCookingSkillAndRecipes)
						{
							recipeData = recipeData.Concat(new Dictionary<string, string>
							{
								// Pink Cake: Cake 1 Melon 1
								{
									"Pink Cake",
									$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cake")} 1"
									+ " 254 1"
								}, 
								// Chocolate Cake: Cake 1 Chocolate Bar 1
								{
									"Chocolate Cake",
									$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cake")} 1"
									+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.ChocolateName)} 1"
								},
							}).ToDictionary(pair => pair.Key, pair => pair.Value);
						}

						foreach (KeyValuePair<string, string> recipe in recipeData)
							data[recipe.Key] = Utils.UpdateEntry(data[recipe.Key], new[] { recipe.Value });

						if (ModEntry.PrintRename && recipeData != null)
							Log.D(data.Where(pair => recipeData.ContainsKey(pair.Key))
									.Aggregate($"Edited {asset.AssetName}:", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
								ModEntry.Config.DebugMode);

					}

					// LOVE OF COOKING RECIPES
					// Basic Objects recipes:
					recipeData = new Dictionary<string, string>
					{
						// Hot Cocoa: Milk (Any) 1 Chocolate Bar 1
						{
							ModEntry.ObjectPrefix + "hotcocoa",
							"-6 1"
							+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.ChocolateName)} 1"
						},
					};
					// New Crops recipes:
					if (ModEntry.Config.AddNewCropsAndStuff || Interface.Interfaces.UsingPPJACrops) 
					{
						recipeData = recipeData.Concat(new Dictionary<string, string>
						{
							// Beet Burger: Bread 1 Beet 1 Onion 1 Red Cabbage 1
							{
								ModEntry.ObjectPrefix + "burger",
								"216 1 284 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.OnionName)} 1"
								+ " 266 1"
							},
							// Cabbage Pot: Cabbage 2 Onion 2
							{
								ModEntry.ObjectPrefix + "cabbagepot",
								$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.CabbageName)} 2"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.OnionName)} 2"
							},
							// Garden Pie: Flour 1 Cabbage 1 Onion 1 Tomato 1
							{
								ModEntry.ObjectPrefix + "gardenpie",
								"246 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.CabbageName)} 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.OnionName)} 1"
								+ " 256 1"
							},
							// Hearty Stew: Carrot 2 Potato 1
							{
								ModEntry.ObjectPrefix + "stew",
								$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.CarrotName)} 2"
								+ " 192 1"
							},
							// Hot Pot Roast: Cranberry Sauce 1 Roots Platter 1 Stuffing 1 Onion 1
							{
								ModEntry.ObjectPrefix + "roast",
								"238 1 244 1 239 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.OnionName)} 1"
							},
							// Hunter's Plate: Potato 1 Cabbage 1 Horseradish 1
							{
								ModEntry.ObjectPrefix + "hunters",
								"192 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.CabbageName)} 1"
								+ " 16 1"
							},
							// Kebab: Tortilla 1 Tomato 1 Cabbage 1
							{
								ModEntry.ObjectPrefix + "kebab",
								"229 1 256 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.CabbageName)} 1"
							},
							// Onion Soup: Onion 1 Garlic 1 Cheese 1
							{
								ModEntry.ObjectPrefix + "onionsoup",
								$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.OnionName)} 1"
								+ " 248 1 424 1"
							},
							// Pineapple Skewers: Pineapple 1 Onion 1 Eggplant 1
							{
								ModEntry.ObjectPrefix + "skewers",
								"832 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.OnionName)} 1"
								+ " 272 1"
							},
							// Redberry Pie: Flour 1 Sugar 1 Redberries 3
							/*{
								ModEntry.ObjectPrefix + "redberrypie",
								"246 1 245 1"
								+ $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry")} 3"
							},*/
							// Tropical Salad: Pineapple 1 Apple 1 Pomegranate 1
							{
								ModEntry.ObjectPrefix + "tropicalsalad",
								"832 1 613 1 637 1"
							},
						}).ToDictionary(pair => pair.Key, pair => pair.Value);
					}

					if (recipeData != null)
						foreach (KeyValuePair<string, string> recipe in recipeData.Where(r => data.ContainsKey(r.Key)))
							data[recipe.Key] = Utils.UpdateEntry(data[recipe.Key], new[] { recipe.Value });

					foreach (KeyValuePair<string, string> recipe in data.ToDictionary(pair => pair.Key, pair => pair.Value))
					{
						string[] recipeSplit = data[recipe.Key].Split('/');

						// Remove Oil from all cooking recipes in the game
						string[] ingredients = recipeSplit[0].Split(' ');
						if (!ingredients.Contains("247"))
							continue;

						recipeSplit[0] = Utils.UpdateEntry(recipeSplit[0],
							ingredients.Where((ingredient, i) => 
								ingredient != "247" && (i <= 0 || ingredients[i - 1] != "247")).ToArray(), 
							false, true, 0, ' ');
						data[recipe.Key] = Utils.UpdateEntry(data[recipe.Key], recipeSplit, false, true);
					}

					// Strip recipes with invalid, missing, or duplicate ingredients from the recipe data list
					Dictionary<string, string> badRecipes = data.Where(
						pair => pair.Value.Split('/')[0].Split(' ').ToList() is List<string> ingredients
							&& ingredients.Any(s =>
								(ingredients.IndexOf(s) % 2 == 0) is bool isItemId
								&& 
									// Missing ingredients 
									((isItemId && (s == "0" || s == "-1"))
									// Duplicate ingredients
									|| (isItemId && ingredients.Count(x => x == s) > 1)
									// Bad ingredient quantities
									|| (!isItemId && (!int.TryParse(s, out int i) || (i < 1 || i > 999))))))
						.ToDictionary(pair => pair.Key, pair => pair.Value);
					if (badRecipes.Count() > 0)
					{
						string str = badRecipes.Aggregate($"Removing {badRecipes.Count()} malformed recipes.\nThese recipes may use items from mods that aren't installed:",
							(str, cur) => $"{str}\n{cur.Key}: {cur.Value.Split('/')[0]}");
						if (Game1.activeClickableMenu is StardewValley.Menus.TitleMenu)
						{
							Log.D("At TitleMenu: " + str,
								ModEntry.Config.DebugMode);
						}
						else
						{
							Log.W(str);
						}
						foreach (string recipe in badRecipes.Keys)
						{
							data.Remove(recipe);
						}
					}

					asset.AsDictionary<string, string>().ReplaceWith(data);

					if (ModEntry.PrintRename)
					{
						if (recipeData != null)
						{
							Log.D(data.Where(pair => recipeData.ContainsKey(pair.Key))
									.Aggregate($"Edited {asset.AssetName}:", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
								ModEntry.Config.DebugMode);
						}
						Log.D(data.Aggregate("", (str, recipe) => $"{str}\n{recipe.Key}: {recipe.Value}"),
							ModEntry.Config.DebugMode);
					}
				}
				catch (Exception e) when (e is ArgumentException || e is NullReferenceException || e is KeyNotFoundException)
				{
					Log.E($"Did not patch {asset.AssetName}: {(!ModEntry.Config.DebugMode ? e.Message : e.ToString())}");
				}

				return;
			}

			if (asset.AssetNameEquals(@"Data/mail"))
			{
				var data = asset.AsDictionary<string, string>().Data;
				data.Add(ModEntry.MailCookbookUnlocked, i18n.Get("mail.cookbook_unlocked"));

				// lol pan
				string whoops = "Umm, hello @."
						+ $"^There was a mix-up at the forge with your {i18n.Get("menu.cooking_equipment.name")}."
							+ $" This is a bit embarrassing, so I'll return your materials as an apology."
						+ "^Come back to the shop and we'll see about getting you that upgrade."
						+ "^ - Clint, the blacksmith"
					+ "^^^                     $ 1000g"
					+ " %item object 334 5 %% [#] Love of Cooking Meta Menu Mix-Up";
				data.Add(ModEntry.MailFryingPanWhoops, whoops);

				asset.ReplaceWith(data);

				return;
			}
			if (asset.AssetNameEquals(@"Data/ObjectContextTags"))
			{
				var dict = Game1.content.Load
					<Dictionary<string, string>>
					(GameContentContextTagDataPath);
				var data = asset.AsDictionary<string, string>().Data;
				foreach (KeyValuePair<string, string> entry in dict)
				{
					data[ModEntry.ObjectPrefix + entry.Key] = entry.Value;
				}
				asset.AsDictionary<string, string>().ReplaceWith(data);

				return;
			}
			if (asset.AssetNameEquals(@"Data/ObjectInformation"))
			{
				// Edit fields of vanilla objects to revalue and recategorise some produce

				if (Interface.Interfaces.JsonAssets == null || ModEntry.IngredientBuffChart == null || Game1.currentLocation == null)
					return;

				var data = asset.AsDictionary<int, string>().Data;

				// Add localised names and descriptions for new objects
				foreach (KeyValuePair<int, string> pair in data.Where(pair => pair.Value.Split('/') is string[] split && split[0].StartsWith(ModEntry.ObjectPrefix)).ToList())
				{
					string[] name = pair.Value.Split('/')[0].Split(new [] { '.' }, 3);
					data[pair.Key] = Utils.UpdateEntry(data[pair.Key],
						new[] { i18n.Get($"item.{name[2]}.name").ToString(),
							i18n.Get($"item.{name[2]}.description").ToString() },
						false, false, 4);
					if (ModEntry.PrintRename)
						Log.D($"Named {name[2]} ({i18n.Get($"item.{name[2]}.name")})", ModEntry.Config.DebugMode);
				}
				asset.AsDictionary<int, string>().ReplaceWith(data);

				if (!ModEntry.Config.AddRecipeRebalancing)
				{
					Log.D($"Did not edit {asset.AssetName}: New recipe scaling is disabled in config file.",
						ModEntry.Config.DebugMode);
					return;
				}

				try
				{
					var objectData = new Dictionary<int, string[]>
					{
						{206, new[] {null, null, "45"}}, // Pizza
						{220, new[] {null, null, "60"}}, // Chocolate Cake
						{221, new[] {null, null, "75"}}, // Pink Cake
						{419, new[] {null, "220", "-300", "Basic -26"}}, // Vinegar
						{247, new[] {null, null, "-300", "Basic -26", null, i18n.Get("item.oil.description")}}, // Oil
						{432, new[] {null, null, "-300", null, null, i18n.Get("item.truffleoil.description")}}, // Truffle Oil
						{917, new[] {null, null, null, null, null, data[917].Split('/')[5].Split('.')[0] + '.'}}, // Qi Seasoning
						//{Interface.Interfaces.JsonAssets.GetObjectId(ObjectPrefix + "sugarcane"), new[] {null, null, null, "Basic"}},
					};
					
					// Apply above recipe changes
					foreach (KeyValuePair<int, string[]> obj in objectData.Where(o => !ModEntry.ItemDefinitions["FoodsThatGiveLeftovers"].Contains(data[o.Key].Split('/')[0])))
						data[obj.Key] = Utils.UpdateEntry(data[obj.Key], obj.Value);

					if (ModEntry.Config.AddRecipeRebalancing)
						this.RebuildBuffs(ref data);

					asset.AsDictionary<int, string>().ReplaceWith(data);

					if (ModEntry.PrintRename)
						Log.D($"Edited {asset.AssetName}:" + data.Where(pair => objectData.ContainsKey(pair.Key))
								.Aggregate("", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
							ModEntry.Config.DebugMode);
				}
				catch (Exception e) when (e is ArgumentException || e is NullReferenceException || e is KeyNotFoundException)
				{
					Log.D($"Did not patch {asset.AssetName}: {(!ModEntry.Config.DebugMode ? e.Message : e.ToString())}",
						ModEntry.Config.DebugMode);
				}

				return;
			}

			if (asset.AssetNameEquals(@"Data/Events/Town"))
			{
				var data = asset.AsDictionary<string, string>().Data;

				string key = data.Keys.FirstOrDefault(key => key.StartsWith("191393"));
				string value = data[key];
				data.Remove(key);
				data.Add("191393/n ccIsComplete/w sunny/H", value);
				
				asset.AsDictionary<string, string>().ReplaceWith(data);
			}

			if (asset.AssetNameEquals(@"Data/Monsters"))
			{
				if (Interface.Interfaces.JsonAssets == null || Game1.currentLocation == null)
					return;
				if (!ModEntry.Config.AddNewCropsAndStuff)
				{
					Log.D($"Did not edit {asset.AssetName}: New crops are disabled in config file.",
						ModEntry.Config.DebugMode);
					return;
				}
				if (!ModEntry.RedberriesEnabled)
				{
					Log.D($"Did not edit {asset.AssetName}: Redberries not yet enabled in code.",
						ModEntry.Config.DebugMode);
					return;
				}

				try
				{
					var data = asset.AsDictionary<string, string>().Data;
					var monsterData = new Dictionary<string, string[]>
					{
						{"Shadow Shaman", new[] {$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .0035"
							+ (Utils.AreNettlesActive() ? $" {Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.Instance.NettleName)} .05" : "")}},
						{"Wilderness Golem", new[] {$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .0065"}},
						{"Mummy", new[] {$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .0022"}},
						{"Pepper Rex", new[] {$"{Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .02"}},
					};
					foreach (KeyValuePair<string, string[]> monster in monsterData)
						data[monster.Key] = Utils.UpdateEntry(data[monster.Key], monster.Value, append: true);

					asset.AsDictionary<string, string>().ReplaceWith(data);

					if (ModEntry.PrintRename)
						Log.D($"Edited {asset.AssetName}:" + data.Where(pair => monsterData.ContainsKey(pair.Key))
								.Aggregate("", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
							ModEntry.Config.DebugMode);
				}
				catch (Exception e) when (e is ArgumentException || e is NullReferenceException || e is KeyNotFoundException)
				{
					Log.D($"Did not patch {asset.AssetName}: {(!ModEntry.Config.DebugMode ? e.Message : e.ToString())}",
						ModEntry.Config.DebugMode);
				}

				return;
			}

			if (asset.AssetNameEquals(@"LooseSprites/Cursors"))
			{
				// Home-cook a notification icon for under the HUD money tray:

				if (ModEntry.SpriteSheet == null)
					return;

				// Prime a canvas as a clipboard to hold in sequence both a copy of the vanilla icon
				// and our custom icon to merge together into some particular open space in Cursors
				var data = asset.AsImage().Data;
				Color[] canvas = new Color[NotificationIconTargetArea.Width * NotificationIconTargetArea.Height];
				Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, NotificationIconTargetArea.Width, NotificationIconTargetArea.Height);
				Rectangle vanillaIconArea = new Rectangle(383, 493, NotificationIconTargetArea.Width, NotificationIconTargetArea.Height);
				Rectangle targetArea = NotificationIconTargetArea;

				// Patch in a copy of the vanilla quest log icon
				data.GetData(0, vanillaIconArea, canvas, 0, canvas.Length);
				texture.SetData(canvas);
				asset.AsImage().PatchImage(texture, null, targetArea, PatchMode.Replace);

				// Chroma-key our custom icon with colours from the vanilla icon
				Color colorSampleA = canvas[NotificationIconTargetArea.Width * 5 + 1];
				Color colorSampleB = canvas[NotificationIconTargetArea.Width * 11 + 1];

				Color colorR = new Color(255, 0, 0);
				Color colorC = new Color(255, 0, 255);
				Color colorG = new Color(0, 255, 0);
				Color colorA = new Color(0, 0, 0, 0);

				ModEntry.SpriteSheet.GetData(0, new Rectangle(0, 0, NotificationIconTargetArea.Width, NotificationIconTargetArea.Height), canvas, 0, canvas.Length);

				for (int i = 0; i < canvas.Length; ++i)
				{
					if (canvas[i] == colorC)
						canvas[i] = colorA;
					else if (canvas[i] == colorG)
						canvas[i] = colorSampleA;
					else if (canvas[i] == colorR)
						canvas[i] = colorSampleB;
				}

				// Patch in the custom icon over the vanilla icon copy
				texture.SetData(canvas);
				asset.AsImage().PatchImage(texture, null, targetArea, PatchMode.Overlay);

				// Patch in an alpha-shaded copy of the custom icon to use for the pulse animation
				Color colorShade = new Color(0, 0, 0, 0.35f);

				for (int i = 0; i < canvas.Length; ++i)
				{
					if (canvas[i] == colorSampleB)
						canvas[i] = colorShade;
					else if (canvas[i] == colorSampleA)
						canvas[i] = colorA;
				}

				// Apply changes to the Cursors sheet
				texture.SetData(canvas);
				targetArea.X -= targetArea.Width;
				asset.AsImage().PatchImage(texture, null, targetArea, PatchMode.Overlay);
			}
			else if (asset.AssetNameEquals(@"LooseSprites/JunimoNote"))
			{
				// Add icons for a new community centre bundle
				
				if (!ModEntry.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						ModEntry.Config.DebugMode);
					return;
				}

				Rectangle sourceArea = new Rectangle(160, 208, 32 * 3, 32);
				Rectangle destArea = new Rectangle(544, 212, 32 * 3, 32);
				IAssetDataForImage destImage = asset.AsImage();
				destImage.PatchImage(ModEntry.SpriteSheet, sourceArea, destArea, PatchMode.Replace);
				asset.ReplaceWith(destImage.Data);
			}
			else if (asset.AssetNameEquals(@"Maps/townInterior"))
			{
				// Patch in changes for the community centre

				if (!(Game1.currentLocation is StardewValley.Locations.CommunityCenter))
					return;

				var image = asset.AsImage();

				// Openable fridge in the kitchen
				Rectangle destArea = Bundles.FridgeOpenedSpriteArea; // Target some unused area of the sheet for this location

				Rectangle sourceArea = new Rectangle(320, 224, destArea.Width, destArea.Height); // Apply base fridge sprite
				image.PatchImage(image.Data, sourceArea, destArea, PatchMode.Replace);

				sourceArea = new Rectangle(0, 192, 16, 32); // Patch in opened-door fridge sprite from mouseCursors sheet
				image.PatchImage(Game1.mouseCursors2, sourceArea, destArea, PatchMode.Overlay);

				// New star on the community centre bundle tracker wall
				if (!ModEntry.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						ModEntry.Config.DebugMode);
				}
				else
				{
					sourceArea = new Rectangle(370, 705, 7, 7);
					destArea = new Rectangle(380, 710, sourceArea.Width, sourceArea.Height);
					image.PatchImage(image.Data, sourceArea, destArea, PatchMode.Replace);
				}

				asset.ReplaceWith(image.Data);
			}
			else if (asset.AssetNameEquals(@"Strings/Locations"))
			{
				// Make changes to facilitate a new community centre bundle

				if (!ModEntry.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						ModEntry.Config.DebugMode);
					return;
				}

				var data = asset.AsDictionary<string, string>().Data;

				// Add area name
				data["CommunityCenter_AreaName_" + Bundles.CommunityCentreAreaName] = i18n.Get("world.community_centre.kitchen");

				// Insert a new AreaCompletion line to account for our extra bundle
				const int newJunimoLineNumber = 3;
				for (int i = Bundles.CommunityCentreAreaNumber + 1; i > newJunimoLineNumber; --i)
				{
					string below = data["CommunityCenter_AreaCompletion" + (i - 1)];
					data["CommunityCenter_AreaCompletion" + i] = below;
				}
				data["CommunityCenter_AreaCompletion" + newJunimoLineNumber] = i18n.Get("world.community_centre.newjunimoline");

				asset.AsDictionary<string, string>().ReplaceWith(data);
			}
			else if (asset.AssetNameEquals(@"Strings/UI"))
			{
				// Make changes to facilitate a new community centre bundle

				if (!ModEntry.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						ModEntry.Config.DebugMode);
					return;
				}

				var data = asset.AsDictionary<string, string>().Data;
				data["JunimoNote_Reward" + Bundles.CommunityCentreAreaName] = i18n.Get("world.community_centre.reward");
				asset.AsDictionary<string, string>().ReplaceWith(data);
			}
			else if (asset.AssetNameEquals(@"TileSheets/tools"))
			{
				// Patch in tool sprites for cooking equipment

				if (ModEntry.SpriteSheet == null)
					return;
				
				if (!ModEntry.Config.AddCookingToolProgression)
				{
					Log.D($"Did not edit {asset.AssetName}: Cooking equipment is disabled in config file.",
						ModEntry.Config.DebugMode);
					return;
				}

				var destImage = asset.AsImage();
				Rectangle sourceArea = new Rectangle(192, 272, 16 * 4, 16);
				Rectangle destArea = new Rectangle(272, 0, sourceArea.Width, sourceArea.Height);
				destImage.PatchImage(ModEntry.SpriteSheet, sourceArea, destArea, PatchMode.Replace);
				asset.ReplaceWith(destImage.Data);
			}
		}

		private void RebuildBuffs(ref IDictionary<int, string> data)
		{
			// Reconstruct buffs of all cooking items in the game using our ingredients-to-buffs chart
			var cookingRecipes = Game1.content.Load
				<Dictionary<string, string>>
				(@"Data/CookingRecipes");
			int[] keys = new int[data.Keys.Count];
			data.Keys.CopyTo(keys, 0);
			foreach (int key in keys)
			{
				string[] objectSplit = data[key].Split('/');
				if (!objectSplit[3].Contains(ModEntry.CookingCategory.ToString())
				    || !cookingRecipes.ContainsKey(objectSplit[0]) // v-- Json Assets custom recipe convention for unused fields
				    || cookingRecipes[objectSplit[0]].Split('/')[1].StartsWith("what"))
					continue;
				string[] ingredients = cookingRecipes[objectSplit[0]].Split('/')[0].Split(' ');
				string[] buffArray = new string[_buffIndex.Values.Max() + 1];
				for (int i = 0; i < buffArray.Length; ++i)
					buffArray[i] = "0";
				int buffDuration = 0;

				// Populate buff values using ingredients for this object in CookingRecipes
				for (int i = 0; i < ingredients.Length; i += 2)
				{
					Object o = new Object(int.Parse(ingredients[i]), 0);
					string[] buffSplit = ModEntry.IngredientBuffChart.ContainsKey(o.Name)
						? ModEntry.IngredientBuffChart[o.Name].Split(' ')
						: null;
					if (o.ParentSheetIndex >= 2000)
					{
						// Skip custom items
						break;
					}
					else
					{
						// Reapply buffs at random, seeded by their name, ensuring buffs are consistent to foods between games
						Random random = new Random(1337 + o.Name.GetHashCode());
						// Each ingredient has a roughly 1 in 6 chance of having some buff
						if (random.NextDouble() < 0.175f)
						{
							// Buff keys are taken from valid values listed in BuffIndex, and values range from 0 to 3
							buffSplit = new[] {
								_buffIndex.Keys.ToArray()[random.Next(_buffIndex.Count)],
								random.Next(4).ToString()
							};
						}
					}
					if (buffSplit == null)
						continue;
					for (int j = 0; j < buffSplit.Length; j += 2)
					{
						string buffName = buffSplit[j];
						int buffValue = int.Parse(buffSplit[j + 1]);
						if (buffName == "Edibility")
						{
							objectSplit[1] =
								(int.Parse(objectSplit[1]) + o.Edibility / 8 * buffValue).ToString();
						}
						else
						{
							buffArray[_buffIndex[buffName]] =
								(int.Parse(buffArray[_buffIndex[buffName]])
									+ (buffName == "Energy"
										? buffValue * 10 + 20
										: buffName == "Magnetism"
											? buffValue * 16 + 16
											: buffValue)).ToString();
						}
					}
					buffDuration += 4 * o.Price + Math.Max(0, o.Edibility);
				}

				buffDuration -= (int)(int.Parse(objectSplit[2]) * 0.15f);
				buffDuration = Math.Min(1600, Math.Max(300, (buffDuration / 10) * 10));

				string[] newData;
				{
					string newBuffs = buffArray.Aggregate((entry, field)
						=> $"{entry} {field}").Remove(0, 0);
					newData = new string[9];
					objectSplit.CopyTo(newData, 0);
					newData[6] ??= "food";
					if (ModEntry.Config.AddBuffReassigning)
					{
						newData[7] = newBuffs;
					}
					newData[8] = buffDuration.ToString();
				}

				data[key] = Utils.UpdateEntry(data[key], newData);
			}
		}
	}
}
