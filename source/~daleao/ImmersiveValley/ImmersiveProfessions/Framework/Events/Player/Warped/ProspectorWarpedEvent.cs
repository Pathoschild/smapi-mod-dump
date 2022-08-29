/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using Extensions;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorWarpedEvent : WarpedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ProspectorWarpedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (ModEntry.State.ProspectorHunt.Value.IsActive) ModEntry.State.ProspectorHunt.Value.Fail();
        if (!Game1.eventUp && e.NewLocation is MineShaft shaft && !shaft.IsTreasureOrSafeRoom())
            ModEntry.State.ProspectorHunt.Value.TryStart(e.NewLocation);
    }
}