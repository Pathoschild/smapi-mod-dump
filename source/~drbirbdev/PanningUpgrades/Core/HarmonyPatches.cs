/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using BirbShared;
using System.Reflection;
using StardewValley.Tools;
using System.Reflection.Emit;

namespace PanningUpgrades
{
    [HarmonyPatch(typeof(Utility), nameof(Utility.getBlacksmithUpgradeStock))]
    class Utility_GetBlacksmithUpgradeStock
    {
        /// <summary>
        /// Tries to add cooking tool to Blacksmith shop stock.
        /// </summary>
        static void Postfix(
            Dictionary<ISalable, int[]> __result,
            Farmer who)
        {
            try
            {
                UpgradeablePan.AddToShopStock(itemPriceAndStock: __result, who: who);

            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(Farmer), nameof(Farmer.showHoldingItem))]
    class Farmer_ShowHoldingItem
    {

        /// <summary>
        /// Draws the correct tool sprite when receiving an upgrade.
        /// </summary>
        static bool Prefix(
            Farmer who)
        {
            try
            {
                if (who.mostRecentlyGrabbedItem is UpgradeablePan)
                {
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                        textureName: ModEntry.Assets.SpritesPath,
                        sourceRect: UpgradeablePan.IconSourceRectangle((who.mostRecentlyGrabbedItem as Tool).UpgradeLevel),
                        animationInterval: 2500f,
                        animationLength: 1,
                        numberOfLoops: 0,
                        position: who.Position + new Vector2(0f, -124f),
                        flicker: false,
                        flipped: false,
                        layerDepth: 1f,
                        alphaFade: 0f,
                        color: Color.White,
                        scale: 4f,
                        scaleChange: 0f,
                        rotation: 0f,
                        rotationChange: 0f)
                    {
                        motion = new Vector2(0f, -0.1f)
                    });
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Utility), nameof(Utility.getFishShopStock))]
    class Utility_GetFishShopStock
    {

