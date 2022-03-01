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
using StardewValley;
using StardewValley.Locations;

using Extensions;
using TreasureHunt;

#endregion using directives

internal class ProspectorWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        ModEntry.PlayerState.Value.ProspectorHunt ??= new ProspectorHunt();
        if (ModEntry.PlayerState.Value.ProspectorHunt.IsActive) ModEntry.PlayerState.Value.ProspectorHunt.Fail();
        if (!Game1.eventUp && e.NewLocation is MineShaft shaft && !shaft.IsTreasureOrSafeRoom())
            ModEntry.PlayerState.Value.ProspectorHunt.TryStartNewHunt(e.NewLocation);
    }
}