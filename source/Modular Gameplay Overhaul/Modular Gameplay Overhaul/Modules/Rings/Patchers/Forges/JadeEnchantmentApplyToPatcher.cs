/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
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
internal sealed class JadeEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentApplyToPatcher"/> class.</summary>
    internal JadeEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Resonate with Jade chord.</summary>
    [HarmonyPostfix]
    private static void JadeEnchantmentApplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (!ArsenalModule.IsEnabled || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var chord = player.Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Jade)
            .ArgMax(c => c.Amplitude);
        if (chord is null)
        {
            return;
        }

        tool.UpdateResonatingChord<JadeEnchantment>(chord);
    }

    #endregion harmony patches
}
