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
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Stardew;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class CropsanityInjections
    {
        private static readonly string[] _cropsanityExceptions = new[]
        {
            ObjectIds.WEEDS, ObjectIds.SPRING_ONION, ObjectIds.ANCIENT_FRUIT, ObjectIds.FIBER, ObjectIds.QI_FRUIT,
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
                if (!__result && !__instance.fullyGrown.Value || __instance.indexOfHarvest.Value == null)
                {
                    return;
                }

                var itemId = __instance.indexOfHarvest.Value;

                if (itemId == ObjectIds.SUNFLOWER_SEEDS)
                {
                    itemId = ObjectIds.SUNFLOWER; // Sunflower instead of sunflower seeds
                }

                if (!_itemManager.ObjectExistsById(itemId))
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

        // public virtual void shake(Vector2 tileLocation, bool doEvenIfStillShaking)
        public static bool Shake_CheckCropsanityFruitTreeLocation_Prefix(FruitTree __instance, Vector2 tileLocation, bool doEvenIfStillShaking)
        {
            try
            {
                if (!__instance.fruit.Any())
                {
                    return true; // run original logic
                }

                var fruit = __instance.fruit.First();
                var apLocation = $"Harvest {fruit.Name}";

                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
                else
                {
                    _monitor.Log($"Unrecognized Cropsanity Tree Fruit Location: {fruit.Name} [{fruit.ItemId}]", LogLevel.Error);
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
