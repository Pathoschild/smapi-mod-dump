/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Triggers;
using Object = StardewValley.Object;
using static ItemExtensions.Additions.ModKeys;

namespace ItemExtensions.Patches;

public partial class ObjectPatches
{
    public static void Post_actionWhenBeingHeld(Farmer who)
    {
        try
        {
            if (ModEntry.Holding)
                return;

            TriggerActionManager.Raise($"{ModEntry.Id}_OnBeingHeld");

            ModEntry.Holding = true;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }

    public static void Post_actionWhenStopBeingHeld(Farmer who)
    {
        try
        {
#if DEBUG
            Log($"Clearing cached seed...(last held item {who.mostRecentlyGrabbedItem?.QualifiedItemId})");
#endif
            CropPatches.Cached = null;
            CropPatches.Chosen = false;
            ModEntry.Holding = false;
            TriggerActionManager.Raise($"{ModEntry.Id}_OnStopHolding");
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    public static void Post_performRemoveAction()
    {
        try
        {
            TriggerActionManager.Raise($"{ModEntry.Id}_OnItemRemoved");
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    public static void Post_dropItem(Object __instance, GameLocation location, Vector2 origin, Vector2 destination)
    {
        try
        {
            TriggerActionManager.Raise($"{ModEntry.Id}_OnItemDropped");

            if (!ModEntry.Data.TryGetValue(__instance.QualifiedItemId, out var mainData))
                return;

            if (mainData.OnDrop == null)
                return;

            ActionButton.CheckBehavior(mainData.OnDrop);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    public static void Pre_maximumStackSize(Object __instance, ref int __result)
    {
        try
        {
            if (__instance.modData.TryGetValue(MaxStack, out var stack))
                __result = int.Parse(stack);

            if (!ModEntry.Data.TryGetValue(__instance.QualifiedItemId, out var data))
                return;

            if (data.MaximumStack == 0)
                return;

            __result = data.MaximumStack;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    public static void Pre_IsHeldOverHead(Object __instance, ref bool __result)
    {
        try
        {
            if (__instance.modData.TryGetValue(ShowAboveHead, out var boolean))
                __result = bool.Parse(boolean);

            if (!ModEntry.Data.TryGetValue(__instance.QualifiedItemId, out var data))
                return;

            __result = data.HideItem;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
}