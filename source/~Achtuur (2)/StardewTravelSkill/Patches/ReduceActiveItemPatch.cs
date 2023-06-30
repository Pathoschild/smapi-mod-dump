/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace StardewTravelSkill.Patches;

internal class ReduceActiveItemPatch : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: this.GetOriginalMethod<Farmer>(nameof(Farmer.reduceActiveItemByOne)),
            prefix: this.GetHarmonyMethod(nameof(this.Prefix_ReduceActiveItemByOne))
        );
    }


    /// <summary>
    /// Postfix patch to <see cref="StardewValley.Farmer.reduceActiveItemByOne"/>.
    /// </summary>
    /// <param name="__result"></param>
    private static bool Prefix_ReduceActiveItemByOne()
    {
        try
        {
            if (!Context.IsWorldReady)
                return true;

            if (Game1.player.ActiveObject is null)
                return true;

            // Check if the held item that was used is a totem
            SObject held_item = Game1.player.ActiveObject;
            if (!isTotem(held_item.ParentSheetIndex))
                return true;

            Random rnd = new Random();
            // Randomly decide if warp totem should be consumed
            // GetWarpTotemConsumeChance() returns 1 if the profession is unlocked, meaning the totem is always consumed
            if (rnd.NextDouble() > ModEntry.GetWarpTotemConsumeChance())
            {

                AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, "Warp totem not consumed!");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {

            AchtuurCore.Logger.ErrorLog(ModEntry.Instance.Monitor, $"Failed in {nameof(Prefix_ReduceActiveItemByOne)}:\n{ex}");
            return true;
        }
    }


    private static bool isTotem(int item_id)
    {
        switch (item_id)
        {
            case 261:
            case 688:
            case 689:
            case 690:
            case 886: return true;
            default: return false;
        }
    }
}
