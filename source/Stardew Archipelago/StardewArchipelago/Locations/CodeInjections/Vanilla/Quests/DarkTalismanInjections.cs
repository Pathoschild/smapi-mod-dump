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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Quests
{
    public class DarkTalismanInjections
    {
        private const string DARK_TALISMAN = "Dark Talisman";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // protected override void resetLocalState()
        public static void ResetLocalState_PlayCutsceneIfConditionsAreMet_Postfix(Railroad __instance)
        {
            try
            {
                var darkTalismanEventId = 529952;
                if (!Game1.player.hasRustyKey || Game1.currentLocation is not Railroad || Game1.eventUp || __instance.currentEvent != null || Game1.farmEvent != null || Game1.player.eventsSeen.Contains(darkTalismanEventId))
                {
                    return;
                }

                var locationEvents = __instance.GetLocationEvents();
                if (locationEvents == null)
                {
                    return;
                }

                var darkTalismanEventKey = $"{darkTalismanEventId}/C";
                var darkTalismanEvent = new Event(locationEvents[darkTalismanEventKey], darkTalismanEventId);
                __instance.currentEvent = darkTalismanEvent;
                __instance.startEvent(__instance.currentEvent);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ResetLocalState_PlayCutsceneIfConditionsAreMet_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        public static bool CheckForAction_BuglandChest_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (justCheckingForActivity || __instance.giftbox.Value || __instance.playerChest.Value || Game1.currentLocation is not BugLand)
                {
                    return true; // run original logic
                }

                if (__instance.items.Count <= 0)
                {
                    return true; // run original logic
                }

                who.currentLocation.playSound("openChest");
                if (__instance.synchronized.Value)
                {
                    __instance.GetMutex().RequestLock(() => __instance.openChestEvent.Fire());
                }
                else
                {
                    __instance.performOpenChest();
                }

                var obj = __instance.items[0];
                __instance.items[0] = null;
                __instance.items.RemoveAt(0);
                __result = true;

                _locationChecker.AddCheckedLocation(DARK_TALISMAN);

                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForAction_BuglandChest_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // private void performRemoveHenchman()
        public static void PerformRemoveHenchman_CheckGoblinProblemLocation_Postfix(NPC __instance)
        {
            try
            {
                _locationChecker.AddCheckedLocation($"Goblin Problem");
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformRemoveHenchman_CheckGoblinProblemLocation_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public virtual bool checkAction(Farmer who, GameLocation l)
        public static void CheckAction_ShowWizardMagicInk_Postfix(NPC __instance, Farmer who, GameLocation l, ref bool __result)
        {
            try
            {
                if (!__instance.Name.Contains("Wizard", StringComparison.OrdinalIgnoreCase) || l is not WizardHouse ||
                    !who.hasMagicInk)
                {
                    return;
                }

                _locationChecker.AddCheckedLocation($"Magic Ink");
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_ShowWizardMagicInk_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public void setUpLocationSpecificFlair()
        public static bool SetUpLocationSpecificFlair_BuglandChest_Prefix(GameLocation __instance)
        {
            try
            {
                if (!__instance.Name.Equals("BugLand"))
                {
                    return true; // run original logic
                }

                if (_locationChecker.IsLocationNotChecked(DARK_TALISMAN) && __instance.isTileLocationTotallyClearAndPlaceable(31, 5))
                {
                    __instance.overlayObjects.Add(new Vector2(31f, 5f), new Chest(0, new List<Item>()
                    {
                        new SpecialItem(6),
                    }, new Vector2(31f, 5f))
                    {
                        Tint = Color.Gray,
                    });
                }
                   
                foreach (var character in __instance.characters)
                {
                    if (character is Grub grub)
                    {
                        grub.setHard();
                    }
                    else if (character is Fly fly)
                    {
                        fly.setHard();
                    }
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetUpLocationSpecificFlair_BuglandChest_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
