/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces;
using Archery.Framework.Interfaces.Internal;
using Archery.Framework.Managers;
using Archery.Framework.Models;
using Archery.Framework.Models.Enums;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Weapons;
using Archery.Framework.Patches.Characters;
using Archery.Framework.Patches.Locations;
using Archery.Framework.Patches.Objects;
using Archery.Framework.Patches.Renderer;
using Archery.Framework.Utilities;
using Archery.Framework.Utilities.Backport;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Archery
{
    public class Archery : Mod
    {
        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static IManifest manifest;
        internal static Multiplayer multiplayer;

        // Managers
        internal static ApiManager apiManager;
        internal static AssetManager assetManager;
        internal static ConditionManager conditionManager;
        internal static ModelManager modelManager;

        // Utilities
        internal static Api internalApi;
        internal static bool shouldShowAmmoCollisionBox;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and manifest
            monitor = Monitor;
            modHelper = helper;
            manifest = ModManifest;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Load managers
            apiManager = new ApiManager(monitor);
            assetManager = new AssetManager(modHelper);
            conditionManager = new ConditionManager(modHelper);
            modelManager = new ModelManager(monitor);

            // Load internal API
            internalApi = new Api(monitor, modHelper);

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(ModManifest.UniqueID);

                // Apply Location patches
                new GameLocationPatch(monitor, modHelper).Apply(harmony);

                // Apply Crafting patches
                new CraftingRecipePatch(monitor, modHelper).Apply(harmony);
                new CraftingPagePatch(monitor, modHelper).Apply(harmony);

                // Apply Object patches
                new ItemPatch(monitor, modHelper).Apply(harmony);
                new ObjectPatch(monitor, modHelper).Apply(harmony);
                new ToolPatch(monitor, modHelper).Apply(harmony);
                new SlingshotPatch(monitor, modHelper).Apply(harmony);

                // Apply Character patches
                new FarmerPatch(monitor, modHelper).Apply(harmony);
                new DrawPatch(monitor, modHelper).Apply(harmony);

                // Apply Menu patches
                new ShopMenuPatch(monitor, modHelper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add in our debug commands
            helper.ConsoleCommands.Add("archery_get_bow", "Gives a random bow, unless a bow id is given.\n\nUsage: archery_get_bow [bow_id]", Toolkit.GiveBow);
            helper.ConsoleCommands.Add("archery_get_arrow", "Gives a random arrow, unless an arrow id is given.\n\nUsage: archery_get_arrow [arrow_id]", Toolkit.GiveArrow);
            helper.ConsoleCommands.Add("archery_list_ids", "Lists all weapon or ammo ids.\n\nUsage: archery_list_ids [WEAPON | AMMO]", Toolkit.DisplayIds);
            helper.ConsoleCommands.Add("archery_arena", "Teleports to an arena for debugging bows.\n\nUsage: archery_arena", Toolkit.TeleportToArena);
            helper.ConsoleCommands.Add("archery_reload", "Reloads all Archery content packs. Can specify a manifest unique ID to only reload that pack.\n\nUsage: archery_reload [manifest_unique_id]", ReloadArchery);

            // Hook into the game events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        public override object GetApi()
        {
            return internalApi;
        }

        private void OnButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.F2)
            {
                shouldShowAmmoCollisionBox = !shouldShowAmmoCollisionBox;
            }
        }

        private void OnUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                conditionManager.Update();
            }
        }

        private void OnAssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    // Add the valid recipes
                    foreach (var model in modelManager.GetAllModels().Where(m => m.Recipe is not null && m.Recipe.IsValid()))
                    {
                        data[model.Id] = model.Recipe.GetData();
                    }
                });
            }
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("PeacefulEnd.FashionSense") && apiManager.HookIntoFashionSense(Helper))
            {
                apiManager.GetFashionSenseApi().SetSpriteDirtyTriggered += OnVanillaRecolorMethodTriggered;

                apiManager.GetFashionSenseApi().RegisterAppearanceDrawOverride(IFashionSenseApi.Type.Sleeves, ModManifest, Bow.Draw);
            }

            if (Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets") && apiManager.HookIntoDynamicGameAssets(Helper))
            {
                // Do nothing
            }

            if (Helper.ModRegistry.IsLoaded("leclair.bettercrafting") && apiManager.HookIntoBetterCrafting(Helper))
            {
                // Do nothing
            }

            if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets") && apiManager.HookIntoJsonAssets(Helper))
            {
                // Do nothing
            }

            // Register the native special attacks from our framework
            apiManager.RegisterNativeSpecialAttacks();

            // Register the native enchantments from our framework
            apiManager.RegisterNativeEnchantments();

            // Load any owned content packs
            LoadContentPacks();
        }

        private void OnVanillaRecolorMethodTriggered(object sender, EventArgs e)
        {
            RecolorSleeves();
        }

        internal static void RecolorSleeves()
        {
            assetManager.recoloredArmsTexture = RendereringHelper.RecolorBowArms(Game1.player, assetManager.baseArmsTexture);
            assetManager.recoloredCrossbowArmsTexture = RendereringHelper.RecolorBowArms(Game1.player, assetManager.crossbowArmsTexture);
            foreach (WeaponModel model in modelManager.GetAllModels().Where(m => m is WeaponModel weaponModel))
            {
                if (model.ArmsTexture is not null)
                {
                    model.RecoloredArmsTexture = RendereringHelper.RecolorBowArms(Game1.player, model.ArmsTexture);
                }
            }
        }

        private void ReloadArchery(string command, string[] args)
        {
            var packFilter = args.Length > 0 ? args[0] : null;
            LoadContentPacks(packId: packFilter);
        }

        private void LoadContentPacks(bool silent = false, string packId = null)
        {
            modelManager.Reset(packId);

            // Load owned content packs
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned().Where(c => String.IsNullOrEmpty(packId) is true || c.Manifest.UniqueID.Equals(packId, StringComparison.OrdinalIgnoreCase)))
            {
                Monitor.Log($"Loading data from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", silent ? LogLevel.Trace : LogLevel.Debug);

                // Load Weapons
                Monitor.Log($"Loading weapons from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", LogLevel.Trace);
                AddContentPacks<WeaponModel>(contentPack, PackType.Weapon);

                // Load Ammo
                Monitor.Log($"Loading ammo from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", LogLevel.Trace);
                AddContentPacks<AmmoModel>(contentPack, PackType.Ammo);
            }

            // Set up the backported GameStateQuery
            GameStateQuery.SetupQueryTypes();

            // Invalidate Data/CraftingRecipes
            Helper.GameContent.InvalidateCache("Data/CraftingRecipes");

            // Add recipes to custom category via Better Crafting
            if (apiManager.GetBetterCraftingApi() is not null)
            {
                apiManager.SyncRecipesWithBetterCrafting();
            }
        }

        private void AddContentPacks<T>(IContentPack contentPack, PackType type) where T : BaseModel
        {
            string folderKeyword = String.Empty;
            string fileKeyword = String.Empty;

            switch (type)
            {
                case PackType.Ammo:
                    folderKeyword = "Ammo";
                    fileKeyword = "ammo";
                    break;
                case PackType.Weapon:
                    folderKeyword = "Weapons";
                    fileKeyword = "weapon";
                    break;
            }

            if (String.IsNullOrEmpty(folderKeyword) || String.IsNullOrEmpty(fileKeyword))
            {
                Monitor.Log($"Skipping the content pack {contentPack.Manifest.Name} due to an internal error", LogLevel.Warn);
                return;
            }

            try
            {
                var directoryPath = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, folderKeyword));
                if (!directoryPath.Exists)
                {
                    Monitor.Log($"No {folderKeyword} folder found for the content pack {contentPack.Manifest.Name}", LogLevel.Trace);
                    return;
                }

                var modelFolders = directoryPath.GetDirectories("*", SearchOption.AllDirectories);
                if (modelFolders.Count() == 0)
                {
                    Monitor.Log($"No sub-folders found under {folderKeyword} for the content pack {contentPack.Manifest.Name}", LogLevel.Warn);
                    return;
                }

                // Load in the models
                foreach (var textureFolder in modelFolders)
                {
                    if (!File.Exists(Path.Combine(textureFolder.FullName, $"{fileKeyword}.json")))
                    {
                        if (textureFolder.GetDirectories().Count() == 0)
                        {
                            Monitor.Log($"Content pack {contentPack.Manifest.Name} is missing a {fileKeyword}.json under {textureFolder.Name}", LogLevel.Warn);
                        }

                        continue;
                    }

                    var parentFolderName = textureFolder.Parent.FullName.Replace(contentPack.DirectoryPath + Path.DirectorySeparatorChar, String.Empty);
                    var modelPath = Path.Combine(parentFolderName, textureFolder.Name, $"{fileKeyword}.json");

                    // Parse the model and assign it the content pack's owner
                    var model = contentPack.ReadJsonFile<T>(modelPath);
                    model.ContentPack = contentPack;
                    model.SetId(contentPack);

                    // Verify the required Name property is set
                    if (String.IsNullOrEmpty(model.Name))
                    {
                        Monitor.Log($"Unable to add {fileKeyword} from {model.ContentPack.Manifest.Author}: Missing the Name property", LogLevel.Warn);
                        continue;
                    }

                    // Verify that a model with the name doesn't exist in this pack
                    if (modelManager.GetSpecificModel<WeaponModel>(model.Id) != null)
                    {
                        Monitor.Log($"Unable to add {fileKeyword} from {contentPack.Manifest.Name}: This pack already contains a {fileKeyword} with the name of {model.Name}", LogLevel.Warn);
                        continue;
                    }

                    // Verify we are given a texture and if so, track it
                    if (!File.Exists(Path.Combine(textureFolder.FullName, $"{fileKeyword}.png")))
                    {
                        Monitor.Log($"Unable to add {fileKeyword} for {model.Name} from {contentPack.Manifest.Name}: No associated {fileKeyword}.png given", LogLevel.Warn);
                        continue;
                    }

                    // Load in the texture
                    model.TexturePath = contentPack.ModContent.GetInternalAssetName(Path.Combine(parentFolderName, textureFolder.Name, $"{fileKeyword}.png")).Name;
                    model.Texture = contentPack.ModContent.Load<Texture2D>(model.TexturePath);

                    // Load in any provided translations
                    if (contentPack.Translation is not null)
                    {
                        model.Translations = contentPack.Translation;
                    }

                    // Handle model specific properties
                    switch (model)
                    {
                        case WeaponModel weaponModel:
                            // Load in the optional arm textures, if given
                            if (File.Exists(Path.Combine(textureFolder.FullName, $"arms.png")))
                            {
                                weaponModel.ArmsTexture = contentPack.ModContent.Load<Texture2D>(contentPack.ModContent.GetInternalAssetName(Path.Combine(parentFolderName, textureFolder.Name, $"arms.png")).Name);
                            }
                            break;
                        case AmmoModel ammoModel:
                            break;
                    }

                    // Verify the model has all required properties
                    List<string> missingRequiredProperties = DoesModelHaveRequiredProperties(model);
                    if (missingRequiredProperties.Count > 0)
                    {
                        Monitor.Log($"Unable to add {fileKeyword} for {model.Name} from {contentPack.Manifest.Name}: Missing the following required properties: {string.Join(", ", missingRequiredProperties)}", LogLevel.Warn);
                        continue;
                    }

                    // Track the model
                    modelManager.AddModel(model);

                    // Log it
                    Monitor.Log(model.ToString(), LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error loading {fileKeyword} from content pack {contentPack.Manifest.Name}: {ex}", LogLevel.Error);
            }
        }

        private List<string> DoesModelHaveRequiredProperties(BaseModel model)
        {
            // Check for shared required properties
            List<string> missingPropertyNames = new List<string>();

            // Perform model specific checks
            switch (model)
            {
                case WeaponModel weaponModel:
                    // Verify that any sound names given are valid
                    if (weaponModel.StartChargingSound is not null && weaponModel.StartChargingSound.IsValid() is false)
                    {
                        Monitor.LogOnce($"The StartChargingSound.Name {weaponModel.StartChargingSound} does not exist for the weapon {model.Id}", LogLevel.Warn);
                    }
                    if (weaponModel.FinishChargingSound is not null && weaponModel.FinishChargingSound.IsValid() is false)
                    {
                        Monitor.LogOnce($"The FinishChargingSound.Name {weaponModel.FinishChargingSound} does not exist for the weapon {model.Id}", LogLevel.Warn);
                    }
                    if (weaponModel.FiringSound is not null && weaponModel.FiringSound.IsValid() is false)
                    {
                        Monitor.LogOnce($"The FiringSound.Name {weaponModel.FiringSound} does not exist for the weapon {model.Id}", LogLevel.Warn);
                    }

                    if (weaponModel.Type is WeaponType.Any)
                    {
                        missingPropertyNames.Add("WeaponType");
                    }
                    if (weaponModel.DamageRange is null)
                    {
                        missingPropertyNames.Add("DamageRange");
                    }
                    break;
                case AmmoModel ammoModel:
                    // Verify that any sound names given are valid
                    if (ammoModel.ImpactSound is not null && ammoModel.ImpactSound.IsValid() is false)
                    {
                        Monitor.LogOnce($"The ImpactSound.Name {ammoModel.ImpactSound} does not exist for the ammo {model.Id}", LogLevel.Warn);
                    }

                    if (ammoModel.Type is AmmoType.Any)
                    {
                        missingPropertyNames.Add("AmmoType");
                    }
                    if (ammoModel.ProjectileSprite is null)
                    {
                        missingPropertyNames.Add("ProjectileSprite");
                    }
                    break;
            }

            return missingPropertyNames;
        }
    }
}
