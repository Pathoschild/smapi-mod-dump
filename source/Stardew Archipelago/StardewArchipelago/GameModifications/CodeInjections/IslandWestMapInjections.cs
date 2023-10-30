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
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Locations;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class IslandWestMapInjections
    {

        private static IMonitor _monitor;
        private static IModHelper _modHelper;

        public static void PatchMapInjections(IMonitor monitor, IModHelper helper, Harmony harmony)
        {
            _monitor = monitor;
            _modHelper = helper;
            harmony.Patch(
                original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.ApplyFarmHouseRestore)),
                prefix: new HarmonyMethod(typeof(IslandWestMapInjections), nameof(ApplyFarmHouseRestore_RestoreOnlyCorrectParts_Prefix)));
        }

        // public void ApplyFarmHouseRestore()
        public static bool ApplyFarmHouseRestore_RestoreOnlyCorrectParts_Prefix(IslandWest __instance)
        {
            try
            {
                if (__instance.map == null)
                {
                    return false; // don't run original logic;
                }

                // protected HashSet<string> _appliedMapOverrides;
                var appliedMapOverridesField = _modHelper.Reflection.GetField<HashSet<string>>(__instance, "_appliedMapOverrides");
                var appliedMapOverrides = appliedMapOverridesField.GetValue();
                if (__instance.farmhouseRestored.Value && !appliedMapOverrides.Contains("Island_House_Restored"))
                {
                    __instance.ApplyMapOverride("Island_House_Restored", destination_rect: new Rectangle(74, 33, 7, 9));
                    __instance.ApplyMapOverride("Island_House_Bin", destination_rect: new Rectangle(__instance.shippingBinPosition.X, __instance.shippingBinPosition.Y - 1, 2, 2));
                    __instance.ApplyMapOverride("Island_House_Cave", destination_rect: new Rectangle(95, 30, 3, 4));
                }

                if (__instance.farmhouseMailbox.Value)
                {
                    __instance.setMapTileIndex(81, 40, 771, "Buildings");
                    __instance.setMapTileIndex(81, 39, 739, "Front");
                    __instance.setTileProperty(81, 40, "Buildings", "Action", "Mailbox");
                }

                return false; // don't run original logic;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ApplyFarmHouseRestore_RestoreOnlyCorrectParts_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic;
            }
        }
    }
}
