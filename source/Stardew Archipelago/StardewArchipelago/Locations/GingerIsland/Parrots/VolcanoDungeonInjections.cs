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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;

namespace StardewArchipelago.Locations.GingerIsland.Parrots
{
    public class VolcanoDungeonInjections
    {
        private const string AP_VOLCANO_BRIDGE_PARROT = "Volcano Bridge";
        private const string AP_VOLCANO_EXIT_SHORTCUT_PARROT = "Volcano Exit Shortcut";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public virtual void GenerateContents(bool use_level_level_as_layout = false)
        public static void GenerateContents_ReplaceParrots_Postfix(VolcanoDungeon __instance, bool use_level_level_as_layout)
        {
            try
            {
                if (!Game1.IsMasterGame)
                {
                    return;
                }

                if (__instance.level.Value == 0)
                {
                    __instance.parrotUpgradePerches.Clear();
                    AddVolcanoBridgeParrot(__instance);
                }
                else if (__instance.level.Value == 5)
                {
                    __instance.parrotUpgradePerches.Clear();
                    AddVolcanoExitShortcutParrot(__instance);
                }
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GenerateContents_ReplaceParrots_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void AddVolcanoBridgeParrot(IslandLocation __instance)
        {
            __instance.parrotUpgradePerches.Add(new ParrotUpgradePerch(__instance,
                new Point(27, 39),
                new Rectangle(28, 34, 5, 4),
                5,
                PurchaseVolcanoBridgeParrot,
                IsVolcanoBridgeParrotPurchased,
                "VolcanoBridge",
                "reachedCaldera, Island_Turtle"));
        }

        private static void PurchaseVolcanoBridgeParrot()
        {
            _locationChecker.AddCheckedLocation(AP_VOLCANO_BRIDGE_PARROT);
        }

        private static bool IsVolcanoBridgeParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_VOLCANO_BRIDGE_PARROT);
        }

        private static void AddVolcanoExitShortcutParrot(IslandLocation __instance)
        {
            var shortcutOutPositionField =
                _modHelper.Reflection.GetField<Point>(typeof(VolcanoDungeon), "shortcutOutPosition");
            var shortcutOutPosition = shortcutOutPositionField.GetValue();
            __instance.parrotUpgradePerches.Add(new ParrotUpgradePerch(__instance,
                new Point(shortcutOutPosition.X, shortcutOutPosition.Y),
                new Rectangle(shortcutOutPosition.X - 1, shortcutOutPosition.Y - 1, 3, 3),
                5,
                PurchaseVolcanoExitShortcutParrot,
                IsVolcanoExitShortcutParrotPurchased,
                "VolcanoShortcutOut",
                "Island_Turtle"));
        }

        private static void PurchaseVolcanoExitShortcutParrot()
        {
            _locationChecker.AddCheckedLocation(AP_VOLCANO_EXIT_SHORTCUT_PARROT);
        }

        private static bool IsVolcanoExitShortcutParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_VOLCANO_EXIT_SHORTCUT_PARROT);
        }
    }
}
