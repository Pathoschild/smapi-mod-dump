/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

using Display;
using Extensions;

#endregion using directives

internal class SuperModeWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsCombatZone() && ModEntry.Config.EnableSuperMode)
        {
            EventManager.Enable(typeof(SuperModeGaugeRenderingHudEvent));
            if (ModEntry.PlayerState.Value.SuperMode is {IsActive: true} superMode)
                superMode.Deactivate();
        }
        else
        {
            ModEntry.PlayerState.Value.SuperMode.ChargeValue = 0.0;
            EventManager.Disable(typeof(SuperModeGaugeRenderingHudEvent));
        }
    }
}