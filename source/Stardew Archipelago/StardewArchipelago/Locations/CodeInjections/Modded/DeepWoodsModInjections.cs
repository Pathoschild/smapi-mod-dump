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
using System.Reflection;
using Netcode;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewArchipelago.Archipelago;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public static class DeepWoodsModInjections
    {
        //private const string EXCALIBUR_AP_LOCATION = "Pull Excalibur From the Stone";
        private const string MEET_UNICORN_AP_LOCATION = "Pet the Deep Woods Unicorn";
        private const string DESTROY_HOUSE_AP_LOCATION = "Breaking Up Deep Woods Gingerbread House";
        private const string DESTROY_TREE_AP_LOCATION = "Chop Down a Deep Woods Iridium Tree";
        private const string FOUNTAIN_DRINK_LOCATION = "Drinking From Deep Woods Fountain";
        private const string TREASURE1_AP_LOCATION = "Deep Woods Trash Bin";
        private const string TREASURE2_AP_LOCATION = "Deep Woods Treasure Chest";
        private const string DEINFEST_AP_LOCATION = "Purify an Infested Lichtung";
        private const string WOODS_OBELISK_SIGILS = "Progressive Woods Obelisk Sigils";
        private const string WOODS_DEPTH_LOCATION = "The Deep Woods: Depth {0}";
        private const int LEVEL_STEP = 10;
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Chest chestOrTrashThatGrantedAPCheck = null;
        private static LargeTerrainFeature fountainThatGrantedAPCheck = null;


        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago,
            LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

         // Future Goal; Currently Unimplemented - Albrekka
        //public class ExcaliburStone : LargeTerrainFeature
        //public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        /*public static bool PerformUseAction_ExcaliburLocation_Prefix(LargeTerrainFeature __instance, Vector2 tileLocation, GameLocation location, bool __result)
        {
            try
            {
                var swordPulledOutField = _helper.Reflection.GetField<NetBool>(__instance, "swordPulledOut");
                var swordPulledOut = swordPulledOutField.GetValue();
                if (swordPulledOut.Value)
                    return false; //don't run original logic

                if (Game1.player.DailyLuck >= 0.07f
                    && Game1.player.LuckLevel >= 8
                    && Game1.player.MiningLevel >= 10
                    && Game1.player.ForagingLevel >= 10
                    && Game1.player.FishingLevel >= 10
                    && Game1.player.FarmingLevel >= 10
                    && Game1.player.CombatLevel >= 10
                    && (Game1.player.timesReachedMineBottom >= 1 || Game1.MasterPlayer.timesReachedMineBottom >= 1)
                    && Game1.getFarm().grandpaScore.Value >= 4
                    && (!Game1.player.mailReceived.Contains("JojaMember"))
                    && (Game1.player.hasCompletedCommunityCenter()))
                {
                    Game1.playSound("yoba");
                    _locationChecker.AddCheckedLocation(EXCALIBUR_AP_LOCATION);
                    swordPulledOut.Value = true;
                }
                else
                {
                    Game1.playSound("thudStep");
                    Game1.showRedMessage("It won't budge.");
                }
                return false; //don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformUseAction_ExcaliburLocation_Prefix)}:\n{ex}", LogLevel.Error);
                return true; //run original logic
            }
        }*/

        //public class DeepWoods
        //public DeepWoods(DeepWoods parent, int level, EnterDirection enterDir)
        public static void Constructor_WoodsDepthChecker_Postfix(GameLocation __instance, int level, Enum enterDir)
        {
            try
            {
                var deepWoodsSettingsType = AccessTools.TypeByName("DeepWoodsMod.DeepWoodsSettings");
                var deepWoodsStateDataType = AccessTools.TypeByName("DeepWoodsMod.DeepWoodsStateData");

                var deepWoodsStateProperty = deepWoodsSettingsType.GetProperty("DeepWoodsState", BindingFlags.Public | BindingFlags.Static);
                var deepWoodsState = deepWoodsStateProperty.GetValue(null);
                var lowestLevelReachedField = _helper.Reflection.GetField<int>(deepWoodsState, "lowestLevelReached");
                
                lowestLevelReachedField.SetValue(10 * _archipelago.GetReceivedItemCount(WOODS_OBELISK_SIGILS));
                var levelIndexedAt1 = level - 1;

                if (levelIndexedAt1 % LEVEL_STEP != 0)
                {
                    return;
                }

                if (_archipelago.SlotData.ElevatorProgression == ElevatorProgression.ProgressiveFromPreviousFloor &&
                    __instance == null)
                {
                    return;
                }

                _locationChecker.AddCheckedLocation(string.Format(WOODS_DEPTH_LOCATION, levelIndexedAt1));
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Constructor_WoodsDepthChecker_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
}


        //It makes the chime if you pet after reload, but not really a problem.  Also patches out being scared
        //since its highly likely by the time you check unicorn, you'll be moving too fast naturally from buffs. - Albrekka
        // public class Unicorn : Horse
        // public override bool checkAction(Farmer who, GameLocation l)
        public static bool CheckAction_PetUnicornLocation_Prefix(Horse __instance, Farmer who, GameLocation l, ref bool __result)
        {
            try
            {
                __result = true; //conclude with same value as original method
                var isPettedField = _helper.Reflection.GetField<NetBool>(__instance, "isPetted");
                var isPetted = isPettedField.GetValue();
                if (isPetted)
                {
                    return false; // don't run original logic
                }

                // I am not sure which of the following two lines truly does what we need. Maybe both? To be tested
                isPettedField.SetValue(new NetBool(true));
                // isPetted.Value = true;

                who.farmerPassesThrough = true;
                who.health = who.maxHealth;
                who.Stamina = who.MaxStamina;
                who.addedLuckLevel.Value = Math.Max(10, who.addedLuckLevel.Value);
                if (!_locationChecker.IsLocationChecked(MEET_UNICORN_AP_LOCATION))
                {
                    _locationChecker.AddCheckedLocation(MEET_UNICORN_AP_LOCATION);
                }

                l.playSoundAt("achievement", __instance.getTileLocation());
                l.playSoundAt("healSound", __instance.getTileLocation());
                l.playSoundAt("reward", __instance.getTileLocation());
                l.playSoundAt("secret1", __instance.getTileLocation());
                l.playSoundAt("shiny4", __instance.getTileLocation());
                l.playSoundAt("yoba", __instance.getTileLocation());

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_PetUnicornLocation_Prefix)}:\n{ex}", LogLevel.Error);
                return true; //run original logic
            }
        }

        // public class Unicorn : Horse
        // private void CheckScared()
        public static bool CheckScared_MakeUnicornLessScared_Prefix(Horse __instance)
        {
            try
            {
                var isPettedField = _helper.Reflection.GetField<NetBool>(__instance, "isPetted");
                var isPetted = isPettedField.GetValue();
                var isScaredField = _helper.Reflection.GetField<NetBool>(__instance, "isScared");
                var isScared = isScaredField.GetValue();    
            if (isScared || isPetted)
                return false;

            foreach (Farmer farmer in __instance.currentLocation.farmers)
            {
                if ((farmer.Position - __instance.Position).Length() < 512)
                {
                    if (farmer.running)
                    {
                        isScared.Value = true;
                        __instance.farmerPassesThrough = true;
                        Game1.player.team.sharedDailyLuck.Value = -0.12;
                        farmer.addedLuckLevel.Value = Math.Min(-10, farmer.addedLuckLevel.Value);
                        __instance.currentLocation.playSoundAt("thunder_small", __instance.getTileLocation());
                        __instance.currentLocation.playSoundAt("ghost", __instance.getTileLocation());
                        Game1.isRaining = true;
                        Game1.isLightning = true;
                        Game1.changeMusicTrack("rain");
                        return false;
                    }
                }
            }
                return false; //don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckScared_MakeUnicornLessScared_Prefix)}:\n{ex}", LogLevel.Error);
                return true; //run original logic
            }
        }

        //Later, should also make it so the spawned drops for chest and also gingerbread + tree
        //don't show up first time, but don't know how to just yet due to DeepWoods type reference in method call. - Albrekka
        // public class TreasureChest : Chest
        // public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        public static bool CheckForAction_TreasureChestLocation_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity = false)
        {
            try
            {
                if (justCheckingForActivity)
                    return false; //don't run original logic
                var isTrashCanField = _helper.Reflection.GetField<NetBool>(__instance, "isTrashCan");
                var isTrashCan = isTrashCanField.GetValue();
                if (_locationChecker.IsLocationNotChecked(isTrashCan.Value ? TREASURE1_AP_LOCATION : TREASURE2_AP_LOCATION))
                {
                    _locationChecker.AddCheckedLocation(isTrashCan.Value ? TREASURE1_AP_LOCATION : TREASURE2_AP_LOCATION);
                    Game1.playSound(isTrashCan.Value ? "trashcan" : "openChest");
                    chestOrTrashThatGrantedAPCheck = __instance;
                }
                if (chestOrTrashThatGrantedAPCheck != __instance)
                {
                    return true; //run original logic (all other treasure save first is vanilla)
                }
                return false; //don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForAction_TreasureChestLocation_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        // public class GingerBreadHouse : ResourceClump
        // private void PlayDestroyedSounds(GameLocation location)
        public static void PlayDestroyedSounds_GingerbreadLocation_Postfix(ResourceClump __instance, GameLocation location)
        {
            try
            {
                _locationChecker.AddCheckedLocation(DESTROY_HOUSE_AP_LOCATION);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PlayDestroyedSounds_GingerbreadLocation_Postfix)}:\n{ex}",
                    LogLevel.Error);
                return;
            }
        }

        // public class IridiumTree : ResourceClump
        // private void PlayDestroyedSounds(GameLocation location)
        public static void PlayDestroyedSounds_IridiumTreeLocation_Postfix(ResourceClump __instance, GameLocation location)
        {
            try
            {
                _locationChecker.AddCheckedLocation(DESTROY_TREE_AP_LOCATION);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PlayDestroyedSounds_IridiumTreeLocation_Postfix)}:\n{ex}",
                    LogLevel.Error);
                return;
            }
        }

        // public class InfestedTree: FruitTree
        // public void DeInfest()
        public static void Deinfest_DeinfestLocation_Postfix()
        {
            try
            {
                _locationChecker.AddCheckedLocation(DEINFEST_AP_LOCATION);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Deinfest_DeinfestLocation_Postfix)}:\n{ex}",
                    LogLevel.Error);
                return;
            }
        }

        // public class HealingFountain : LargeTerrainFeature
        // public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        public static bool PerformUseAction_HealingFountainLocation_Prefix(LargeTerrainFeature __instance, Vector2 tileLocation, GameLocation location,
            ref bool __result)
        {
            try
            {
                __result = true;
                
                if (_locationChecker.IsLocationMissingAndExists(FOUNTAIN_DRINK_LOCATION))
                {
                    var apMessage = "You drink the water... it tastes like a stale Burger King Meal...?";
                    _locationChecker.AddCheckedLocation(FOUNTAIN_DRINK_LOCATION);
                    location.playSoundAt("gulp", tileLocation);
                    DelayedAction.playSoundAfterDelay("yoba", 800, location, -1);
                    Game1.addHUDMessage(new HUDMessage(apMessage) { noIcon = true });
                    fountainThatGrantedAPCheck = __instance;
                    return false; //don't run original logic
                }

                if (fountainThatGrantedAPCheck != __instance)
                {
                    return true; //run original logic
                }

                return false; //don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformUseAction_HealingFountainLocation_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; //run original logic
            }
        }
    }
}