        /// <summary>
        /// Removes the old Copper Pan tool from the fishing shop.
        /// </summary>
        public static void Postfix(Dictionary<ISalable, int[]> __result)
        {
            try
            {
                // Keying off of `new Pan()` doesn't work.
                // Iterate over items for sale, and remove any by the name "Copper Pan".
                foreach (ISalable key in __result.Keys)
                {
                    if (key.Name.Equals("Copper Pan"))
                    {
                        __result.Remove(key);
                    }
                }
                if (ModEntry.Config.BuyablePan)
                {
                    __result.Add(new UpgradeablePan(0), new int[2] { ModEntry.Config.BuyCost, 2147483647 });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(Utility), nameof(Utility.PerformSpecialItemPlaceReplacement))]
    class Utility_PerformSpecialItemPlaceReplacement
    {
        /// <summary>
        /// Handles using pan as a hat in certain menus.
        /// </summary>
        public static bool Prefix(
            ref Item __result,
            Item placedItem)
        {
            try
            {
                if (placedItem != null && placedItem is UpgradeablePan upgradeablePan)
                {
                    __result = UpgradeablePan.PanToHat(upgradeablePan);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Utility), nameof(Utility.PerformSpecialItemGrabReplacement))]
    class Utility_PerformSpecialItemGrabReplacement
    {

        /// <summary>
        /// Handles using pan as a hat in certain menus.
        /// </summary>
        public static bool Prefix(
            ref Item __result,
            Item heldItem)
        {
            try
            {
                if (heldItem != null && heldItem is Hat)
                {
                    int hatId = (int)(heldItem as Hat).which;
                    if (hatId == ModEntry.JsonAssets.GetHatId("Pan"))
                    {
                        __result = new UpgradeablePan(0);
                    }
                    else if (hatId == 71) // Using original copper pan hat.
                    {
                        __result = new UpgradeablePan(1);
                    }
                    else if (hatId == ModEntry.JsonAssets.GetHatId("Steel Pan"))
                    {
                        __result = new UpgradeablePan(2);
                    }
                    else if (hatId == ModEntry.JsonAssets.GetHatId("Gold Pan"))
                    {
                        __result = new UpgradeablePan(3);
                    }
                    else if (hatId == ModEntry.JsonAssets.GetHatId("Iridium Pan"))
                    {
                        __result = new UpgradeablePan(4);
                    }
                    else
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(InventoryPage), nameof(InventoryPage.receiveLeftClick))]
    class InventoryPage_ReceiveLeftClick
    {
        /// <summary>
        /// Handles pan to hat conversion in Inventory page.  Since there's no good entry point for patching,
        /// detects changes to player.hat.Value and player.CursorSlotItem using __state.
        /// </summary>
        public static void Prefix(ref Item[] __state)
        {
            if (Game1.player.CursorSlotItem is UpgradeablePan)
            {
                __state = new Item[] {
                    Game1.player.CursorSlotItem,
                    Game1.player.hat.Value,
                };
            }
        }

        /// <summary>
        /// Handles pan to hat conversion in Inventory page.  Since there's no good entry point for patching,
        /// detects changes to player.hat.Value and player.CursorSlotItem using __state.
        /// </summary>
        public static void Postfix(Item[] __state)
        {
            try
            {
                if (__state is not null && __state[0] is UpgradeablePan upgradeablePan && __state[1] != Game1.player.hat.Value)
                {
                    Game1.player.hat.Value = UpgradeablePan.PanToHat(upgradeablePan);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(Farmer), nameof(Farmer.toolPowerIncrease))]
    class Farmer_ToolPowerIncrease
    {
        static void Postfix(Farmer __instance)
        {
            if (__instance.CurrentTool is UpgradeablePan)
            {
                __instance.FarmerSprite.CurrentFrame = 123;
            }
        }
    }

    [HarmonyPatch(typeof(FarmerSprite), nameof(FarmerSprite.getAnimationFromIndex))]
    class FarmerSprite_GetAnimationFromIndex
    {
        /// <summary>
        /// Use a TemporaryAnimatedSprite to make the panning animation reflect upgrade level.
        /// </summary>
        public static void Postfix(int index)
        {
            try
            {
                if (index == 303)
                {
                    int upgradeLevel = Game1.player.CurrentTool.UpgradeLevel;
                    int genderOffset = Game1.player.IsMale ? -1 : 0;

                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                        textureName: ModEntry.Assets.SpritesPath,
                        sourceRect: UpgradeablePan.AnimationSourceRectangle(upgradeLevel),
                        animationInterval: ModEntry.Config.AnimationFrameDuration,
                        animationLength: 4,
                        numberOfLoops: 3,
                        position: Game1.player.Position + new Vector2(0f, (ModEntry.Config.AnimationYOffset + genderOffset) * 4),
                        flicker: false,
                        flipped: false,
                        layerDepth: 1f,
                        alphaFade: 0f,
                        color: Color.White,
                        scale: 4f,
                        scaleChange: 0f,
                        rotation: 0f,
                        rotationChange: 0f)
                    {
                        endFunction = extraInfo =>
                        {
                            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                                textureName: ModEntry.Assets.SpritesPath,
                                sourceRect: UpgradeablePan.AnimationSourceRectangle(upgradeLevel),
                                animationInterval: ModEntry.Config.AnimationFrameDuration,
                                animationLength: 3,
                                numberOfLoops: 0,
                                position: Game1.player.position + new Vector2(0f, (ModEntry.Config.AnimationYOffset + genderOffset) * 4),
                                flicker: false,
                                flipped: false,
                                layerDepth: 1f,
                                alphaFade: 0f,
                                color: Color.White,
                                scale: 4f,
                                scaleChange: 0f,
                                rotation: 0f,
                                rotationChange: 0f)
                            {
                                endFunction = extraInfo =>
                                {
                                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                                        textureName: ModEntry.Assets.SpritesPath,
                                        sourceRect: UpgradeablePan.AnimationSourceRectangle(upgradeLevel),
                                        animationInterval: ModEntry.Config.AnimationFrameDuration * 2.5f,
                                        animationLength: 1,
                                        numberOfLoops: 0,
                                        position: Game1.player.position + new Vector2(0f, (ModEntry.Config.AnimationYOffset + genderOffset) * 4),
                                        flicker: false,
                                        flipped: false,
                                        layerDepth: 1f,
                                        alphaFade: 0f,
                                        color: Color.White,
                                        scale: 4f,
                                        scaleChange: 0f,
                                        rotation: 0f,
                                        rotationChange: 0f));
                                }
                            });
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(Event), nameof(Event.command_awardFestivalPrize))]
    class Event_Command_AwardFestivalPrize
    {
        /// <summary>
        /// Changes which pan tool is rewarded during events.
        /// </summary>
        public static bool Prefix(Event __instance, string[] split)
        {
            try
            {
                if (split.Length > 1 && split[1].ToLower() == "pan")
                {
                    Game1.player.addItemByMenuIfNecessary(new UpgradeablePan());
                    if (Game1.activeClickableMenu == null)
                    {
                        __instance.CurrentCommand++;
                    }
                    __instance.CurrentCommand++;
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Event), nameof(Event.command_itemAboveHead))]
    class Event_Command_ItemAboveHead
    {
        /// <summary>
        /// Changes which pan tool is shown being held during events.
        /// </summary>
        public static bool Prefix(Event __instance, string[] split)
        {
            try
            {
                if (split.Length > 1 && split[1].Equals("pan"))
                {
                    __instance.farmer.holdUpItemThenMessage(new UpgradeablePan());
                    __instance.CurrentCommand++;
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Event), nameof(Event.skipEvent))]
    class Event_SkipEvent
    {
        /// <summary>
        /// Rewards modded pan tool if event is skipped.
        /// </summary>
        public static bool Prefix(
            Event __instance,
            Dictionary<string, Vector3> ___actorPositionsAfterMove)
        {
            try
            {
                if (__instance.id == 404798)
                {
                    // Generic skip logic copied from skipEvent.
                    // If other mods patch skipEvent to change this logic, things might break.
                    if (__instance.playerControlSequence)
                    {
                        __instance.EndPlayerControlSequence();
                    }
                    Game1.playSound("drumkit6");
                    ___actorPositionsAfterMove.Clear();
                    foreach (NPC i in __instance.actors)
                    {
                        bool ignore_stop_animation = i.Sprite.ignoreStopAnimation;
                        i.Sprite.ignoreStopAnimation = true;
                        i.Halt();
                        i.Sprite.ignoreStopAnimation = ignore_stop_animation;
                        __instance.resetDialogueIfNecessary(i);
                    }
                    __instance.farmer.Halt();
                    __instance.farmer.ignoreCollisions = false;
                    Game1.exitActiveMenu();
                    Game1.dialogueUp = false;
                    Game1.dialogueTyping = false;
                    Game1.pauseTime = 0f;

                    // Event specific skip logic.
                    if (Game1.player.getToolFromName("Pan") is null)
                    {
                        Game1.player.addItemByMenuIfNecessary(new UpgradeablePan());
                    }
                    __instance.endBehaviors(new string[1] { "end" }, Game1.currentLocation);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    // 3rd party
    // Fix Wear More Rings incompatibility
    [HarmonyPatch("StardewHack.WearMoreRings.ModEntry", "EquipmentClick")]
    class WearMoreRings_ModEntry_EquipmentClick
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("bcmpinc.WearMoreRings");
        }

        public static void Prefix(
            ClickableComponent icon
            )
        {
            try
            {
                if (icon.name == "Hat" && Game1.player.CursorSlotItem is UpgradeablePan pan)
                {
                    Game1.player.CursorSlotItem = UpgradeablePan.PanToHat(pan);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }

    }

    // Allow sending pan to upgrade in the mail with Mail Services
    [HarmonyPatch("MailServicesMod.ToolUpgradeOverrides", "mailbox")]
    class MailServicesMod_ToolUpgradeOverrides_Mailbox
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("Digus.MailServicesMod");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].Is(OpCodes.Isinst, typeof(Axe)))
                {
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(UpgradeablePan));
                    yield return code[i + 1];
                    yield return code[i + 2];
                    yield return code[i + 3];
                    yield return code[i];
                } else
                {
                    yield return code[i];
                }
            }
        }
    }
}
