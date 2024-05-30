/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ManualDetonationUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ManualDetonationUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Config.ModKey.IsDown())
        {
            return;
        }

        for (var i = Game1.currentLocation.TemporarySprites.Count - 1; i >= 0; i--)
        {
            var sprite = Game1.currentLocation.TemporarySprites[i];
            if (sprite.bombRadius > 0 && sprite.totalNumberOfLoops == int.MaxValue)
            {
                sprite.currentNumberOfLoops = sprite.totalNumberOfLoops - 1;
            }
        }

        this.Disable();
    }
}
