/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BullseyeRenderedEvent : RenderedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BullseyeRenderedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BullseyeRenderedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedImpl(object? sender, RenderedEventArgs e)
    {
        if (Game1.player.CurrentTool is not Slingshot slingshot)
        {
            this.Disable();
            return;
        }

        var mouseX = slingshot.aimPos.X;
        var mouseY = slingshot.aimPos.Y;
        e.SpriteBatch.Draw(
            Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, new Vector2(mouseX, mouseY)),
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43),
            Color.White,
            0f,
            new Vector2(32f, 32f),
            1f,
            SpriteEffects.None,
            0.999999f);
    }
}
