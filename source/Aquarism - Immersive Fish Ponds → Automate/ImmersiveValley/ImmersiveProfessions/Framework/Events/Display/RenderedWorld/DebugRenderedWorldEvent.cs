/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

#if DEBUG
namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Common.Extensions.Xna;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class DebugRenderedWorldEvent : RenderedWorldEvent
{
    private readonly Texture2D _pixel;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugRenderedWorldEvent(ProfessionEventManager manager)
        : base(manager)
    {
        _pixel = new(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        _pixel.SetData(new[] { Color.White });
    }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (!ModEntry.Config.DebugKey.IsDown()) return;

        var bb = new Rectangle();
        if (Game1.currentLocation.Objects.TryGetValue(ModEntry.DebugCursorPosition.Tile, out var o))
        {
            bb = o.getBoundingBox(o.TileLocation);
        }
        else
        {
            foreach (var c in Game1.currentLocation.characters.Cast<Character>()
                         .Concat(Game1.currentLocation.farmers))
            {
                var tileLocation = c.getTileLocation();
                if (tileLocation != ModEntry.DebugCursorPosition.Tile) continue;

                bb = c.GetBoundingBox();
                break;
            }
        }

        bb.X -= Game1.viewport.X;
        bb.Y -= Game1.viewport.Y;
        bb.DrawBorder(_pixel, 3, Color.Red, e.SpriteBatch);
    }
}
#endif