/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace BetterTappers
{
    internal class Patcher
    {
        private static readonly Log log = ModEntry.Instance.log;
        private static ModConfig Config { get; set; } = ModEntry.Config;

        public static void PatchAll()
        {
            var harmony = new Harmony(ModEntry.UID);

            try
            {
                log.T(typeof(Patcher).GetMethods().Take(typeof(Patcher).GetMethods().Length - 4).Select(mi => mi.Name)
                .Aggregate("Applying Harmony patches:", (str, s) => $"{str}{Environment.NewLine}{s}"));

                /*
                harmony.Patch(
                   original: AccessTools.Method(typeof(SObject), "placementAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperPlacementAction))
                );*/
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "UpdateTapperProduct"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(PatchUpdateTapperProduct))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(SObject), "checkForAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchCheckForAction))
                );
            }
            catch (Exception e)
            {
                log.E("Error while trying to setup required patches:", e);
            }
            log.T("Patches applied successfully.");
        }

        /**
         * From SObject : public virtual bool placementAction(GameLocation location, int x, int y, Farmer who = null)
         */
        /*
        public static bool PatchTapperPlacementAction(ref SObject __instance, ref bool __result, ref GameLocation location, ref int x, ref int y, Farmer who = null)
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
                    // For custom tapper class
                    case -9090909:
                        if (location.terrainFeatures.ContainsKey(placementTile) && location.terrainFeatures[placementTile] is Tree)
                        {
                            Tree tree = location.terrainFeatures[placementTile] as Tree;
                            if (tree.growthStage.Value >= 5 && !tree.stump.Value && !location.objects.ContainsKey(placementTile))
                            {
                                Tapper tapper_instance = new(__instance.TileLocation, __instance.ParentSheetIndex);
                                tapper_instance.CopyObjTapper(__instance);
                                tapper_instance.owner.Value = __instance.owner.Value;
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
        */

        /**
         * From Tree object: public void UpdateTapperProduct(Object tapper_instance, Object previous_object = null)
         */
        public static void PatchUpdateTapperProduct(ref Tree __instance, SObject tapper_instance, SObject previous_object)
        {
            try
            {
                // For custom tappers
                if (tapper_instance is Tapper)
                {

                }
                
                //If the previous object wasn't null, then the tapper should have been harvested rather than just placed
                if (previous_object is not null)
                {
                    // Increment times harvested for the tapper
                    CoreLogic.IncreaseTimesHarvested(tapper_instance);
                    log.D("New times harvested: " + CoreLogic.GetTimesHarvested(tapper_instance), Config.DebugMode);

                    // If tapper was not harvested by a player (i.e. automation) and auto xp is allowed, get the owner
                    Farmer who = null;
                    if (CoreLogic.GetTmpUMID(tapper_instance) is -1)
                    {
                        if (Config.AllowAutomatedXP)
                        {
                            who = Game1.getFarmerMaybeOffline(tapper_instance.owner.Value);
                        }
                    }
                    // Otherwise get the player who harvested
                    else
                    {
                        who = Game1.getFarmer(CoreLogic.GetTmpUMID(tapper_instance));
                    }
                    CoreLogic.SetTmpUMID(tapper_instance, -1);

                    // If harvester/owner were found, give them xp
                    if (who is not null && !Config.DisableAllModEffects)
                    {
                        log.D("Farmer getting " + Config.TapperXP + " XP: " + who.Name, Config.DebugMode);
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

                // Once product has been updated by the game, recalculate and set time based on mod configs
                log.D("Tapper original time: " + tapper_instance.MinutesUntilReady, Config.DebugMode);
                int i = CoreLogic.CalculateTapperMinutes(__instance.treeType.Value, tapper_instance.ParentSheetIndex);
                if (i > 0) {
                    tapper_instance.MinutesUntilReady = i;
                }
            }
            catch (Exception e)
            {
                log.E("There was an exception in PatchUpdateTapperProduct", e);
            }
        }

        /**
         * From SObject: public virtual bool checkForAction(Farmer who, bool justCheckingForActivity = false)
         */
        public static bool PatchCheckForAction(ref SObject __instance, ref bool __result, Farmer who, bool justCheckingForActivity = false)
        {
            if (__instance.isTemporarilyInvisible || justCheckingForActivity || !CoreLogic.IsAnyTapper(__instance))
            {
                return true;
            }
            if (!justCheckingForActivity && who is not null && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1) && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1) && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY()) && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY()) && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
            {
                __instance.performToolAction(null, who.currentLocation);
            }

            SObject objectThatWasHeld = __instance.heldObject.Value;

            if (__instance.readyForHarvest.Value)
            {
                if (who.isMoving())
                {
                    Game1.haltAfterCheck = false;
                }
                bool check_for_reload = false;

                Tree tree = null;
                if (who.IsLocalPlayer)
                {
                    __instance.heldObject.Value = null;

                    //Change quality value of objectThatWasHeld, then apply gatherer perk
                    int ogStackSize = objectThatWasHeld.Stack;
                    int ogQuality = objectThatWasHeld.Quality;
                    log.D("Og Stack Size: " + ogStackSize + "    Og Quality: " + ogQuality, Config.DebugMode);
                    if (who.currentLocation.terrainFeatures.ContainsKey(__instance.TileLocation) && who.currentLocation.terrainFeatures[__instance.TileLocation] is Tree)
                    {
                        tree = (who.currentLocation.terrainFeatures[__instance.TileLocation] as Tree);
                        if (tree.treeType.Value is not 8)
                        {
                            int quality = CoreLogic.GetQualityLevel(who, CoreLogic.GetTreeAgeMonths(tree), CoreLogic.GetTimesHarvested(__instance));
                            log.D("New quality: " + quality, Config.DebugMode);
                            objectThatWasHeld.Quality = quality;
                        }
                        objectThatWasHeld.Stack += CoreLogic.TriggerGathererPerk(who);
                        log.D("New Stack Size: " + objectThatWasHeld.Stack, Config.DebugMode);
                    }

                    if (!who.addItemToInventoryBool(objectThatWasHeld))
                    {
                        //if harvesting failed, reset quality of the ready item back to low and stack size back to 1
                        objectThatWasHeld.Quality = ogQuality;
                        objectThatWasHeld.Stack = ogStackSize;
                        __instance.heldObject.Value = objectThatWasHeld;
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                        return false;
                    }

                    Game1.playSound("coin");
                    check_for_reload = true;
                }

                //vanilla if statement moved up because quality needs to know if there's a tree. replaced with this check.
                if (tree is not null)
                {
                    CoreLogic.SetTmpUMID(__instance, who.UniqueMultiplayerID);
                    tree.UpdateTapperProduct(__instance, objectThatWasHeld);
                }

                __instance.readyForHarvest.Value = false;
                __instance.showNextIndex.Value = false;

                if (check_for_reload)
                {
                    __instance.AttemptAutoLoad(who);
                }
                __result = true;
                return false;
            }

            return true;
        }
    }
}
