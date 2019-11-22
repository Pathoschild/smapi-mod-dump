using BetterMixedSeeds.Config;
using BetterMixedSeeds.Data;
using BetterMixedSeeds.Patches;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Crop = BetterMixedSeeds.Config.Crop;

namespace BetterMixedSeeds
{
    public class ModEntry : Mod
    {
        public static List<Seed> Seeds { get; private set; }
        public static IMonitor MMonitor { get; private set; }
        public static IModHelper MHelper { get; private set; }
        public static ModConfig ModConfig { get; private set; }
        public static SeedIndex SeedIndex { get; private set; } = new SeedIndex();

        public override void Entry(IModHelper helper)
        {
            MMonitor = this.Monitor;
            MHelper = this.Helper;
            ModConfig = this.Helper.ReadConfig<ModConfig>();

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += Events_SaveLoaded;
        }

        private void ApplyHarmonyPatches()
        {
            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            // Apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Crop), nameof(StardewValley.Crop.getRandomLowGradeCropForThisSeason)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(randomCropPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), "cutWeed"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(cutWeedPrefix)))
            );

            // Apply the assembly patch
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        
        private void Events_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            PopulateSeeds();
        }
        
        private void PopulateSeeds()
        {
            List<Seed> enabledSeeds = new List<Seed>();

            // Add seeds from Stardew Valley
            enabledSeeds = enabledSeeds
                .Union(CheckModForEnabledCrops("StardewValley"))
                .ToList();

            // Add seeds from integrated mods
            List<string> integratedModsInstalled = CheckIntegratedMods();

            object jaApi = null;
            if (integratedModsInstalled.Count() != 0)
            {
                jaApi = this.Helper.ModRegistry.GetApi("spacechase0.JsonAssets");

                if (jaApi != null)
                {
                    foreach (string modName in integratedModsInstalled)
                    {
                        MMonitor.Log($"{modName} loaded");

                        enabledSeeds = enabledSeeds
                            .Union(CheckModForEnabledCrops(modName))
                            .ToList();
                    }
                }
                else
                {
                    MMonitor.Log("Failed to retrieve Json Assets API", LogLevel.Error);
                }
            }

            // Create a new collection, so no exceptions occur from changing values in the current collection loop
            List<Seed> updatedEnabledSeeds = new List<Seed>();

            foreach (var enabledSeed in enabledSeeds)
            {
                Seed newEnabledSeed = enabledSeed;
                int seedIndex;

                // Check if the current seedName is a number, meaning it's a Stardew seed
                if (int.TryParse(enabledSeed.Name, out seedIndex))
                {
                    newEnabledSeed.Id = seedIndex;
                }
                else
                {
                    // Get seed if the current seed is from an integrated mod
                    seedIndex = this.Helper.Reflection.GetMethod(jaApi, "GetObjectId").Invoke<int>(enabledSeed.Name);

                    if (seedIndex == -1)
                    {
                        MMonitor.Log($"Could find seed index for item: {enabledSeed.Name}", LogLevel.Error);
                        continue;
                    }

                    newEnabledSeed.Id = seedIndex;
                }

                updatedEnabledSeeds.Add(newEnabledSeed);
            }

            Seeds = updatedEnabledSeeds;
        }

