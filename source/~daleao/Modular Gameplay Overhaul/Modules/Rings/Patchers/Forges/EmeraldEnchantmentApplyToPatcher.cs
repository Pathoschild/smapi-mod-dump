/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Patchers.Forges;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EmeraldEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EmeraldEnchantmentApplyToPatcher"/> class.</summary>
    internal EmeraldEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<EmeraldEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Resonate with Emerald chord.</summary>
    [HarmonyPostfix]
    private static void EmeraldEnchantmentApplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (item is not Tool tool || tool != player.CurrentTool)
        {
            return;
        }

        if ((tool is MeleeWeapon && !WeaponsModule.ShouldEnable) || (tool is Slingshot && !SlingshotsModule.ShouldEnable) ||
            tool is not (MeleeWeapon or Slingshot))
        {
            return;
        }

        var chord = player
            .Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Emerald)
            .ArgMax(c => c.Amplitude);
        if (chord is not null)
        {
            tool.UpdateResonatingChord<EmeraldEnchantment>(chord);
        }
    }

    #endregion harmony patches
}
