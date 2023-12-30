/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class LegendaryFishInjections
    {
        private const int CURIOSITY_LURE = 856;
        private static IMonitor _monitor;

        public static void Initialize(IMonitor monitor)
        {
            _monitor = monitor;
        }

        // public override StardewValley.Object getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        public static bool GetFish_CrimsonfishAtBeach_PreFix(Beach __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref Object __result)
        {
            try
            {
                if (who.getTileX() < 82 || who.FishingLevel < 5 || waterDepth < 3 || (!Game1.currentSeason.Equals("summer") && !__instance.IsUsingMagicBait(who)))
                {
                    return true; // run original logic
                }

                var bonusLegendaryChance = GetCuriosityLureBonusChance(who, 0.07);
                if (HookedLegendaryFish(0.18, bonusLegendaryChance))
                {
                    __result = GetLegendaryFish(159, 898);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetFish_CrimsonfishAtBeach_PreFix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override StardewValley.Object getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        public static bool GetFish_AnglerInTown_PreFix(Town __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref Object __result)
        {
            try
            {
                if (bobberTile.X < 30.0 && bobberTile.Y < 30.0)
                {
                    return true; // run original logic
                } 
                if (who.getTileLocation().Y >= 15.0 || who.FishingLevel < 3 || (!Game1.currentSeason.Equals("fall") && !__instance.IsUsingMagicBait(who)))
                {
                    return true; // run original logic
                }

                var bonusLegendaryChance = GetCuriosityLureBonusChance(who, 0.05);
                if (HookedLegendaryFish(0.2, bonusLegendaryChance))
                {
                    __result = GetLegendaryFish(160, 899);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetFish_AnglerInTown_PreFix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override StardewValley.Object getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        public static bool GetFish_LegendAtMountain_PreFix(Mountain __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref Object __result)
        {
            try
            {
                var magicBait = __instance.IsUsingMagicBait(who);
                var correctSeason = Game1.currentSeason.Equals("spring") || magicBait;
                var correctWeather = Game1.isRaining || magicBait;
                if (who.FishingLevel < 10 || waterDepth < 4 || !correctSeason || !correctWeather)
                {
                    return true; // run original logic
                }

                var bonusLegendaryChance = GetCuriosityLureBonusChance(who, 0.1);
                if (HookedLegendaryFish(0.1, bonusLegendaryChance))
                {
                    __result = GetLegendaryFish(163, 900);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetFish_LegendAtMountain_PreFix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override StardewValley.Object getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        public static bool GetFish_GlacierfishInForest_PreFix(Forest __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref Object __result)
        {
            try
            {
                var magicBait = __instance.IsUsingMagicBait(who);
                var correctSeason = Game1.currentSeason.Equals("winter") || magicBait;
                if (who.getTileX() != 58 || who.getTileY() != 87 || who.FishingLevel < 6 || waterDepth < 3 || !correctSeason)
                {
                    return true; // run original logic
                }
                
                if (HookedLegendaryFish(0.5))
                {
                    __result = GetLegendaryFish(775, 902);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetFish_GlacierfishInForest_PreFix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override StardewValley.Object getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        public static bool GetFish_MutantCarpInSewer_PreFix(Sewer __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref Object __result)
        {
            try
            {
                var bonusLegendaryChance = GetCuriosityLureBonusChance(who, 0.1);
                if (who.getTileX() > 14 && who.getTileY() > 42)
                {
                    bonusLegendaryChance += 0.08;
                }

                if (HookedLegendaryFish(0.1, bonusLegendaryChance))
                {
                    __result = GetLegendaryFish(682, 901);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetFish_MutantCarpInSewer_PreFix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static bool HookedLegendaryFish(double normalLegendaryChance, double bonusLegendaryChance = 0.0)
        {
            return Game1.random.NextDouble() < normalLegendaryChance + bonusLegendaryChance;
        }

        private static double GetCuriosityLureBonusChance(Farmer who, double bonusChance)
        {
            var bonusLegendaryChance = 0.0;
            if (who is { CurrentTool: FishingRod fishingRod } && fishingRod.getBobberAttachmentIndex() == CURIOSITY_LURE)
            {
                bonusLegendaryChance += bonusChance;
            }

            return bonusLegendaryChance;
        }

        private static Object GetLegendaryFish(int legendaryFishId, int extendedFamilyFishId)
        {
            if (Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"))
            {
                return new Object(extendedFamilyFishId, 1);
            }

            // You can catch the legendary multiple times
            return new Object(legendaryFishId, 1);
        }
    }
}
