/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuCraftItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuCraftItemPatcher"/> class.</summary>
    internal NewForgeMenuCraftItemPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuCraftItemPostfix(ref Item __result, Item left_item, Item right_item)
    {
        if (!ToolsModule.Config.EnableForgeUpgrading)
        {
            return;
        }

        if (left_item is not (Tool tool and (Axe or Hoe or Pickaxe or WateringCan)))
        {
            return;
        }

        var maxToolUpgrade = MoonMisadventuresIntegration.Instance?.IsLoaded == true ? 6 : 5;
        if (tool.UpgradeLevel >= maxToolUpgrade)
        {
            return;
        }

        var upgradeItemIndex = tool.UpgradeLevel switch
        {
            0 => SObject.copperBar,
            1 => SObject.ironBar,
            2 => SObject.goldBar,
            3 => SObject.iridiumBar,
            4 => ItemIDs.RadioactiveBar,
            5 => "spacechase0.MoonMisadventures/Mythicite Bar".GetDeterministicHashCode(),
            _ => SObject.prismaticShardIndex,
        };

        if (right_item.ParentSheetIndex == upgradeItemIndex && right_item.Stack >= 5)
        {
            ((Tool)left_item).UpgradeLevel++;
        }

        __result = left_item;
    }

    #endregion harmony patches
}
