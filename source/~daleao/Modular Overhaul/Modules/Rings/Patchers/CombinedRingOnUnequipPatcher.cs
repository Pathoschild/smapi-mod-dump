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

using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnUnequipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnUnequipPatcher"/> class.</summary>
    internal CombinedRingOnUnequipPatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onUnequip));
    }

    #region harmony patches

    /// <summary>Remove Infinity Band resonance effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnUnequipPostfix(CombinedRing __instance, Farmer who)
    {
        var chord = __instance.Get_Chord();
        if (chord is null)
        {
            return;
        }

        chord.Unapply(who.currentLocation, who);
        if (chord.Root is not null && who.CurrentTool is { } tool &&
            ((tool is MeleeWeapon && WeaponsModule.ShouldEnable) || (tool is Slingshot && SlingshotsModule.ShouldEnable)) &&
            tool.Get_ResonatingChord(chord.Root.EnchantmentType) == chord)
        {
            tool.UnsetResonatingChord(chord.Root.EnchantmentType);
        }
    }

    #endregion harmony patches
}
