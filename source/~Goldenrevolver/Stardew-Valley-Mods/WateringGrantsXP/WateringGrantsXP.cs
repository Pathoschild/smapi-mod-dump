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
    using Harmony;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using StardewValley.Tools;

    public class WateringGrantsXP : Mod
    {
        private static WateringGrantsXP mod;

        private WateringGrantsXPConfig config;

        private string key;

        public override void Entry(IModHelper helper)
        {
            mod = this;
            key = $"{this.ModManifest.UniqueID}/notWatered";

            config = Helper.ReadConfig<WateringGrantsXPConfig>();

            WateringGrantsXPConfig.VerifyConfigValues(config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { WateringGrantsXPConfig.SetUpModConfigMenu(config, this); };

            Helper.Events.GameLoop.DayEnding += delegate { CheckForUnwateredCrops(); };

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.performToolAction)),
               prefix: new HarmonyMethod(typeof(WateringGrantsXP), nameof(WateringGrantsXP.GiveWateringExp))
            );
        }

        public void DebugLog(object o)
        {
            this.Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        private static bool GiveWateringExp(HoeDirt __instance, ref Tool t)
        {
            try
            {
                double chance = mod.config.WateringChanceToGetXP / 100.0;

                if (t != null && t is WateringCan && __instance.state.Value == 0 && __instance.needsWatering() && !__instance.crop.dead)
                {
                    if (Game1.random.NextDouble() < chance)
                    {
                        if (__instance.crop.isWildSeedCrop() && mod.config.ForageSeedWateringGrantsForagingXP)
                        {
                            if (t.getLastFarmerToUse() != null)
                            {
                                t.getLastFarmerToUse().gainExperience(2, mod.config.WateringExperienceAmount);
                            }
                        }
                        else
                        {
                            if (t.getLastFarmerToUse() != null)
                            {
                                t.getLastFarmerToUse().gainExperience(0, mod.config.WateringExperienceAmount);
                            }
                        }
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                mod.Monitor.Log($"Failed in {nameof(GiveWateringExp)}:\n{ex}", LogLevel.Error);

                return true;
            }
        }

        private void CheckForUnwateredCrops()
        {
            if (!Context.IsMainPlayer || !config.CropsCanDieWithoutWater)
            {
                return;
            }

            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is HoeDirt dirt)
                    {
                        if (dirt.crop == null || dirt.crop.dead || dirt.state == 1)
                        {
                            if (dirt.modData.ContainsKey(key))
                            {
                                dirt.modData.Remove(key);
                            }
                        }
                        else if (dirt.needsWatering() && !dirt.crop.dead && dirt.state != 1)
                        {
                            CheckForCropDeath(dirt);
                        }
                    }
                }
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