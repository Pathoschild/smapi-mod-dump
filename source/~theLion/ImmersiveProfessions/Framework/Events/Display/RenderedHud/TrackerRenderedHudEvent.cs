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

using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

using Extensions;

#endregion using directives

internal class TrackerRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        // reveal on-screen trackable objects
        foreach (var pair in Game1.currentLocation.Objects.Pairs.Where(p => p.Value.ShouldBeTracked()))
            ModEntry.PlayerState.Pointer.DrawOverTile(pair.Key, Color.Yellow);

        if (!Game1.player.HasProfession(Profession.Prospector) || Game1.currentLocation is not MineShaft shaft) return;

        // reveal on-screen ladders and shafts
        foreach (var tile in shaft.GetLadderTiles()) ModEntry.PlayerState.Pointer.DrawOverTile(tile, Color.Lime);
    }
}