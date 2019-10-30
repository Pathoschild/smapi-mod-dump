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
        public static void RemoveItemAnywhere(Type itemType)
        {
            foreach (Farmer player in Game1.getAllFarmers())
            {
                for (int index1 = player.Items.Count - 1; index1 >= 0; --index1)
                {
                    if (player.Items[index1] != null && player.Items[index1].GetType() == itemType)
                        player.removeItemFromInventory(player.Items[index1]);
                }
            }
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {
                foreach (KeyValuePair<Vector2, Object> pair in location.objects.Pairs)
                {
                    if (pair.Value != null)
                    {
                        if (pair.Value.GetType() == itemType)
                            location.objects.Remove(pair.Key);
                        if (pair.Value is Chest)
                        {
                            foreach (Item obj in ((NetList<Item, NetRef<Item>>)(pair.Value as Chest).items).ToList())
                            {
                                if (obj != null && obj.GetType() == itemType)
                                    (pair.Value as Chest).items.Remove(obj);
                            }
                        }

                        if (pair.Value.heldObject.Value != null && pair.Value.heldObject.Value.GetType() == itemType)
                            pair.Value.heldObject.Value = null;
                    }
                }
                foreach (Debris debri in location.debris.ToList())
                {
                    if (debri.item != null && debri.item.GetType() == itemType)
                        location.debris.Remove(debri);
                }
                if (location is Farm)
                {
                    foreach (Building building in (location as Farm).buildings)
                    {
                        if (building.indoors.Value != null)
                        {
                            foreach (KeyValuePair<Vector2, Object> pair in building.indoors.Value.objects.Pairs)
                            {
                                if (pair.Value != null)
                                {
                                    if (pair.Value.GetType() == itemType)
                                        location.objects.Remove(pair.Key);
                                    if (pair.Value is Chest)
                                    {
                                        foreach (Item obj in ((NetList<Item, NetRef<Item>>)(pair.Value as Chest).items).ToList())
                                        {
                                            if (obj != null && obj.GetType() == itemType)
                                                (pair.Value as Chest).items.Remove(obj);
                                        }
                                    }
                                }
                            }
                            foreach (Debris debri in building.indoors.Value.debris.ToList())
                            {
                                if (debri.item != null && debri.item.GetType() == itemType)
                                    location.debris.Remove(debri);
                            }
                            if (building.indoors.Value is DecoratableLocation)
                            {
                                foreach (Furniture furniture in (building.indoors.Value as DecoratableLocation).furniture)
                                {
                                    if (furniture.heldObject.Value != null && furniture.heldObject.Value.GetType() == itemType)
                                        furniture.heldObject.Value = null;
                                }
                            }
                        }
                        else if (building is Mill)
                        {
                            foreach (Item obj in ((NetList<Item, NetRef<Item>>)(building as Mill).output.Value.items).ToList())
                            {
                                if (obj != null && obj.GetType() == itemType)
                                    (building as Mill).output.Value.items.Remove(obj);
                            }
                        }
                        else if (building is JunimoHut)
                        {
                            foreach (Item obj in ((NetList<Item, NetRef<Item>>)(building as JunimoHut).output.Value.items).ToList())
                            {
                                if (obj != null && obj.GetType() == itemType)
                                    (building as JunimoHut).output.Value.items.Remove(obj);
                            }
                        }
                    }
                }
                else if (location is FarmHouse)
                {
                    foreach (Item obj in ((NetList<Item, NetRef<Item>>)(location as FarmHouse).fridge.Value.items).ToList())
                    {
                        if (obj != null && obj.GetType() == itemType)
                            (location as FarmHouse).fridge.Value.items.Remove(obj);
                    }
                }
                if (location is DecoratableLocation)
                {
                    foreach (Furniture furniture in (location as DecoratableLocation).furniture)
                    {
                        if (furniture.heldObject.Value != null && furniture.heldObject.Value.GetType() == itemType)
                            furniture.heldObject.Value = null;
                    }
                }
            }
        }
    }
}
