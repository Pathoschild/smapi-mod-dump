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
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Machines;
using StardewValley.Tools;
using xTile.Dimensions;

namespace CustomCrystalariumMod
{
    internal class ObjectOverrides
    {
        public static void OutputMachine(Object __instance, bool probe, Item inputItem, ref bool __result)
        {
            if (__instance.QualifiedItemId == DataLoader.VanillaClonerQualifiedItemId && !probe)
            {
                if (DataLoader.ModConfig.KeepQuality)
                {
                    __instance.heldObject.Value.Quality = inputItem.Quality;
                }
                if (DataLoader.CrystalariumDataId.TryGetValue(inputItem.QualifiedItemId, out var value))
                {
                    __instance.MinutesUntilReady = value;
                }
                else
                {
                    var itemCategory = inputItem.Category.ToString();
                    if (DataLoader.CrystalariumDataId.TryGetValue(itemCategory, out value))
                    {
                        __instance.MinutesUntilReady = value;
                    }
                    else if (DataLoader.ModConfig.EnableCrystalariumCloneEveryObject && !inputItem.HasTypeBigCraftable())
                    {
                        __instance.MinutesUntilReady = DataLoader.ModConfig.DefaultCloningTime;
                    }
                }
            }               
            return;
        }

        public static bool PerformObjectDropInAction(ref Object __instance, ref Item dropInItem, ref bool probe, ref Farmer who, ref bool returnFalseIfItemConsumed, ref bool __result)
        {
            if (dropInItem is Object object1 && !__instance.isTemporarilyInvisible && !__instance.IsSprinkler())
            {
                if (IsCrystalarium(__instance) || ClonerController.HasCloner(__instance.QualifiedItemId))
                {
                    if (__instance.heldObject.Value == null || (__instance.heldObject.Value.ItemId != object1.ItemId && __instance.MinutesUntilReady > 0))
                    {
                        int minutesUntilReady = 0;

                        Item currentObject = __instance.heldObject.Value;

                        if (IsCrystalarium(__instance))
                        {
                            if (dropInItem.QualifiedItemId == "(O)872" && Object.autoLoadFrom == null)
                            {
                                return true;
                            }
                            if (DataLoader.CrystalariumDataId.TryGetValue(object1.QualifiedItemId, out var value))
                            {
                                minutesUntilReady = value;
                            }
                            else if (DataLoader.CrystalariumDataId.TryGetValue(object1.Category.ToString(), out value))
                            {
                                minutesUntilReady = value;
                            }
                            else if (object1.Category is -2 or -12 && object1.ParentSheetIndex != 74)
                            {
                                MachineData machineData = __instance.GetMachineData();
                                minutesUntilReady = (int)Utility.ApplyQuantityModifiers(machineData.OutputRules.Find(r=> r.Id== "Default").MinutesUntilReady, __instance.GetMachineData().ReadyTimeModifiers, __instance.GetMachineData().ReadyTimeModifierMode, __instance.Location, who, object1, object1);
                            }
                            else if (DataLoader.ModConfig.EnableCrystalariumCloneEveryObject && !object1.HasTypeBigCraftable())
                            {
                                minutesUntilReady = DataLoader.ModConfig.DefaultCloningTime;
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
                        else if (ClonerController.GetCloner(__instance.QualifiedItemId) is { } cloner)
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
                        if (!probe)
                        {
                            __instance.heldObject.Value = (Object)object1.getOne();
                            if ((IsCrystalarium(__instance) && !DataLoader.ModConfig.KeepQuality) || (ClonerController.GetCloner(__instance.QualifiedItemId) !=null && !ClonerController.GetCloner(__instance.QualifiedItemId).KeepQuality))
                            {
                                __instance.heldObject.Value.Quality = 0;
                            }
                            who.currentLocation.playSound("select");
                            __instance.MinutesUntilReady = minutesUntilReady;
                            Object.ConsumeInventoryItem(who, dropInItem, 1);
                            if (IsCrystalarium(__instance) || DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.GetObjectBackImmediately : ClonerController.GetCloner(__instance.QualifiedItemId).GetObjectBackImmediately)
                            {
                                __instance.MinutesUntilReady = 0;
                                __instance.minutesElapsed(0);
                            }
                            else if (currentObject != null && (IsCrystalarium(__instance) || DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.GetObjectBackOnChange : ClonerController.GetCloner(__instance.Name).GetObjectBackOnChange))
                            {
                                who.addItemByMenuIfNecessary(currentObject.getOne());
                            }
                            if (object1 != null)
                            {
                                __instance.lastInputItem.Value = object1.getOne();
                                __instance.lastInputItem.Value.Stack = 1;
                            }
                            else
                            {
                                __instance.lastInputItem.Value = null;
                            }
                            __instance.initializeLightSource(__instance.TileLocation, false);
                            var machineData = __instance.GetMachineData();
                            if (machineData?.LoadEffects != null)
                            {
                                foreach (MachineEffects effect in machineData.LoadEffects)
                                {
                                    if (__instance.PlayMachineEffect(effect, true))
                                    {
                                        break;
                                    }
                                }
                            }
                            MachineDataUtility.UpdateStats(machineData?.StatsToIncrementWhenLoaded, object1, 1);
                            __result = !returnFalseIfItemConsumed;
                        }
                        else
                        {
                            __result = true;
                        }
                        return false;
                    }
                }
                
            }
            return true;
        }

        public static bool PerformRemoveAction(ref Object __instance)
        {
            if (ClonerController.GetCloner(__instance.Name) is { } cloner)
            {
                if (__instance.heldObject.Value != null)
                {
                    if (DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? !DataLoader.ModConfig.GetObjectBackImmediately : !cloner.GetObjectBackImmediately)
                    {
                        __instance.Location.debris.Add(new Debris((Object) __instance.heldObject.Value, __instance.TileLocation * 64f + new Vector2(32f, 32f)));
                    }

                    __instance.heldObject.Value = null;
                }
            }
            return true;
        }

        public static bool PerformToolAction(ref Object __instance, Tool t)
        {
            if (__instance.isTemporarilyInvisible || t == null)
            {
                return true;
            }

            if (__instance.Type is "Crafting" && t is not MeleeWeapon && t.isHeavyHitter())
            {
                if ((bool)__instance.bigCraftable.Value && __instance.ParentSheetIndex == 21 && __instance.heldObject.Value != null)
                {
                    if (DataLoader.ModConfig.GetObjectBackImmediately && !__instance.readyForHarvest.Value)
                    {
                        __instance.heldObject.Value = null;
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
            if (ClonerController.GetCloner(__instance.QualifiedItemId) is { } cloner && __instance.Name != "Crystalarium" && __state != null && !justCheckingForActivity)
            {
                __instance.MinutesUntilReady = ClonerController.GetMinutesUntilReady(cloner, __state) ?? __instance.MinutesUntilReady;
                if (__instance.MinutesUntilReady != 0)
                {
                    __instance.heldObject.Value = (Object)__state.getOne();
                    __instance.initializeLightSource(__instance.TileLocation, false);
                }
            }
        }

        internal static bool TryApplyFairyDust(Object __instance, bool probe, ref bool __result)
        {
            if (__instance.GetMachineData() != null) return true;
            if (__instance.MinutesUntilReady <= 0) return true;
            if (ClonerController.GetCloner(__instance.QualifiedItemId) is null) return true;
            if (!probe)
            {
                Utility.addSprinklesToLocation(__instance.Location, (int)__instance.TileLocation.X,
                    (int)__instance.TileLocation.Y, 1, 2, 400, 40, Color.White);
                Game1.playSound("yoba");
                __instance.MinutesUntilReady = 0;
                __instance.minutesElapsed(0);
            }
            __result = true;
            return false;
        }

        private static void ShowBlockChangeDialog()
        {
            string dialogue = DataLoader.I18N.Get("CustomCrystalarium.Dialogue.BlockChange");
            Game1.showRedMessage(dialogue);
        }

        private static bool IsCrystalarium(Object instance)
        {
            return instance.QualifiedItemId.Equals(DataLoader.VanillaClonerQualifiedItemId);
        }
    }
}
