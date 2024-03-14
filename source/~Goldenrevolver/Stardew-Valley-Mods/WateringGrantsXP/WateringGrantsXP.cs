/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace WateringGrantsXP
{
    using HarmonyLib;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Locations;
    using StardewValley.Objects;
    using StardewValley.TerrainFeatures;
    using StardewValley.Tools;

    public class WateringGrantsXP : Mod
    {
        private static WateringGrantsXP mod;

        private WateringGrantsXPConfig config;

        private string key;

        private const string unqualifiedQiFruitId = "889";

        public override void Entry(IModHelper helper)
        {
            mod = this;
            key = $"{this.ModManifest.UniqueID}/notWatered";

            config = Helper.ReadConfig<WateringGrantsXPConfig>();

            WateringGrantsXPConfig.VerifyConfigValues(config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { WateringGrantsXPConfig.SetUpModConfigMenu(config, this); };

            Helper.Events.GameLoop.DayEnding += delegate { CheckForUnwateredCrops(); };

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.performToolAction)),
               prefix: new HarmonyMethod(typeof(WateringGrantsXP), nameof(WateringGrantsXP.GiveWateringExp)));

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.CanRefillWateringCanOnTile)),
               postfix: new HarmonyMethod(typeof(WateringGrantsXP), nameof(WateringGrantsXP.CantRefillWateringCanWithSaltWater)));
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public static void CantRefillWateringCanWithSaltWater(GameLocation __instance, ref bool __result, int tileX, int tileY)
        {
            if (__result && mod.config.CantRefillCanWithSaltWater)
            {
                if ((__instance.doesTileHaveProperty(tileX, tileY, "Water", "Back") != null || __instance.doesTileHaveProperty(tileX, tileY, "WaterSource", "Back") != null)
                    && (__instance is Beach || __instance.catchOceanCrabPotFishFromThisSpot(tileX, tileY)))
                {
                    __result = false;
                }
            }
        }

        private static bool GiveWateringExp(HoeDirt __instance, ref Tool t)
        {
            double chance = mod.config.WateringChanceToGetXP / 100.0;

            if (t is WateringCan && __instance.state.Value == HoeDirt.dry && __instance.needsWatering() && __instance.crop != null && !__instance.crop.dead.Value)
            {
                if (Game1.random.NextDouble() < chance)
                {
                    if (t.getLastFarmerToUse() != null)
                    {
                        bool shouldBoostForaging = __instance.crop.isWildSeedCrop() && mod.config.ForageSeedWateringGrantsForagingXP;

                        var skillToBoost = shouldBoostForaging ? Farmer.foragingSkill : Farmer.farmingSkill;

                        t.getLastFarmerToUse().gainExperience(skillToBoost, mod.config.WateringExperienceAmount);
                    }
                }
            }

            return true;
        }

        private void CheckForUnwateredCrops()
        {
            if (!Context.IsMainPlayer || !config.CropsCanDieWithoutWater)
            {
                return;
            }

            Utility.ForEachLocation(delegate (GameLocation location)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is HoeDirt dirt)
                    {
                        HandleHoeDirt(dirt);
                    }
                }

                if (config.WitheringAlsoChecksGardenPots)
                {
                    foreach (var locObject in location.Objects.Values)
                    {
                        if (locObject is IndoorPot pot && pot.hoeDirt.Value != null)
                        {
                            HandleHoeDirt(pot.hoeDirt.Value);
                        }
                    }
                }

                return true;
            });
        }

        private void HandleHoeDirt(HoeDirt dirt)
        {
            if (dirt.crop == null || dirt.crop.dead.Value || dirt.isWatered())
            {
                if (dirt.modData.ContainsKey(key))
                {
                    dirt.modData.Remove(key);
                }
            }
            else if (dirt.needsWatering() && !dirt.crop.dead.Value && !dirt.isWatered()
                && dirt.crop.indexOfHarvest.Value != unqualifiedQiFruitId)
            {
                CheckForCropDeath(dirt);
            }
        }

        private void CheckForCropDeath(HoeDirt dirt)
        {
            if (!dirt.modData.ContainsKey(key))
            {
                dirt.modData[key] = "|";
            }
            else
            {
                dirt.modData[key] += "|";
            }

            int daysWithoutWatering = dirt.modData[key].Length;

            if (daysWithoutWatering >= config.DaysWithoutWaterForChanceToDie)
            {
                double chance = mod.config.ChanceToDieWhenLeftForTooLong / 100.0;

                if (Game1.random.NextDouble() < chance)
                {
                    dirt.crop.Kill();
                    dirt.modData.Remove(key);
                }
            }
        }
    }
}