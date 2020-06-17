using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Network;

namespace PufferEggsToMayonnaise
{
    internal class ObjectOverrides
    {
        [HarmonyPriority(500)]
        public static bool PerformObjectDropInAction(ref Object __instance, ref Item dropInItem, ref bool probe, ref Farmer who, ref bool __result)
        {
            if (dropInItem is Object object1)
            {
                if (!(__instance.heldObject.Value != null && !__instance.Name.Equals("Recycling Machine") &&
                      !__instance.Name.Equals("Mayonnaise Machine") || object1 != null && (bool)(object1.bigCraftable.Value)))
                {
                    if (__instance.name.Equals("Mayonnaise Machine"))
                    {
                        if (__instance.heldObject.Value == null)
                        {
                            if (object1.ParentSheetIndex == ModEntry.instance.lEggID)
                            {
                                // Checking whether the user has Json Assets installed and everything is OK with IDs first. They would get an error item otherwise!
                                if ((ModEntry.instance.mayoID == -1 || ModEntry.instance.mayoID == -2) && !probe)
                                {
                                    Game1.addHUDMessage(new HUDMessage("Oops! That's an error. See console for details.", HUDMessage.error_type));
                                    who.currentLocation.playSound("cancel");
                                    __result = false;
                                    return false;
                                }
                                __instance.heldObject.Value = new Object(Vector2.Zero, ModEntry.instance.mayoID, (string)null, false, true, false, false)
                                {
                                    Quality = 2
                                };
                                if (!probe)
                                {
                                    __instance.MinutesUntilReady = 180;
                                    who.currentLocation.playSound("Ship");
                                }
                                __result = true;
                                return false;
                            }
                            else if (object1.ParentSheetIndex == ModEntry.instance.eggID)
                            {
                                // Checking whether the user has Json Assets installed and everything is OK with IDs first. They would get an error item otherwise!
                                if ((ModEntry.instance.mayoID == -1 || ModEntry.instance.mayoID == -2) && !probe)
                                {
                                    Game1.addHUDMessage(new HUDMessage("Oops! That's an error. See console for details.", HUDMessage.error_type));
                                    who.currentLocation.playSound("cancel");
                                    __result = false;
                                    return false;
                                }
                                __instance.heldObject.Value = new Object(Vector2.Zero, ModEntry.instance.mayoID, (string)null, false, true, false, false);
                                if (!probe)
                                {
                                    __instance.MinutesUntilReady = 180;
                                    who.currentLocation.playSound("Ship");
                                }
                                __result = true;
                                return false;
                            }
                        }
                    }
                    else if (__instance.Name.Equals("Incubator"))
                    {
                        if (__instance.name.Equals("Incubator"))
                        {
                            if ((__instance.heldObject.Value == null && (object1.Category == -5 || Utility.IsNormalObjectAtParentSheetIndex((Item)object1, 107))) || (__instance.heldObject.Value == null && (object1.ParentSheetIndex == ModEntry.instance.eggID || object1.ParentSheetIndex == ModEntry.instance.lEggID)))
                            {
                                __instance.heldObject.Value = new Object(object1.ParentSheetIndex, 1, false, -1, 0);
                                if (!probe)
                                {
                                    who.currentLocation.playSound("coin", NetAudio.SoundContext.Default);
                                    __instance.MinutesUntilReady = 9000 * (object1.ParentSheetIndex == 107 ? 2 : 1);
                                    if (who.professions.Contains(2))
                                        __instance.MinutesUntilReady /= 2;
                                    if (object1.ParentSheetIndex == 180 || object1.ParentSheetIndex == 182 || object1.ParentSheetIndex == 305)
                                        __instance.ParentSheetIndex += 2;
                                    else
                                        ++__instance.ParentSheetIndex;

                                    __result = true;
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
