/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using System;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SullySDVcore;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewObject = StardewValley.Object;

namespace BetterTappers
{
    internal class Patcher
    {
        private static readonly Log log = BetterTappers.Instance.log;
        private static Config Config { get; set; } = BetterTappers.Config;

        public static void PatchAll()
        {
            var harmony = new Harmony(BetterTappers.UID);

            try
            {
                log.T(typeof(Patcher).GetMethods().Take(typeof(Patcher).GetMethods().Length - 4).Select(mi => mi.Name)
                .Aggregate("Applying Harmony patches:", (str, s) => $"{str}{Environment.NewLine}{s}"));

                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), "placementAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperPlacementAction))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "UpdateTapperProduct"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(PatchUpdateTapperProduct))
                );
            }
            catch (Exception e)
            {
                log.E("Error while trying to setup required patches:", e);
            }
            log.T("Patches applied successfully.");
        }

        /**
         * From StardewObject : public virtual bool placementAction(GameLocation location, int x, int y, Farmer who = null)
         */
        public static bool PatchTapperPlacementAction(ref StardewObject __instance, ref bool __result, ref GameLocation location, ref int x, ref int y, Farmer who = null)
        {
            try
            {
                Vector2 placementTile = new(x / 64, y / 64);
                if (who is not null)
                {
                    __instance.owner.Value = who.UniqueMultiplayerID;
                }
                else
                {
                    __instance.owner.Value = Game1.player.UniqueMultiplayerID;
                }

                switch (__instance.ParentSheetIndex)
                {
                    case 105:
                    case 264:
                        if (location.terrainFeatures.ContainsKey(placementTile) && location.terrainFeatures[placementTile] is Tree)
                        {
                            Tree tree = location.terrainFeatures[placementTile] as Tree;
                            if (tree.growthStage.Value >= 5 && !tree.stump.Value && !location.objects.ContainsKey(placementTile))
                            {
                                Tapper tapper_instance = new(__instance.TileLocation, __instance.ParentSheetIndex);
                                tapper_instance.CopyObjTapper(__instance);
                                tapper_instance.SetOwnerVal(__instance.owner.Value);
                                tapper_instance.heldObject.Value = null;
                                tapper_instance.TileLocation = placementTile;
                                location.objects.Add(placementTile, tapper_instance);
                                tree.tapped.Value = true;
                                tree.UpdateTapperProduct(tapper_instance);
                                location.playSound("axe");

                                __result = true;
                                return false;
                            }
                        }
                        __result = false;
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                log.E("There was an exception in PatchTapperPlacementAction", e);
                return true;
            }
        }

        /**
         * From the Tree object: public void UpdateTapperProduct(Object tapper_instance, Object previous_object = null)
         */
        public static void PatchUpdateTapperProduct(ref Tree __instance, StardewObject tapper_instance, StardewObject previous_object)
        {
            try
            {
                Tapper tapper;
                if (tapper_instance is not Tapper)
                {
                    tapper = new(tapper_instance.TileLocation, tapper_instance.ParentSheetIndex);
                    tapper.CopyObjTapper(tapper_instance);
                    tapper.SetOwnerVal(tapper_instance.owner.Value);
                    tapper.heldObject.Value = tapper_instance.heldObject.Value;
                    tapper_instance = tapper;
                }
                else {
                    tapper = (Tapper)tapper_instance;
                }
                tapper.Config = BetterTappers.Config;

                //If the previous object wasn't null, then the tapper should have been harvested rather than just placed
                if (previous_object is not null)
                {
                    tapper.TimesHarvested++;
                    log.D("New times harvested: " + tapper.TimesHarvested, Config.DebugMode);

                    Farmer who = null;
                    if (tapper.TmpUMID is -1)
                    {
                        if (Config.AllowAutomatedXP)
                        {
                            who = Game1.getFarmer(tapper.owner.Value);
                        }
                    }
                    else
                    {
                        who = Game1.getFarmer(tapper.TmpUMID);
                    }
                    tapper.TmpUMID = -1;

                    if (who is not null && !Config.DisableAllModEffects)
                    {
                        log.D("Farmer getting XP: " + who.Name, Config.DebugMode);
                        who.gainExperience(2, Math.Max(Config.TapperXP, 0));
                    }
                    else
                    {
                        log.D("No xp awarded", Config.DebugMode);
                    }
                }
                else
                {
                    log.D("Tapper placed; don't increment, no xp", Config.DebugMode);
                }

                //Once product has been updated by the game, recalculate and apply a time based on mod configs
                log.D("Tapper original time: " + tapper.MinutesUntilReady, Config.DebugMode);
                int i = tapper.CalculateTapperMinutes(__instance.treeType.Value, tapper.ParentSheetIndex);
                if (i > 0) {
                    tapper.MinutesUntilReady = i;
                }
            }
            catch (Exception e)
            {
                log.E("There was an exception in PatchUpdateTapperProduct", e);
            }
        }
    }
}
