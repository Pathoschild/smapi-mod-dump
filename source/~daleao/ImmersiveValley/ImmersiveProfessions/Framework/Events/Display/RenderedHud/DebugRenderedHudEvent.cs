/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

#if DEBUG
namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class DebugRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        // show FPS counter
        ModEntry.FpsCounter?.Draw(Game1.currentGameTime);

        if (ModEntry.DebugCursorPosition is null) return;

        var coords =
            $"X: {ModEntry.DebugCursorPosition.Tile.X} Tile / {ModEntry.DebugCursorPosition.GetScaledAbsolutePixels().X} Absolute";
        coords +=
            $"\nY: {ModEntry.DebugCursorPosition.Tile.Y} Tile / {ModEntry.DebugCursorPosition.GetScaledAbsolutePixels().Y} Absolute";

        // draw cursor coordinates
        e.SpriteBatch.DrawString(Game1.dialogueFont, coords, new(33f, 82f), Color.Black);
        e.SpriteBatch.DrawString(Game1.dialogueFont, coords, new(32f, 81f), Color.White);

        // draw current location
        e.SpriteBatch.DrawString(Game1.dialogueFont, $"Location: {Game1.player.currentLocation.NameOrUniqueName}", new(33f, 167f), Color.Black);
        e.SpriteBatch.DrawString(Game1.dialogueFont, $"Location: {Game1.player.currentLocation.NameOrUniqueName}", new(32f, 166f), Color.White);
    }
}
#endif