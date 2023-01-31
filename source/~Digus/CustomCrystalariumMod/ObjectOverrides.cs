/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/


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
            if (DataLoader.CrystalariumDataId.ContainsKey(whichGem))
            {
                __result = DataLoader.CrystalariumDataId[whichGem];
                return false;
            }
            else
            {
                var itemCategory = new Object(whichGem, 1, false).Category;
                if (DataLoader.CrystalariumDataId.ContainsKey(itemCategory))
                {
                    __result = DataLoader.CrystalariumDataId[itemCategory];
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
                      !__instance.Name.Equals("Crystalarium") && !ClonerController.HasCloner(__instance.Name) || object1 != null && (bool)(object1.bigCraftable.Value)))
                {
                    if (__instance.Name.Equals("Crystalarium") || ClonerController.HasCloner(__instance.Name))
                    {
                        if (__instance.heldObject.Value == null || (__instance.heldObject.Value.ParentSheetIndex != object1.ParentSheetIndex && __instance.MinutesUntilReady > 0))
                        {
                            int minutesUntilReady = 0;

                            Item currentObject = __instance.heldObject.Value;

                            if (__instance.Name.Equals("Crystalarium"))
                            {
                                if (DataLoader.CrystalariumDataId.ContainsKey(object1.ParentSheetIndex))
                                {
                                    minutesUntilReady = DataLoader.CrystalariumDataId[object1.ParentSheetIndex];
                                }
                                else if (DataLoader.CrystalariumDataId.ContainsKey(object1.Category))
                                {
                                    minutesUntilReady = DataLoader.CrystalariumDataId[object1.Category];
                                }
                                else if ((object1.Category == -2 || object1.Category == -12) && object1.ParentSheetIndex != 74)
                                {
                                    minutesUntilReady = DataLoader.Helper.Reflection.GetMethod(__instance, "getMinutesForCrystalarium").Invoke<int>(object1.ParentSheetIndex);
                                }
                                else
                                {
                                    return true;
                                }
                                if (DataLoader.ModConfig.BlockChange && currentObject != null)
                                {
                                    ShowBlockChangeDialog();
                                    __result = false;
                                    return false;
                                }
                            }
                            else if (ClonerController.GetCloner(__instance.Name) is CustomCloner cloner)
                            {
                                if (cloner.UsePfmForInput) return true;
                                var clonerMinutes = ClonerController.GetMinutesUntilReady(cloner, object1);
                                if (clonerMinutes.HasValue)
                                {
                                    minutesUntilReady = clonerMinutes.Value;
                                }
                                else
                                {
                                    return true;
                                }
                                if ((DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.BlockChange : cloner.BlockChange) && currentObject != null)
                                {
                                    ShowBlockChangeDialog();
                                    __result = false;
                                    return false;
                                }
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
                                if (__instance.Name.Equals("Crystalarium") || DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.GetObjectBackImmediately : ClonerController.GetCloner(__instance.Name).GetObjectBackImmediately)
                                {
                                    __instance.MinutesUntilReady = 0;
                                    __instance.minutesElapsed(0, who.currentLocation);
                                }
                                else if (currentObject != null && (__instance.Name.Equals("Crystalarium") || DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.GetObjectBackOnChange : ClonerController.GetCloner(__instance.Name).GetObjectBackOnChange))
                                {
                                    who.addItemByMenuIfNecessary(currentObject.getOne());
                                }
                                __instance.initializeLightSource(__instance.TileLocation, false);
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
                if (DataLoader.ModConfig.GetObjectBackOnChange && !DataLoader.ModConfig.GetObjectBackImmediately)
                {
                    if (__instance.heldObject.Value != null)
                    {
                        Game1.createItemDebris(__instance.heldObject.Value.getOne(), tileLocation * 64f, (Game1.player.FacingDirection + 2) % 4, (GameLocation)null, -1);
                    }
                }
            }
            else if (ClonerController.GetCloner(__instance.Name) is CustomCloner cloner)
            {
                if (DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.GetObjectBackOnChange && !DataLoader.ModConfig.GetObjectBackImmediately : cloner.GetObjectBackOnChange && !cloner.GetObjectBackImmediately)
                {
                    if (__instance.heldObject.Value != null)
                    {
                        Game1.createItemDebris(__instance.heldObject.Value.getOne(), tileLocation * 64f, (Game1.player.FacingDirection + 2) % 4, (GameLocation)null, -1);
                    }
                }
            }
            return true;
        }

        public static bool CheckForAction_prefix(ref Object __instance, out Object __state)
        {
            __state = __instance.heldObject.Value;
            return true;
        }

        public static void CheckForAction_postfix(ref Object __instance, Object __state, bool justCheckingForActivity)
        {
            if (ClonerController.GetCloner(__instance.Name) is CustomCloner cloner && __instance.Name != "Crystalarium" && __state != null && !justCheckingForActivity)
            {
                __instance.MinutesUntilReady = ClonerController.GetMinutesUntilReady(cloner, __state) ?? __instance.MinutesUntilReady;
                if (__instance.MinutesUntilReady != 0)
                {
                    __instance.heldObject.Value = (Object)__state.getOne();
                    __instance.initializeLightSource(__instance.TileLocation, false);
                }
            }
        }

        private static void ShowBlockChangeDialog()
        {
            string dialogue = DataLoader.I18N.Get("CustomCrystalarium.Dialogue.BlockChange");
            Game1.showRedMessage(dialogue);
        }
    }
}
