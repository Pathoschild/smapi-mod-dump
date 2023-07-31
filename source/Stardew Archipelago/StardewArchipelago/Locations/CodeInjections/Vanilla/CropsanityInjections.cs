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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Unlocks;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using xTile.Tiles;
using Rectangle = xTile.Dimensions.Rectangle;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class CropsanityInjections
    {
        private const int ANCIENT_FRUIT = 454;
        private const int FIBER = 771;

        private static readonly int[] _cropsanityExceptions = new[]
        {
            ANCIENT_FRUIT, FIBER
        };

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static StardewItemManager _itemManager;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager itemManager)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _itemManager = itemManager;
        }

        // public virtual bool harvest(int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester = null)
        public static void Harvest_CheckCropsanityLocation_Postfix(Crop __instance, int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester, ref bool __result)
        {
            try
            {
                if (!__result && !__instance.fullyGrown.Value)
                {
                    return;
                }

                var itemId = __instance.indexOfHarvest.Value;

                if (itemId == 431)
                {
                    itemId = 421; // Sunflower instead of sunflower seeds
                }

                if (!_itemManager.ObjectExists(itemId))
                {
                    _monitor.Log($"Unrecognized Cropsanity Crop: [{itemId}]", LogLevel.Error);
                    return;
                }

                var item = _itemManager.GetObjectById(itemId);
                var apLocation = $"Harvest {item.Name}";

                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
                else if (!_cropsanityExceptions.Contains(itemId))
                {
                    _monitor.Log($"Unrecognized Cropsanity Location: {item.Name} [{itemId}]", LogLevel.Error);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Harvest_CheckCropsanityLocation_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public virtual void shake(Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location)
        public static bool Shake_CheckCropsanityFruitTreeLocation_Prefix(FruitTree __instance, Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location)
        {
            try
            {
                if (__instance.fruitsOnTree.Value <= 0)
                {
                    return true; // run original logic
                }

                var itemId = __instance.indexOfFruit.Value;
                if (!_itemManager.ObjectExists(itemId))
                {
                    _monitor.Log($"Unrecognized Cropsanity Tree Fruit: [{itemId}]", LogLevel.Error);
                    return true; // run original logic
                }

                var item = _itemManager.GetObjectById(itemId);
                var apLocation = $"Harvest {item.Name}";

                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
                else // if (!_cropsanityExceptions.Contains(itemId))
                {
                    _monitor.Log($"Unrecognized Cropsanity Tree Fruit Location: {item.Name} [{itemId}]", LogLevel.Error);
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Shake_CheckCropsanityFruitTreeLocation_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
