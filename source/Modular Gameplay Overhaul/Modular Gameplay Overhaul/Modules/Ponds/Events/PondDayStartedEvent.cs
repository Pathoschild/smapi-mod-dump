/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Ponds.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class PondDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PondDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    public override bool IsEnabled => Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                     (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                     !p.isUnderConstruction()))
        {
            pond.Write(DataFields.CheckedToday, false.ToString());
        }
    }
}
