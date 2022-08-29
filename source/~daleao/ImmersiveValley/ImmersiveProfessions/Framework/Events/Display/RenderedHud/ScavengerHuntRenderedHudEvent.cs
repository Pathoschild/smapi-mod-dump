/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using System;
using TreasureHunts;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent : RenderedHudEvent
{
    private ScavengerHunt? Hunt;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerHuntRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        Hunt ??= (ScavengerHunt)ModEntry.State.ScavengerHunt.Value;
        if (!Hunt.TreasureTile.HasValue) return;

        var treasureTile = Hunt.TreasureTile.Value;

        // track target
        ModEntry.Pointer.Value.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        var distanceSquared = (Game1.player.getTileLocation() - treasureTile).LengthSquared();
        if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
            ModEntry.Pointer.Value.DrawOverTile(treasureTile, Color.Violet);
    }
}