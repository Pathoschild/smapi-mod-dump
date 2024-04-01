/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using ItemExtensions.Models.Enums;
using ItemExtensions.Models.Internal;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
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
        Log($"Applying Harmony patch \"{nameof(InventoryPatches)}\": postfixing SDV method \"InventoryMenu.rightClick(int, int, Item, bool, bool)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(InventoryMenu), nameof(InventoryMenu.rightClick)),
            postfix: new HarmonyMethod(typeof(InventoryPatches), nameof(Post_rightClick))
        );
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

            /*#if DEBUG
            Log($"\nPosition: {x}, {y}\nHeld item: {heldItem?.QualifiedItemId} ({heldItem?.DisplayName}), Affected item: {affectedItem?.QualifiedItemId}\nplaySound: {playSound}, onlyCheckToolAttachments: {onlyCheckToolAttachments}\n", LogLevel.Debug);
        #endif*/

            if (affectedItem == null || heldItem == null)
                return;

            if (onlyCheckToolAttachments)
                return;

            if (ModEntry.MenuActions == null || ModEntry.MenuActions?.Count == 0)
                return;


            // ReSharper disable once PossibleNullReferenceException
            if (!ModEntry.MenuActions.TryGetValue(heldItem.QualifiedItemId, out var options))
                return;

            Log("Found conversion data for item.");

            foreach (var data in options)
            {
                if (data.TargetId != affectedItem.QualifiedItemId)
                    continue;

                if (!string.IsNullOrWhiteSpace(data.Conditions) && !GameStateQuery.CheckConditions(data.Conditions))
                {
                    Log($"Conditions for {data.TargetId} don't match.");
                    break;
                }

                IWorldChangeData.Solve(data);
                
                //removeamount is PER item to avoid cheating
                if (heldItem.Stack < data.RemoveAmount)
                {
                    Log($"Minimum to remove from {data.TargetId} isn't avaiable.");
                    break;
                }

                //if we can't convert entire stack AND no more spaces, return
                if (heldItem.Stack < data.RemoveAmount * affectedItem.Stack && Game1.player.freeSpotsInInventory() == 0)
                {
                    Game1.showRedMessageUsingLoadString("Strings/StringsFromCSFiles:BlueprintsMenu.cs.10002");
                    Game1.playSound("cancel");

                    Log("No spaces avaiable in inventory. Can't partially convert stack.");
                    break;
                }

                if (!string.IsNullOrWhiteSpace(data.ReplaceBy))
                {
                    Log($"Replacing {affectedItem.QualifiedItemId} for {data.ReplaceBy}.");
                    var indexOf = __instance.actualInventory.IndexOf(affectedItem);
                    if (data.RemoveAmount <= 0)
                    {
                        var newItem = ItemRegistry.Create(data.ReplaceBy, data.RetainAmount ? affectedItem.Stack : 1,
                            data.RetainQuality ? affectedItem.Quality : 0);
                        __instance.actualInventory[indexOf] = newItem;
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
                        var maxToCreate = (heldItem.Stack - heldItem.Stack % data.RemoveAmount) / data.RemoveAmount;
                        var actualCreateCount = affectedItem.Stack < maxToCreate ? affectedItem.Stack : maxToCreate;
                        var newItem = ItemRegistry.Create(data.ReplaceBy, actualCreateCount,
                            data.RetainQuality ? affectedItem.Quality : 0);

                        Log($"Created {actualCreateCount} items from stack with {heldItem.Stack} items ({data.RemoveAmount} per change).");

                        //if stack is the same, replace.
                        if (affectedItem.Stack == actualCreateCount)
                            __instance.actualInventory[indexOf] = newItem;
                        else
                        {
                            __instance.actualInventory[indexOf]
                                .ConsumeStack(actualCreateCount); //Stack -= newItem.Stack;
                            Game1.player.addItemByMenuIfNecessary(newItem);
                        }

                        Log($"Removing {data.RemoveAmount} for each converted item.");
                        var consumed = actualCreateCount * data.RemoveAmount;

                        Log($"New stack will be {heldItem.Stack - consumed} ...");

                        //either reduce count OR remove item
                        if (heldItem.Stack - consumed > 0)
                        {
                            heldItem.ConsumeStack(consumed);
                        }
                        else
                        {
                            //__instance.actualInventory.Remove(heldItem); //not part of inventory so this won't work
                            //heldItem.Stack = 0;
                            __result = null;
                        }

                        //this is to avoid copying new values on a preexisting item (that isnt supposed to be changed)
                        Log($"New affected item will be {newItem.QualifiedItemId} ({newItem.DisplayName}).");
                        affectedItem = newItem;
                    }
                }

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

                if (data.AddModData.Count > 0)
                {
                    foreach (var p in data.AddModData)
                    {
                        Log($"Attempting to add mod data \"{p.Key}\":\"{p.Value}\"");
                        if (!affectedItem.modData.TryAdd(p.Key, p.Value))
                            affectedItem.modData[p.Key] = p.Value;
                    }
                }

                if (!string.IsNullOrWhiteSpace(data.QualityChange))
                {
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

                if (!string.IsNullOrWhiteSpace(data.PriceChange) && affectedItem is Object or Ring or Boots)
                {
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

                if (data.TextureIndex >= 0)
                {
                    Log($"Changing texture index to {data.TextureIndex}");
                    affectedItem.ParentSheetIndex = data.TextureIndex;
                }

                break;
            }
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
}