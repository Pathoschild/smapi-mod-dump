/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuGetPlayerCurrencyAmountPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuGetPlayerCurrencyAmountPatcher"/> class.</summary>
    internal ShopMenuGetPlayerCurrencyAmountPatcher()
    {
        this.Target = this.RequireMethod<ShopMenu>(nameof(ShopMenu.getPlayerCurrencyAmount));
    }

    #region harmony patches

    /// <summary>Set up Clint's forge shop.</summary>
    [HarmonyPrefix]
    private static bool ShopMenuGetPlayerCurrencyAmountPrefix(ref int __result, Farmer who, int currencyType)
    {
        try
        {
            if (currencyType != Constants.DragonToothIndex &&
                (!Globals.DwarvenScrapIndex.HasValue || currencyType != Globals.DwarvenScrapIndex.Value) &&
                (!Globals.ElderwoodIndex.HasValue || currencyType != Globals.ElderwoodIndex.Value))
            {
                return true; // run original logic
            }

            __result = who.Items.Where(i => i.ParentSheetIndex == currencyType).Sum(i => i.Stack);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
