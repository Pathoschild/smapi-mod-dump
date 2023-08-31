/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LoveOfCooking.Interface
{
	internal static class Interfaces
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IManifest ModManifest => ModEntry.Instance.ModManifest;
		private static ITranslationHelper i18n => Helper.Translation;

		private static bool IsLoaded;
		private static double TotalSecondsOnLoaded;

		// Loaded APIs
		internal static IJsonAssetsApi JsonAssets;
		internal static ILevelExtenderAPI LevelExtender;
		internal static IManaBarAPI ManaBar;

		// Loaded mods
		internal static bool UsingSVE;
		internal static bool UsingPPJACrops;
		internal static bool UsingPPJATreesAndRecipes;
		internal static bool UsingCustomCC;
		internal static bool UsingNettlesCrops;
		internal static bool UsingManaBar;
		internal static bool UsingLevelExtender;
		internal static bool UsingBigBackpack;
		internal static bool UsingFarmhouseKitchenStart;


		/// <summary>
		/// Perform first-time checks for mod-provided APIs used for registering events and hooks, or adding custom content.
		/// </summary>
		/// <returns>Whether mod-provided APIs were initialised without issue.</returns>
		internal static bool Init()
		{
			try
			{
				if (!IsLoaded)
				{
					IdentifyLoadedOptionalMods();

					JsonAssets = Helper.ModRegistry
						.GetApi<IJsonAssetsApi>
						("spacechase0.JsonAssets");
					if (JsonAssets is null)
					{
						Log.E("Can't access the Json Assets API. Is the mod installed correctly?");
						return false;
					}
				}
				return true;
			}
			catch (Exception e)
			{
				Log.E($"Failed to initialise mod-provided APIs:{Environment.NewLine}{e}");
				return false;
			}
		}

		/// <summary>
		/// Load content only once from available mod-provided APIs.
		/// </summary>
		/// <returns>Whether assets have been successfully loaded.</returns>
		internal static bool Load()
		{
			try
			{
				if (!IsLoaded)
				{
					LoadCustomCommunityCentreContent();
					IsLoaded = LoadSpaceCoreAPI()
						&& LoadJsonAssetsObjects()
						&& LoadModConfigMenuElements()
						&& LoadLevelExtenderApi();
				}
				return IsLoaded;
			}
			catch (Exception e)
			{
				Log.E($"Failed to load content from mod-provided APIs:{Environment.NewLine}{e}");
				return false;
			}
		}

		private static void IdentifyLoadedOptionalMods()
		{
			ManaBar = Helper.ModRegistry
				.GetApi<IManaBarAPI>
				("spacechase0.ManaBar");
			UsingManaBar = ManaBar is not null;

			UsingSVE = Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP");
			UsingPPJACrops = Helper.ModRegistry.IsLoaded("PPJA.FruitsAndVeggies");
			UsingPPJATreesAndRecipes = Helper.ModRegistry.IsLoaded("paradigmnomad.morefood");
			UsingCustomCC = Helper.ModRegistry.IsLoaded("blueberry.CustomCommunityCentre");
			UsingNettlesCrops = Helper.ModRegistry.IsLoaded("uberkwefty.wintercrops");
			UsingLevelExtender = Helper.ModRegistry.IsLoaded("Devin_Lematty.Level_Extender");
			UsingBigBackpack = Helper.ModRegistry.IsLoaded("spacechase0.BiggerBackpack");
			UsingFarmhouseKitchenStart = new string[]
			{
				"Allayna.Kitchen",
				"Froststar11.CustomFarmhouse",
				"burakmese.products",
				"minervamaga.FR.BiggerFarmhouses"
			}
			.Any(id => Helper.ModRegistry.IsLoaded(id));
		}

		internal static void SaveLoadedBehaviours()
		{
			// Attempt to register Level Extender compatibility
			if (LevelExtender is not null)
			{
				TotalSecondsOnLoaded = Game1.currentGameTime.TotalGameTime.TotalSeconds;
				Helper.Events.GameLoop.OneSecondUpdateTicked += Event_RegisterLevelExtenderLate;
			}
		}

		private static void Event_RegisterLevelExtenderLate(object sender, OneSecondUpdateTickedEventArgs e)
		{
			// LevelExtender/LEModApi.cs:
			// Please [initialise skill] ONCE in the Save Loaded event (to be safe, PLEASE ADD A 5 SECOND DELAY BEFORE initialization)
			if (Game1.currentGameTime.TotalGameTime.TotalSeconds - TotalSecondsOnLoaded >= 5)
			{
				Helper.Events.GameLoop.OneSecondUpdateTicked -= Event_RegisterLevelExtenderLate;
				RegisterSkillsWithLevelExtender();
			}
		}

		private static bool LoadSpaceCoreAPI()
		{
			ISpaceCoreAPI spaceCore = Helper.ModRegistry
				.GetApi<ISpaceCoreAPI>
				("spacechase0.SpaceCore");
			if (spaceCore is null)
			{
				Log.E("Can't access the SpaceCore API. Is the mod installed correctly?");
				return false;
			}

			spaceCore.RegisterSerializerType(type: typeof(Objects.CookingTool));
			spaceCore.RegisterSerializerType(type: typeof(CustomBush));

			return true;
		}

		private static void LoadCustomCommunityCentreContent()
		{
			ICustomCommunityCentreAPI ccc = Helper.ModRegistry
				.GetApi<ICustomCommunityCentreAPI>
				("blueberry.CustomCommunityCentre");
			if (UsingCustomCC && ccc is not null && Utils.AreNewCropsActive())
			{
				Log.D("Registering CustomCommunityCentre content.",
					ModEntry.Config.DebugMode);
				ccc.LoadContentPack(absoluteDirectoryPath: Path.Combine(Helper.DirectoryPath, AssetManager.CommunityCentreContentPackPath));
			}
			else
            {
				Log.D("Did not register CustomCommunityCentre content.",
					ModEntry.Config.DebugMode);
			}
		}

		private static bool LoadJsonAssetsObjects()
		{
			if (ModEntry.Config.DebugMode)
				Log.W("Loading Basic Objects Pack.");
			JsonAssets.LoadAssets(path: Path.Combine(Helper.DirectoryPath, AssetManager.BasicObjectsPackPath));

			if (!ModEntry.Config.AddCookingSkillAndRecipes)
			{
				Log.W("Did not add new recipes: Recipe additions are disabled in config file.");
			}
			else
			{
				if (ModEntry.Config.DebugMode)
					Log.W("Loading New Recipes Pack.");
				JsonAssets.LoadAssets(path: Path.Combine(Helper.DirectoryPath, AssetManager.NewRecipesPackPath));
			}

			if (!ModEntry.Config.AddNewCropsAndStuff)
			{
				Log.W("Did not add new objects: New stuff is disabled in config file.");
			}
			else if (UsingPPJACrops)
			{
				Log.I("Did not add new crops: [PPJA] Fruits and Veggies already adds these objects.");
			}
			else
			{
				if (ModEntry.Config.DebugMode)
					Log.W("Loading New Crops Pack.");
				JsonAssets.LoadAssets(path: Path.Combine(Helper.DirectoryPath, AssetManager.NewCropsPackPath));
			}

			if (UsingNettlesCrops)
			{
				Log.I("Did not add nettles: Other mods already add these items.");
			}
			else if (!Utils.AreNettlesActive())
			{
				Log.I("Did not add nettles: Currently disabled in code.");
			}
			else
			{
				if (ModEntry.Config.DebugMode)
					Log.W("Loading Nettles Pack.");
				JsonAssets.LoadAssets(path: Path.Combine(Helper.DirectoryPath, AssetManager.NettlesPackPath));
			}
			return true;
		}

		private static bool LoadModConfigMenuElements()
		{
			IGenericModConfigMenuAPI gmcm = Helper.ModRegistry
				.GetApi<IGenericModConfigMenuAPI>
				("spacechase0.GenericModConfigMenu");
			if (gmcm is null)
			{
				return true;
			}

			gmcm.Register(
				mod: ModEntry.Instance.ModManifest,
				reset: () => ModEntry.Config = new Config(),
				save: () => Helper.WriteConfig(ModEntry.Config));
			gmcm.OnFieldChanged(
				mod: ModManifest,
				onChange: (string key, object value) =>
				{
					Log.D($"Config check: {key} => {value}",
						ModEntry.Config.DebugMode);

					if (key == i18n.Get($"config.option.{"AddNewCropsAndStuff".ToLower()}_name")
						&& Helper.ModRegistry.IsLoaded("blueberry.CommunityKitchen"))
                    {
						// Show warning when using 
						Log.W("Changes to Community Kitchen bundles won't be applied until you reopen the game.");
                    }
				});

			string[] entries = new[]
			{
				"features",

				"AddCookingMenu",
				"AddCookingSkillAndRecipes",
				"AddCookingToolProgression",
				//"AddCookingQuestline",
				"AddNewCropsAndStuff",

				"changes",

				"PlayCookingAnimation",
				"AddRecipeRebalancing",
				"AddBuffReassigning",
				"HideFoodBuffsUntilEaten",
				"FoodHealingTakesTime",
				"FoodCanBurn",

				"others",

				"ShowFoodRegenBar",
				"RememberLastSearchFilter",
				"DefaultSearchFilter",
				"ResizeKoreanFonts",
			};
			foreach (string entry in entries)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
				PropertyInfo property = typeof(Config).GetProperty(entry, flags);
				if (property is not null)
				{
					string i18nKey = $"config.option.{entry.ToLower()}_";
					if (property.PropertyType == typeof(bool))
					{
						gmcm.AddBoolOption(
							mod: ModManifest,
							name: () => i18n.Get(i18nKey + "name"),
							tooltip: () => i18n.Get(i18nKey + "description"),
							getValue: () => (bool)property.GetValue(ModEntry.Config),
							setValue: (bool value) =>
							{
								Log.D($"Config edit: {property.Name} - {property.GetValue(ModEntry.Config)} => {value}",
									ModEntry.Config.DebugMode);
								property.SetValue(ModEntry.Config, value);
							});
					}
					else if (property.Name == "DefaultSearchFilter")
					{
						gmcm.AddTextOption(
							mod: ModManifest,
							name: () => i18n.Get(i18nKey + "name"),
							tooltip: () => i18n.Get(i18nKey + "description"),
							getValue: () => (string)property.GetValue(ModEntry.Config),
							setValue: (string value) => property.SetValue(ModEntry.Config, value),
							allowedValues: Enum.GetNames(typeof(Objects.CookingMenu.Filter)));
					}
				}
				else
				{
					string i18nKey = $"config.{entry}_";
					gmcm.AddSectionTitle(
						mod: ModManifest,
						text: () => i18n.Get(i18nKey + "label"));
				}
			}
			return true;
		}

		private static bool LoadLevelExtenderApi()
		{
			if (UsingLevelExtender)
			{
				try
				{
					LevelExtender = Helper.ModRegistry
						.GetApi<ILevelExtenderAPI>
						("Devin_Lematty.Level_Extender");
				}
				catch (Exception e)
				{
					Log.T("Encountered exception in reading ILevelExtenderAPI from LEApi:");
					Log.T("" + e);
				}
				finally
				{
					if (LevelExtender is null)
					{
						Log.W("Level Extender is loaded, but the API was inaccessible.");
					}
				}
			}
			return true;
		}

		private static void RegisterSkillsWithLevelExtender()
		{
			LevelExtender.initializeSkill(
				name: Objects.CookingSkill.InternalName,
				xp: ModEntry.CookingSkillApi.GetTotalCurrentExperience(),
				xp_mod: float.Parse((ModEntry.ItemDefinitions)["CookingSkillExperienceGlobalScaling"][0]),
				xp_table: ModEntry.CookingSkillApi.GetSkill().ExperienceCurve.ToList(),
				cats: null);
		}

		internal static bool IsManaBarReadyToDraw(Farmer who)
		{
			int mana = ManaBar.GetMana(farmer: who);
			int maxMana = ManaBar.GetMaxMana(farmer: who);
			bool gameFlags = Context.IsWorldReady && Game1.activeClickableMenu is null && !Game1.eventUp;
			bool manaFlags = mana > 0 && maxMana > 0 && mana < maxMana;
			return gameFlags && manaFlags;
		}

		internal static StardewValley.Objects.Chest GetCommunityCentreFridge(StardewValley.Locations.CommunityCenter cc)
        {
			StardewValley.Objects.Chest chest = null;

			Type kitchen = Type.GetType("CommunityKitchen.Kitchen, CommunityKitchen");
			if (kitchen is not null)
            {
				chest = Helper.Reflection
					.GetMethod(type: kitchen, name: "GetKitchenFridge")
					.Invoke<StardewValley.Objects.Chest>(cc);
            }

			return chest;
        }

		internal static Type GetMod_RemoteFridgeStorage()
		{
			Type mod = Type.GetType("RemoteFridgeStorage.ModEntry, RemoteFridgeStorage");
			if (mod is null && Helper.ModRegistry.IsLoaded("EternalSoap.RemoteFridgeStorage"))
			{
				Log.E("Unable to load Remote Fridge Storage: one or both of these mods is now incompatible."
					  + "\nChests will not be usable from the cooking page.");
			}
			return mod;
		}

		internal static Type GetMod_ConvenientChests()
		{
			Type mod = Type.GetType("ConvenientChests.ModEntry, ConvenientChests");
			if (mod is null && Helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests"))
			{
				Log.E("Unable to load Convenient Chests: one or both of these mods is now incompatible."
					  + "\nChests will not be usable from the cooking page.");
			}
			return mod;
		}
	}
}
