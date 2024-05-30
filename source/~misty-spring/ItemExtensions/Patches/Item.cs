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
using ItemExtensions.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Triggers;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches;

public class ItemPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(ItemPatches)}\": postfixing SDV method \"Item.addToStack()\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Item), nameof(Item.addToStack)),
            postfix: new HarmonyMethod(typeof(ItemPatches), nameof(Post_addToStack))
        );

        if (ModEntry.Config.OnBehavior == false)
            return;

        Log($"Applying Harmony patch \"{nameof(ItemPatches)}\": postfixing SDV method \"Item.actionWhenPurchased\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Item), nameof(Item.actionWhenPurchased)),
            postfix: new HarmonyMethod(typeof(ItemPatches), nameof(Post_actionWhenPurchased))
        );
        
        Log($"Applying Harmony patch \"{nameof(ItemPatches)}\": postfixing SDV method \"Item.onEquip\".");
        harmony.Patch(
          original: AccessTools.Method(typeof(Item), nameof(Item.onEquip)),
          postfix: new HarmonyMethod(typeof(ItemPatches), nameof(Post_onEquip))
        );
        
        Log($"Applying Harmony patch \"{nameof(ItemPatches)}\": postfixing SDV method \"Item.onUnequip\".");
        harmony.Patch(
          original: AccessTools.Method(typeof(Item), nameof(Item.onUnequip)),
          postfix: new HarmonyMethod(typeof(ItemPatches), nameof(Post_onUnequip))
        );
    }

    private static void Post_CreateItem(ParsedItemData data, ref Item __result)
    {
        try
        {
            if (__result is not Object o)
                return;
            ObjectPatches.Post_new(ref o, o.TileLocation, o.ItemId, o.IsRecipe);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    public static void Post_addToStack(Item otherStack)
    {
        try
        {
            TriggerActionManager.Raise($"{ModEntry.Id}_AddedToStack");
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    public static void Post_actionWhenPurchased(Item __instance, string shopId)
    {
        try
        {
            TriggerActionManager.Raise($"{ModEntry.Id}_OnPurchased");

#if DEBUG
            Log($"Called OnPurchased, id {__instance.QualifiedItemId}");
#endif

            if (!ModEntry.Data.TryGetValue(__instance.QualifiedItemId, out var mainData))
                return;

            if (mainData.OnPurchase == null)
                return;

            ActionButton.CheckBehavior(mainData.OnPurchase);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }

    /// <summary>Handle the item being equipped by the player (i.e. added to an equipment slot, or selected as the active tool).</summary>
    /// <param name="__instance">Item equipped.</param>
    /// <param name="who">The player who equipped the item.</param>
    public static void Post_onEquip(Item __instance, Farmer who)
    {
        try
        {
            TriggerActionManager.Raise($"{ModEntry.Id}_OnEquip");

            if (!ModEntry.Data.TryGetValue(__instance.QualifiedItemId, out var mainData))
                return;

            if (mainData.OnEquip == null)
                return;

            ActionButton.CheckBehavior(mainData.OnEquip);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }

    /// <summary>Handle the item being unequipped by the player (i.e. removed from an equipment slot, or deselected as the active tool).</summary>
    /// <param name="__instance">Item unequipped.</param>
    /// <param name="who">The player who unequipped the item.</param>
    public static void Post_onUnequip(Item __instance, Farmer who)
    {
        try
        {
            TriggerActionManager.Raise($"{ModEntry.Id}_OnUnequip");

            if (!ModEntry.Data.TryGetValue(__instance.QualifiedItemId, out var mainData))
                return;

            if (mainData.OnUnequip == null)
                return;

            ActionButton.CheckBehavior(mainData.OnUnequip);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
}