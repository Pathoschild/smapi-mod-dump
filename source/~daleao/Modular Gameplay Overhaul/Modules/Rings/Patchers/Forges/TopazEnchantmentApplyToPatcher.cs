/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentApplyToPatcher"/> class.</summary>
    internal TopazEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Resonate with Topaz chord.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentApplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (!ArsenalModule.IsEnabled || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var chord = player.Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Topaz)
            .ArgMax(c => c.Amplitude);
        if (chord is null)
        {
            return;
        }

        tool.UpdateResonatingChord<TopazEnchantment>(chord);
    }

    #endregion harmony patches
}
