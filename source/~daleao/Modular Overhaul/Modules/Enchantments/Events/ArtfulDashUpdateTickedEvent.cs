/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Events;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArtfulDashUpdateTickedEvent : UpdateTickedEvent
{
    private int _timer;

    /// <summary>Initializes a new instance of the <see cref="ArtfulDashUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArtfulDashUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._timer = 18;
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Game1.player.DoStabbingSpecielCooldown();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (--this._timer <= 0)
        {
            this.Disable();
        }
    }
}
