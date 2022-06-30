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
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using TreasureHunts;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerWarpedEvent : WarpedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerWarpedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        ModEntry.PlayerState.ScavengerHunt ??= new ScavengerHunt();
        if (ModEntry.PlayerState.ScavengerHunt.IsActive) ModEntry.PlayerState.ScavengerHunt.Fail();
        if (!Game1.eventUp && e.NewLocation.IsOutdoors &&
            (ModEntry.Config.AllowScavengerHuntsOnFarm || !e.NewLocation.IsFarm))
            ModEntry.PlayerState.ScavengerHunt.TryStart(e.NewLocation);
    }
}