/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace AnimalHusbandryMod.common
{
    public class ItemUtility
    {
        public static void RemoveModdedItemAnywhere(String itemKey)
        {
            foreach (Farmer player in Game1.getAllFarmers())
            {
                for (int index1 = player.Items.Count - 1; index1 >= 0; --index1)
                {
                    if (IsModdedItem(player.Items[index1], itemKey))
                        player.removeItemFromInventory(player.Items[index1]);
                }
            }
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {
                foreach (KeyValuePair<Vector2, Object> pair in location.objects.Pairs)
                {
                    if (pair.Value != null)
                    {
                        if (pair.Value.modData.ContainsKey(itemKey))
                            location.objects.Remove(pair.Key);
                        if (pair.Value is Chest chest)
                        {
                            foreach (Item obj in (chest.Items).ToList())
                            {
                                if (IsModdedItem(obj, itemKey))
                                    chest.Items.Remove(obj);
                            }
                        }
                        if (pair.Value.heldObject.Value is Chest autoGrabber)
                        {
                            var items = autoGrabber.Items;
                            foreach (Item obj in (autoGrabber.Items).ToList())
                            {
                                if (IsModdedItem(obj, itemKey))
                                    autoGrabber.Items.Remove(obj);
                            }
                        }

                        if (IsModdedItem(pair.Value.heldObject.Value, itemKey))
                            pair.Value.heldObject.Value = null;
                    }
                }
                foreach (Debris debri in location.debris.ToList())
                {
                    if (IsModdedItem(debri.item, itemKey))
                        location.debris.Remove(debri);
                }
                if (location is Farm farm)
                {
                    foreach (Building building in farm.buildings)
                    {
                        if (building.indoors.Value != null)
                        {
                            foreach (KeyValuePair<Vector2, Object> pair in building.indoors.Value.objects.Pairs)
                            {
                                if (pair.Value != null)
                                {
                                    if (pair.Value.modData.ContainsKey(itemKey))
                                        farm.objects.Remove(pair.Key);
                                    if (pair.Value is Chest)
                                    {
                                        foreach (Item obj in ((pair.Value as Chest).Items).ToList())
                                        {
                                            if (IsModdedItem(obj, itemKey))
                                                (pair.Value as Chest).Items.Remove(obj);
                                        }
                                    }
                                    if (pair.Value.heldObject.Value is Chest autoGrabber)
                                    {
                                        var items = autoGrabber.Items;
                                        foreach (Item obj in (autoGrabber.Items).ToList())
                                        {
                                            if (IsModdedItem(obj, itemKey))
                                                autoGrabber.Items.Remove(obj);
                                        }
                                    }
                                }
                            }
                            foreach (Debris debri in building.indoors.Value.debris.ToList())
                            {
                                if (IsModdedItem(debri.item, itemKey))
                                    farm.debris.Remove(debri);
                            }
                            if (building.indoors.Value is DecoratableLocation indoorDecoratableLocation)
                            {
                                foreach (Furniture furniture in indoorDecoratableLocation.furniture)
                                {
                                    if (IsModdedItem(furniture.heldObject.Value, itemKey))
                                        furniture.heldObject.Value = null;
                                }
                            }
                        }
                        else if (building is Building plainBuilding)
                        {
                            foreach (Chest objList in plainBuilding.buildingChests)
                            {
                                foreach (Item obj in objList.Items)
                                {
                                    if (IsModdedItem(obj, itemKey))
                                        objList.Items.Remove(obj);
                                }
                            }
                        }
                        else if (building is JunimoHut junimoHut)
                        {
                            foreach (Item obj in (junimoHut.GetOutputChest().Items).ToList())
                            {
                                if (IsModdedItem(obj, itemKey))
                                    junimoHut.GetOutputChest().Items.Remove(obj);
                            }
                        }
                    }
                }
                else if (location is DecoratableLocation decoratableLocation)
                {
                    foreach (Furniture furniture in decoratableLocation.furniture)
                    {
                        if (IsModdedItem(furniture.heldObject.Value, itemKey))
                            furniture.heldObject.Value = null;
                    }
                    if (location is FarmHouse farmHouse)
                    {
                        foreach (Item obj in (farmHouse.fridge.Value.Items).ToList())
                        {
                            if (IsModdedItem(obj, itemKey))
                                farmHouse.fridge.Value.Items.Remove(obj);
                        }
                    } 
                    else if (location is IslandFarmHouse islandFarmHouse)
                    {
                        foreach (Item obj in (islandFarmHouse.fridge.Value.Items).ToList())
                        {
                            if (IsModdedItem(obj, itemKey))
                                islandFarmHouse.fridge.Value.Items.Remove(obj);
                        }
                    }
                }
            }
        }

        public static bool IsModdedItem(Item item, string key)
        {
            return item != null && item.modData.ContainsKey(key);
        }

        public static bool HasModdedItem(string itemKey)
        {
            foreach (Farmer player in Game1.getAllFarmers())
            {
                if (player.Items.Any(i => ItemUtility.IsModdedItem(i, itemKey))) return true;
            }
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {
                foreach (KeyValuePair<Vector2, Object> pair in location.objects.Pairs)
                {
                    if (pair.Value != null)
                    {
                        if (pair.Value.modData.ContainsKey(itemKey)) return true;
                        if (pair.Value is Chest chest)
                        {
                            foreach (Item obj in (chest.Items).ToList())
                            {
                                if (IsModdedItem(obj, itemKey)) return true;
                            }
                        }
                        if (pair.Value.heldObject.Value is Chest autoGrabber)
                        {
                            foreach (Item obj in (autoGrabber.Items).ToList())
                            {
                                if (IsModdedItem(obj, itemKey)) return true;
                            }
                        }

                        if (IsModdedItem(pair.Value.heldObject.Value, itemKey))
                            pair.Value.heldObject.Value = null;
                    }
                }
                foreach (Debris debri in location.debris.ToList())
                {
                    if (IsModdedItem(debri.item, itemKey)) return true;
                }
                if (location is Farm farm)
                {
                    foreach (Building building in farm.buildings)
                    {
                        if (building.indoors.Value != null)
                        {
                            foreach (KeyValuePair<Vector2, Object> pair in building.indoors.Value.objects.Pairs)
                            {
                                if (pair.Value != null)
                                {
                                    if (pair.Value.modData.ContainsKey(itemKey)) return true;
                                    if (pair.Value is Chest chest)
                                    {
                                        foreach (Item obj in (chest.Items).ToList())
                                        {
                                            if (IsModdedItem(obj, itemKey)) return true;
                                        }
                                    }
                                    if (pair.Value.heldObject.Value is Chest autoGrabber)
                                    {
                                        foreach (Item obj in (autoGrabber.Items).ToList())
                                        {
                                            if (IsModdedItem(obj, itemKey)) return true;
                                        }
                                    }
                                }
                            }
                            foreach (Debris debri in building.indoors.Value.debris.ToList())
                            {
                                if (IsModdedItem(debri.item, itemKey)) return true;
                            }
                            if (building.indoors.Value is DecoratableLocation indoorDecoratableLocation)
                            {
                                foreach (Furniture furniture in indoorDecoratableLocation.furniture)
                                {
                                    if (IsModdedItem(furniture.heldObject.Value, itemKey)) return true;
                                }
                            }
                        }
                        else if (building is Building plainBuilding)
                        {
                            foreach (Item obj in (plainBuilding.buildingChests.SelectMany(i=>i.Items)).ToList())
                            {
                                if (IsModdedItem(obj, itemKey)) return true;
                            }
                        }
                        else if (building is JunimoHut junimoHut)
                        {
                            foreach (Item obj in (junimoHut.GetOutputChest().Items).ToList())
                            {
                                if (IsModdedItem(obj, itemKey)) return true;
                            }
                        }
                    }
                }
                else if (location is DecoratableLocation decoratableLocation)
                {
                    foreach (Furniture furniture in decoratableLocation.furniture)
                    {
                        if (IsModdedItem(furniture.heldObject.Value, itemKey)) return true;
                    }
                    if (location is FarmHouse farmHouse)
                    {
                        foreach (Item obj in (farmHouse.fridge.Value.Items).ToList())
                        {
                            if (IsModdedItem(obj, itemKey)) return true;
                        }
                    }
                    else if (location is IslandFarmHouse islandFarmHouse)
                    {
                        foreach (Item obj in (islandFarmHouse.fridge.Value.Items).ToList())
                        {
                            if (IsModdedItem(obj, itemKey)) return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
