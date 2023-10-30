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

using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidCraftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftPatcher"/> class.</summary>
    internal NewForgeMenuIsValidCraftPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftPostfix(ref bool __result, Item left_item, Item right_item)
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

        switch (tool.UpgradeLevel)
        {
            case < 5:
            {
                var upgradeItemIndex = tool.UpgradeLevel switch
                {
                    0 => ObjectIds.CopperBar,
                    1 => ObjectIds.IronBar,
                    2 => ObjectIds.GoldBar,
                    3 => ObjectIds.IridiumBar,
                    4 => ObjectIds.RadioactiveBar,
                    _ => -1,
                };

                if (upgradeItemIndex < 0)
                {
                    Log.E("Go home game. You're drunk.");
                }

                if (right_item.ParentSheetIndex == upgradeItemIndex && right_item.Stack >= 5)
                {
                    __result = true;
                }

                break;
            }

            case 5 when right_item.ParentSheetIndex == 1720 &&
                        Reflector.GetUnboundPropertyGetter<object, string>(right_item, "FullId").Invoke(right_item) ==
                        "spacechase0.MoonMisadventures/Mythicite Bar" && right_item.Stack >= 5:
                __result = true;
                break;
        }
    }

    #endregion harmony patches
}
