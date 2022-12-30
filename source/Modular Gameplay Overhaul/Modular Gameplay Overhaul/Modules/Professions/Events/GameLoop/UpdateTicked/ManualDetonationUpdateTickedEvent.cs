/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ManualDetonationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ManualDetonationUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ManualDetonationUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ProfessionsModule.Config.ModKey.IsDown())
        {
            return;
        }

        foreach (var sprite in Game1.currentLocation.TemporarySprites.Where(sprite =>
                     sprite.bombRadius > 0 && sprite.totalNumberOfLoops == int.MaxValue))
        {
            sprite.currentNumberOfLoops = sprite.totalNumberOfLoops - 1;
        }

        this.Disable();
    }
}
