/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CatGiftsRedux.Framework;

/// <summary>
/// A custom subclass of the Hudmessage to draw in the pet's head and the item.
/// </summary>
internal sealed class PetHudMessage : HUDMessage
{
    private readonly Item spawnedItem;
    private readonly bool cat;
    private readonly float messageWidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetHudMessage"/> class.
    /// </summary>
    /// <param name="message">The message to include.</param>
    /// <param name="color">The color of something...not sure.</param>
    /// <param name="timeLeft">How much time the boxen should hang around for.</param>
    /// <param name="fadeIn">Whether or not the boxen should fade in.</param>
    /// <param name="spawnedItem">The item spawned.</param>
    /// <param name="cat">Whether or not to draw a cat icon. (instead of the dog icon).</param>
    public PetHudMessage(string message, Color color, float timeLeft, bool fadeIn, Item spawnedItem, bool cat)
        : base(message, color, timeLeft, fadeIn)
    {
        this.spawnedItem = spawnedItem;
        this.cat = cat;
        this.messageWidth = ModEntry.StringUtils.MeasureWord(Game1.smallFont, this.message);
    }

    /// <inheritdoc />
    /// <remarks>Draws in the hudmessage. Copied and edited from game code.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override void draw(SpriteBatch b, int i)
    {
        Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
        Vector2 itemBoxPosition = new(tsarea.Left + 16, tsarea.Bottom - (i * 64 * 7 / 4) - 176);
        if (Game1.isOutdoorMapSmallerThanViewport())
        {
            itemBoxPosition.X = Math.Max(tsarea.Left + 16, -Game1.uiViewport.X + 16);
        }

        if (Game1.uiViewport.Width < 1400)
        {
            itemBoxPosition.Y -= 48f;
        }

        // draws the left boxen.
        b.Draw(
            texture: Game1.mouseCursors,
            position: itemBoxPosition,
            new Rectangle(293, 360, 26, 24),
            color: Color.White * this.transparency,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: Game1.pixelZoom,
            effects: SpriteEffects.None,
            layerDepth: 1f);

        // draws the bit the message sits in.
        b.Draw(
            texture: Game1.mouseCursors,
            new Vector2(itemBoxPosition.X + 104f, itemBoxPosition.Y),
            new Rectangle(319, 360, 1, 24),
            color: Color.White * this.transparency,
            rotation: 0f,
            origin: Vector2.Zero,
            new Vector2(this.messageWidth, 4f),
            effects: SpriteEffects.None,
            layerDepth: 1f);

        // draw the right side of the box.
        b.Draw(
            texture: Game1.mouseCursors,
            new Vector2(itemBoxPosition.X + 104f + this.messageWidth, itemBoxPosition.Y),
            new Rectangle(323, 360, 6, 24),
            color: Color.White * this.transparency,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: Game1.pixelZoom,
            effects: SpriteEffects.None,
            layerDepth: 1f);
        itemBoxPosition.X += 16f;
        itemBoxPosition.Y += 16f;

        // draw item.
        this.spawnedItem.drawInMenu(
            spriteBatch: b,
            location: itemBoxPosition,
            scaleSize: 1f,
            transparency: this.transparency,
            layerDepth: 1f,
            drawStackNumber: StackDrawType.Hide);

        // draw pet head.
        b.Draw(
            texture: Game1.mouseCursors,
            position: itemBoxPosition + (new Vector2(8f, 8f) * 4f),
            new Rectangle(160 + (this.cat ? 0 : 48) + (Game1.player.whichPetBreed * 16), 208, 16, 16),
            color: Color.White * this.transparency,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: Game1.pixelZoom,
            effects: SpriteEffects.None,
            layerDepth: 1f);

        itemBoxPosition.X += 83f;
        itemBoxPosition.Y += 18f;
        Utility.drawTextWithShadow(
            b,
            text: this.message,
            font: Game1.smallFont,
            position: itemBoxPosition,
            color: Game1.textColor * this.transparency,
            scale: 1f,
            layerDepth: 1f,
            horizontalShadowOffset: -1,
            verticalShadowOffset: -1,
            shadowIntensity: this.transparency);
    }
}
