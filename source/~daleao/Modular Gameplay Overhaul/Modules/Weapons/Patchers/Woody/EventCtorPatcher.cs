/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Woody;

#region using directives

using System.Linq;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Modules.Weapons.Integrations;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EventCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventCtorPatcher"/> class.</summary>
    internal EventCtorPatcher()
    {
        this.Target = this.RequireConstructor<Event>(typeof(string), typeof(int), typeof(Farmer));
    }

    #region harmony patches

    /// <summary>Immersively adjust Marlon's intro event.</summary>
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void EventCtorPrefix(ref string eventString, int eventID)
    {
        if (!WeaponsModule.Config.WoodyReplacesRusty || eventID != 100162)
        {
            return;
        }

        eventString = StardewValleyExpandedIntegration.Instance?.IsLoaded == true
            ? I18n.Get(
                Game1.player.Items.Any(item => item is MeleeWeapon weapon && !weapon.isScythe())
                    ? "events.100162.nosword.sve"
                    : "events.100162.sword.sve")
            : I18n.Get(
                Game1.player.Items.Any(item => item is MeleeWeapon weapon && !weapon.isScythe())
                    ? "events.100162.nosword"
                    : "events.100162.sword");
    }

    #endregion harmony patches
}
