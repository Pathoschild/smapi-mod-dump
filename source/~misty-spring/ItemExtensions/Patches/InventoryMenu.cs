/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Text;
using HarmonyLib;
using ItemExtensions.Events;
using ItemExtensions.Models;
using ItemExtensions.Models.Enums;
using ItemExtensions.Models.Internal;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Triggers;
using Object = StardewValley.Object;
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace ItemExtensions.Patches;

public static class InventoryPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    
    internal static void Apply(Harmony harmony)
    {
        if (ModEntry.Config.MenuActions)
        {
            Log($"Applying Harmony patch \"{nameof(InventoryPatches)}\": postfixing SDV method \"InventoryMenu.rightClick(int, int, Item, bool, bool)\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryMenu), nameof(InventoryMenu.rightClick)),
                postfix: new HarmonyMethod(typeof(InventoryPatches), nameof(Post_rightClick))
            );
        }

        if (ModEntry.Config.OnBehavior)
        {
            Log($"Applying Harmony patch \"{nameof(InventoryPatches)}\": postfixing SDV method \"InventoryMenu.rightClick(int, int, Item, bool, bool)\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryMenu), nameof(InventoryMenu.rightClick)),
                postfix: new HarmonyMethod(typeof(InventoryPatches), nameof(Post_Attachment))
            );
        }
    }

    /// <summary>
    /// Does item behavior actions.
    /// </summary>
    /// <param name="__instance">This menu instance</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="toAddTo">Item being held.</param>
    /// <param name="__result">The resulting item.</param>
    /// <param name="playSound">Sound to play.</param>
    /// <param name="onlyCheckToolAttachments"></param>
    /// <exception cref="ArgumentOutOfRangeException">If the price modifier isn't valid.</exception>
    /// <see cref="StardewValley.Preconditions.FreeInventorySlots"/>
    internal static void Post_Attachment(InventoryMenu __instance, int x, int y, ref Item toAddTo, ref Item __result, bool playSound = true, bool onlyCheckToolAttachments = false)
    {
        var affectedItem = __instance.getItemAt(x, y);
#if DEBUG
        var isTool = "is not tool";
        var attachmentResult = false;
        if (affectedItem is Tool debugTool)
        {
            attachmentResult = debugTool.canThisBeAttached((Object)toAddTo);
            isTool = "is tool";
        }
        Log($"{affectedItem?.QualifiedItemId ?? "NO ITEM"} & {isTool}, {toAddTo?.QualifiedItemId ?? "NO toAddTo"}, can be attached? {attachmentResult}, result is {__result?.QualifiedItemId ?? "NONE"}");
#endif

        if (affectedItem is not Tool tool)
        {
            return;
        }

        //actions if an item was detached
        if (toAddTo is null || __result is not null)
        {
            if (__result is not Object obj)
                return;

            //if can't be detached
            if (tool.canThisBeAttached(obj) == false)
                return;

            //trigger "on detached"
            TriggerActionManager.Raise($"{ModEntry.Id}_OnItemAttached");

            //try get data for tool
            if (!ModEntry.Data.TryGetValue(obj.QualifiedItemId, out var dataDetached))
                return;

            if (dataDetached.OnDetached == null)
                return;

            ActionButton.CheckBehavior(dataDetached.OnDetached);
            
            return;
        }

        if (tool.canThisBeAttached((Object)toAddTo) == false)
        {
            return;
        }

#if DEBUG
        Log("It works!", LogLevel.Warn);
#endif
        //trigger on attached
        TriggerActionManager.Raise($"{ModEntry.Id}_OnItemAttached");

        //try get data for tool
        if (!ModEntry.Data.TryGetValue(toAddTo.QualifiedItemId, out var mainData))
            return;

        if (mainData.OnAttached == null)
            return;

        ActionButton.CheckBehavior(mainData.OnAttached);
    }

    /// <summary>
    /// Does item behavior actions.
    /// </summary>
    /// <param name="__instance">This menu instance</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="toAddTo">Item being held.</param>
    /// <param name="__result">The resulting item.</param>
    /// <param name="playSound">Sound to play.</param>
    /// <param name="onlyCheckToolAttachments"></param>
    /// <exception cref="ArgumentOutOfRangeException">If the price modifier isn't valid.</exception>
    /// <see cref="StardewValley.Preconditions.FreeInventorySlots"/>
    internal static void Post_rightClick(InventoryMenu __instance, int x, int y, ref Item toAddTo, ref Item __result, bool playSound = true, bool onlyCheckToolAttachments = false)
    {
        try
        {
            var affectedItem = __instance.getItemAt(x, y);
            var heldItem = toAddTo;

            //if there's no using item OR target item
            if (affectedItem == null || heldItem == null)
            {
                //if mouse grabbed something
                if (__result != null)
                {
#if DEBUG
                    Log($"\nPosition: {x}, {y}\nChecking item {__result.DisplayName} ({__result?.QualifiedItemId})\nplaySound: {playSound}, onlyCheckToolAttachments: {onlyCheckToolAttachments}\n");
#endif
                    CallWithoutItem(__instance, ref __result, x, y);
                }
                return;
            }

#if DEBUG
            Log($"\nPosition: {x}, {y}\nHeld item: {heldItem?.QualifiedItemId} ({heldItem?.DisplayName}), Affected item: {affectedItem?.QualifiedItemId}\nplaySound: {playSound}, onlyCheckToolAttachments: {onlyCheckToolAttachments}\n");
#endif

            if (onlyCheckToolAttachments)
                return;

            //if asset doesn't exist, return
            if (Game1.content.DoesAssetExist<Dictionary<string, MenuBehavior>>($"Mods/{ModEntry.Id}/MenuActions/{heldItem.QualifiedItemId}") == false)
                return;

            //try loading & checking behavior
            var particularMenuData = ModEntry.Help.GameContent.Load<Dictionary<string, MenuBehavior>>($"Mods/{ModEntry.Id}/MenuActions/{heldItem.QualifiedItemId}");

            if (particularMenuData is null)
            {
#if DEBUG
                Log("Asset doesn't exist.");
#endif
                return;
            }

            if (particularMenuData.Any())
            {
                foreach (var data in particularMenuData)
                {
                    if (data.Value.Parse(out var rightInfo))
                    {
                        var shouldBreak = CheckMenuActions(ref __instance, ref heldItem, ref affectedItem, rightInfo, x, y, out var shouldNullSpecific) == false;
                        
                        if (shouldNullSpecific)
                            __result = null;

                        if (shouldBreak == false)
                            continue;
                        
                        Log($"Finished applying action {data.Key} for item.");
                        return;
                    }
                    
                    var sb = new StringBuilder("There was an error while validating ");
                    sb.Append(data.Key ?? "this action");
                    sb.Append(". Skipping... (Make sure the format is valid)");
                    Log(sb.ToString(), LogLevel.Info);
                }
                return;
            }
#if DEBUG
            else
            {
                Log($"Asset for {heldItem.QualifiedItemId} couldn't be found.");
            }
#endif
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Checks for a menu action.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="heldItem"></param>
    /// <param name="affectedItem"></param>
    /// <param name="behavior"></param>
    /// <param name="shouldNullResult"></param>
    /// <returns>Whether to continue iterating.</returns>
    private static bool CheckMenuActions(ref InventoryMenu menu, ref Item heldItem, ref Item affectedItem, MenuBehavior behavior, int x, int y, out bool shouldNullResult)
    {
        shouldNullResult = false;

        if (behavior.TargetId != affectedItem.QualifiedItemId)
        {
            return true;
        }
            
        if (!string.IsNullOrWhiteSpace(behavior.Conditions) && !GameStateQuery.CheckConditions(behavior.Conditions))
        {
            Log($"Conditions for {behavior.TargetId} don't match.");
            return true;
        }

        IWorldChangeData.Solve(behavior);
                
        //removeamount is PER item to avoid cheating
        if (heldItem.Stack < behavior.RemoveAmount)
        {
            Log($"Minimum to remove from {behavior.TargetId} isn't available.");
            return true;
        }

        //if we can't convert entire stack AND no more spaces, return
        if (heldItem.Stack < behavior.RemoveAmount * affectedItem.Stack && Game1.player.freeSpotsInInventory() == 0)
        {
            Game1.showRedMessageUsingLoadString("Strings/StringsFromCSFiles:BlueprintsMenu.cs.10002");
            Game1.playSound("cancel");

            Log("No spaces available in inventory. Can't partially convert stack.");
            return false;
        }

        if (behavior.RandomItemId.Any() || !string.IsNullOrWhiteSpace(behavior.ReplaceBy))
        {
            Log($"Replacing {affectedItem.QualifiedItemId} for {behavior.ReplaceBy}.");
            var indexOf = menu.actualInventory.IndexOf(affectedItem);
                    
            //if there's a random item list, it'll be preferred over normal Id
            var whichItem = behavior.RandomItemId.Any() ? Game1.random.ChooseFrom(behavior.RandomItemId) : behavior.ReplaceBy;
                    
            if (behavior.RemoveAmount <= 0)
            {
                var newItem = ItemRegistry.Create(whichItem, behavior.RetainAmount ? affectedItem.Stack : 1, behavior.RetainQuality ? affectedItem.Quality : 0);
                menu.actualInventory[indexOf] = newItem;
            }
            else
            {
                /* e.g: stack of 8, remove 3 per created
                 *
                 * (heldItem.Stack - heldItem.Stack % data.RemoveAmount) / data.RemoveAmount
                 * (8 - (8 % 3)) / 3
                 * (8 - 2) / 3
                 * 6 / 3
                 * 2
                 *
                 * if we have 8 and need 3 per conversion, we can make 2 max.
                 * if affected stack is smaller than max, make stack count. else, maxpossible is set
                 */
                var maxToCreate = (heldItem.Stack - heldItem.Stack % behavior.RemoveAmount) / behavior.RemoveAmount;
                var actualCreateCount = affectedItem.Stack < maxToCreate ? affectedItem.Stack : maxToCreate;
                var newItem = ItemRegistry.Create(whichItem, actualCreateCount,
                    behavior.RetainQuality ? affectedItem.Quality : 0);

                Log($"Created {actualCreateCount} items from stack with {heldItem.Stack} items ({behavior.RemoveAmount} per change).");

                //if stack is the same, replace.
                if (affectedItem.Stack == actualCreateCount)
                    menu.actualInventory[indexOf] = newItem;
                else
                {
                    menu.actualInventory[indexOf].ConsumeStack(actualCreateCount);
                    Game1.player.addItemByMenuIfNecessary(newItem);
                }

                Log($"Removing {behavior.RemoveAmount} for each converted item.");
                var consumed = actualCreateCount * behavior.RemoveAmount;

                Log($"New stack will be {heldItem.Stack - consumed} ...");

                //either reduce count OR remove item
                if (heldItem.Stack - consumed > 0)
                {
                    heldItem.ConsumeStack(consumed);
                }
                else
                {
                    shouldNullResult = true;
                }

                //this is to avoid copying new values on a preexisting item (that isnt supposed to be changed)
                Log($"New affected item will be {newItem.QualifiedItemId} ({newItem.DisplayName}).");
                affectedItem = newItem;
            }
        }

        TryContextTags(behavior, affectedItem);
                
        TryModData(behavior, affectedItem);

        TryQualityChange(behavior, affectedItem);

        TryPriceChange(behavior, affectedItem);

        TryTextureChange(behavior.TextureIndex, affectedItem);
        
        return false;
    }

    private static void CallWithoutItem(InventoryMenu menu, ref Item target, int x, int y)
    {
        if (Game1.content.DoesAssetExist<Dictionary<string, MenuBehavior>>($"Mods/{ModEntry.Id}/MenuActions/None") == false)
            return;
        
        var particularMenuData = ModEntry.Help.GameContent.Load<Dictionary<string, MenuBehavior>>($"Mods/{ModEntry.Id}/MenuActions/None");
                
        if (particularMenuData is null)
        {
#if DEBUG
                    Log("Asset doesn't exist.");
#endif
            return;
        }

        Log("Found conversion data for item. (Mouse/empty action)");

        foreach (var (id, data) in particularMenuData)
        {
            //if not id, keep searching
            if (data.TargetId != target.QualifiedItemId)
                continue;

            if (data.Parse(out var rightInfo) == false)
            {
                var sb = new StringBuilder("There was an error while validating ");
                sb.Append(id ?? "this action");
                sb.Append(". Skipping... (Make sure the format is valid)");
                Log(sb.ToString(), LogLevel.Info);
                continue;
            }

            //if conditions don't match
            if (!string.IsNullOrWhiteSpace(data.Conditions) && !GameStateQuery.CheckConditions(data.Conditions))
            {
                Log($"Conditions for {data.TargetId} don't match.");
                break;
            }

            //solve basic data
            IWorldChangeData.Solve(data);

            //check if item should be replaced. this ignores the Remove field, because we're holding no item
            if (data.RandomItemId.Any() || !string.IsNullOrWhiteSpace(data.ReplaceBy))
            {
                Log($"Replacing {target.QualifiedItemId} for {data.ReplaceBy}.");
                var indexOf = menu.actualInventory.IndexOf(target);

                //if there's a random item list, it'll be preferred over normal Id
                var whichItem = data.RandomItemId.Any() ? Game1.random.ChooseFrom(data.RandomItemId) : data.ReplaceBy;

                if (whichItem.Equals("Remove", StringComparison.OrdinalIgnoreCase))
                {
                    if (data.RemoveAmount > 0)
                    {
                    }
                    else
                    {
                        menu.actualInventory[indexOf] = null;
                    }
                }
                else
                {
                    var newItem = ItemRegistry.Create(whichItem, data.RetainAmount ? target.Stack : 1,
                        data.RetainQuality ? target.Quality : 0);
                    menu.actualInventory[indexOf] = newItem;
                }
            }

            //check for changes in these fields
            TryContextTags(data, target);
            TryModData(data, target);
            TryQualityChange(data, target);
            TryPriceChange(data, target);
            TryTextureChange(data.TextureIndex, target);

            menu.leftClick(x, y, target, false);

            Log($"Finished applying action {data} for item.");
            break;
        }
    }

    private static void TryContextTags(MenuBehavior data, Item affectedItem)
    {
        if (data.AddContextTags.Count > 0)
        {
            var tags = ModEntry.Help.Reflection.GetField<HashSet<string>>(affectedItem, "_contextTags");
            var value = tags.GetValue();

            foreach (var tag in data.AddContextTags)
            {
                Log($"Attempting to add tag {tag}");
                value.Add(tag);
            }

            tags.SetValue(value);
        }

        if (data.RemoveContextTags.Count > 0)
        {
            var tags = ModEntry.Help.Reflection.GetField<HashSet<string>>(affectedItem, "_contextTags");
            var value = tags.GetValue();

            foreach (var tag in data.RemoveContextTags)
            {
                Log($"Attempting to remove tag {tag}");
                value.Remove(tag);
            }

            tags.SetValue(value);
        }
    }
    
    private static void TryModData(MenuBehavior data, Item affectedItem)
    {
        if (data.AddModData is null || data.AddModData.Count <= 0) 
            return;
        
        foreach (var p in data.AddModData)
        {
            Log($"Attempting to add mod data \"{p.Key}\":\"{p.Value}\"");
            if (!affectedItem.modData.TryAdd(p.Key, p.Value))
                affectedItem.modData[p.Key] = p.Value;
        }
    }

    private static void TryQualityChange(MenuBehavior data, Item affectedItem)
    {
        if (string.IsNullOrWhiteSpace(data.QualityChange)) 
            return;
        
        Log($"Changing quality: modifier {data.QualityModifier}, int {data.ActualQuality}");
        switch (data.QualityModifier)
        {
            case Modifier.Set when data.ActualQuality is >= 0 and <= 4:
                affectedItem.Quality = data.ActualQuality;
                if (affectedItem.Quality == 3)
                    affectedItem.Quality = 4;
                break;
            case Modifier.Sum when affectedItem.Quality < 4:
                affectedItem.Quality++;
                if (affectedItem.Quality == 3)
                    affectedItem.Quality = 4;
                break;
            case Modifier.Substract when affectedItem.Quality > 0:
                affectedItem.Quality--;
                if (affectedItem.Quality == 3)
                    affectedItem.Quality = 2;
                break;
            //not considered for quality
            case Modifier.Divide:
            case Modifier.Multiply:
            case Modifier.Percentage:
            default:
                break;
        }
    }

    private static void TryPriceChange(MenuBehavior data, Item affectedItem)
    {
        if (string.IsNullOrWhiteSpace(data.PriceChange) || affectedItem is not (Object or Ring or Boots)) 
            return;
        
        Log($"Changing price: modifier {data.PriceModifier}, int {data.ActualPrice}");
        Item obj = affectedItem switch
        {
            Object o => o,
            Ring r => r,
            Boots b => b,
            _ => throw new ArgumentOutOfRangeException(affectedItem.GetType().ToString())
        };

        var reflectedField = ModEntry.Help.Reflection.GetField<NetInt>(obj, "price");
        var netPrice = reflectedField.GetValue();
        var price = (double)netPrice.Value;

        switch (data.PriceModifier)
        {
            case Modifier.Set when data.ActualPrice >= 0:
                price = data.ActualPrice;
                break;
            case Modifier.Sum:
                price += data.ActualPrice;
                break;
            case Modifier.Substract:
                price -= data.ActualPrice;
                if (price < 0)
                    price = 0;
                break;
            case Modifier.Divide:
                price /= data.ActualPrice;
                break;
            case Modifier.Multiply:
                price *= data.ActualPrice;
                break;
            case Modifier.Percentage:
                price /= data.ActualPrice / 100;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(data.PriceModifier),
                    "Value isn't an allowed one.");
        }

        netPrice.Set((int)price);
        reflectedField.SetValue(netPrice);
    }

    private static void TryTextureChange(int index, Item affectedItem)
    {
        if (index < 0) 
            return;
        
        Log($"Changing texture index to {index}");
        affectedItem.ParentSheetIndex = index;
    }
}