/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using Common.Events;
using Common.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class PondDayStartedEvent : DayStartedEvent
{
    public override bool IsEnabled => Context.IsMainPlayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondDayStartedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                     (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                     !p.isUnderConstruction()))
            pond.Write("CheckedToday", false.ToString());
    }
}