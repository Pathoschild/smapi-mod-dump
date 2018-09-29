
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

namespace CustomCrystalariumMod
{
    internal class ObjectOverrides
    {
        public static bool GetMinutesForCrystalarium(ref int whichGem, ref int __result)
        {
            if (DataLoader.CrystalariumData.ContainsKey(whichGem))
            {
                __result = DataLoader.CrystalariumData[whichGem];
                return false;
            }
            else
            {
                var itemCategory = new Object(whichGem, 1, false).Category;
                if (DataLoader.CrystalariumData.ContainsKey(itemCategory))
                {
                    __result = DataLoader.CrystalariumData[itemCategory];
                    return false;
                }
            }
            return true;
        }

        public static bool PerformObjectDropInAction(ref Object __instance, ref Item dropInItem, ref bool probe, ref Farmer who, ref bool __result)
        {
            if (dropInItem is Object object1)
            {
                if (!(__instance.heldObject.Value != null && !__instance.Name.Equals("Recycling Machine") &&
                      !__instance.Name.Equals("Crystalarium") ||object1 != null && (bool) (object1.bigCraftable.Value)))
                {
                    if (__instance.Name.Equals("Crystalarium"))
                    {
                        if ((__instance.heldObject.Value == null || __instance.heldObject.Value.ParentSheetIndex != object1.ParentSheetIndex) 
                             && (__instance.heldObject.Value == null || __instance.MinutesUntilReady > 0))
                        { 
                            int minutesUntilReady = 0;

                            Item currentObject = __instance.heldObject.Value;

                            if (DataLoader.CrystalariumData.ContainsKey(object1.ParentSheetIndex))
                            {
                                minutesUntilReady = DataLoader.CrystalariumData[object1.ParentSheetIndex];
                            }
                            else if (DataLoader.CrystalariumData.ContainsKey(object1.Category))
                            {
                                minutesUntilReady = DataLoader.CrystalariumData[object1.Category];
                            }
                            else if ((object1.Category == -2 || object1.Category == -12) && object1.ParentSheetIndex != 74)
                            {
                                minutesUntilReady = DataLoader.Helper.Reflection.GetMethod(__instance, "getMinutesForCrystalarium").Invoke<int>(object1.ParentSheetIndex);
                            }
                            else
                            {
                                return true;
                            }

                            if (__instance.bigCraftable.Value && !probe &&
                                (object1 != null && __instance.heldObject.Value == null))
                            {
                                __instance.scale.X = 5f;
                            }
                            __instance.heldObject.Value = (Object)object1.getOne();
                            if (!probe)
                            {
                                who.currentLocation.playSound("select");
                                __instance.MinutesUntilReady = minutesUntilReady;
                                if (DataLoader.ModConfig.GetObjectBackImmediately)
                                {
                                    __instance.MinutesUntilReady = 0;
                                    __instance.minutesElapsed(0, who.currentLocation);
                                }
                                else if (currentObject != null && DataLoader.ModConfig.GetObjectBackOnChange)
                                {
                                    who.addItemByMenuIfNecessary(currentObject.getOne());
                                }
                            }
                            
                            __result = true;
                            return false;
                        }
                    }

                }
            }

            return true;
        }

        public static bool PerformRemoveAction(ref Object __instance, ref Vector2 tileLocation)
        {
            if (__instance.Name == "Crystalarium")
            {
                if (__instance.heldObject.Value != null)
                {
                    Game1.createItemDebris(__instance.heldObject.Value.getOne(), tileLocation * 64f, (Game1.player.FacingDirection + 2) % 4, (GameLocation) null, -1);
                }
                
            }

            return true;
        }
    }
}
