/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.UI;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Pointer for highlighting on-screen and off-screen objects of interest for tracker professions.</summary>
internal sealed class HudPointer
{
    private const float MaxStep = 3f;
    private const float MinStep = -3f;

    private readonly Texture2D _texture;
    private readonly Rectangle _sourceRect;

    private float _height = -42f;
    private float _jerk = 1f;
    private float _step;

    /// <summary>Initializes a new instance of the <see cref="HudPointer"/> class.</summary>
    /// <param name="texture">The pointer <see cref="Texture2D"/>.</param>
    /// <param name="scale">The scale for drawing the pointer.</param>
    /// <param name="rate">The rate at which the pointer animates (higher is faster).</param>
    public HudPointer(Texture2D texture, float scale, float rate)
    {
        this._texture = texture;
        this._sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
        this.Scale = scale;
        this.BobRate = rate;
    }

    /// <summary>Gets the singleton <see cref="HudPointer"/> instance.</summary>
    internal static Lazy<HudPointer> Instance { get; } = new(() => new HudPointer(
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/HudPointer"),
        ProfessionsModule.Config.TrackingPointerScale,
        ProfessionsModule.Config.TrackingPointerBobRate));

    /// <summary>Gets or sets the scale for drawing the pointer.</summary>
    internal float Scale { get; set; }

    /// <summary>Gets or sets the rate at which the pointer animates (higher is faster).</summary>
    internal float BobRate { get; set; }

    /// <summary>Gets or sets a value indicating whether or not the pointer is currently being rendered.</summary>
    internal bool ShouldBob { get; set; }

    /// <summary>Advance the pointer's bobbing motion one step.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    public void Bob(uint ticks)
    {
        if (!this.ShouldBob)
        {
            return;
        }

        if (ticks % (8f / this.BobRate) != 0)
        {
            return;
        }

        if (this._step is MaxStep or MinStep)
        {
            this._jerk = -this._jerk;
        }

        this._step += this._jerk;
        this._height += this._step;
    }

    /// <summary>Draw the pointer at the edge of the screen, pointing to a target tile off-screen.</summary>
    /// <param name="target">The target tile to point to.</param>
    /// <param name="color">The color of the pointer.</param>
    public void DrawAsTrackingPointer(Vector2 target, Color color)
    {
        if (Utility.isOnScreen((target * 64f) + new Vector2(32f, 32f), 64))
        {
            return;
        }

        var vpBounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
        Vector2 onScreenPosition = default;
        var rotation = 0f;
        if (target.X * 64f > Game1.viewport.MaxCorner.X - 64)
        {
            onScreenPosition.X = vpBounds.Right - 8;
            rotation = (float)Math.PI / 2f;
        }
        else if (target.X * 64f < Game1.viewport.X)
        {
            onScreenPosition.X = 8f;
            rotation = -(float)Math.PI / 2f;
        }
        else
        {
            onScreenPosition.X = (target.X * 64f) - Game1.viewport.X;
        }

        if (target.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
        {
            onScreenPosition.Y = vpBounds.Bottom - 8;
            rotation = (float)Math.PI;
        }
        else if (target.Y * 64f < Game1.viewport.Y)
        {
            onScreenPosition.Y = 8f;
        }
        else
        {
            onScreenPosition.Y = (target.Y * 64f) - Game1.viewport.Y;
        }

        if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == 8)
        {
            rotation += (float)Math.PI / 4f;
        }

        if ((int)onScreenPosition.X == 8 && (int)onScreenPosition.Y == vpBounds.Bottom - 8)
        {
            rotation += (float)Math.PI / 4f;
        }

        if ((int)onScreenPosition.X == vpBounds.Right - 8 && (int)onScreenPosition.Y == 8)
        {
            rotation -= (float)Math.PI / 4f;
        }

        if ((int)onScreenPosition.X == vpBounds.Right - 8 && (int)onScreenPosition.Y == vpBounds.Bottom - 8)
        {
            rotation -= (float)Math.PI / 4f;
        }

        var safePos = Utility.makeSafe(
            renderSize: new Vector2(
                this._sourceRect.Width * Game1.pixelZoom * this.Scale,
                this._sourceRect.Height * Game1.pixelZoom * this.Scale),
            renderPos: onScreenPosition);

        Game1.spriteBatch.Draw(
            this._texture,
            safePos,
            this._sourceRect,
            color,
            rotation,
            new Vector2(2f, 2f),
            Game1.pixelZoom * this.Scale,
            SpriteEffects.None,
            1f);
    }

    /// <summary>Draw the pointer over a target tile on-screen.</summary>
    /// <param name="target">A target tile.</param>
    /// <param name="color">The color of the pointer.</param>
    /// <remarks>Credit to <c>Bpendragon</c>.</remarks>
    public void DrawOverTile(Vector2 target, Color color)
    {
        if (!Utility.isOnScreen((target * 64f) + new Vector2(32f, 32f), 64))
        {
            return;
        }

        var targetPixel = new Vector2(
            (target.X * Game1.tileSize) + 32f,
            (target.Y * Game1.tileSize) + 32f + this._height);
        var adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
        adjustedPixel = Utility.ModifyCoordinatesForUIScale(adjustedPixel);
        Game1.spriteBatch.Draw(
            this._texture,
            adjustedPixel,
            this._sourceRect,
            color,
            (float)Math.PI,
            new Vector2(2f, 2f),
            Game1.pixelZoom * this.Scale,
            SpriteEffects.None,
            1f);
    }
}
