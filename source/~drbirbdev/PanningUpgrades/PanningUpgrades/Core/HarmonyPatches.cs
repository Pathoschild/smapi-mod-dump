/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using BirbCore.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

namespace PanningUpgrades;

class PanUtility
{
    public static Hat PanToHat(Pan pan)
    {
        return pan.UpgradeLevel switch
        {
            0 => (Hat)ItemRegistry.Create("(H)drbirbdev.PanningUpgrades_NormalPanHat"),
            1 => (Hat)ItemRegistry.Create("(H)71"),
            2 => (Hat)ItemRegistry.Create("(H)drbirbdev.PanningUpgrades_SteelPanHat"),
            3 => (Hat)ItemRegistry.Create("(H)drbirbdev.PanningUpgrades_GoldPanHat"),
            4 => (Hat)ItemRegistry.Create("(H)drbirbdev.PanningUpgrades_IridiumPanHat"),
            _ => (Hat)ItemRegistry.Create("(H)drbirbdev.PanningUpgrades_IridiumPanHat")
        };
    }

    public static Pan HatToPan(Hat hat)
    {
        return hat.QualifiedItemId switch
        {
            "(H)drbirbdev.PanningUpgrades_NormalPanHat" => (Pan)ItemRegistry.Create("(T)drbirbdev.PanningUpgrades_NormalPan"),
            "(H)71" => (Pan)ItemRegistry.Create("(T)Pan"),
            "(H)drbirbdev.PanningUpgrades_SteelPanHat" => (Pan)ItemRegistry.Create("(T)drbirbdev.PanningUpgrades_SteelPan"),
            "(H)drbirbdev.PanningUpgrades_GoldPanHat" => (Pan)ItemRegistry.Create("(T)drbirbdev.PanningUpgrades_GoldPan"),
            "(H)drbirbdev.PanningUpgrades_IridiumPanHat" => (Pan)ItemRegistry.Create("(T)drbirbdev.PanningUpgrades_IridiumPan"),
            _ => null
        };
    }
}

