/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionSavingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionSavingEvent(EventManager? manager = null)
    : SavingEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        var player = Game1.player;
        Data.Write(player, DataKeys.OrderedProfessions, string.Join(',', State.OrderedProfessions));
        Data.Write(player, DataKeys.LimitBreakId, State.LimitBreak?.Id.ToString());
        Data.Write(
            player,
            DataKeys.PrestigedEcologistBuffLookup,
            State.PrestigedEcologistBuffsLookup.Stringify());
    }
}
