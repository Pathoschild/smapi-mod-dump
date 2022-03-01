/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class ProspectorHuntRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.PlayerState.Value.ProspectorHunt.TreasureTile.HasValue) return;

        var treasureTile = ModEntry.PlayerState.Value.ProspectorHunt.TreasureTile.Value;

        // track target
        ModEntry.PlayerState.Value.Pointer.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.PlayerState.Value.Pointer.DrawOverTile(treasureTile, Color.Violet);
    }
}