[HarmonyPatch(typeof(Pan), nameof(Pan.DoFunction))]
class Pan_DoFunction
{
    public static void Postfix(Pan __instance, GameLocation location, int x, int y, Farmer who)
    {
        try
        {
            List<Item> extraPanItems = new();
            float dailyLuck = (float)who.DailyLuck * ModEntry.Config.DailyLuckMultiplier;
            Log.Debug($"Daily Luck {who.DailyLuck} * Multiplier {ModEntry.Config.DailyLuckMultiplier} = Weighted Daily Luck {dailyLuck}");
            float buffLuck = who.LuckLevel * ModEntry.Config.LuckLevelMultiplier;
            Log.Debug($"Buff Luck {who.LuckLevel} * Multiplier {ModEntry.Config.LuckLevelMultiplier} = Weighted Buff Luck {buffLuck}");
            float chance = ModEntry.Config.ExtraDrawBaseChance + dailyLuck + buffLuck;
            Log.Debug($"Chance of Extra Draw {chance} = Base Chance {ModEntry.Config.ExtraDrawBaseChance} + Weighted Daily Luck {dailyLuck} + Weighted Buff Luck {buffLuck}");
            int panCount = 1;

            for (int i = 0; i < __instance.UpgradeLevel; i++)
            {
                if (chance > Game1.random.NextDouble())
                {
                    // location is used to seed the random number for selecting which treasure is received in vanilla.
                    // do something to reseed it so that we aren't just getting the same treasure up to 5 times.
                    location.orePanPoint.X++;
                    panCount++;
                    extraPanItems.AddRange(__instance.getPanItems(location, who));
                }
            }
            Log.Debug($"Did {panCount} draws using level {__instance.UpgradeLevel} pan.");

            who.addItemsByMenuIfNecessary(extraPanItems);
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

[HarmonyPatch(typeof(Tool), nameof(Tool.doesShowTileLocationMarker))]
class Tool_DoesShowTileLocationMarker
{
    public static void Postfix(ref bool __result, Tool __instance)
    {
        if (__instance is Pan)
        {
            __result = false;
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
            if (placedItem != null && placedItem is Pan pan)
            {
                __result = PanUtility.PanToHat(pan);
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
            if (heldItem != null && heldItem is Hat hat)
            {
                __result = PanUtility.HatToPan(hat);
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
        if (Game1.player.CursorSlotItem is Pan)
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
            if (__state is not null && __state[0] is Pan pan && __state[1] != Game1.player.hat.Value)
            {
                Game1.player.hat.Value = PanUtility.PanToHat(pan);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

[HarmonyPatch(typeof(FarmerSprite), nameof(FarmerSprite.getAnimationFromIndex))]
class FarmerSprite_GetAnimationFromIndex
{
    /// <summary>
    /// Use a TemporaryAnimatedSprite to make the panning animation reflect upgrade level.
    /// </summary>
    public static void Postfix(int index, FarmerSprite requester)
    {
        try
        {
            if (index == 303)
            {
                Farmer owner = Traverse.Create(requester).Field("owner").GetValue<Farmer>();
                if (owner is null)
                {
                    return;
                }



                int upgradeLevel = owner.CurrentTool.UpgradeLevel;
                int genderOffset = owner.IsMale ? -1 : 0;
                string texture = "Mods/drbirbdev.PanningUpgrades/PanTool";
                Rectangle sourceRect = new Rectangle(16, upgradeLevel * 16, 16, 16);
                GameLocation location = Game1.currentLocation;
                location.temporarySprites.Add(new TemporaryAnimatedSprite(
                    textureName: texture,
                    sourceRect: sourceRect,
                    animationInterval: ModEntry.Config.AnimationFrameDuration,
                    animationLength: 4,
                    numberOfLoops: 3,
                    position: owner.Position + new Vector2(0f, (ModEntry.Config.AnimationYOffset + genderOffset) * 4),
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
                    // TODO: figure out why endFunction doesn't get called in most multiplayer contexts.
                    endFunction = extraInfo =>
                    {
                        location.temporarySprites.Add(new TemporaryAnimatedSprite(
                            textureName: texture,
                            sourceRect: sourceRect,
                            animationInterval: ModEntry.Config.AnimationFrameDuration,
                            animationLength: 3,
                            numberOfLoops: 0,
                            position: owner.Position + new Vector2(0f, (ModEntry.Config.AnimationYOffset + genderOffset) * 4),
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
                                location.temporarySprites.Add(new TemporaryAnimatedSprite(
                                    textureName: texture,
                                    sourceRect: sourceRect,
                                    animationInterval: ModEntry.Config.AnimationFrameDuration * 2.5f,
                                    animationLength: 1,
                                    numberOfLoops: 0,
                                    position: owner.Position + new Vector2(0f, (ModEntry.Config.AnimationYOffset + genderOffset) * 4),
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

[HarmonyPatch(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AwardFestivalPrize))]
class Event_Command_AwardFestivalPrize
{
    /// <summary>
    /// Changes which pan tool is rewarded during events.
    /// </summary>
    public static bool Prefix(Event @event, string[] args)
    {
        try
        {
            if (args.Length > 1 && args[1].ToLower() == "pan")
            {
                Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)drbirbdev.PanningUpgrades_NormalPan"));
                if (Game1.activeClickableMenu == null)
                {
                    @event.CurrentCommand++;
                }
                @event.CurrentCommand++;
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

[HarmonyPatch(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.ItemAboveHead))]
class Event_Command_ItemAboveHead
{
    /// <summary>
    /// Changes which pan tool is shown being held during events.
    /// </summary>
    public static bool Prefix(Event @event, string[] args)
    {
        try
        {
            if (args.Length > 1 && args[1].Equals("pan"))
            {
                @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(T)drbirbdev.PanningUpgrades_NormalPan"));
                @event.CurrentCommand++;
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
            if (__instance.id == "404798")
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
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)drbirbdev.PanningUpgrades_NormalPan"));
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
