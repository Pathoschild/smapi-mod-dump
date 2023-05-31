/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpelunkerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpelunkerWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Spelunker);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation is MineShaft || e.OldLocation is not MineShaft)
        {
            return;
        }

        ProfessionsModule.State.SpelunkerLadderStreak = 0;
        this.Manager.Disable<SpelunkerUpdateTickedEvent>();
    }
}
