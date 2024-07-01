/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="OutOfCombatOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class OutOfCombatOneSecondUpdateTickedEvent(EventManager? manager = null)
    : OneSecondUpdateTickedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsWorldReady && Game1.game1.ShouldTimePass() && State.SecondsOutOfCombat < 300;

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        State.SecondsOutOfCombat++;
    }
}
