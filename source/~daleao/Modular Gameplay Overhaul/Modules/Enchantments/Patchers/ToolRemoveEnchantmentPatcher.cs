/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.Events;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolRemoveEnchantmentPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolRemoveEnchantmentPatcher"/> class.</summary>
    internal ToolRemoveEnchantmentPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.RemoveEnchantment));
    }

    #region harmony patches

    /// <summary>Disposes certain enchantments.</summary>
    [HarmonyPostfix]
    private static void ToolRemoveEnchantmentPostifx(BaseEnchantment enchantment)
    {
        switch (enchantment)
        {
            case EnergizedEnchantment energized:
                EventManager.Disable<EnergizedUpdateTickedEvent>();
                break;
            case ExplosiveEnchantment explosive:
                EventManager.Disable<ExplosiveUpdateTickedEvent>();
                break;
        }
    }

    #endregion harmony patches
}
