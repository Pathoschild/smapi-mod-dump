/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using StardewValley.Tools;
    using System;
    using StardewObject = StardewValley.Object;

    internal class QualityAndXPPatches
    {
        private static ForageFantasyConfig config;

        internal static void ApplyPatches(ForageFantasyConfig forageFantasyConfig, Harmony harmony)
        {
            config = forageFantasyConfig;

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), "CheckForActionOnMachine"),
               prefix: new HarmonyMethod(typeof(QualityAndXPPatches), nameof(PatchTapperAndMushroomBoxQuality)));

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), "CheckForActionOnMachine"),
               postfix: new HarmonyMethod(typeof(QualityAndXPPatches), nameof(GiveTapperAndMushroomBoxXP)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Bush), nameof(Bush.shake)),
               prefix: new HarmonyMethod(typeof(QualityAndXPPatches), nameof(DetectHarvestableBerryBush)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Bush), nameof(Bush.shake)),
               postfix: new HarmonyMethod(typeof(QualityAndXPPatches), nameof(FixBerryQuality)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Axe), nameof(Axe.DoFunction)),
               prefix: new HarmonyMethod(typeof(QualityAndXPPatches), nameof(CheckForTwigToAxe)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Axe), nameof(Axe.DoFunction)),
               postfix: new HarmonyMethod(typeof(QualityAndXPPatches), nameof(GiveXPForAxedTwig)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Tree), "performTreeFall"),
               prefix: new HarmonyMethod(typeof(QualityAndXPPatches), nameof(GiveBonusXPForStump)));
        }

        public static void GiveBonusXPForStump(Tree __instance, Tool t)
        {
            if (!__instance.stump.Value || config.SmallStumpBonusXPAmount <= 0)
            {
                return;
            }

            if (t != null && __instance.health.Value != -100f && t.getLastFarmerToUse().IsLocalPlayer)
            {
                t?.getLastFarmerToUse().gainExperience(Farmer.foragingSkill, config.SmallStumpBonusXPAmount);
            }
        }

        public static void CheckForTwigToAxe(GameLocation location, int x, int y, Farmer who, ref bool __state)
        {
            __state = false;

            if (config.TwigDebrisXPAmount <= 0)
            {
                return;
            }

            int tileX = x / 64;
            int tileY = y / 64;
            var toolTilePosition = new Vector2(tileX, tileY);

            if (location.Objects.TryGetValue(toolTilePosition, out var obj) && obj.Type != null && obj.IsTwig())
            {
                __state = true;
            }
        }

        public static void GiveXPForAxedTwig(GameLocation location, int x, int y, Farmer who, ref bool __state)
        {
            if (config.TwigDebrisXPAmount <= 0)
            {
                return;
            }

            int tileX = x / 64;
            int tileY = y / 64;
            var toolTilePosition = new Vector2(tileX, tileY);

            if (__state && !location.Objects.ContainsKey(toolTilePosition))
            {
                who.gainExperience(Farmer.foragingSkill, config.TwigDebrisXPAmount);
            }
        }

        public static void GiveTapperAndMushroomBoxXP(ref StardewObject __instance, ref Farmer who, ref bool justCheckingForActivity, ref bool __state, ref bool __result)
        {
            if (justCheckingForActivity || !__result)
            {
                return;
            }

            if (__state && !__instance.readyForHarvest.Value)
            {
                if (__instance.IsTapper())
                {
                    TapperAndMushroomQualityLogic.RewardTapperExp(config, who);
                }

                if (__instance.IsMushroomBox())
                {
                    TapperAndMushroomQualityLogic.RewardMushroomBoxExp(config, who);
                }
            }
        }

        public static void PatchTapperAndMushroomBoxQuality(ref StardewObject __instance, ref Farmer who, ref bool justCheckingForActivity, ref bool __state)
        {
            __state = false;

            if (justCheckingForActivity)
            {
                return;
            }

            if (__instance.readyForHarvest.Value && __instance.heldObject.Value != null)
            {
                if (__instance.IsTapper())
                {
                    // for XP after successful harvest
                    __state = true;

                    // if tapper quality feature is disabled
                    if (config.TapperQualityOptions <= 0 || config.TapperQualityOptions > 4)
                    {
                        return;
                    }

                    who.currentLocation.terrainFeatures.TryGetValue(__instance.TileLocation, out TerrainFeature terrain);

                    if (terrain is Tree tree)
                    {
                        __instance.heldObject.Value.Quality = TapperAndMushroomQualityLogic.DetermineTapperQuality(config, who, tree);
                    }

                    return;
                }

                if (__instance.IsMushroomBox())
                {
                    // for XP after successful harvest
                    __state = true;

                    if (config.MushroomBoxQuality)
                    {
                        __instance.heldObject.Value.Quality = ForageFantasy.DetermineForageQuality(who);
                    }
                }
            }
        }

        // set to high so it hopefully catches that the tileSheetOffset before some other mod wants to harvest this bush in prepatch
        // if other mods also define a __state variable of type bool they will have different values (aka harmony does not make us fight over the __state variable)
        [HarmonyPriority(Priority.High)]
        public static void DetectHarvestableBerryBush(ref Bush __instance, ref bool __state)
        {
            // tileSheetOffset == 1 means it currently has berries to harvest
            __state = BerryBushLogic.IsHarvestableBush(__instance) && __instance.tileSheetOffset.Value == 1;
        }

        // config calls are in ChangeBerryQualityAndGiveExp
        public static void FixBerryQuality(ref Bush __instance, ref bool __state, float ___maxShake)
        {
            // __state && tileSheetOffset == 0 means the bush was harvested between prepatch and this
            if (__state && BerryBushLogic.IsHarvestableBush(__instance) && __instance.tileSheetOffset.Value == 0)
            {
                if (___maxShake == (float)Math.PI / 128f)
                {
                    BerryBushLogic.ChangeBerryQualityAndGiveExp(__instance, config);
                }
            }
        }
    }
}