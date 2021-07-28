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
		private static IModHelper Helper;
		private static IManifest ModManifest;
		private static ITranslationHelper i18n => Helper.Translation;

		private static double TotalSecondsOnLoaded;

		// Loaded APIs
		internal static IJsonAssetsApi JsonAssets;
		internal static ILevelExtenderAPI LevelExtender;

		// Loaded mods
		internal static bool UsingSVE;
		internal static bool UsingPPJACrops;
		internal static bool UsingNettlesCrops;
		internal static bool UsingLevelExtender;
		internal static bool UsingBigBackpack;
		internal static bool UsingFarmhouseKitchenStart;


		internal static void Init(IModHelper helper, IManifest manifest)
		{
			Helper = helper;
			ModManifest = manifest;
			GetLoadedMods();
		}

		internal static void LoadInterfaces()
		{
			LoadJsonAssetsObjects();
			LoadProducerFrameworkRules();
			LoadModConfigMenuElements();
			LoadLevelExtenderApi();
		}

		private static void GetLoadedMods()
		{
			UsingSVE = Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP");
			UsingPPJACrops = Helper.ModRegistry.IsLoaded("PPJA.FruitsAndVeggies");
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

		internal static void RegisterEvents()
		{
			Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
		}

		internal static void SaveLoadedBehaviours()
		{
			// Attempt to register Level Extender compatibility
			if (LevelExtender != null)
			{
				TotalSecondsOnLoaded = Game1.currentGameTime.TotalGameTime.TotalSeconds;
				Helper.Events.GameLoop.OneSecondUpdateTicked += Event_RegisterLevelExtenderLate;
			}
		}

		private static void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			LoadSpaceCoreAPI();

			// Entoarox Framework save serialiser overrides SpaceCore save serialiser and causes errors when saving
			// with registered [XmlType] objects (eg. CustomBush) in the world.
			const string entoSafeVersion = "2.5.4";
			if (Helper.ModRegistry.IsLoaded("Entoarox.EntoaroxFramework")
				&& Helper.ModRegistry.Get("Entoarox.EntoaroxFramework") is IModInfo entoInfo
				&& entoInfo.Manifest.Version.IsOlderThan(entoSafeVersion))
			{
				ISemanticVersion entoCurrentVersion = entoInfo.Manifest.Version;
				Log.W("This version of Entoarox Framework doesn't allow for persistent custom bushes."
					+ "\nNettles will be cleared at the end of each day to prevent errors."
					+ "\nIf you have a save with nettle bushes, you will need to remove Ento " + entoCurrentVersion + " to load the game."
					+ "\nCheck your SMAPI console for an update notice to Ento " + entoSafeVersion + " or higher.");
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

		private static void LoadSpaceCoreAPI()
		{
			ISpaceCoreAPI spaceCore = Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");
			spaceCore.RegisterSerializerType(typeof(CustomBush));
		}

		private static void LoadJsonAssetsObjects()
		{
			JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
			if (JsonAssets == null)
			{
				Log.E("Can't access the Json Assets API. Is the mod installed correctly?");
				return;
			}

			if (ModEntry.Config.DebugMode)
				Log.W("Loading Basic Objects Pack.");
			JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, AssetManager.BasicObjectsPack));

			if (!ModEntry.Config.AddCookingSkillAndRecipes)
			{
				Log.W("Did not add new recipes: Recipe additions are disabled in config file.");
			}
			else
			{
				if (ModEntry.Config.DebugMode)
					Log.W("Loading New Recipes Pack.");
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, AssetManager.NewRecipesPackPath));
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
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, AssetManager.NewCropsPackPath));
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
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, AssetManager.NettlesPackPath));
			}
		}

		private static void LoadProducerFrameworkRules()
		{
			if (!ModEntry.PFMEnabled)
				return;

			IProducerFrameworkAPI producerFramework = Helper.ModRegistry.GetApi<IProducerFrameworkAPI>("DIGUS.ProducerFrameworkMod");
			if (producerFramework == null)
			{
				Log.E("Can't access the Producer Framework API. Is the mod installed correctly?");
				return;
			}

			if (ModEntry.Config.DebugMode)
				Log.W("Loading Producer Framework Pack.");

			producerFramework.AddContentPack(Path.Combine(Helper.DirectoryPath, AssetManager.ProducerFrameworkPackPath));
		}

		private static void LoadModConfigMenuElements()
		{
			IGenericModConfigMenuAPI gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (gmcm == null)
			{
				return;
			}

			gmcm.RegisterModConfig(ModEntry.Instance.ModManifest, () => ModEntry.Config = new Config(), () => Helper.WriteConfig(ModEntry.Config));

			string[] entries = new[]
			{
				"features",

				"AddCookingMenu",
				"AddCookingCommunityCentreBundles",
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

				"RememberLastSearchFilter",
				"DefaultSearchFilter",
				"ResizeKoreanFonts",
			};
			foreach (string entry in entries)
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
				PropertyInfo property = typeof(Config).GetProperty(entry, flags);
				if (property != null)
				{
					string i18nKey = $"config.option.{entry.ToLower()}_";
					if (property.PropertyType == typeof(bool))
						gmcm.RegisterSimpleOption(
							ModManifest,
							optionName: i18n.Get(i18nKey + "name"),
							optionDesc: i18n.Get(i18nKey + "description"),
							optionGet: () => (bool)property.GetValue(ModEntry.Config),
							optionSet: (bool value) => property.SetValue(ModEntry.Config, value));
					else if (property.Name == "DefaultSearchFilter")
						gmcm.RegisterChoiceOption(
							ModManifest,
							optionName: i18n.Get(i18nKey + "name"),
							optionDesc: i18n.Get(i18nKey + "description"),
							optionGet: () => (string)property.GetValue(ModEntry.Config),
							optionSet: (string value) => property.SetValue(ModEntry.Config, value),
							choices: Enum.GetNames(typeof(Objects.CookingMenu.Filter)));
				}
				else
				{
					string i18nKey = $"config.{entry}_";
					gmcm.RegisterLabel(
						ModManifest,
						labelName: i18n.Get(i18nKey + "label"),
						labelDesc: null);
				}
			}
		}

		private static void LoadLevelExtenderApi()
		{
			if (UsingLevelExtender)
			{
				try
				{
					LevelExtender = Helper.ModRegistry.GetApi<ILevelExtenderAPI>("Devin_Lematty.Level_Extender");
				}
				catch (Exception e)
				{
					Log.T("Encountered exception in reading ILevelExtenderAPI from LEApi:");
					Log.T("" + e);
				}
				finally
				{
					if (LevelExtender == null)
					{
						Log.W("Level Extender is loaded, but the API was inaccessible.");
					}
				}
			}
		}

		private static void RegisterSkillsWithLevelExtender()
		{
			LevelExtender.initializeSkill(
				name: Objects.CookingSkill.InternalName,
				xp: ModEntry.CookingSkillApi.GetTotalCurrentExperience(),
				xp_mod: Objects.CookingSkill.GlobalExperienceRate,
				xp_table: ModEntry.CookingSkillApi.GetSkill().ExperienceCurve.ToList(), cats: null);
		}

		internal static Type GetMod_RemoteFridgeStorage()
		{
			Type mod = Type.GetType("RemoteFridgeStorage.ModEntry, RemoteFridgeStorage");
			if (mod == null && Helper.ModRegistry.IsLoaded("EternalSoap.RemoteFridgeStorage"))
			{
				Log.E("Unable to load Remote Fridge Storage: one or both of these mods is now incompatible."
					  + "\nChests will not be usable from the cooking page.");
			}
			return mod;
		}

		internal static Type GetMod_ConvenientChests()
		{
			Type mod = Type.GetType("ConvenientChests.ModEntry, ConvenientChests");
			if (mod == null && Helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests"))
			{
				Log.E("Unable to load Convenient Chests: one or both of these mods is now incompatible."
					  + "\nChests will not be usable from the cooking page.");
			}
			return mod;
		}
	}
}
