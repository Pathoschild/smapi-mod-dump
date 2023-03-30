/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlickMovesUpdateTickingEvent : UpdateTickingEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlickMovesUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlickMovesUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var (x, y) = WeaponsModule.State.DriftVelocity;
        if (x == 0f && y == 0f)
        {
            this.Disable();
        }

        var player = Game1.player;
        (player.xVelocity, player.yVelocity) = (x, y);
        x -= x / 16f;
        y -= y / 16f;
        WeaponsModule.State.DriftVelocity = new Vector2(x, y);
    }
}
