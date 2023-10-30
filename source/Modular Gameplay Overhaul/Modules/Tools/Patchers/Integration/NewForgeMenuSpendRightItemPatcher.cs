/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuSpendRightItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuSpendRightItemPatcher"/> class.</summary>
    internal NewForgeMenuSpendRightItemPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.SpendRightItem));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuSpendRightItemPrefix(NewForgeMenu __instance)
    {
        if (!ToolsModule.Config.EnableForgeUpgrading || __instance.rightIngredientSpot.item is null)
        {
            return true; // run original logic
        }

        var item = __instance.rightIngredientSpot.item;
        if (item.ParentSheetIndex is not (ObjectIds.CopperBar or ObjectIds.IronBar or ObjectIds.GoldBar
                or ObjectIds.IridiumBar or ObjectIds.RadioactiveBar) &&
            (item.ParentSheetIndex != 1720 ||
             Reflector.GetUnboundPropertyGetter<object, string>(item, "FullId").Invoke(item) != "spacechase0.MoonMisadventures/Mythicite Bar"))
        {
            return true; // run original logic
        }

        __instance.rightIngredientSpot.item.Stack -= 5;
        if (__instance.rightIngredientSpot.item.Stack <= 0)
        {
            __instance.rightIngredientSpot.item = null;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
