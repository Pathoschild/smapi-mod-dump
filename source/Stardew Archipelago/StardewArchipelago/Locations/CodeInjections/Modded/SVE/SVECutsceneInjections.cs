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
using System.Linq;
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.GameModifications.CodeInjections;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.CodeInjections.Modded.SVE
{
    public class SVECutsceneInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private const int AURORA_EVENT = 658059254;
        private const int MORGAN_EVENT = 658078924;
        private const string RAILROAD_KEY = "Clint2Again";
        private const int RAILROAD_BOULDER_ID = 8050108;
        private const int IRIDIUM_BOMB_ID = 8050109;
        private const string LANCE_CHEST = "Lance's Diamond Wand";
        private static readonly Dictionary<int, string> sveEventSpecialOrders = new(){
            {8050108, "Clint2"},
            {2551994, "Clint3"},
            {8033859, "Lewis2"},
            {2554903, "Robin3"},
            {2554928, "Robin4"},
            {7775926, "Apples"},
            {65360183, "MarlonFay2"},
            {65360186, "Lance"},
            {1090506, "Krobus"}
        };
        private static ShopMenu _lastShopMenuUpdated = null;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }
        
        // public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        public static bool CheckForAction_LanceChest_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (justCheckingForActivity || __instance.giftbox.Value || __instance.playerChest.Value)
                {
                    return true; // run original logic
                }

                if (__instance.items.Count is <= 0 or > 1 || __instance.items.First().Name != "Diamond Wand")
                {
                    return true; // run original logic
                }

                who.currentLocation.playSound("openChest");
                if (__instance.synchronized.Value)
                    __instance.GetMutex().RequestLock(() => __instance.openChestEvent.Fire());
                else
                    __instance.performOpenChest();

                var obj = __instance.items[0];
                __instance.items[0] = null;
                __instance.items.RemoveAt(0);
                __result = true;

                _locationChecker.AddCheckedLocation(LANCE_CHEST);

                return false; // don't run original logic

            }

            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForAction_LanceChest_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void endBehaviors(string[] split, GameLocation location)
        public static bool EndBehaviors_AddSpecialOrderAfterEvent_Prefix(string[] split, Event __instance)
        {
            try
            {
                if (!sveEventSpecialOrders.ContainsKey(__instance.id))
                {
                    return true; // run original logic
                }
                //Change the key so it doesn't get deleted
                var specialOrder = SpecialOrder.GetSpecialOrder(sveEventSpecialOrders[__instance.id], null);
                Game1.player.team.specialOrders.Add(specialOrder);

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(EndBehaviors_AddSpecialOrderAfterEvent_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // Railroad Boulder Special Order won't load if Iridium Bomb is sent early, so we duplicate it so the player gets it.
        // private static void UpdateSpecialOrders()
        public static bool UpdateSpecialOrders_StopDeletingSpecialOrders_Prefix()
        {
            try
            {
                return false; // we're not using this, its too strict.
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(UpdateSpecialOrders_StopDeletingSpecialOrders_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}