        private List<string> CheckIntegratedMods()
        {
            List<string> integratedModsInstalled = new List<string>();

            if (this.Helper.ModRegistry.IsLoaded("ParadigmNomad.FantasyCrops"))
                integratedModsInstalled.Add("FantasyCrops");
            if (this.Helper.ModRegistry.IsLoaded("paradigmnomad.freshmeat"))
                integratedModsInstalled.Add("FreshMeat");
            if (this.Helper.ModRegistry.IsLoaded("ppja.fruitsandveggies"))
                integratedModsInstalled.Add("FruitAndVeggies");
            if (this.Helper.ModRegistry.IsLoaded("mizu.flowers"))
                integratedModsInstalled.Add("MizusFlowers");
            if (this.Helper.ModRegistry.IsLoaded("PPJA.cannabiskit"))
                integratedModsInstalled.Add("CannabisKit");
            if (this.Helper.ModRegistry.IsLoaded("Popobug.SPCFW"))
                integratedModsInstalled.Add("SixPlantableCrops");
            if (this.Helper.ModRegistry.IsLoaded("BFV.FruitVeggie"))
                integratedModsInstalled.Add("BonsterCrops");
            if (this.Helper.ModRegistry.IsLoaded("RevenantCrops"))
                integratedModsInstalled.Add("RevenantCrops");
            if (this.Helper.ModRegistry.IsLoaded("kildarien.farmertoflorist"))
                integratedModsInstalled.Add("FarmerToFlorist");
            if (this.Helper.ModRegistry.IsLoaded("Fish.LuckyClover"))
                integratedModsInstalled.Add("LuckyClover");
            if (this.Helper.ModRegistry.IsLoaded("Fish.FishsFlowers"))
                integratedModsInstalled.Add("FishsFlowers");
            if (this.Helper.ModRegistry.IsLoaded("StephansLotsOfCrops"))
                integratedModsInstalled.Add("StephansLotsOfCrops");
            if (this.Helper.ModRegistry.IsLoaded("minervamaga.JA.EemieCrops"))
                integratedModsInstalled.Add("EemiesCrops");
            if (this.Helper.ModRegistry.IsLoaded("jfujii.TeaTime"))
                integratedModsInstalled.Add("TeaTime");
            if (this.Helper.ModRegistry.IsLoaded("Mae.foragetofarm"))
                integratedModsInstalled.Add("ForageToFarm");
            if (this.Helper.ModRegistry.IsLoaded("rearda88.GemandMineralCrops"))
                integratedModsInstalled.Add("GemAndMineralCrops");
            if (this.Helper.ModRegistry.IsLoaded("6480.crops.arabidopsis"))
                integratedModsInstalled.Add("MouseEarCress");
            if (this.Helper.ModRegistry.IsLoaded("ppja.ancientcrops"))
                integratedModsInstalled.Add("AncientCrops");

            return integratedModsInstalled;
        }

        private List<Seed> CheckModForEnabledCrops(string modName)
        {
            List<Seed> seedNames = new List<Seed>();

            // Get the CropMod object for the current mod from config
            PropertyInfo configInfo = ModConfig.GetType().GetProperty(modName);
            CropMod mod = (CropMod)configInfo.GetValue(ModConfig);
            
            // Get the 4 season properties for the current CropMod
            PropertyInfo[] modSeasonsInfo = mod.GetType().GetProperties();

            // Get the seed index for finding seed names for the crops
            PropertyInfo seedIndexInfo = SeedIndex.GetType().GetProperty(modName);
            Dictionary<string, string> seedIndex = (Dictionary<string, string>)seedIndexInfo.GetValue(SeedIndex);

            for (int i = 0; i < 4; i++)
            {
                PropertyInfo modSeasonInfo = modSeasonsInfo[i];
                Season season = (Season)modSeasonInfo.GetValue(mod);
                string seasonName = modSeasonInfo.Name.ToLower();

                if (season == null)
                {
                    continue;
                }

                foreach (Crop crop in season.Crops)
                {
                    if (crop.Enabled)
                    {
                        string seedName = seedIndex
                            .Where(seed => seed.Key == crop.Name)
                            .Select(seed => seed.Value)
                            .FirstOrDefault();

                        if (string.IsNullOrEmpty(seedName))
                        {
                            MMonitor.Log($"Seed name for {crop.Name} couldn't be found", LogLevel.Error);
                            continue;
                        }

                        // If the seed is already in the dictionary, add the season to the array
                        if (seedNames.Where(seed => seed.Name == seedName).Any())
                        {
                            // Add the season to all instances of the seed
                            seedNames
                                .Where(seed => seed.Name == seedName)
                                .Select(seed => seed.Seasons.Add(seasonName));
                        }
                        else
                        {
                            // Add the to the list [Chance] number of times, so it has an effect on the final result
                            for (int j = 0; j < crop.Chance; j++)
                            {
                                seedNames.Add(new Seed(0, seedName, new string[1] { seasonName }));
                            }

                            MMonitor.Log($"{seedName} has been added to the seed list");
                        }
                    }
                }
            }

            return seedNames;
        }

        #region Patches

        internal static Type GetSDVType(string type)
        {
            const string prefix = "StardewValley.";
            Type defaultSDV = Type.GetType($"{prefix}{type}, Stardew Valley");

            return defaultSDV ?? Type.GetType($"{prefix}{type}, StardewValley");
        }

