/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolIndexSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolIndexSetterPatcher"/> class.</summary>
    internal FarmerCurrentToolIndexSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentToolIndex));
    }

    #region harmony patches

    /// <summary>Trigger playback of gemstone vibration.</summary>
    [HarmonyPostfix]
    private static void FarmerCurrentToolIndexPostfix(Farmer __instance, ref int value)
    {
        if (!CombatModule.Config.RingsEnchantments.AudibleGemstones)
        {
            return;
        }

        EventManager.Disable<HoldingGemstoneUpdateTickedEvent>();
        if (value < 0 || value >= __instance.Items.Count || __instance.Items[value] is not Item held ||
            !Gemstone.TryFromObject(held.ParentSheetIndex, out var gemstone))
        {
            return;
        }

        HoldingGemstoneUpdateTickedEvent.Gemstone = gemstone;
        EventManager.Enable<HoldingGemstoneUpdateTickedEvent>();
    }

    #endregion harmony patches
}
