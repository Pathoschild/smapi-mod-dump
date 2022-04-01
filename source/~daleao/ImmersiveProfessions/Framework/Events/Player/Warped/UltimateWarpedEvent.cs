/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

using Display;
using Extensions;

#endregion using directives

internal class UltimateWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsDungeon())
        {
            EventManager.Enable(typeof(UltimateMeterRenderingHudEvent));
        }
        else
        {
            ModEntry.PlayerState.RegisteredUltimate.ChargeValue = 0.0;
            EventManager.Disable(typeof(UltimateMeterRenderingHudEvent));
        }
    }
}