        private static bool randomCropPrefix(string season, ref int __result)
        {
            List<int> possibleSeeds = new List<int>();

            if (Game1.currentLocation.IsGreenhouse)
            {
                possibleSeeds = ModEntry.Seeds
                    .Select(seed => seed.Id)
                    .ToList();
            }
            else
            {
                possibleSeeds = ModEntry.Seeds
                    .Where(seed => seed.Seasons.Contains(season))
                    .Select(seed => seed.Id)
                    .ToList();
            }

            if (possibleSeeds.Any())
            {
                __result = possibleSeeds[new Random().Next(possibleSeeds.Count())];
                var test2 = __result;
                var test = ModEntry.Seeds.Where(seed => seed.Id == test2);
            }
            else
            {
                ModEntry.MMonitor.Log("No possible seeds in seed list", LogLevel.Error);
                return true;
            }

            return false;
        }

        private static bool cutWeedPrefix(Farmer who, StardewValley.Object __instance)
        {
            // Custom added code for mixed seeds drop chance
            double upperBound = Math.Min(1, ModEntry.ModConfig.PercentDropChanceForMixedSeedsWhenNotFiber / 100f);
            double mixedSeedDropChance = Math.Round(Math.Max(0, upperBound), 3);

            int parentSheetIndex = -1;

            if (Game1.random.NextDouble() > 0.5)
            {
                parentSheetIndex = 771;
            }

            if (Game1.random.NextDouble() < mixedSeedDropChance)
            {
                parentSheetIndex = 770;
            }

            // Default game method
            Color color = Color.Green;
            string audioName = "cut";
            int rowInAnimationTexture = 50;
            __instance.Fragility = 2;

            switch (__instance.ParentSheetIndex)
            {
                case 313:
                case 314:
                case 315:
                    color = new Color(84, 101, 27);
                    break;
                case 316:
                case 317:
                case 318:
                    color = new Color(109, 49, 196);
                    break;
                case 319:
                    color = new Color(30, 216, (int)byte.MaxValue);
                    audioName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    who.currentLocation.playSound("drumkit2");
                    parentSheetIndex = -1;
                    break;
                case 320:
                    color = new Color(175, 143, (int)byte.MaxValue);
                    audioName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    who.currentLocation.playSound("drumkit2");
                    parentSheetIndex = -1;
                    break;
                case 321:
                    color = new Color(73, (int)byte.MaxValue, 158);
                    audioName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    who.currentLocation.playSound("drumkit2");
                    parentSheetIndex = -1;
                    break;
                case 678:
                    color = new Color(228, 109, 159);
                    break;
                case 679:
                    color = new Color(253, 191, 46);
                    break;
                case 792:
                case 793:
                case 794:
                    parentSheetIndex = 770;
                    break;
            }

            if (audioName.Equals("breakingGlass") && Game1.random.NextDouble() < 1.0 / 400.0)
            {
                parentSheetIndex = 338;
            }

            who.currentLocation.playSound(audioName);

            StardewValley.Multiplayer multiplayer = ModEntry.MHelper.Reflection.GetField<StardewValley.Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f, color, 8, false, 100f, 0, -1, -1f, -1, 0));
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f + new Vector2((float)Game1.random.Next(-16, 16), (float)Game1.random.Next(-48, 48)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                flipped = true
            });
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f + new Vector2((float)Game1.random.Next(-16, 16), (float)Game1.random.Next(-48, 48)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                delayBeforeAnimationStart = 50
            });
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(rowInAnimationTexture, __instance.tileLocation.Value * 64f + new Vector2((float)Game1.random.Next(-16, 16), (float)Game1.random.Next(-48, 48)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                flipped = true,
                delayBeforeAnimationStart = 100
            });

            if (parentSheetIndex != -1)
            {
                who.currentLocation.debris.Add(new Debris((Item)new StardewValley.Object(parentSheetIndex, 1, false, -1, 0), __instance.TileLocation * 64f + new Vector2(32f, 32f)));
            }

            if (Game1.random.NextDouble() < 0.02)
            {
                who.currentLocation.addJumperFrog(__instance.TileLocation);
            }

            if (!who.hasMagnifyingGlass || Game1.random.NextDouble() >= 0.009)
            {
                return false;
            }

            StardewValley.Object unseenSecretNote = who.currentLocation.tryToCreateUnseenSecretNote(who);

            if (unseenSecretNote == null)
            {
                return false;
            }

            Game1.createItemDebris((Item)unseenSecretNote, new Vector2(__instance.TileLocation.X + 0.5f, __instance.TileLocation.Y + 0.75f) * 64f, (int)Game1.player.facingDirection, (GameLocation)null, -1);

            return false;
        }

        #endregion
    }
}
