/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal class DebugRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object sender, RenderedHudEventArgs e)
    {
        if (!ModEntry.Config.DebugKey.IsDown()) return;
        
        // show FPS counter
        ModEntry.FpsCounter?.Draw(Game1.currentGameTime);

        if (ModEntry.State.Value.CursorPosition is null) return;

        var coords =
            $"X: {ModEntry.State.Value.CursorPosition.Tile.X} Tile / {ModEntry.State.Value.CursorPosition.GetScaledAbsolutePixels().X} Absolute";
        coords +=
            $"\nY: {ModEntry.State.Value.CursorPosition.Tile.Y} Tile / {ModEntry.State.Value.CursorPosition.GetScaledAbsolutePixels().Y} Absolute";

        // draw cursor coordinates
        e.SpriteBatch.DrawString(Game1.dialogueFont, coords, new(33f, 82f), Color.Black);
        e.SpriteBatch.DrawString(Game1.dialogueFont, coords, new(32f, 81f), Color.White);
    }
}