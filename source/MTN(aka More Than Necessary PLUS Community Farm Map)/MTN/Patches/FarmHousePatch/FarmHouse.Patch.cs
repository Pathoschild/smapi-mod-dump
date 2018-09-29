using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace MTN.Patches.FarmHousePatch
{
    //[HarmonyPatch(typeof(FarmHouse))]
    //[HarmonyPatch(new Type[] { typeof(string), typeof(string) })]
    class FarmHouseConstructorPatch
    {
        public static void Postfix(FarmHouse __instance)
        {
            if (__instance is Cabin && __instance.furniture.Count > 0) return;

            if (Memory.isCustomFarmLoaded)
            {
                if (Memory.loadedFarm.furnitureLayoutFromCanon != -1)
                {
                    switch (Memory.loadedFarm.furnitureLayoutFromCanon)
                    {
                        case 0:
                            __instance.furniture.Add(new Furniture(1120, new Vector2(5f, 4f)));
                            __instance.furniture.Last<Furniture>().heldObject.Value = new Furniture(1364, new Vector2(5f, 4f));
                            __instance.furniture.Add(new Furniture(1376, new Vector2(1f, 10f)));
                            __instance.furniture.Add(new Furniture(0, new Vector2(4f, 4f)));
                            __instance.furniture.Add(new TV(1466, new Vector2(1f, 4f)));
                            __instance.furniture.Add(new Furniture(1614, new Vector2(3f, 1f)));
                            __instance.furniture.Add(new Furniture(1618, new Vector2(6f, 8f)));
                            __instance.furniture.Add(new Furniture(1602, new Vector2(5f, 1f)));
                            if (!(__instance is Cabin)) {
                                __instance.furniture.Add(new Furniture(1792, Utility.PointToVector2(__instance.getFireplacePoint())));
                            }
                            __instance.objects.Add(new Vector2(3f, 7f), new Chest(0, new List<Item>
                            {
                                new StardewValley.Object(472, 15, false, -1, 0)
                            }, new Vector2(3f, 7f), true));
                            break;
                        case 1:
                            __instance.setWallpaper(11, -1, true);
                            __instance.setFloor(1, -1, true);
                            __instance.furniture.Add(new Furniture(1122, new Vector2(1f, 6f)));
                            __instance.furniture.Last<Furniture>().heldObject.Value = new Furniture(1367, new Vector2(1f, 6f));
                            __instance.furniture.Add(new Furniture(3, new Vector2(1f, 5f)));
                            __instance.furniture.Add(new TV(1680, new Vector2(5f, 4f)));
                            __instance.furniture.Add(new Furniture(1673, new Vector2(1f, 1f)));
                            __instance.furniture.Add(new Furniture(1673, new Vector2(3f, 1f)));
                            __instance.furniture.Add(new Furniture(1676, new Vector2(5f, 1f)));
                            __instance.furniture.Add(new Furniture(1737, new Vector2(6f, 8f)));
                            __instance.furniture.Add(new Furniture(1742, new Vector2(5f, 5f)));
                            if (!(__instance is Cabin)) {
                                __instance.furniture.Add(new Furniture(1792, Utility.PointToVector2(__instance.getFireplacePoint())));
                            }
                            __instance.furniture.Add(new Furniture(1675, new Vector2(10f, 1f)));
                            __instance.objects.Add(new Vector2(4f, 7f), new Chest(0, new List<Item>
                            {
                                new StardewValley.Object(472, 15, false, -1, 0)
                            }, new Vector2(4f, 7f), true));
                            break;
                        case 2:
                            __instance.setWallpaper(92, -1, true);
                            __instance.setFloor(34, -1, true);
                            __instance.furniture.Add(new Furniture(1134, new Vector2(1f, 7f)));
                            __instance.furniture.Last<Furniture>().heldObject.Value = new Furniture(1748, new Vector2(1f, 7f));
                            __instance.furniture.Add(new Furniture(3, new Vector2(1f, 6f)));
                            __instance.furniture.Add(new TV(1680, new Vector2(6f, 4f)));
                            __instance.furniture.Add(new Furniture(1296, new Vector2(1f, 4f)));
                            __instance.furniture.Add(new Furniture(1682, new Vector2(3f, 1f)));
                            __instance.furniture.Add(new Furniture(1777, new Vector2(6f, 5f)));
                            __instance.furniture.Add(new Furniture(1745, new Vector2(6f, 1f)));
                            if (!(__instance is Cabin)) {
                                __instance.furniture.Add(new Furniture(1792, Utility.PointToVector2(__instance.getFireplacePoint())));
                            }
                            __instance.furniture.Add(new Furniture(1747, new Vector2(5f, 4f)));
                            __instance.furniture.Add(new Furniture(1296, new Vector2(10f, 4f)));
                            __instance.objects.Add(new Vector2(4f, 7f), new Chest(0, new List<Item>
                            {
                                new StardewValley.Object(472, 15, false, -1, 0)
                            }, new Vector2(4f, 7f), true));
                            break;
                        case 3:
                            __instance.setWallpaper(12, -1, true);
                            __instance.setFloor(18, -1, true);
                            __instance.furniture.Add(new Furniture(1218, new Vector2(1f, 6f)));
                            __instance.furniture.Last<Furniture>().heldObject.Value = new Furniture(1368, new Vector2(1f, 6f));
                            __instance.furniture.Add(new Furniture(1755, new Vector2(1f, 5f)));
                            __instance.furniture.Add(new Furniture(1755, new Vector2(3f, 6f), 1));
                            __instance.furniture.Add(new TV(1680, new Vector2(5f, 4f)));
                            __instance.furniture.Add(new Furniture(1751, new Vector2(5f, 10f)));
                            __instance.furniture.Add(new Furniture(1749, new Vector2(3f, 1f)));
                            __instance.furniture.Add(new Furniture(1753, new Vector2(5f, 1f)));
                            __instance.furniture.Add(new Furniture(1742, new Vector2(5f, 5f)));
                            __instance.objects.Add(new Vector2(2f, 9f), new Chest(0, new List<Item>
                            {
                                new StardewValley.Object(472, 15, false, -1, 0)
                            }, new Vector2(2f, 9f), true));
                            if (!(__instance is Cabin)) {
                                __instance.furniture.Add(new Furniture(1794, Utility.PointToVector2(__instance.getFireplacePoint())));
                            }
                            break;
                        case 4:
                            __instance.setWallpaper(95, -1, true);
                            __instance.setFloor(4, -1, true);
                            __instance.furniture.Add(new TV(1680, new Vector2(1f, 4f)));
                            __instance.furniture.Add(new Furniture(1628, new Vector2(1f, 5f)));
                            __instance.furniture.Add(new Furniture(1393, new Vector2(3f, 4f)));
                            __instance.furniture.Last<Furniture>().heldObject.Value = new Furniture(1369, new Vector2(3f, 4f));
                            __instance.furniture.Add(new Furniture(1678, new Vector2(10f, 1f)));
                            __instance.furniture.Add(new Furniture(1812, new Vector2(3f, 1f)));
                            __instance.furniture.Add(new Furniture(1630, new Vector2(1f, 1f)));
                            if (!(__instance is Cabin)) {
                                __instance.furniture.Add(new Furniture(1794, Utility.PointToVector2(__instance.getFireplacePoint())));
                            }
                            __instance.furniture.Add(new Furniture(1811, new Vector2(6f, 1f)));
                            __instance.furniture.Add(new Furniture(1389, new Vector2(10f, 4f)));
                            __instance.objects.Add(new Vector2(4f, 7f), new Chest(0, new List<Item>
                            {
                                new StardewValley.Object(472, 15, false, -1, 0)
                            }, new Vector2(4f, 7f), true));
                            __instance.furniture.Add(new Furniture(1758, new Vector2(1f, 10f)));
                            break;
                    }

                    if (Memory.loadedFarm.furnitureList != null)
                    {
                        foreach (Furniture f in Memory.loadedFarm.furnitureList)
                        {
                            __instance.furniture.Add(f);
                        }
                    }

                    if (Memory.loadedFarm.objectList != null)
                    {
                        foreach (StardewValley.Object o in Memory.loadedFarm.objectList)
                        {
                            //__instance.objects.Add(o);
                        }
                    }
                }
            }
            return;
        }
    }
    
}
