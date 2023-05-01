/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NPCArrows.Framework;

using StardewModdingAPI.Events;

using StardewValley;

namespace NPCArrows;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// Gets the logging instance for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;

        AssetManager.Initialize(helper.GameContent);

        helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Apply(e);
        helper.Events.Content.AssetsInvalidated += static (_, e) => AssetManager.Reset(e.NamesWithoutLocale);

        helper.Events.Display.RenderedHud += this.Display_RenderedHud;
    }

    private void Display_RenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (Context.IsPlayerFree && Game1.currentLocation?.characters is { } characters)
        {
            foreach (NPC? character in characters)
            {
                if (character?.CanSocialize != true)
                {
                    continue;
                }

                DrawArrowForNPC(e.SpriteBatch, character);
            }
        }
    }

    /// <summary>
    /// Draws in an arrow pointing at an NPC.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to use.</param>
    /// <param name="character">Character to draw arrow pointing at.</param>
    private static void DrawArrowForNPC(SpriteBatch spriteBatch, NPC character)
    {
        Vector2 pos = character.Position + new Vector2(32f, 64f);

        Vector2 arrowPos = Game1.GlobalToLocal(Game1.uiViewport, pos);
        Direction direction = Direction.None;

        if (arrowPos.X <= 0)
        {
            direction |= Direction.Left;
            arrowPos.X = 8f;
        }
        else if (arrowPos.X >= Game1.viewport.Width)
        {
            direction |= Direction.Right;
            arrowPos.X = Game1.viewport.Width - 8f;
        }

        if (arrowPos.Y <= 0)
        {
            direction |= Direction.Up;
            arrowPos.Y = 8f;
        }
        else if (arrowPos.Y >= Game1.viewport.Height)
        {
            direction |= Direction.Down;
            arrowPos.Y = Game1.viewport.Height - 8f;
        }

        if (direction == Direction.None)
        {
            return;
        }

        arrowPos = Utility.snapToInt(arrowPos);

        spriteBatch.Draw(
            texture: AssetManager.ArrowTexture,
            position: arrowPos,
            sourceRectangle: null,
            color: Color.MediumPurple,
            rotation: direction.GetRotationFacing(),
            origin: new Vector2(2f, 2f),
            scale: Game1.pixelZoom,
            effects: SpriteEffects.None,
            layerDepth: 1f);
    }
}
