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

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolActionWhenPurchasedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolActionWhenPurchasedPatcher"/> class.</summary>
    internal ToolActionWhenPurchasedPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.actionWhenPurchased));
    }

    #region harmony patches

    /// <summary>Inject forging.</summary>
    [HarmonyPostfix]
    private static void ToolActionWhenPurchasedPostfix(Tool __instance, ref bool __result)
    {
        if (Game1.player.toolBeingUpgraded.Value is not null || __instance is not MeleeWeapon weapon ||
            !weapon.CanBeCrafted())
        {
            return;
        }

        Game1.player.toolBeingUpgraded.Value = __instance;
        Game1.player.daysLeftForToolUpgrade.Value = 3;
        Game1.playSound("parry");
        Game1.exitActiveMenu();
        Game1.drawDialogue(Game1.getCharacterFromName("Clint"), I18n.Get("blacksmith.forge.confirmation"));
        __result = true;
    }

    #endregion harmony patches
}
