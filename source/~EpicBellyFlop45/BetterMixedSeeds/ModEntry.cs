using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BetterMixedSeeds
{
    public class ModEntry : Mod
    {
        public static ILookup<int, string> Seeds;
        public static IMonitor ModMonitor;
        public static IModHelper ModHelper;
        public static ModConfig ModConfig;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            ModHelper = this.Helper;
            ModConfig = this.Helper.ReadConfig<ModConfig>();

            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Get the methods we want to patch
            MethodInfo targetMethod = AccessTools.Method(typeof(Crop), nameof(Crop.getRandomLowGradeCropForThisSeason));
            MethodInfo cutWeedTargetMethod = AccessTools.Method(typeof(StardewValley.Object), "cutWeed");

            // Get the patchs that were created
            MethodInfo prefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Prefix));
            MethodInfo cutWeedPrefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.cutWeedPrefix));

            // Apply the patch
            harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix));
            harmony.Patch(cutWeedTargetMethod, prefix: new HarmonyMethod(cutWeedPrefix));

            // Apply the assembly patch
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Add an event handler when all mods are loaded, so the JA api can be connected to
            this.Helper.Events.GameLoop.SaveLoaded += Events_SaveLoaded;
        }

        private void Events_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Populate the seeds dictionary
            Seeds = PopulateSeeds(ModConfig);
        }

        private static bool cutWeedPrefix(Farmer who, StardewValley.Object __instance)
        {
            // Make sure mixed seed drop rate is within bounds
            double mixedSeedDropChance = Math.Round(Math.Max(0, Math.Min(1, ModEntry.ModConfig.PercentDropChanceForMixedSeedsWhenNotFiber / 100f)), 3);

            Color color = Color.Green;
            string audioName = "cut";
            int rowInAnimationTexture = 50;
            __instance.Fragility = 2;
            int parentSheetIndex = -1;

            if (Game1.random.NextDouble() > 0.5)
            {
                parentSheetIndex = 771;
            }
            else if (Game1.random.NextDouble() < mixedSeedDropChance)
            {
                parentSheetIndex = 770;
            }

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

            StardewValley.Multiplayer multiplayer = ModEntry.ModHelper.Reflection.GetField<StardewValley.Multiplayer>(typeof(Game1), "multiplayer").GetValue();

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

        private static bool Prefix(string season, ref int __result)
        {
            ILookup<int, string> seeds = ModEntry.Seeds;
            IList<int> selectedSeeds = new List<int>();

            if (Game1.currentLocation.IsGreenhouse)
            {
                foreach (var seed in seeds)
                {
                    selectedSeeds.Add(seed.Key);
                }
            }
            else
            {
                switch (season.ToUpper())
                {
                    case "SPRING":

                        foreach (var seed in seeds)
                        {
                            foreach (string obj in seed)
                            {
                                if (obj == "SPRING")
                                {
                                    selectedSeeds.Add(seed.Key);
                                }
                            }
                        }

                        break;

                    case "SUMMER":

                        foreach (var seed in seeds)
                        {
                            foreach (string obj in seed)
                            {
                                if (obj == "SUMMER")
                                {
                                    selectedSeeds.Add(seed.Key);
                                }
                            }
                        }

                        break;

                    case "FALL":

                        foreach (var seed in seeds)
                        {
                            foreach (string obj in seed)
                            {
                                if (obj == "FALL")
                                {
                                    selectedSeeds.Add(seed.Key);
                                }
                            }
                        }

                        break;

                    case "WINTER":

                        foreach (var seed in seeds)
                        {
                            foreach (string obj in seed)
                            {
                                if (obj == "WINTER")
                                {
                                    selectedSeeds.Add(seed.Key);
                                }
                            }
                        }

                        break;
                }
            }

            if (selectedSeeds.Count() != 0)
            {
                __result = selectedSeeds[new Random().Next(selectedSeeds.Count())];
            }

            return false;
        }

        internal static Type GetSDVType(string type)
        {
            const string prefix = "StardewValley.";
            Type defaultSDV = Type.GetType($"{prefix}{type}, Stardew Valley");

            return defaultSDV ?? Type.GetType($"{prefix}{type}, StardewValley");
        }

        public ILookup<int, string> PopulateSeeds(ModConfig config)
        {
            IDictionary<string, int> integratedCrops = new Dictionary<string, int>();

            bool hasPPJAFantasyCrops = this.Helper.ModRegistry.IsLoaded("ParadigmNomad.FantasyCrops");
            bool hasPPJAFreshMeat = this.Helper.ModRegistry.IsLoaded("paradigmnomad.freshmeat");
            bool hasPPJAFruitsAndVeggies = this.Helper.ModRegistry.IsLoaded("ppja.fruitsandveggies");
            bool hasPPJAMizusFlowers = this.Helper.ModRegistry.IsLoaded("mizu.flowers");
            bool hasCannabisKit = this.Helper.ModRegistry.IsLoaded("PPJA.cannabiskit");
            bool hasSixPlantableCrops = this.Helper.ModRegistry.IsLoaded("Popobug.SPCFW");
            bool hasBonsterCrops = this.Helper.ModRegistry.IsLoaded("BFV.FruitVeggie");
            bool hasRevenantCrops = this.Helper.ModRegistry.IsLoaded("RevenantCrops");
            bool hasFarmerToFlorist = this.Helper.ModRegistry.IsLoaded("kildarien.farmertoflorist");
            bool hasLuckyClover = this.Helper.ModRegistry.IsLoaded("Fish.LuckyClover");
            bool hasFishFlowers = this.Helper.ModRegistry.IsLoaded("Fish.FishsFlowers");
            bool hasStephansLotsOfCrops = this.Helper.ModRegistry.IsLoaded("StephansLotsOfCrops");
            bool hasEemiesCrops = this.Helper.ModRegistry.IsLoaded("minervamaga.JA.EemieCrops");
            bool hasTeaTime = this.Helper.ModRegistry.IsLoaded("jfujii.TeaTime");
            bool hasForageToFarm = this.Helper.ModRegistry.IsLoaded("Mae.foragetofarm");

            object api = this.Helper.ModRegistry.GetApi("spacechase0.JsonAssets");

            if (hasPPJAFantasyCrops || hasPPJAFreshMeat || hasPPJAFruitsAndVeggies || hasPPJAMizusFlowers || hasCannabisKit || hasSixPlantableCrops || hasBonsterCrops || hasRevenantCrops || hasFarmerToFlorist || hasLuckyClover || hasFishFlowers || hasStephansLotsOfCrops || hasEemiesCrops || hasTeaTime || hasForageToFarm)
            {
                if (api != null)
                {
                    if (hasPPJAFantasyCrops)
                    {
                        this.Monitor.Log("PPJAFantasyCrops loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> fantasySeedNames = new List<string> { "Coal Seeds", "Copper Seeds", "Gold Seeds", "Iridum Seeds", "Iron Seeds", "Doubloom Seeds" };

                        foreach (string fantasySeedName in fantasySeedNames)
                        {
                            integratedCrops.Add(fantasySeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(fantasySeedName));
                            this.Monitor.Log($"Added {fantasySeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasPPJAFreshMeat)
                    {
                        this.Monitor.Log("PPJAFreshMeat loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> freshMeatSeedNames = new List<string> { "Beefvine Seeds", "Chevonvine Seeds", "Chickenvine Seeds", "Duckvine Seeds", "Muttonvine Seeds", "Porkvine Seeds", "Rabbitvine Seeds" };

                        foreach (var freshMeatSeedName in freshMeatSeedNames)
                        {
                            integratedCrops.Add(freshMeatSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(freshMeatSeedName));
                            this.Monitor.Log($"Added {freshMeatSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasPPJAFruitsAndVeggies)
                    {
                        this.Monitor.Log("PPJAFruitsAndVeggies loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> fruitAndVeggieSeedNames = new List<string> { "Adzuki Bean Seeds", "Aloe Pod", "Barley Seeds", "Basil Seeds", "Bell Pepper Seeds", "Blackberry Seeds", "Broccoli Seeds", "Cabbage Seeds", "Carrot Seeds", "Cassava Seeds", "Celery Seeds", "Chive Seeds", "Cotton Seeds", "Cucumber Starter", "Elderberry Seeds", "Fennel Seeds", "Ginger Seeds", "Gooseberry Seeds", "Green Pea Seeds", "Juniper Berry Seeds", "Kiwi Seeds", "Lettuce Seeds", "Mint Seeds", "Muskmelon Seeds", "Navy Bean Seeds", "Onion Seeds", "Oregano Seeds", "Parsley Seeds", "Passion Fruit Seeds", "Peanut Seeds", "Pineapple Seeds", "Raspberry Seeds", "Rice Seeds", "Rosemary Seeds", "Sage Seeds", "Soybean Seeds", "Spinach Seeds", "Sugar Beet Seeds", "Sugar Cane Seeds", "Sweet Canary Melon Seeds", "Sweet Potato Seeds", "Tea Seeds", "Thyme Seeds", "Wasabi Seeds", "Watermelon Seeds" };

                        foreach (var fruitAndVeggieSeedName in fruitAndVeggieSeedNames)
                        {
                            integratedCrops.Add(fruitAndVeggieSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(fruitAndVeggieSeedName));
                            this.Monitor.Log($"Added {fruitAndVeggieSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasPPJAMizusFlowers)
                    {
                        this.Monitor.Log("PPJAMizusFlowers loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> mizusFlowerSeedNames = new List<string> { "Bee Balm Seeds", "Blue Mist Seeds", "Chamomile Seeds", "Clary Sage Seeds", "Fairy Duster Pod", "Fall Rose Starter", "Fragrant Lilac Pod", "Herbal Lavender Seeds", "Honeysuckle Starter", "Passion Flower Seeds", "Pink Cat Seeds", "Purple Coneflower Seeds", "Rose Starter", "Shaded Violet Seeds", "Spring Rose Starter", "Summer Rose Starter", "Sweet Jasmine Seeds" };

                        foreach (var mizusFlowerSeedName in mizusFlowerSeedNames)
                        {
                            integratedCrops.Add(mizusFlowerSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(mizusFlowerSeedName));
                            this.Monitor.Log($"Added {mizusFlowerSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasCannabisKit)
                    {
                        this.Monitor.Log("CannabisKit loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> cannabisKitSeedNames = new List<string> { "Blue Dream Starter", "Cannabis Starter", "Girl Scout Cookies Starter", "Green Crack Starter", "Hemp Starter", "Hybrid Starter", "Indica Starter", "Northern Lights Starter", "OG Kush Starter", "Sativa Starter", "Sour Diesel Starter", "Strawberry Cough Starter", "Tobacco Seeds", "White Widow Starter" };

                        foreach (var cannabisKitSeedName in cannabisKitSeedNames)
                        {
                            integratedCrops.Add(cannabisKitSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(cannabisKitSeedName));
                            this.Monitor.Log($"Added {cannabisKitSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasSixPlantableCrops)
                    {
                        this.Monitor.Log("SixPlantableCrops loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> sixPlantableSeedNames = new List<string> { "Blue Rose Seeds", "Daikon Seeds", "Gentian Seeds", "Napa Cabbage Seeds", "Snowdrop Seeds", "Winter Broccoli Seeds" };

                        foreach (var sixPlantableSeedName in sixPlantableSeedNames)
                        {
                            integratedCrops.Add(sixPlantableSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(sixPlantableSeedName));
                            this.Monitor.Log($"Added {sixPlantableSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasBonsterCrops)
                    {
                        this.Monitor.Log("BonsterCrops loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> bonsterCropSeedNames = new List<string> { "Blackcurrant Seeds", "Blue Corn Seeds", "Cardamom Seeds", "Cranberry Bean Seeds", "Maypop Seeds", "Peppercorn Seeds", "Red Currant Seeds", "Rose Hip Seeds", "Roselle Hibiscus Seeds", "Summer Squash Seeds", "Taro Root", "White Currant Seeds" };

                        foreach (var bonsterCropSeedName in bonsterCropSeedNames)
                        {
                            integratedCrops.Add(bonsterCropSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(bonsterCropSeedName));
                            this.Monitor.Log($"Added {bonsterCropSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasRevenantCrops)
                    {
                        this.Monitor.Log("RevenantCrops loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> revenantCropSeedNames = new List<string> { "Enoki Mushroom Kit", "Gai Lan Seeds", "Maitake Mushroom Kit", "Oyster Mushroom Kit" };

                        foreach (var revenantCropSeedName in revenantCropSeedNames)
                        {
                            integratedCrops.Add(revenantCropSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(revenantCropSeedName));
                            this.Monitor.Log($"Added {revenantCropSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasFarmerToFlorist)
                    {
                        this.Monitor.Log("FarmerToFlorist loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> farmerToFloristSeedNames = new List<string> { "Allium Seeds", "Camellia Seeds", "Carnation Seeds", "Chrysanthemum Seeds", "Clematis Starter", "Dahlia Seeds", "Delphinium Seeds", "English Rose Seeds", "Freesia Bulb", "Geranium Seeds", "Herbal Peony Seeds", "Hyacinth Seeds", "Hydrangea Seeds", "Iris Bulb", "Lavender Seeds", "Lilac Seeds", "Lily Bulb", "Lotus Starter", "Petunia Seeds", "Violet Seeds", "Wisteria Seeds" };

                        foreach (var farmerToFloristSeedName in farmerToFloristSeedNames)
                        {
                            integratedCrops.Add(farmerToFloristSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(farmerToFloristSeedName));
                            this.Monitor.Log($"Added {farmerToFloristSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasLuckyClover)
                    {
                        this.Monitor.Log("LuckyClover loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> luckyCloverSeedNames = new List<string> { "Clover Seeds" };

                        foreach (var luckyCloverSeedName in luckyCloverSeedNames)
                        {
                            integratedCrops.Add(luckyCloverSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(luckyCloverSeedName));
                            this.Monitor.Log($"Added {luckyCloverSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasFishFlowers)
                    {
                        this.Monitor.Log("FishFlowers loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> fishFlowerSeedNames = new List<string> { "Hyacinth Bulb", "Pansy Seeds" };

                        foreach (var fishFlowerSeedName in fishFlowerSeedNames)
                        {
                            integratedCrops.Add(fishFlowerSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(fishFlowerSeedName));
                            this.Monitor.Log($"Added {fishFlowerSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasStephansLotsOfCrops)
                    {
                        this.Monitor.Log("StephansLotsOfCrops loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> stephansLotsOfCropsSeedNames = new List<string> { "Carrot Seeds", "Cucumber Seeds", "Onion Seeds", "Pea Starter", "Peanut Sprouts", "Pineapple Seeds", "Spinach Seeds", "Turnip Seeds", "Watermelon Seeds" };

                        foreach (var stephanLotsOfCropsSeedName in stephansLotsOfCropsSeedNames)
                        {
                            integratedCrops.Add(stephanLotsOfCropsSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(stephanLotsOfCropsSeedName));
                            this.Monitor.Log($"Added {stephanLotsOfCropsSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasEemiesCrops)
                    {
                        this.Monitor.Log("EemiesCrops loaded", LogLevel.Trace);

                        // Create a list of crop seeds to pass to JA API
                        List<string> eemiesCropsSeedNames = new List<string> { "Acorn Squash Seeds", "Black Forest Squash Seeds", "Cantaloupe Melon Seeds", "Charentais Melon Seeds", "Crookneck Squash Seeds", "Golden Hubbard Squash Seeds", "Jack O Lantern Pumpkin Seeds", "Korean Melon Seeds", "Large Watermelon Seeds", "Rich Canary Melon Seeds", "Rich Sweetness Melon Seeds", "Sweet Lightning Pumpkin Seeds" };

                        foreach (var eemiesCropsSeedName in eemiesCropsSeedNames)
                        {
                            integratedCrops.Add(eemiesCropsSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(eemiesCropsSeedName));
                            this.Monitor.Log($"Added {eemiesCropsSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasTeaTime)
                    {
                        this.Monitor.Log("TeaTime loaded", LogLevel.Trace);

                        // Create a list of crops to pass to JA API
                        List<string> teaTimeCropSeedNames = new List<string> { "Fresh Mint Seeds", "Tea Leaf Seeds" };

                        foreach (var teaTimeCropSeedName in teaTimeCropSeedNames)
                        {
                            integratedCrops.Add(teaTimeCropSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(teaTimeCropSeedName));
                            this.Monitor.Log($"Added {teaTimeCropSeedName} crop to list", LogLevel.Trace);
                        }
                    }

                    if (hasForageToFarm)
                    {
                        this.Monitor.Log("ForageToFarm loaded", LogLevel.Trace);

                        // Create a list of crops to pass to JA API
                        List<string> forageToFarmCropSeedNames = new List<string> { "Cave Carrot Seeds", "Chanterelle Mushroom Spores", "Coconut Seed", "Common Mushroom Spores", "Crocus Seeds", "Crystal Fruit Seeds", "Daffodil Seeds", "Dandelion Seeds", "Fiddlehead Fern Spores", "Hazelnut Seed", "Holly Seeds", "Wild Horseradish Seeds", "Leek Seeds", "Morel Mushroom Spores", "Purple Mushroom Spores", "Red Mushroom Spores", "Salmonberry Seeds", "Snow Yam Seeds", "Spice Berry Seeds", "Spring Onion Seeds", "Sweet Pea Seeds", "Wild Blackberry Seeds", "Wild Plum Seed", "Winter Root Seeds" };

                        foreach (var forageToFarmCropSeedName in forageToFarmCropSeedNames)
                        {
                            integratedCrops.Add(forageToFarmCropSeedName, this.Helper.Reflection.GetMethod(api, "GetObjectId").Invoke<int>(forageToFarmCropSeedName));
                            this.Monitor.Log($"Added {forageToFarmCropSeedName} crop to list", LogLevel.Trace);
                        }
                    }
                }
                else
                {
                    this.Monitor.Log("Failed to retrieve Json Assets API", LogLevel.Error);
                }
            }
            else
            {
                this.Monitor.Log("No integrated mods detected", LogLevel.Trace);
            }

            IList<KeyValuePair<int, string>> seeds = new List<KeyValuePair<int, string>>();
            
            bool springSeedEnabled = false;
            bool summerSeedEnabled = false;
            bool fallSeedEnabled = false;

            // Add all the seeds that are enabled in config
            if (config.UseAncientFruit_SPRING) { seeds.Add(new KeyValuePair<int, string>(499, "SPRING")); springSeedEnabled = true; }
            if (config.UseBlueJazz) { seeds.Add(new KeyValuePair<int, string>(429, "SPRING")); springSeedEnabled = true; }
            if (config.UseCauliflower) { seeds.Add(new KeyValuePair<int, string>(474, "SPRING")); springSeedEnabled = true; }
            if (config.UseCoffeeBean) { seeds.Add(new KeyValuePair<int, string>(433, "SPRING")); springSeedEnabled = true; }
            if (config.UseGarlic) { seeds.Add(new KeyValuePair<int, string>(476, "SPRING")); springSeedEnabled = true; }
            if (config.UseGreenBean) { seeds.Add(new KeyValuePair<int, string>(473, "SPRING")); springSeedEnabled = true; }
            if (config.UseKale) { seeds.Add(new KeyValuePair<int, string>(477, "SPRING")); springSeedEnabled = true; }
            if (config.UseParsnip) { seeds.Add(new KeyValuePair<int, string>(472, "SPRING")); springSeedEnabled = true; }
            if (config.UsePotato) { seeds.Add(new KeyValuePair<int, string>(475, "SPRING")); springSeedEnabled = true; }
            if (config.UseRhubarb) { seeds.Add(new KeyValuePair<int, string>(478, "SPRING")); springSeedEnabled = true; }
            if (config.UseStrawberry) { seeds.Add(new KeyValuePair<int, string>(745, "SPRING")); springSeedEnabled = true; }
            if (config.UseTulip) { seeds.Add(new KeyValuePair<int, string>(427, "SPRING")); springSeedEnabled = true; }

            if (config.UseAncientFruit_SUMMER) { seeds.Add(new KeyValuePair<int, string>(499, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseBlueberry) { seeds.Add(new KeyValuePair<int, string>(481, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseCorn_SUMMER) { seeds.Add(new KeyValuePair<int, string>(487, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseHops) { seeds.Add(new KeyValuePair<int, string>(302, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseHotPepper) { seeds.Add(new KeyValuePair<int, string>(482, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseMelon) { seeds.Add(new KeyValuePair<int, string>(479, "SUMMER")); summerSeedEnabled = true; }
            if (config.UsePoppy) { seeds.Add(new KeyValuePair<int, string>(453, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseRadish) { seeds.Add(new KeyValuePair<int, string>(484, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseRedCabbage) { seeds.Add(new KeyValuePair<int, string>(485, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseStarfruit) { seeds.Add(new KeyValuePair<int, string>(486, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseSummerSpangle) { seeds.Add(new KeyValuePair<int, string>(455, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseSunflower_SUMMER) { seeds.Add(new KeyValuePair<int, string>(431, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseTomato) { seeds.Add(new KeyValuePair<int, string>(480, "SUMMER")); summerSeedEnabled = true; }
            if (config.UseWheat_SUMMER) { seeds.Add(new KeyValuePair<int, string>(483, "SUMMER")); summerSeedEnabled = true; }

            if (config.UseAncientFruit_FALL) { seeds.Add(new KeyValuePair<int, string>(499, "FALL")); fallSeedEnabled = true; }
            if (config.UseAmaranth) { seeds.Add(new KeyValuePair<int, string>(299, "FALL")); fallSeedEnabled = true; }
            if (config.UseArtichoke) { seeds.Add(new KeyValuePair<int, string>(489, "FALL")); fallSeedEnabled = true; }
            if (config.UseBeet) { seeds.Add(new KeyValuePair<int, string>(494, "FALL")); fallSeedEnabled = true; }
            if (config.UseBokChoy) { seeds.Add(new KeyValuePair<int, string>(491, "FALL")); fallSeedEnabled = true; }
            if (config.UseCorn_FALL) { seeds.Add(new KeyValuePair<int, string>(487, "FALL")); fallSeedEnabled = true; }
            if (config.UseCranberries) { seeds.Add(new KeyValuePair<int, string>(493, "FALL")); fallSeedEnabled = true; }
            if (config.UseEggplant) { seeds.Add(new KeyValuePair<int, string>(488, "FALL")); fallSeedEnabled = true; }
            if (config.UseFairyRose) { seeds.Add(new KeyValuePair<int, string>(425, "FALL")); fallSeedEnabled = true; }
            if (config.UseGrape) { seeds.Add(new KeyValuePair<int, string>(301, "FALL")); fallSeedEnabled = true; }
            if (config.UsePumpkin) { seeds.Add(new KeyValuePair<int, string>(490, "FALL")); fallSeedEnabled = true; }
            if (config.UseSunflower_FALL) { seeds.Add(new KeyValuePair<int, string>(431, "FALL")); fallSeedEnabled = true; }
            if (config.UseSweetGemBerry) { seeds.Add(new KeyValuePair<int, string>(347, "FALL")); fallSeedEnabled = true; }
            if (config.UseWheat_FALL) { seeds.Add(new KeyValuePair<int, string>(483, "FALL")); fallSeedEnabled = true; }
            if (config.UseYam) { seeds.Add(new KeyValuePair<int, string>(492, "FALL")); fallSeedEnabled = true; }

            if (config.UseCactusFruit) { seeds.Add(new KeyValuePair<int, string>(802, "NONE")); }

            if (hasPPJAFantasyCrops && api != null)
            {
                if (config.UseCoal_Root_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Coal Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseCoal_Root_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Coal Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCopper_Leaf) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Copper Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseGold_Leaf) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Gold Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseIridium_Fern) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Iridum Seeds"], "WINTER")); }
                if (config.UseMoney_Plant) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Doubloom Seeds"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasPPJAFreshMeat && api != null)
            {
                if (config.UseBeef_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Beefvine Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseBeef_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Beefvine Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseChevon_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Chevonvine Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseChevon_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Chevonvine Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseDuck_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Duckvine Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseDuck_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Duckvine Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseMutton_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Muttonvine Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseMutton_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Muttonvine Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePork_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Porkvine Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePork_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Porkvine Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseRabbit_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rabbitvine Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseRabbit_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rabbitvine Seeds"], "SUMMER")); summerSeedEnabled = true; }
            }

            if (hasPPJAFruitsAndVeggies && api != null)
            {
                if (config.UseAdzuki_Bean) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Adzuki Bean Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseAloe) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Aloe Pod"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseBarley) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Barley Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseBasil) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Basil Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseBell_Pepper) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Bell Pepper Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseBlackberry) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blackberry Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseBroccoli) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Broccoli Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCabbage_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cabbage Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseCabbage_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cabbage Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCarrot_FruitAndVeggies) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Carrot Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCassava) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cassava Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCelery) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Celery Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseChives) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Chive Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCotton_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cotton Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCotton_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cotton Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCucumber_FruitAndVeggies) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cucumber Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseElderberry) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Elderberry Seeds"], "WINTER")); }
                if (config.UseFennel) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fennel Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseGinger) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Ginger Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseGooseberry) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Gooseberry Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseGreen_Pea) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Green Pea Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseJuniper) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Juniper Berry Seeds"], "WINTER")); }
                if (config.UseKiwi_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Kiwi Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseKiwi_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Kiwi Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseLettuce) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Lettuce Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseMint) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Mint Seeds"], "WINTER")); }
                if (config.UseMuskmelon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Muskmelon Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseNavy_Bean) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Navy Bean Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseOnion_FruitAndVeggies) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Onion Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseOregano) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Oregano Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseParsley) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Parsley Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UsePassion_Fruit) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Passion Fruit Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UsePeanut_FruitAndVeggies) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Peanut Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UsePineapple_FruitAndVeggies) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Pineapple Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseRaspberry) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Raspberry Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseRice_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rice Seeds"], "SPING")); springSeedEnabled = true; }
                if (config.UseRice_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rice Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseRice_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rice Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseRosemary) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rosemary Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseSage) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sage Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseSoybean) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Soybean Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseSpinach_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Spinach Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseSpinach_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Spinach Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseSugar_Beet) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sugar Beet Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseSugar_Cane) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sugar Cane Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseSweet_Canary_Melon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sweet Canary Melon Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseSweet_Potato) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sweet Potato Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseTea_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tea Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseTea_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tea Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseTea_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tea Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseThyme) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Thyme Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseWasabi) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Wasabi Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseWatermelon_Mizu) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Watermelon Seeds"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasPPJAMizusFlowers && api != null)
            {
                if (config.UseBee_Balm) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Bee Balm Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseBlue_Mist) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blue Mist Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseChamomile) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Chamomile Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseClary_Sage) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Clary Sage Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseFairy_Duster) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fairy Duster Pod"], "FALL")); fallSeedEnabled = true; }
                if (config.UseFall_Rose) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fall Rose Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseFragrant_Lilac) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fragrant Lilac Pod"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseHerbal_Lavender) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Herbal Lavender Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseHoneysuckle_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Honeysuckle Starter"], "SPRING")); springSeedEnabled = true; }
                if (config.UseHoneysuckle_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Honeysuckle Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePassion_Flower) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Passion Flower Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePink_Cat) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Pink Cat Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UsePurple_Coneflower) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Purple Coneflower Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseRose_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rose Starter"], "SPRING")); springSeedEnabled = true; }
                if (config.UseRose_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rose Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseRose_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rose Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseShaded_Violet) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Shaded Violet Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseSpring_Rose) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Spring Rose Starter"], "SPRING")); springSeedEnabled = true; }
                if (config.UseSummer_Rose) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Summer Rose Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseSweet_Jasmine) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sweet Jasmine Seeds"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasCannabisKit && api != null)
            {
                if (config.UseBlue_Dream_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blue Dream Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseBlue_Dream_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blue Dream Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCannabis_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cannabis Starter"], "SPRING")); springSeedEnabled = true; }
                if (config.UseCannabis_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cannabis Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCannabis_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cannabis Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseGirl_Scout_Cookies_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Girl Scout Cookies Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseGirl_Scout_Cookies_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Girl Scout Cookies Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseGreen_Crack_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Green Crack Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseGreen_Crack_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Green Crack Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseHemp_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hemp Starter"], "SPRING")); springSeedEnabled = true; }
                if (config.UseHemp_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hemp Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseHemp_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hemp Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseHybrid_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hybrid Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseHybrid_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hybrid Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseIndica) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Indica Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseNorthern_Lights) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Northern Lights Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseOG_Kush_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["OG Kush Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseOG_Kush_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["OG Kush Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseSativa) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sativa Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseSour_Diesel) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sour Diesel Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseStrawberry_Cough_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Strawberry Cough Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseStrawberry_Cough_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Strawberry Cough Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseTobacco_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tobacco Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseTobacco_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tobacco Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseWhite_Widow_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["White Widow Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseWhite_Widow_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["White Widow Starter"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasSixPlantableCrops && api != null)
            {
                if (config.UseBlue_Rose) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blue Rose Seeds"], "WINTER")); }
                if (config.UseDaikon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Daikon Seeds"], "WINTER")); }
                if (config.UseGentian) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Gentian Seeds"], "WINTER")); }
                if (config.UseNapa_Cabbage) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Napa Cabbage Seeds"], "WINTER")); }
                if (config.UseSnowdrop) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Snowdrop Seeds"], "WINTER")); }
                if (config.UseWinter_Broccoli) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Winter Broccoli Seeds"], "WINTER")); }
            }

            if (hasBonsterCrops && api != null)
            {
                if (config.UseBlackcurrant) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blackcurrant Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseBlue_Corn_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blue Corn Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseBlue_Corn_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Blue Corn Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCardamom) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cardamom Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCranberry_Beans) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cranberry Bean Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseMaypop) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Maypop Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePeppercorn_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Peppercorn Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePeppercorn_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Peppercorn Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseRedCurrant) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Red Currant Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseRose_Hips_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rose Hip Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseRose_Hips_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rose Hip Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseRoselle_Hibiscus) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Roselle Hibiscus Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseSummer_Squash) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Summer Squash Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseTaro_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Taro Root"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseTaro_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Taro Root"], "FALL")); fallSeedEnabled = true; }
                if (config.UseWhite_Currant) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["White Currant Seeds"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasRevenantCrops && api != null)
            {
                if (config.UseEnoki_Mushroom_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Enoki Mushroom Kit"], "SPRING")); springSeedEnabled = true; }
                if (config.UseEnoki_Mushroom_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Enoki Mushroom Kit"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseGai_Lan) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Gai Lan Seeds"], "WINTER")); }
                if (config.UseMaitake_Mushroom) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Maitake Mushroom Kit"], "FALL")); fallSeedEnabled = true; }
                if (config.UseOyster_Mushroom) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Oyster Mushroom Kit"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasFarmerToFlorist && api != null)
            {
                if (config.UseAllium) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Allium Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCamellia_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Camellia Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseCamellia_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Camellia Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCarnation_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Carnation Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseCarnation_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Carnation Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseChrysanthemum) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Chrysanthemum Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseClematis) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Clematis Starter"], "FALL")); fallSeedEnabled = true; }
                if (config.UseDahlia) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Dahlia Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseDelphinium) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Delphinium Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseEnglish_Rose) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["English Rose Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseFreesia) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Freesia Bulb"], "FALL")); fallSeedEnabled = true; }
                if (config.UseGeranium) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Geranium Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseHerbalPeony) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Herbal Peony Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseHyacinth_FarmerToFlorist) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hyacinth Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseHydrangea) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hydrangea Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseIris) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Iris Bulb"], "FALL")); fallSeedEnabled = true; }
                if (config.UseLavender) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Lavender Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseLilac) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Lilac Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseLily) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Lily Bulb"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseLotus) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Lotus Starter"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePetunia) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Petunia Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseViolet) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Violet Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseWisteria) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Wisteria Seeds"], "SPRING")); springSeedEnabled = true; }
            }

            if (hasLuckyClover && api != null)
            {
                if (config.UseLuckyClover) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Clover Seeds"], "SPRING")); springSeedEnabled = true; }
            }

            if (hasFishFlowers && api != null)
            {
                if (config.UseHyacinth_FishsFlowers) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hyacinth Bulb"], "SPRING")); springSeedEnabled = true; }
                if (config.UsePansy_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Pansy Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UsePansy_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Pansy Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePansy_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Pansy Seeds"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasStephansLotsOfCrops && api != null)
            {
                if (config.UseCarrot_StephanLotsOfCrops) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Carrot Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCucumber_StephanLotsOfCrops) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cucumber Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseOnion_StephanLotsOfCrops) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Onion Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UsePea_Pod) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Pea Starter"], "SPRING")); springSeedEnabled = true; }
                if (config.UsePeanut_StephanLotsOfCrops) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Peanut Sprouts"], "FALL")); fallSeedEnabled = true; }
                if (config.UsePineapple_StephanLotsOfCrops) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Pineapple Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseSpinach) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Spinach Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseTurnip) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Turnip Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseWatermelon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Watermelon Seeds"], "SUMMER")); summerSeedEnabled = true; }
            }

            if (hasEemiesCrops && api != null)
            {
                if (config.UseAcorn_Squash) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Acorn Squash Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseBlack_Forest_Squash) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Black Forest Squash Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCantaloupe_Melon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cantaloupe Melon Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCharentais_Melon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Charentais Melon Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCrookneck_Squash) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Crookneck Squash Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseGolden_Hubbard_Squash) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Golden Hubbard Squash Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseJack_O_Lantern_Pumpkin) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Jack O Lantern Pumpkin Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseKorean_Melon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Korean Melon Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseLarge_Watermelon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Large Watermelon Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseRich_Canary_Melon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rich Canary Melon Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseRich_Sweetness_Melon) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Rich Sweetness Melon Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseSweet_Lightning_Pumpkin) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sweet Lightning Pumpkin Seeds"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasTeaTime && api != null)
            {
                if (config.UseMint_Tea_Plant_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fresh Mint Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseMint_Tea_Plant_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fresh Mint Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseMint_Tea_Plant_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fresh Mint Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseTea_Leaf_Plant_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tea Leaf Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseTea_Leaf_Plant_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tea Leaf Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseTea_Leaf_Plant_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Tea Leaf Seeds"], "FALL")); fallSeedEnabled = true; }
            }

            if (hasForageToFarm && api != null)
            {
                if (config.UseCave_Carrot_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cave Carrot Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseCave_Carrot_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cave Carrot Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCave_Carrot_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Cave Carrot Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseChanterelle_Mushroom) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Chanterelle Mushroom Spores"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCoconut) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Coconut Seed"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseCommon_Mushroom_SPRING) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Common Mushroom Spores"], "SPRING")); springSeedEnabled = true; }
                if (config.UseCommon_Mushroom_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Common Mushroom Spores"], "FALL")); fallSeedEnabled = true; }
                if (config.UseCrocus) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Crocus Seeds"], "WINTER")); }
                if (config.UseCrystal_Fruit) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Crystal Fruit Seeds"], "WINTER")); }
                if (config.UseDaffodil) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Daffodil Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseDandelion) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Dandelion Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseFiddlehead_Fern) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Fiddlehead Fern Spores"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseHazelnut) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Hazelnut Seed"], "FALL")); fallSeedEnabled = true; }
                if (config.UseHolly) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Holly Seeds"], "WINTER")); }
                if (config.UseWild_Horseradish) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Wild Horseradish Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseLeek) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Leek Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseMorel_Mushroom) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Morel Mushroom Spores"], "SPRING")); springSeedEnabled = true; }
                if (config.UsePurple_Mushroom) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Purple Mushroom Spores"], "FALL")); fallSeedEnabled = true; }
                if (config.UseRed_Mushroom_SUMMER) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Red Mushroom Spores"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseRed_Mushroom_FALL) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Red Mushroom Spores"], "FALL")); fallSeedEnabled = true; }
                if (config.UseSalmonberry) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Salmonberry Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseSnow_Yam) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Snow Yam Seeds"], "WINTER")); }
                if (config.UseSpice_Berry) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sprice Berry Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseSpring_Onion) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Spring Onion Seeds"], "SPRING")); springSeedEnabled = true; }
                if (config.UseSweet_Pea) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Sweet Pea Seeds"], "SUMMER")); summerSeedEnabled = true; }
                if (config.UseWild_Blackberry) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Wild Blackberry Seeds"], "FALL")); fallSeedEnabled = true; }
                if (config.UseWild_Plum) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Wild Plum Seed"], "FALL")); fallSeedEnabled = true; }
                if (config.UseWinter_Root) { seeds.Add(new KeyValuePair<int, string>(integratedCrops["Winter Root Seeds"], "WINTER")); }
            }

            // Check that atleast one seed from each season is enabled
            if (!springSeedEnabled)
            {
                seeds.Add(new KeyValuePair<int, string>(474, "SPRING"));
                seeds.Add(new KeyValuePair<int, string>(472, "SPRING"));
                seeds.Add(new KeyValuePair<int, string>(475, "SPRING"));
            }
            if (!summerSeedEnabled)
            {
                seeds.Add(new KeyValuePair<int, string>(487, "SUMMER"));
                seeds.Add(new KeyValuePair<int, string>(482, "SUMMER"));
                seeds.Add(new KeyValuePair<int, string>(484, "SUMMER"));
                seeds.Add(new KeyValuePair<int, string>(483, "SUMMER"));
            }
            if (!fallSeedEnabled)
            {
                seeds.Add(new KeyValuePair<int, string>(487, "FALL"));
                seeds.Add(new KeyValuePair<int, string>(489, "FALL"));
                seeds.Add(new KeyValuePair<int, string>(488, "FALL"));
                seeds.Add(new KeyValuePair<int, string>(490, "FALL"));
            }

            // Print all available seeds to console
            foreach (var seed in seeds)
            {
                this.Monitor.Log($"List of available seeds include: {seed.Key}", LogLevel.Trace);
            }

            ILookup<int, string> lookup = seeds.ToLookup(kvp => kvp.Key, kvp => kvp.Value);

            return lookup;
        }
    }
}
